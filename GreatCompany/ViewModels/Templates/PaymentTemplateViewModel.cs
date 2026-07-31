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

public class PaymentTemplateViewModel : FormViewModelBase {
	readonly Repository repo;

	public PaymentTemplateViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new PaymentTemplate() : repo.Get<PaymentTemplate>(id) ?? new PaymentTemplate();
		Title = Entity.Id == 0 ? "Новый шаблон платежа" : Entity.Purpose;

		AccountPicker = new ReferencePickerViewModel(repo.References<Account>(), onChosen => NavigationManager.OpenReferenceSelect<AccountJournalViewModel>(this, onChosen));
		ArticlePicker = new ReferencePickerViewModel(repo.References<ExpenseArticle>(), onChosen => NavigationManager.OpenReferenceSelect<ExpenseArticleJournalViewModel>(this, onChosen));
		ProjectPicker = new ReferencePickerViewModel(repo.References<Project>(), onChosen => NavigationManager.OpenReferenceSelect<ProjectJournalViewModel>(this, onChosen));
		DivisionPicker = new ReferencePickerViewModel(repo.References<Division>(), onChosen => NavigationManager.OpenReferenceSelect<DivisionJournalViewModel>(this, onChosen));

		AccountPicker.SelectById(Entity.AccountId);
		ArticlePicker.SelectById(Entity.ExpenseArticleId);
		if(Entity.ProjectId is int pid) ProjectPicker.SelectById(pid);
		if(Entity.DivisionId is int did) DivisionPicker.SelectById(did);

		TrackChanges(Entity, AccountPicker, ArticlePicker, ProjectPicker, DivisionPicker);
	}

	public PaymentTemplate Entity { get; }
	public ReferencePickerViewModel AccountPicker { get; }
	public ReferencePickerViewModel ArticlePicker { get; }
	public ReferencePickerViewModel ProjectPicker { get; }
	public ReferencePickerViewModel DivisionPicker { get; }

	protected override IDomainObject? SaveEntity() {
		Entity.AccountId = AccountPicker.Selected?.Id ?? 0;
		Entity.ExpenseArticleId = ArticlePicker.Selected?.Id ?? 0;
		if(ProjectPicker.Selected != null) {
			Entity.ProjectId = ProjectPicker.Selected.Id;
			Entity.DivisionId = repo.Get<Project>(ProjectPicker.Selected.Id)?.DivisionId;
		} else {
			Entity.ProjectId = null;
			Entity.DivisionId = DivisionPicker.Selected?.Id;
		}
		if(!Validate(Entity))
			return null;
		repo.Save(Entity);
		return Entity;
	}
}
