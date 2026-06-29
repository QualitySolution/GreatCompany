using FluentNHibernate.Mapping;
using GreatCompany.Data.Models;

namespace GreatCompany.Data.Mappings;

public class AccountMap : ClassMap<Account> {
	public AccountMap() {
		Table("accounts");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Name).Column("name");
		Map(x => x.TaxRegime).Column("tax_regime").CustomType<TaxRegimeType>();
	}
}

public class DivisionMap : ClassMap<Division> {
	public DivisionMap() {
		Table("divisions");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Name).Column("name");
		Map(x => x.ParentDivisionId).Column("parent_division_id");
	}
}

public class ProjectMap : ClassMap<Project> {
	public ProjectMap() {
		Table("projects");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Name).Column("name");
		Map(x => x.DivisionId).Column("division_id");
	}
}

public class IncomeArticleMap : ClassMap<IncomeArticle> {
	public IncomeArticleMap() {
		Table("income_articles");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Name).Column("name");
	}
}

public class ExpenseArticleMap : ClassMap<ExpenseArticle> {
	public ExpenseArticleMap() {
		Table("expense_articles");
		Id(x => x.Id).Column("id").GeneratedBy.Native();
		Map(x => x.Name).Column("name");
	}
}
