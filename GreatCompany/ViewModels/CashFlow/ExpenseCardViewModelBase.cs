using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.ViewModels.Reference;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.CashFlow;

/// <summary>карточка расхода, у шаблона платежа, плана и факта одни и те же поля выбора</summary>
public abstract class ExpenseCardViewModelBase<TEntity> : CardViewModelBase<TEntity>
	where TEntity : ExpenseDocument, new() {

	protected ExpenseCardViewModelBase(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
		var builder = new CommonEEVMBuilderFactory<TEntity>(this, Entity, UoW, deps.Navigation, deps.Scope);

		AccountEntry = builder.ForProperty(x => x.Account)
			.UseViewModelJournalAndAutocompleter<AccountJournalViewModel>()
			.UseViewModelDialog<AccountViewModel>()
			.Finish();

		ExpenseArticleEntry = builder.ForProperty(x => x.ExpenseArticle)
			.UseViewModelJournalAndAutocompleter<ExpenseArticleJournalViewModel>()
			.UseViewModelDialog<ExpenseArticleViewModel>()
			.Finish();

		ProjectEntry = builder.ForProperty(x => x.Project)
			.UseViewModelJournalAndAutocompleter<ProjectJournalViewModel>()
			.UseViewModelDialog<ProjectViewModel>()
			.Finish();

		DivisionEntry = builder.ForProperty(x => x.Division)
			.UseViewModelJournalAndAutocompleter<DivisionJournalViewModel>()
			.UseViewModelDialog<DivisionViewModel>()
			.Finish();
	}

	public IEntityEntryViewModel AccountEntry { get; }
	public IEntityEntryViewModel ExpenseArticleEntry { get; }
	public IEntityEntryViewModel ProjectEntry { get; }
	public IEntityEntryViewModel DivisionEntry { get; }

	/// <summary>
	/// заполняет карточку по шаблону платежа.
	/// 0 - обычное создание с нуля
	/// </summary>
	protected void FillFromTemplate(int templateId) {
		if(templateId == 0)
			return;

		var template = UoW.GetById<PaymentTemplate>(templateId)
			?? throw new AbortCreatingPageException(
				$"Шаблон платежа №{templateId} не найден, возможно его удалили.", "Не удалось создать по шаблону");

		Entity.FillFrom(template);
	}
}
