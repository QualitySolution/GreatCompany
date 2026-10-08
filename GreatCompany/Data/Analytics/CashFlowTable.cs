using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GreatCompany.Data.Models;
using NHibernate;
using NHibernate.Linq;

namespace GreatCompany.Data.Analytics;

public enum CashFlowKind {
	[Display(Name = "Приход")] Income,
	[Display(Name = "Расход")] Expense
}

public enum CashFlowSource {
	[Display(Name = "Факт")] Fact,
	[Display(Name = "План")] Plan
}

public enum CashFlowColumns {
	[Display(Name = "По статьям")] Articles,
	[Display(Name = "По проектам")] Projects,
	[Display(Name = "По счетам")] Accounts
}

/// <param name="DivisionIds">подразделение вместе со всеми дочерними</param>
/// <param name="FromMonth">первый месяц периода, важны только год и месяц</param>
/// <param name="ToMonth">последний месяц периода включительно</param>
/// <param name="WithVat">false - суммы за вычетом НДС</param>
public record CashFlowFilter(
	CashFlowKind Kind,
	CashFlowSource Source,
	CashFlowColumns Columns,
	IReadOnlyCollection<int> DivisionIds,
	DateTime FromMonth,
	DateTime ToMonth,
	bool WithVat);

/// <param name="Values">суммы по колонкам, последняя - итог строки</param>
public record CashFlowRow(string Title, decimal[] Values);

/// <param name="Columns">заголовки колонок с суммами без итоговой</param>
/// <param name="Rows">строки по месяцам, последняя - итог за период</param>
public record CashFlowTable(IReadOnlyList<string> Columns, IReadOnlyList<CashFlowRow> Rows) {
	public static CashFlowTable Empty { get; } = new(Array.Empty<string>(), Array.Empty<CashFlowRow>());

	/// <summary>
	/// приход или расход подразделения по месяцам с разбивкой на статьи, проекты или счета.
	/// колонки - только те, по которым за период что-то было, крупные первыми
	/// </summary>
	public static CashFlowTable Build(ISession session, CashFlowFilter filter) {
		var from = new DateTime(filter.FromMonth.Year, filter.FromMonth.Month, 1);
		var to = new DateTime(filter.ToMonth.Year, filter.ToMonth.Month, 1).AddMonths(1);
		if(to <= from)
			return Empty;

		var entries = Load(session, filter, from, to);
		var names = ColumnNames(session, filter);

		var columns = entries
			.GroupBy(x => x.ColumnId)
			.Select(g => (Id: g.Key, Total: g.Sum(x => x.Amount)))
			.OrderByDescending(x => x.Total)
			.Select(x => x.Id)
			.ToList();
		var index = columns.Select((id, i) => (id, i)).ToDictionary(x => x.id, x => x.i);

		var rows = new List<CashFlowRow>();
		var total = new decimal[columns.Count + 1];
		for(var month = from; month < to; month = month.AddMonths(1)) {
			var values = new decimal[columns.Count + 1];
			foreach(var entry in entries.Where(x => x.Month == month)) {
				values[index[entry.ColumnId]] += entry.Amount;
				values[^1] += entry.Amount;
			}
			for(var i = 0; i < values.Length; i++)
				total[i] += values[i];
			rows.Add(new CashFlowRow(MonthTitle(month), values));
		}
		rows.Add(new CashFlowRow("Итого", total));

		return new CashFlowTable(columns.Select(id => names(id)).ToList(), rows);
	}

	record Entry(DateTime Month, int ColumnId, decimal Amount);

	// документы грузятся целиком, у ссылок нужен только Id, он есть у прокси без загрузки.
	// документов за период немного, группировка в памяти проще запроса под каждый тип
	static List<Entry> Load(ISession session, CashFlowFilter filter, DateTime from, DateTime to) {
		var ids = filter.DivisionIds;

		return (filter.Kind, filter.Source) switch {
			(CashFlowKind.Income, CashFlowSource.Fact) => Incomes(session.Query<ActualIncome>()),
			(CashFlowKind.Income, CashFlowSource.Plan) => Incomes(session.Query<PlannedIncome>()),
			(CashFlowKind.Expense, CashFlowSource.Fact) => Expenses(session.Query<ActualExpense>()),
			_ => Expenses(session.Query<PlannedExpense>()),
		};

		List<Entry> Incomes<T>(IQueryable<T> query) where T : IncomeOperationBase =>
			query.Where(x => x.Date >= from && x.Date < to && ids.Contains(x.Division.Id)).ToList()
				.Select(x => ToEntry(x.Date!.Value, x.IncomeArticle.Id, x.Project?.Id, x.Account.Id, x.Cost, x.Vat))
				.ToList();

		List<Entry> Expenses<T>(IQueryable<T> query) where T : ExpenseOperationBase =>
			query.Where(x => x.Date >= from && x.Date < to && ids.Contains(x.Division.Id)).ToList()
				.Select(x => ToEntry(x.Date!.Value, x.ExpenseArticle.Id, x.Project?.Id, x.Account.Id, x.Cost, x.Vat))
				.ToList();

		Entry ToEntry(DateTime date, int articleId, int? projectId, int accountId, decimal? cost, decimal? vat) {
			var columnId = filter.Columns switch {
				CashFlowColumns.Articles => articleId,
				CashFlowColumns.Projects => projectId ?? NoProject,
				_ => accountId,
			};
			var amount = (cost ?? 0) - (filter.WithVat ? 0 : vat ?? 0);
			return new Entry(new DateTime(date.Year, date.Month, 1), columnId, amount);
		}
	}

	// у проекта Id с нуля не бывает, им помечаем документы без проекта
	const int NoProject = 0;

	static Func<int, string> ColumnNames(ISession session, CashFlowFilter filter) {
		var names = filter.Columns switch {
			CashFlowColumns.Articles when filter.Kind == CashFlowKind.Income =>
				session.Query<IncomeArticle>().ToDictionary(x => x.Id, x => x.Name),
			CashFlowColumns.Articles =>
				session.Query<ExpenseArticle>().ToDictionary(x => x.Id, x => x.Name),
			CashFlowColumns.Projects =>
				session.Query<Project>().ToDictionary(x => x.Id, x => x.Name),
			_ => session.Query<Account>().ToDictionary(x => x.Id, x => x.Name),
		};
		return id => id == NoProject && filter.Columns == CashFlowColumns.Projects
			? "Без проекта"
			: names.GetValueOrDefault(id, $"№{id}");
	}

	static readonly CultureInfo Russian = CultureInfo.GetCultureInfo("ru-RU");

	static string MonthTitle(DateTime month) {
		var name = Russian.DateTimeFormat.MonthNames[month.Month - 1];
		return $"{char.ToUpper(name[0], Russian)}{name[1..]} {month.Year}";
	}
}
