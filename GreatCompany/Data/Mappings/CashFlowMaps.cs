using FluentNHibernate.Mapping;
using GreatCompany.Data.Models;

namespace GreatCompany.Data.Mappings;

public class PlannedIncomeMap : ClassMap<PlannedIncome> {
	public PlannedIncomeMap() {
		Table("planned_incomes");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Date).Column("date");
		Map(x => x.Purpose).Column("purpose");
		Map(x => x.Amount).Column("amount");
		Map(x => x.VatAmount).Column("vat_amount");
		Map(x => x.AccountId).Column("account_id");
		Map(x => x.ProjectId).Column("project_id");
		Map(x => x.IncomeArticleId).Column("income_article_id");
	}
}

public class ActualIncomeMap : ClassMap<ActualIncome> {
	public ActualIncomeMap() {
		Table("actual_incomes");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Date).Column("date");
		Map(x => x.Purpose).Column("purpose");
		Map(x => x.Amount).Column("amount");
		Map(x => x.VatAmount).Column("vat_amount");
		Map(x => x.AccountId).Column("account_id");
		Map(x => x.ProjectId).Column("project_id");
		Map(x => x.IncomeArticleId).Column("income_article_id");
		Map(x => x.PlannedIncomeId).Column("planned_income_id");
	}
}

public class PlannedExpenseMap : ClassMap<PlannedExpense> {
	public PlannedExpenseMap() {
		Table("planned_expenses");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Date).Column("date");
		Map(x => x.Purpose).Column("purpose");
		Map(x => x.Amount).Column("amount");
		Map(x => x.VatAmount).Column("vat_amount");
		Map(x => x.AccountId).Column("account_id");
		Map(x => x.DivisionId).Column("division_id");
		Map(x => x.ProjectId).Column("project_id");
		Map(x => x.ExpenseArticleId).Column("expense_article_id");
	}
}

public class ActualExpenseMap : ClassMap<ActualExpense> {
	public ActualExpenseMap() {
		Table("actual_expenses");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Date).Column("date");
		Map(x => x.Purpose).Column("purpose");
		Map(x => x.Amount).Column("amount");
		Map(x => x.VatAmount).Column("vat_amount");
		Map(x => x.AccountId).Column("account_id");
		Map(x => x.DivisionId).Column("division_id");
		Map(x => x.ProjectId).Column("project_id");
		Map(x => x.ExpenseArticleId).Column("expense_article_id");
		Map(x => x.PlannedExpenseId).Column("planned_expense_id");
	}
}
