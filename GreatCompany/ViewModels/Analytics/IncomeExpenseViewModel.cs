using System.ComponentModel.DataAnnotations;
using GreatCompany.Data.Analytics;
using NHibernate;
using QS.DomainModel.UoW;
using QS.Navigation;

namespace GreatCompany.ViewModels.Analytics;

public enum IncomeExpenseMode {
	[Display(Name = "За месяц")] Monthly,
	[Display(Name = "Среднее за 3 месяца")] Rolling3
}

/// <summary>приход и расход всех подразделений по месяцам, рядом будут графики по тем же данным</summary>
public class IncomeExpenseViewModel : AnalyticsViewModelBase {
	public IncomeExpenseViewModel(INavigationManager navigation, IUnitOfWorkFactory uowFactory) : base(navigation, uowFactory) {
		Title = "Доходы и расходы";
		Refresh();
	}

	public static IReadOnlyList<IncomeExpenseMode> Modes { get; } = Enum.GetValues<IncomeExpenseMode>();

	IncomeExpenseMode mode;
	public IncomeExpenseMode Mode { get => mode; set => SetAndRefresh(ref mode, value); }

	protected override CashFlowTable BuildTable(ISession session) =>
		IncomeExpenseTable.Build(session, FromMonth, ToMonth, WithVat, Mode == IncomeExpenseMode.Rolling3);
}
