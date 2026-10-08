using GreatCompany.Data.Models;
using NHibernate;
using NHibernate.Linq;

namespace GreatCompany.Data.Analytics;

/// <summary>
/// фактический приход, расход и результат (приход минус расход) по месяцам, по три колонки на каждое подразделение и итог по всем.
/// у подразделения только его собственные документы, дочерние не складываются, иначе итог посчитался бы дважды.
/// расход пока относится к тому подразделению, на котором записан, без распределения общих расходов
/// </summary>
public static class IncomeExpenseTable {
	/// <param name="rolling3">вместо суммы за месяц - среднее за этот и два предыдущих месяца</param>
	public static CashFlowTable Build(ISession session, DateTime fromMonth, DateTime toMonth, bool withVat, bool rolling3) {
		var from = new DateTime(fromMonth.Year, fromMonth.Month, 1);
		var to = new DateTime(toMonth.Year, toMonth.Month, 1).AddMonths(1);
		if(to <= from)
			return CashFlowTable.Empty;

		// для среднего первым месяцам периода нужны два месяца до него
		var loadFrom = rolling3 ? from.AddMonths(-2) : from;

		var incomes = Sums(session.Query<ActualIncome>()
			.Where(x => x.Date >= loadFrom && x.Date < to).ToList()
			.Select(x => (x.Date!.Value, x.Division.Id, Amount(x.Cost, x.Vat))));
		var expenses = Sums(session.Query<ActualExpense>()
			.Where(x => x.Date >= loadFrom && x.Date < to).ToList()
			.Select(x => (x.Date!.Value, x.Division.Id, Amount(x.Cost, x.Vat))));

		var used = incomes.Keys.Concat(expenses.Keys).Select(x => x.DivisionId).ToHashSet();
		var divisions = DivisionTree.Build(session.Query<Division>().ToList())
			.Select(x => x.Division)
			.Where(x => used.Contains(x.Id))
			.ToList();

		var columns = divisions
			.SelectMany(x => new[] { $"{x.Name}\nприход", $"{x.Name}\nрасход", $"{x.Name}\nрезультат" })
			.Concat(new[] { "Итого\nприход", "Итого\nрасход", "Итого\nрезультат" })
			.ToList();

		var rows = new List<CashFlowRow>();
		var total = new decimal[columns.Count];
		for(var month = from; month < to; month = month.AddMonths(1)) {
			var values = rolling3 ? Average(MonthValues(month), MonthValues(month.AddMonths(-1)), MonthValues(month.AddMonths(-2))) : MonthValues(month);
			for(var i = 0; i < values.Length; i++)
				total[i] += values[i];
			rows.Add(new CashFlowRow(CashFlowTable.MonthTitle(month), values));
		}
		// сумма скользящих средних смысла не имеет
		if(!rolling3)
			rows.Add(new CashFlowRow("Итого", total, IsTotal: true));

		// подразделение могло быть только с расходом или только с приходом, пустые колонки не показываем
		var visible = Enumerable.Range(0, columns.Count).Where(i => rows.Any(r => r.Values[i] != 0)).ToList();
		return new CashFlowTable(
			visible.Select(i => columns[i]).ToList(),
			rows.Select(r => r with { Values = visible.Select(i => r.Values[i]).ToArray() }).ToList());

		decimal Amount(decimal? cost, decimal? vat) => (cost ?? 0) - (withVat ? 0 : vat ?? 0);

		decimal[] MonthValues(DateTime month) {
			var values = new decimal[columns.Count];
			for(var i = 0; i < divisions.Count; i++)
				Fill(i * 3, incomes.GetValueOrDefault((month, divisions[i].Id)), expenses.GetValueOrDefault((month, divisions[i].Id)));
			Fill(values.Length - 3,
				incomes.Where(x => x.Key.Month == month).Sum(x => x.Value),
				expenses.Where(x => x.Key.Month == month).Sum(x => x.Value));
			return values;

			void Fill(int start, decimal income, decimal expense) {
				values[start] = income;
				values[start + 1] = expense;
				values[start + 2] = income - expense;
			}
		}
	}

	static Dictionary<(DateTime Month, int DivisionId), decimal> Sums(IEnumerable<(DateTime Date, int DivisionId, decimal Amount)> entries) =>
		entries
			.GroupBy(x => (new DateTime(x.Date.Year, x.Date.Month, 1), x.DivisionId))
			.ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

	static decimal[] Average(params decimal[][] months) =>
		Enumerable.Range(0, months[0].Length)
			.Select(i => Math.Round(months.Sum(m => m[i]) / months.Length, 2))
			.ToArray();
}
