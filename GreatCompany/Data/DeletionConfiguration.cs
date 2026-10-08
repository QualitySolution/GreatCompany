using GreatCompany.Data.Models;
using QS.Deletion.Configuration;

namespace GreatCompany.Data;

/// <summary>
/// обязательная ссылка означает удаление зависимого документа, необязательная - очистку ссылки.
/// приход и расход всегда принадлежат подразделению и удаляются вместе с ним.
/// проект перечислен раньше документов, план раньше факта - ссылка у документа очищается до его удаления
/// </summary>
public static class DeletionConfiguration {
	public static void ConfigureDeletion(DeleteConfiguration configuration) {
		configuration.AddHibernateDeleteInfo<Division>()
			.AddDeleteDependence<Project>(x => x.Division)
			.AddClearDependence<Division>(x => x.ParentDivision)
			.AddDeleteDependence<PlannedIncome>(x => x.Division)
			.AddDeleteDependence<ActualIncome>(x => x.Division)
			.AddDeleteDependence<AccrualTemplate>(x => x.Division)
			.AddDeleteDependence<PlannedExpense>(x => x.Division)
			.AddDeleteDependence<ActualExpense>(x => x.Division)
			.AddDeleteDependence<PaymentTemplate>(x => x.Division);

		configuration.AddHibernateDeleteInfo<Project>()
			.AddClearDependence<PlannedIncome>(x => x.Project)
			.AddClearDependence<ActualIncome>(x => x.Project)
			.AddClearDependence<AccrualTemplate>(x => x.Project)
			.AddClearDependence<PlannedExpense>(x => x.Project)
			.AddClearDependence<ActualExpense>(x => x.Project)
			.AddClearDependence<PaymentTemplate>(x => x.Project);

		configuration.AddHibernateDeleteInfo<Account>()
			.AddDeleteDependence<PlannedIncome>(x => x.Account)
			.AddDeleteDependence<ActualIncome>(x => x.Account)
			.AddDeleteDependence<PlannedExpense>(x => x.Account)
			.AddDeleteDependence<ActualExpense>(x => x.Account)
			.AddDeleteDependence<ActualTransfer>(x => x.FromAccount)
			.AddDeleteDependence<ActualTransfer>(x => x.ToAccount)
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
		configuration.AddHibernateDeleteInfo<ActualTransfer>();
		// созданные по шаблону планы остаются, теряют только ссылку на него
		configuration.AddHibernateDeleteInfo<AccrualTemplate>()
			.AddClearDependence<PlannedIncome>(x => x.AccrualTemplate);
		configuration.AddHibernateDeleteInfo<PaymentTemplate>()
			.AddClearDependence<PlannedExpense>(x => x.PaymentTemplate);
	}
}
