using GreatCompany.Data.Models;
using QS.Deletion.Configuration;

namespace GreatCompany.Data;

/// <summary>
/// обязательная ссылка означает удаление зависимого документа, необязательная - очистку ссылки.
/// расход всегда принадлежит подразделению и удаляется вместе с ним.
/// проект перечислен раньше документов, план раньше факта - ссылка у документа очищается до его удаления
/// </summary>
public static class DeletionConfiguration {
	public static void ConfigureDeletion(DeleteConfiguration configuration) {
		configuration.AddHibernateDeleteInfo<Division>()
			.AddDeleteDependence<Project>(x => x.Division)
			.AddClearDependence<Division>(x => x.ParentDivision)
			.AddDeleteDependence<PlannedExpense>(x => x.Division)
			.AddDeleteDependence<ActualExpense>(x => x.Division)
			.AddDeleteDependence<PaymentTemplate>(x => x.Division);

		configuration.AddHibernateDeleteInfo<Project>()
			.AddDeleteDependence<PlannedIncome>(x => x.Project)
			.AddDeleteDependence<ActualIncome>(x => x.Project)
			.AddDeleteDependence<AccrualTemplate>(x => x.Project)
			.AddClearDependence<PlannedExpense>(x => x.Project)
			.AddClearDependence<ActualExpense>(x => x.Project)
			.AddClearDependence<PaymentTemplate>(x => x.Project);

		configuration.AddHibernateDeleteInfo<Account>()
			.AddDeleteDependence<PlannedIncome>(x => x.Account)
			.AddDeleteDependence<ActualIncome>(x => x.Account)
			.AddDeleteDependence<PlannedExpense>(x => x.Account)
			.AddDeleteDependence<ActualExpense>(x => x.Account)
			.AddDeleteDependence<AccrualTemplate>(x => x.Account)
			.AddDeleteDependence<PaymentTemplate>(x => x.Account);

		configuration.AddHibernateDeleteInfo<IncomeArticle>()
			.AddDeleteDependence<PlannedIncome>(x => x.IncomeArticle)
			.AddDeleteDependence<ActualIncome>(x => x.IncomeArticle)
			.AddDeleteDependence<AccrualTemplate>(x => x.IncomeArticle);

		configuration.AddHibernateDeleteInfo<ExpenseArticle>()
			.AddDeleteDependence<PlannedExpense>(x => x.ExpenseArticle)
			.AddDeleteDependence<ActualExpense>(x => x.ExpenseArticle)
			.AddDeleteDependence<PaymentTemplate>(x => x.ExpenseArticle);

		// факт живёт своей жизнью, удаление плана только рвёт связь
		configuration.AddHibernateDeleteInfo<PlannedIncome>()
			.AddClearDependence<ActualIncome>(x => x.PlannedIncome);

		configuration.AddHibernateDeleteInfo<PlannedExpense>()
			.AddClearDependence<ActualExpense>(x => x.PlannedExpense);

		configuration.AddHibernateDeleteInfo<ActualIncome>();
		configuration.AddHibernateDeleteInfo<ActualExpense>();
		configuration.AddHibernateDeleteInfo<AccrualTemplate>();
		configuration.AddHibernateDeleteInfo<PaymentTemplate>();
	}
}
