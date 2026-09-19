using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.ViewModels.Reference;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.CashFlow;

/// <summary>карточка прихода, у шаблона начисления, плана и факта одни и те же поля выбора</summary>
public abstract class IncomeCardViewModelBase<TEntity> : CardViewModelBase<TEntity>
	where TEntity : IncomeDocument, new() {

	protected IncomeCardViewModelBase(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
		var builder = new CommonEEVMBuilderFactory<TEntity>(this, Entity, UoW, deps.Navigation, deps.Scope);

		AccountEntry = builder.ForProperty(x => x.Account)
			.UseViewModelJournalAndAutocompleter<AccountJournalViewModel>()
			.UseViewModelDialog<AccountViewModel>()
			.Finish();

		ProjectEntry = builder.ForProperty(x => x.Project)
			.UseViewModelJournalAndAutocompleter<ProjectJournalViewModel>()
			.UseViewModelDialog<ProjectViewModel>()
			.Finish();

		IncomeArticleEntry = builder.ForProperty(x => x.IncomeArticle)
			.UseViewModelJournalAndAutocompleter<IncomeArticleJournalViewModel>()
			.UseViewModelDialog<IncomeArticleViewModel>()
			.Finish();
	}

	public IEntityEntryViewModel AccountEntry { get; }
	public IEntityEntryViewModel ProjectEntry { get; }
	public IEntityEntryViewModel IncomeArticleEntry { get; }

	/// <summary>
	/// заполняет карточку по шаблону начисления.
	/// 0 - обычное создание с нуля
	/// </summary>
	protected void FillFromTemplate(int templateId) {
		if(templateId == 0)
			return;

		var template = UoW.GetById<AccrualTemplate>(templateId)
			?? throw new AbortCreatingPageException(
				$"Шаблон начисления №{templateId} не найден, возможно его удалили.", "Не удалось создать по шаблону");

		Entity.FillFrom(template);
	}
}
