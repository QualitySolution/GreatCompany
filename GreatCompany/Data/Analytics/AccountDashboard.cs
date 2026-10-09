using GreatCompany.Data.Models;
using NHibernate;
using NHibernate.Linq;

namespace GreatCompany.Data.Analytics;

/// <param name="Balance">остаток на сегодня</param>
/// <param name="BalanceMonthAgo">остаток в тот же день месяц назад</param>
/// <param name="LastMonthOutflow">сколько ушло за прошлый календарный месяц</param>
/// <param name="LastMonthDays">дней в прошлом месяце</param>
public record AccountSummary(string Name, decimal Balance, decimal BalanceMonthAgo, decimal LastMonthOutflow, int LastMonthDays) {
	public decimal Change => Balance - BalanceMonthAgo;

	/// <summary>средний расход в день за прошлый месяц</summary>
	public decimal DailyOutflow => LastMonthDays == 0 ? 0 : LastMonthOutflow / LastMonthDays;

	/// <summary>на сколько дней хватит остатка при расходе как в прошлом месяце. null - расходов не было, оценить нельзя</summary>
	public int? DaysLeft => DailyOutflow <= 0 ? null : Balance <= 0 ? 0 : (int)Math.Floor(Balance / DailyOutflow);
}

/// <summary>
/// остатки по счетам. Остаток - все приходы минус расходы с переводами на счёт и со счёта.
/// у счёта расходом считаются и переводы с него: с ИП деньги уходят на наличку и тратятся уже там.
/// у суммы по всем счетам переводы взаимно гасятся, поэтому расход - только расходы
/// </summary>
public static class AccountDashboard {
	public static (IReadOnlyList<AccountSummary> Accounts, AccountSummary Total) Build(ISession session, DateTime today) {
		today = today.Date;
		var monthAgo = today.AddMonths(-1);
		var lastMonthTo = new DateTime(today.Year, today.Month, 1);
		var lastMonthFrom = lastMonthTo.AddMonths(-1);
		var lastMonthDays = (lastMonthTo - lastMonthFrom).Days;

		var incomes = session.Query<ActualIncome>().Where(x => x.Date <= today)
			.Select(x => new { AccountId = x.Account.Id, x.Date, x.Cost }).ToList();
		var expenses = session.Query<ActualExpense>().Where(x => x.Date <= today)
			.Select(x => new { AccountId = x.Account.Id, x.Date, x.Cost }).ToList();
		var transfers = session.Query<ActualTransfer>().Where(x => x.Date <= today)
			.Select(x => new { From = x.FromAccount.Id, To = x.ToAccount.Id, x.Date, x.Cost }).ToList();

		// движения денег по счёту: плюс - пришло, минус - ушло
		var moves = incomes.Select(x => (x.AccountId, Date: x.Date!.Value, Amount: x.Cost ?? 0, IsExpense: false))
			.Concat(expenses.Select(x => (x.AccountId, Date: x.Date!.Value, Amount: -(x.Cost ?? 0), IsExpense: true)))
			.Concat(transfers.Select(x => (AccountId: x.To, Date: x.Date!.Value, Amount: x.Cost ?? 0, IsExpense: false)))
			.Concat(transfers.Select(x => (AccountId: x.From, Date: x.Date!.Value, Amount: -(x.Cost ?? 0), IsExpense: false)))
			.ToList();

		bool InLastMonth(DateTime date) => date >= lastMonthFrom && date < lastMonthTo;

		var accounts = session.Query<Account>().ToList()
			.OrderBy(x => x.Name)
			.Select(account => {
				var own = moves.Where(x => x.AccountId == account.Id).ToList();
				return new AccountSummary(
					account.Name,
					own.Sum(x => x.Amount),
					own.Where(x => x.Date <= monthAgo).Sum(x => x.Amount),
					-own.Where(x => x.Amount < 0 && InLastMonth(x.Date)).Sum(x => x.Amount),
					lastMonthDays);
			})
			.ToList();

		var total = new AccountSummary(
			"Все счета",
			accounts.Sum(x => x.Balance),
			accounts.Sum(x => x.BalanceMonthAgo),
			-moves.Where(x => x.IsExpense && InLastMonth(x.Date)).Sum(x => x.Amount),
			lastMonthDays);

		return (accounts, total);
	}
}
