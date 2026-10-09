using System.Globalization;
using GreatCompany.Data.Analytics;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.ViewModels.Dialog;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace GreatCompany.ViewModels.Analytics;

/// <summary>карточка счёта на дашборде, тексты уже готовы к показу</summary>
public record AccountCard(
	string Name,
	string Balance,
	bool IsBalanceNegative,
	string Change,
	bool IsChangePositive,
	bool IsChangeNegative,
	string Outflow,
	string DaysLeft,
	bool IsDaysLeftCritical,
	bool IsTotal);

/// <summary>
/// остатки по счетам карточками, первой - сумма по всем счетам.
/// потом сюда добавятся прогноз по плану и кассовые разрывы
/// </summary>
public class AccountsDashboardViewModel : DialogViewModelBase {
	static readonly CultureInfo Russian = CultureInfo.GetCultureInfo("ru-RU");
	readonly IUnitOfWorkFactory uowFactory;

	public AccountsDashboardViewModel(INavigationManager navigation, IUnitOfWorkFactory uowFactory) : base(navigation) {
		this.uowFactory = uowFactory;
		Title = "Счета";
		RefreshCommand = ReactiveCommand.Create(Refresh);
		Refresh();
	}

	IReadOnlyList<AccountCard> cards = Array.Empty<AccountCard>();
	public IReadOnlyList<AccountCard> Cards { get => cards; private set => SetField(ref cards, value); }

	public ReactiveCommand<RxVoid, RxVoid> RefreshCommand { get; }

	void Refresh() {
		using var uow = uowFactory.Create();
		var (accounts, total) = AccountDashboard.Build(uow.Session, DateTime.Today);
		Cards = accounts.Select(x => Card(x, false)).Prepend(Card(total, true)).ToList();
	}

	static AccountCard Card(AccountSummary summary, bool isTotal) {
		var lastMonth = Russian.DateTimeFormat.MonthNames[DateTime.Today.AddMonths(-1).Month - 1];
		var change = Math.Round(summary.Change, 0);
		return new AccountCard(
			summary.Name,
			Money(summary.Balance),
			Math.Round(summary.Balance, 0, MidpointRounding.AwayFromZero) < 0,
			(change > 0 ? "+" : "") + Money(summary.Change) + " за месяц",
			change > 0,
			change < 0,
			summary.DailyOutflow > 0 ? $"Расход за {lastMonth}: {Money(summary.DailyOutflow)} в день" : $"Расходов за {lastMonth} не было",
			summary.DaysLeft switch {
				null => "Сколько хватит, оценить нельзя",
				0 => "Денег нет",
				var days => $"Хватит примерно на {days} {Plural(days.Value, "день", "дня", "дней")}",
			},
			summary.DaysLeft < CriticalDays,
			isTotal);
	}

	/// <summary>меньше недели - пора что-то делать, подсвечиваем</summary>
	const int CriticalDays = 7;

	static string Money(decimal value) => Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString("#,##0", Russian) + " ₽";

	static string Plural(int number, string one, string few, string many) {
		var n = Math.Abs(number) % 100;
		if(n is >= 11 and <= 14)
			return many;
		return (n % 10) switch { 1 => one, >= 2 and <= 4 => few, _ => many };
	}
}
