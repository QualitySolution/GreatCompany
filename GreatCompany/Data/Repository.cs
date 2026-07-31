using GreatCompany.Data.Models;
using MySqlConnector;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using QS.DomainModel.Entity;
using QS.DomainModel.UoW;

namespace GreatCompany.Data;

public class Repository(IUnitOfWorkFactory uowFactory) {
	public T? Get<T>(int id) where T : class, IDomainObject {
		using var uow = uowFactory.Create();
		return uow.GetById<T>(id);
	}

	public int Save<T>(T entity) where T : class, IDomainObject {
		using var uow = uowFactory.Create();
		uow.Save(entity);
		uow.Commit();
		return entity.Id;
	}

	public bool Delete<T>(int id) where T : class, IDomainObject {
		using var uow = uowFactory.Create();
		var entity = uow.GetById<T>(id);
		if(entity == null)
			return true;
		try {
			uow.Delete(entity);
			uow.Commit();
			return true;
		}
		catch(GenericADOException ex) when(ex.InnerException is MySqlException { Number: 1451 or 1217 }) {
			return false;
		}
	}

	public IReadOnlyList<ReferenceItem> References<T>() where T : class, IReferenceRow {
		using var uow = uowFactory.Create();
		return uow.Session.QueryOver<T>()
			.SelectList(list => list
				.Select(x => x.Id)
				.Select(x => x.Name))
			.OrderBy(x => x.Name).Asc
			.List<object[]>()
			.Select(x => new ReferenceItem((int)x[0], (string)x[1]))
			.ToList();
	}

	public IReadOnlyList<AccountRow> Accounts(string text) {
		using var uow = uowFactory.Create();
		Account acc = null!;
		AccountRow row = null!;

		var query = uow.Session.QueryOver(() => acc);
		if(!string.IsNullOrWhiteSpace(text))
			query = query.WhereRestrictionOn(() => acc.Name).IsLike(text, MatchMode.Anywhere);

		return query
			.SelectList(list => list
				.Select(() => acc.Id).WithAlias(() => row.Id)
				.Select(() => acc.Name).WithAlias(() => row.Name)
				.Select(() => acc.TaxRegime).WithAlias(() => row.TaxRegime))
			.OrderBy(() => acc.Name).Asc
			.TransformUsing(Transformers.AliasToBean<AccountRow>())
			.List<AccountRow>().ToList();
	}

	public IReadOnlyList<DivisionRow> Divisions(string text) {
		using var uow = uowFactory.Create();
		Division d = null!;
		Division parent = null!;
		DivisionRow row = null!;

		var query = uow.Session.QueryOver(() => d)
			.JoinEntityAlias(() => parent, () => parent.Id == d.ParentDivisionId, JoinType.LeftOuterJoin);
		if(!string.IsNullOrWhiteSpace(text))
			query = query.WhereRestrictionOn(() => d.Name).IsLike(text, MatchMode.Anywhere);

		return query
			.SelectList(list => list
				.Select(() => d.Id).WithAlias(() => row.Id)
				.Select(() => d.Name).WithAlias(() => row.Name)
				.Select(() => parent.Name).WithAlias(() => row.ParentName))
			.OrderBy(() => d.Name).Asc
			.TransformUsing(Transformers.AliasToBean<DivisionRow>())
			.List<DivisionRow>().ToList();
	}

	public IReadOnlyList<ProjectRow> Projects(string text) {
		using var uow = uowFactory.Create();
		Project pr = null!;
		Division div = null!;
		ProjectRow row = null!;

		var query = uow.Session.QueryOver(() => pr)
			.JoinEntityAlias(() => div, () => div.Id == pr.DivisionId);
		if(!string.IsNullOrWhiteSpace(text))
			query = query.WhereRestrictionOn(() => pr.Name).IsLike(text, MatchMode.Anywhere);

		return query
			.SelectList(list => list
				.Select(() => pr.Id).WithAlias(() => row.Id)
				.Select(() => pr.Name).WithAlias(() => row.Name)
				.Select(() => div.Name).WithAlias(() => row.DivisionName))
			.OrderBy(() => pr.Name).Asc
			.TransformUsing(Transformers.AliasToBean<ProjectRow>())
			.List<ProjectRow>().ToList();
	}

	public IReadOnlyList<IncomeArticle> IncomeArticles(string text) => Articles<IncomeArticle>(text);
	public IReadOnlyList<ExpenseArticle> ExpenseArticles(string text) => Articles<ExpenseArticle>(text);

	IReadOnlyList<T> Articles<T>(string text) where T : class, IReferenceRow {
		using var uow = uowFactory.Create();
		T a = null!;
		var query = uow.Session.QueryOver(() => a);
		if(!string.IsNullOrWhiteSpace(text))
			query = query.WhereRestrictionOn(() => a.Name).IsLike(text, MatchMode.Anywhere);
		return query.OrderBy(() => a.Name).Asc.List().ToList();
	}

	public IReadOnlyList<PlannedIncomeRow> PlannedIncomes(string text) {
		using var uow = uowFactory.Create();
		PlannedIncome pi = null!;
		Account acc = null!;
		Project proj = null!;
		IncomeArticle art = null!;
		PlannedIncomeRow row = null!;

		var query = uow.Session.QueryOver(() => pi)
			.JoinEntityAlias(() => acc, () => acc.Id == pi.AccountId)
			.JoinEntityAlias(() => proj, () => proj.Id == pi.ProjectId)
			.JoinEntityAlias(() => art, () => art.Id == pi.IncomeArticleId);

		if(!string.IsNullOrWhiteSpace(text))
			query = query.Where(Restrictions.Disjunction()
				.Add(Restrictions.Like(Projections.Property(() => pi.Purpose), text, MatchMode.Anywhere))
				.Add(Restrictions.Like(Projections.Property(() => proj.Name), text, MatchMode.Anywhere)));

		return query
			.SelectList(list => list
				.Select(() => pi.Id).WithAlias(() => row.Id)
				.Select(() => pi.Date).WithAlias(() => row.Date)
				.Select(() => pi.Purpose).WithAlias(() => row.Purpose)
				.Select(() => pi.Amount).WithAlias(() => row.Amount)
				.Select(() => pi.VatAmount).WithAlias(() => row.VatAmount)
				.Select(() => acc.Name).WithAlias(() => row.AccountName)
				.Select(() => proj.Name).WithAlias(() => row.ProjectName)
				.Select(() => art.Name).WithAlias(() => row.ArticleName))
			.OrderBy(() => pi.Date).Desc
			.TransformUsing(Transformers.AliasToBean<PlannedIncomeRow>())
			.List<PlannedIncomeRow>().ToList();
	}

	public IReadOnlyList<ActualIncomeRow> ActualIncomes(string text) {
		using var uow = uowFactory.Create();
		ActualIncome ai = null!;
		Account acc = null!;
		Project proj = null!;
		IncomeArticle art = null!;
		ActualIncomeRow row = null!;

		var query = uow.Session.QueryOver(() => ai)
			.JoinEntityAlias(() => acc, () => acc.Id == ai.AccountId)
			.JoinEntityAlias(() => proj, () => proj.Id == ai.ProjectId)
			.JoinEntityAlias(() => art, () => art.Id == ai.IncomeArticleId);

		if(!string.IsNullOrWhiteSpace(text))
			query = query.Where(Restrictions.Disjunction()
				.Add(Restrictions.Like(Projections.Property(() => ai.Purpose), text, MatchMode.Anywhere))
				.Add(Restrictions.Like(Projections.Property(() => proj.Name), text, MatchMode.Anywhere)));

		return query
			.SelectList(list => list
				.Select(() => ai.Id).WithAlias(() => row.Id)
				.Select(() => ai.Date).WithAlias(() => row.Date)
				.Select(() => ai.Purpose).WithAlias(() => row.Purpose)
				.Select(() => ai.Amount).WithAlias(() => row.Amount)
				.Select(() => ai.VatAmount).WithAlias(() => row.VatAmount)
				.Select(() => acc.Name).WithAlias(() => row.AccountName)
				.Select(() => proj.Name).WithAlias(() => row.ProjectName)
				.Select(() => art.Name).WithAlias(() => row.ArticleName))
			.OrderBy(() => ai.Date).Desc
			.TransformUsing(Transformers.AliasToBean<ActualIncomeRow>())
			.List<ActualIncomeRow>().ToList();
	}

	public IReadOnlyList<PlannedExpenseRow> PlannedExpenses(string text) {
		using var uow = uowFactory.Create();
		PlannedExpense pe = null!;
		Account acc = null!;
		Division div = null!;
		Project proj = null!;
		ExpenseArticle art = null!;
		PlannedExpenseRow row = null!;

		var query = uow.Session.QueryOver(() => pe)
			.JoinEntityAlias(() => acc, () => acc.Id == pe.AccountId)
			.JoinEntityAlias(() => div, () => div.Id == pe.DivisionId, JoinType.LeftOuterJoin)
			.JoinEntityAlias(() => proj, () => proj.Id == pe.ProjectId, JoinType.LeftOuterJoin)
			.JoinEntityAlias(() => art, () => art.Id == pe.ExpenseArticleId);

		if(!string.IsNullOrWhiteSpace(text))
			query = query.Where(Restrictions.Disjunction()
				.Add(Restrictions.Like(Projections.Property(() => pe.Purpose), text, MatchMode.Anywhere))
				.Add(Restrictions.Like(Projections.Property(() => proj.Name), text, MatchMode.Anywhere)));

		return query
			.SelectList(list => list
				.Select(() => pe.Id).WithAlias(() => row.Id)
				.Select(() => pe.Date).WithAlias(() => row.Date)
				.Select(() => pe.Purpose).WithAlias(() => row.Purpose)
				.Select(() => pe.Amount).WithAlias(() => row.Amount)
				.Select(() => pe.VatAmount).WithAlias(() => row.VatAmount)
				.Select(() => acc.Name).WithAlias(() => row.AccountName)
				.Select(() => div.Name).WithAlias(() => row.DivisionName)
				.Select(() => proj.Name).WithAlias(() => row.ProjectName)
				.Select(() => art.Name).WithAlias(() => row.ArticleName))
			.OrderBy(() => pe.Date).Desc
			.TransformUsing(Transformers.AliasToBean<PlannedExpenseRow>())
			.List<PlannedExpenseRow>().ToList();
	}

	public IReadOnlyList<ActualExpenseRow> ActualExpenses(string text) {
		using var uow = uowFactory.Create();
		ActualExpense ae = null!;
		Account acc = null!;
		Division div = null!;
		Project proj = null!;
		ExpenseArticle art = null!;
		ActualExpenseRow row = null!;

		var query = uow.Session.QueryOver(() => ae)
			.JoinEntityAlias(() => acc, () => acc.Id == ae.AccountId)
			.JoinEntityAlias(() => div, () => div.Id == ae.DivisionId, JoinType.LeftOuterJoin)
			.JoinEntityAlias(() => proj, () => proj.Id == ae.ProjectId, JoinType.LeftOuterJoin)
			.JoinEntityAlias(() => art, () => art.Id == ae.ExpenseArticleId);

		if(!string.IsNullOrWhiteSpace(text))
			query = query.Where(Restrictions.Disjunction()
				.Add(Restrictions.Like(Projections.Property(() => ae.Purpose), text, MatchMode.Anywhere))
				.Add(Restrictions.Like(Projections.Property(() => proj.Name), text, MatchMode.Anywhere)));

		return query
			.SelectList(list => list
				.Select(() => ae.Id).WithAlias(() => row.Id)
				.Select(() => ae.Date).WithAlias(() => row.Date)
				.Select(() => ae.Purpose).WithAlias(() => row.Purpose)
				.Select(() => ae.Amount).WithAlias(() => row.Amount)
				.Select(() => ae.VatAmount).WithAlias(() => row.VatAmount)
				.Select(() => acc.Name).WithAlias(() => row.AccountName)
				.Select(() => div.Name).WithAlias(() => row.DivisionName)
				.Select(() => proj.Name).WithAlias(() => row.ProjectName)
				.Select(() => art.Name).WithAlias(() => row.ArticleName))
			.OrderBy(() => ae.Date).Desc
			.TransformUsing(Transformers.AliasToBean<ActualExpenseRow>())
			.List<ActualExpenseRow>().ToList();
	}

	public IReadOnlyList<AccrualTemplateRow> AccrualTemplates(string text) {
		using var uow = uowFactory.Create();
		AccrualTemplate t = null!;
		Account acc = null!;
		Project proj = null!;
		IncomeArticle art = null!;
		AccrualTemplateRow row = null!;

		var query = uow.Session.QueryOver(() => t)
			.JoinEntityAlias(() => acc, () => acc.Id == t.AccountId)
			.JoinEntityAlias(() => proj, () => proj.Id == t.ProjectId)
			.JoinEntityAlias(() => art, () => art.Id == t.IncomeArticleId);

		if(!string.IsNullOrWhiteSpace(text))
			query = query.WhereRestrictionOn(() => t.Purpose).IsLike(text, MatchMode.Anywhere);

		return query
			.SelectList(list => list
				.Select(() => t.Id).WithAlias(() => row.Id)
				.Select(() => t.Purpose).WithAlias(() => row.Purpose)
				.Select(() => t.Amount).WithAlias(() => row.Amount)
				.Select(() => t.VatAmount).WithAlias(() => row.VatAmount)
				.Select(() => acc.Name).WithAlias(() => row.AccountName)
				.Select(() => proj.Name).WithAlias(() => row.ProjectName)
				.Select(() => art.Name).WithAlias(() => row.ArticleName))
			.OrderBy(() => t.Purpose).Asc
			.TransformUsing(Transformers.AliasToBean<AccrualTemplateRow>())
			.List<AccrualTemplateRow>().ToList();
	}

	public IReadOnlyList<PaymentTemplateRow> PaymentTemplates(string text) {
		using var uow = uowFactory.Create();
		PaymentTemplate t = null!;
		Account acc = null!;
		Division div = null!;
		Project proj = null!;
		ExpenseArticle art = null!;
		PaymentTemplateRow row = null!;

		var query = uow.Session.QueryOver(() => t)
			.JoinEntityAlias(() => acc, () => acc.Id == t.AccountId)
			.JoinEntityAlias(() => div, () => div.Id == t.DivisionId, JoinType.LeftOuterJoin)
			.JoinEntityAlias(() => proj, () => proj.Id == t.ProjectId, JoinType.LeftOuterJoin)
			.JoinEntityAlias(() => art, () => art.Id == t.ExpenseArticleId);

		if(!string.IsNullOrWhiteSpace(text))
			query = query.WhereRestrictionOn(() => t.Purpose).IsLike(text, MatchMode.Anywhere);

		return query
			.SelectList(list => list
				.Select(() => t.Id).WithAlias(() => row.Id)
				.Select(() => t.Purpose).WithAlias(() => row.Purpose)
				.Select(() => t.Amount).WithAlias(() => row.Amount)
				.Select(() => t.VatAmount).WithAlias(() => row.VatAmount)
				.Select(() => acc.Name).WithAlias(() => row.AccountName)
				.Select(() => div.Name).WithAlias(() => row.DivisionName)
				.Select(() => proj.Name).WithAlias(() => row.ProjectName)
				.Select(() => art.Name).WithAlias(() => row.ArticleName))
			.OrderBy(() => t.Purpose).Asc
			.TransformUsing(Transformers.AliasToBean<PaymentTemplateRow>())
			.List<PaymentTemplateRow>().ToList();
	}
}
