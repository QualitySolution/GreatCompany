using System.Globalization;
using System.Runtime.CompilerServices;
using GreatCompany.Data.Analytics;
using NHibernate;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.ViewModels.Dialog;

namespace GreatCompany.ViewModels.Analytics;

/// <summary>
/// аналитика по месяцам: период, НДС и вид сумм общие, таблицу считает наследник.
/// наследник вызывает Refresh в конце своего конструктора, когда его поля уже заполнены
/// </summary>
public abstract class AnalyticsViewModelBase : DialogViewModelBase {
	readonly IUnitOfWorkFactory uowFactory;

	protected AnalyticsViewModelBase(INavigationManager navigation, IUnitOfWorkFactory uowFactory) : base(navigation) {
		this.uowFactory = uowFactory;

		// последние 12 месяцев вместе с текущим
		var from = DateTime.Today.AddMonths(-11);
		fromMonthIndex = from.Month - 1;
		fromYear = from.Year;
		toMonthIndex = DateTime.Today.Month - 1;
		toYear = DateTime.Today.Year;
	}

	public static IReadOnlyList<string> Months { get; } =
		CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.MonthNames[..12];
	public static IReadOnlyList<AmountFormat> AmountFormats { get; } = Enum.GetValues<AmountFormat>();

	int fromMonthIndex;
	public int FromMonthIndex { get => fromMonthIndex; set => SetAndRefresh(ref fromMonthIndex, value); }

	int fromYear;
	public int FromYear { get => fromYear; set => SetAndRefresh(ref fromYear, value); }

	int toMonthIndex;
	public int ToMonthIndex { get => toMonthIndex; set => SetAndRefresh(ref toMonthIndex, value); }

	int toYear;
	public int ToYear { get => toYear; set => SetAndRefresh(ref toYear, value); }

	bool withVat;
	public bool WithVat { get => withVat; set => SetAndRefresh(ref withVat, value); }

	AmountFormat amountFormat = AmountFormat.Rubles;
	/// <summary>только вид чисел, данные не пересчитываются</summary>
	public AmountFormat AmountFormat { get => amountFormat; set => SetField(ref amountFormat, value); }

	CashFlowTable table = CashFlowTable.Empty;
	/// <summary>колонки меняются вместе с данными, поэтому таблица перестраивается целиком по смене этого свойства</summary>
	public CashFlowTable Table { get => table; private set => SetField(ref table, value); }

	protected DateTime FromMonth => new(FromYear, FromMonthIndex + 1, 1);
	protected DateTime ToMonth => new(ToYear, ToMonthIndex + 1, 1);

	protected void SetAndRefresh<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
		if(SetField(ref field, value, propertyName))
			Refresh();
	}

	protected void Refresh() {
		// месяц в комбобоксе может быть не выбран, пока его переключают
		if(FromMonthIndex < 0 || ToMonthIndex < 0) {
			Table = CashFlowTable.Empty;
			return;
		}

		using var uow = uowFactory.Create();
		Table = BuildTable(uow.Session) ?? CashFlowTable.Empty;
	}

	/// <returns>null - посчитать нельзя, например не выбрано подразделение</returns>
	protected abstract CashFlowTable? BuildTable(ISession session);

	protected IUnitOfWork CreateUoW() => uowFactory.Create();
}
