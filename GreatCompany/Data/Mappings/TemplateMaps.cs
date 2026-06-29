using FluentNHibernate.Mapping;
using GreatCompany.Data.Models;

namespace GreatCompany.Data.Mappings;

public class AccrualTemplateMap : ClassMap<AccrualTemplate> {
	public AccrualTemplateMap() {
		Table("accrual_templates");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Purpose).Column("purpose");
		Map(x => x.Amount).Column("amount");
		Map(x => x.VatAmount).Column("vat_amount");
		Map(x => x.AccountId).Column("account_id");
		Map(x => x.ProjectId).Column("project_id");
		Map(x => x.IncomeArticleId).Column("income_article_id");
	}
}

public class PaymentTemplateMap : ClassMap<PaymentTemplate> {
	public PaymentTemplateMap() {
		Table("payment_templates");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Purpose).Column("purpose");
		Map(x => x.Amount).Column("amount");
		Map(x => x.VatAmount).Column("vat_amount");
		Map(x => x.AccountId).Column("account_id");
		Map(x => x.DivisionId).Column("division_id");
		Map(x => x.ProjectId).Column("project_id");
		Map(x => x.ExpenseArticleId).Column("expense_article_id");
	}
}
