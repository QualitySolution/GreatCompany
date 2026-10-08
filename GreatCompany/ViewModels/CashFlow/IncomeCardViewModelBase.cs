using GreatCompany.Data.Models;
using GreatCompany.Data.Taxes;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.ViewModels.Reference;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.CashFlow;

/// <summary>карточка прихода, у шаблона начисления, плана и факта одни и те же поля выбора</summary>
public abstract class IncomeCardViewModelBase<TEntity> : CardViewModelBase<TEntity>
	where TEntity : IncomeDocumentBase, new() {

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

		DivisionEntry = builder.ForProperty(x => x.Division)
			.UseViewModelJournalAndAutocompleter<DivisionJournalViewModel>()
			.UseViewModelDialog<DivisionViewModel>()
			.Finish();

		IncomeArticleEntry = builder.ForProperty(x => x.IncomeArticle)
			.UseViewModelJournalAndAutocompleter<IncomeArticleJournalViewModel>()
			.UseViewModelDialog<IncomeArticleViewModel>()
			.Finish();

		// статья подсказывает, облагается ли документ НДС, галочку потом можно поменять
		Entity.PropertyChanged += (_, e) => {
			if(e.PropertyName == nameof(Entity.IncomeArticle) && Entity.IncomeArticle != null)
				ApplyWithoutVat(Entity.IncomeArticle.WithoutVat);
		};
	}

	public IEntityEntryViewModel AccountEntry { get; }
	public IEntityEntryViewModel ProjectEntry { get; }
	public IEntityEntryViewModel DivisionEntry { get; }
	public IEntityEntryViewModel IncomeArticleEntry { get; }

	/// <summary>
	/// заполняет карточку по шаблону начисления.
	/// 0 - обычное создание с нуля
	/// </summary>
	protected AccrualTemplate? FillFromTemplate(int templateId) {
		if(templateId == 0)
			return null;

		var template = UoW.GetById<AccrualTemplate>(templateId)
			?? throw new AbortCreatingPageException(
				$"Шаблон начисления №{templateId} не найден, возможно его удалили.", "Не удалось создать по шаблону");

		Entity.FillFrom(template);
		// шаблон без НДС (0) даёт документ без НДС, иначе НДС пересчитывается на дату документа
		ApplyWithoutVat(TaxCalculator.HasVat(template.Account.TaxRegime) && template.Vat == 0);
		return template;
	}
}
