using GreatCompany.Controls;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Navigation;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Validation;

namespace GreatCompany.ViewModels.Templates;

public class AccrualTemplateViewModel : FormViewModelBase {
	readonly Repository repo;

	public AccrualTemplateViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new AccrualTemplate() : repo.Get<AccrualTemplate>(id) ?? new AccrualTemplate();
		Title = Entity.Id == 0 ? "Новый шаблон начисления" : Entity.Purpose;

		AccountPicker = new ReferencePickerViewModel(repo.References<Account>(), onChosen => NavigationManager.OpenReferenceSelect<AccountJournalViewModel>(this, onChosen));
		ProjectPicker = new ReferencePickerViewModel(repo.References<Project>(), onChosen => NavigationManager.OpenReferenceSelect<ProjectJournalViewModel>(this, onChosen));
		ArticlePicker = new ReferencePickerViewModel(repo.References<IncomeArticle>(), onChosen => NavigationManager.OpenReferenceSelect<IncomeArticleJournalViewModel>(this, onChosen));

		AccountPicker.SelectById(Entity.AccountId);
		ProjectPicker.SelectById(Entity.ProjectId);
		ArticlePicker.SelectById(Entity.IncomeArticleId);

		TrackChanges(Entity, AccountPicker, ProjectPicker, ArticlePicker);
	}

	public AccrualTemplate Entity { get; }
	public ReferencePickerViewModel AccountPicker { get; }
	public ReferencePickerViewModel ProjectPicker { get; }
	public ReferencePickerViewModel ArticlePicker { get; }

	protected override IDomainObject? SaveEntity() {
		Entity.AccountId = AccountPicker.Selected?.Id ?? 0;
		Entity.ProjectId = ProjectPicker.Selected?.Id ?? 0;
		Entity.IncomeArticleId = ArticlePicker.Selected?.Id ?? 0;
		if(!Validate(Entity))
			return null;
		repo.Save(Entity);
		return Entity;
	}
}
