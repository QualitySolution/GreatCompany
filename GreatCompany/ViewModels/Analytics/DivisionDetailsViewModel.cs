using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using GreatCompany.Data.Analytics;
using GreatCompany.Data.Models;
using NHibernate.Linq;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.ViewModels.Dialog;

namespace GreatCompany.ViewModels.Analytics;

/// <summary>вид сумм в таблице, сами суммы не меняются</summary>
public enum AmountFormat {
	[Display(Name = "С копейками")] Kopecks,
	[Display(Name = "Целые")] Rubles,
	[Display(Name = "В тысячах")] Thousands
}

/// <param name="Ids">подразделение вместе со всеми дочерними, по ним суммируются цифры</param>
public record DivisionOption(string Title, IReadOnlyCollection<int> Ids);

/// <summary>приход или расход подразделения по месяцам с разбивкой на статьи, проекты или счета</summary>
public class DivisionDetailsViewModel : DialogViewModelBase {
	readonly IUnitOfWorkFactory uowFactory;

	public DivisionDetailsViewModel(INavigationManager navigation, IUnitOfWorkFactory uowFactory) : base(navigation) {
		this.uowFactory = uowFactory;
		Title = "Детализация по подразделению";

		using(var uow = uowFactory.Create())
			Divisions = DivisionTree(uow.Session.Query<Division>().ToList());
		division = Divisions.FirstOrDefault();

		// последние 12 месяцев вместе с текущим
		var from = DateTime.Today.AddMonths(-11);
		fromMonthIndex = from.Month - 1;
		fromYear = from.Year;
		toMonthIndex = DateTime.Today.Month - 1;
		toYear = DateTime.Today.Year;

		Refresh();
	}

	public static IReadOnlyList<string> Months { get; } =
		CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.MonthNames[..12];
	public static IReadOnlyList<CashFlowKind> Kinds { get; } = Enum.GetValues<CashFlowKind>();
	public static IReadOnlyList<CashFlowColumns> ColumnModes { get; } = Enum.GetValues<CashFlowColumns>();
	public static IReadOnlyList<CashFlowSource> Sources { get; } = Enum.GetValues<CashFlowSource>();
	public static IReadOnlyList<AmountFormat> AmountFormats { get; } = Enum.GetValues<AmountFormat>();

	public IReadOnlyList<DivisionOption> Divisions { get; }

	DivisionOption? division;
	public DivisionOption? Division { get => division; set => SetAndRefresh(ref division, value); }

	CashFlowKind kind;
	public CashFlowKind Kind { get => kind; set => SetAndRefresh(ref kind, value); }

	CashFlowColumns columns;
	public CashFlowColumns Columns { get => columns; set => SetAndRefresh(ref columns, value); }

	CashFlowSource source;
	public CashFlowSource Source { get => source; set => SetAndRefresh(ref source, value); }

	bool withVat;
	public bool WithVat { get => withVat; set => SetAndRefresh(ref withVat, value); }

	AmountFormat amountFormat = AmountFormat.Rubles;
	/// <summary>только вид чисел, данные не пересчитываются</summary>
	public AmountFormat AmountFormat { get => amountFormat; set => SetField(ref amountFormat, value); }

	int fromMonthIndex;
	public int FromMonthIndex { get => fromMonthIndex; set => SetAndRefresh(ref fromMonthIndex, value); }

	int fromYear;
	public int FromYear { get => fromYear; set => SetAndRefresh(ref fromYear, value); }

	int toMonthIndex;
	public int ToMonthIndex { get => toMonthIndex; set => SetAndRefresh(ref toMonthIndex, value); }

	int toYear;
	public int ToYear { get => toYear; set => SetAndRefresh(ref toYear, value); }

	CashFlowTable table = CashFlowTable.Empty;
	/// <summary>колонки меняются вместе с данными, поэтому вью перестраивает таблицу целиком по смене этого свойства</summary>
	public CashFlowTable Table { get => table; private set => SetField(ref table, value); }

	void SetAndRefresh<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
		if(SetField(ref field, value, propertyName))
			Refresh();
	}

	void Refresh() {
		// месяц в комбобоксе может быть не выбран, пока его переключают
		if(Division == null || FromMonthIndex < 0 || ToMonthIndex < 0) {
			Table = CashFlowTable.Empty;
			return;
		}

		var filter = new CashFlowFilter(
			Kind,
			Source,
			Columns,
			Division.Ids,
			new DateTime(FromYear, FromMonthIndex + 1, 1),
			new DateTime(ToYear, ToMonthIndex + 1, 1),
			WithVat);

		using var uow = uowFactory.Create();
		Table = CashFlowTable.Build(uow.Session, filter);
	}

	/// <summary>подразделения деревом с отступами, у каждого - свой Id и Id всех потомков</summary>
	static IReadOnlyList<DivisionOption> DivisionTree(IList<Division> all) {
		var children = all.ToLookup(x => x.ParentDivision?.Id);
		var result = new List<DivisionOption>();

		foreach(var root in children[null].OrderBy(x => x.Name))
			Add(root, 0);
		return result;

		List<int> Add(Division division, int depth) {
			var ids = new List<int> { division.Id };
			var option = new DivisionOption(new string(' ', depth * 4) + division.Name, ids);
			result.Add(option);
			foreach(var child in children[division.Id].OrderBy(x => x.Name))
				ids.AddRange(Add(child, depth + 1));
			return ids;
		}
	}
}
