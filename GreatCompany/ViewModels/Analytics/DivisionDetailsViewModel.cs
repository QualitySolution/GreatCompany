using GreatCompany.Data.Analytics;
using GreatCompany.Data.Models;
using NHibernate;
using NHibernate.Linq;
using QS.DomainModel.UoW;
using QS.Navigation;

namespace GreatCompany.ViewModels.Analytics;

/// <param name="Ids">подразделение вместе со всеми дочерними, по ним суммируются цифры</param>
public record DivisionOption(string Title, IReadOnlyCollection<int> Ids);

/// <summary>приход или расход подразделения по месяцам с разбивкой на статьи, проекты или счета</summary>
public class DivisionDetailsViewModel : AnalyticsViewModelBase {
	public DivisionDetailsViewModel(INavigationManager navigation, IUnitOfWorkFactory uowFactory) : base(navigation, uowFactory) {
		Title = "Детализация по подразделению";

		using(var uow = CreateUoW())
			Divisions = DivisionTree.Build(uow.Session.Query<Division>().ToList())
				.Select(x => new DivisionOption(new string(' ', x.Depth * 4) + x.Division.Name, x.Ids))
				.ToList();
		division = Divisions.FirstOrDefault();

		Refresh();
	}

	public static IReadOnlyList<CashFlowKind> Kinds { get; } = Enum.GetValues<CashFlowKind>();
	public static IReadOnlyList<CashFlowColumns> ColumnModes { get; } = Enum.GetValues<CashFlowColumns>();
	public static IReadOnlyList<CashFlowSource> Sources { get; } = Enum.GetValues<CashFlowSource>();

	public IReadOnlyList<DivisionOption> Divisions { get; }

	DivisionOption? division;
	public DivisionOption? Division { get => division; set => SetAndRefresh(ref division, value); }

	CashFlowKind kind;
	public CashFlowKind Kind { get => kind; set => SetAndRefresh(ref kind, value); }

	CashFlowColumns columns;
	public CashFlowColumns Columns { get => columns; set => SetAndRefresh(ref columns, value); }

	CashFlowSource source;
	public CashFlowSource Source { get => source; set => SetAndRefresh(ref source, value); }

	protected override CashFlowTable? BuildTable(ISession session) =>
		Division == null
			? null
			: CashFlowTable.Build(session, new CashFlowFilter(Kind, Source, Columns, Division.Ids, FromMonth, ToMonth, WithVat));
}
