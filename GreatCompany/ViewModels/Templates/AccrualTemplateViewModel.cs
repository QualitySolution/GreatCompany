using GreatCompany.Controls;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Navigation;
using QS.Navigation;

namespace GreatCompany.ViewModels.Templates;

public class AccrualTemplateViewModel : FormViewModelBase {
	readonly Repository repo;

	public AccrualTemplateViewModel(int id, Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Entity = id == 0 ? new AccrualTemplate() : repo.Get<AccrualTemplate>(id) ?? new AccrualTemplate();
		Title = id == 0 ? "Новый шаблон начисления" : Entity.Purpose;

		AccountPicker = new ReferencePickerViewModel(repo.References<Account>(), onChosen => NavigationManager.OpenReferenceSelect<AccountJournalViewModel>(this, onChosen));
		ProjectPicker = new ReferencePickerViewModel(repo.References<Project>(), onChosen => NavigationManager.OpenReferenceSelect<ProjectJournalViewModel>(this, onChosen));
		ArticlePicker = new ReferencePickerViewModel(repo.References<IncomeArticle>(), onChosen => NavigationManager.OpenReferenceSelect<IncomeArticleJournalViewModel>(this, onChosen));

		AccountPicker.SelectById(Entity.AccountId);
		ProjectPicker.SelectById(Entity.ProjectId);
		ArticlePicker.SelectById(Entity.IncomeArticleId);
	}

	public AccrualTemplate Entity { get; }
	public ReferencePickerViewModel AccountPicker { get; }
	public ReferencePickerViewModel ProjectPicker { get; }
	public ReferencePickerViewModel ArticlePicker { get; }

	protected override bool Save() {
		if(string.IsNullOrWhiteSpace(Entity.Purpose)
			|| AccountPicker.Selected == null || ProjectPicker.Selected == null || ArticlePicker.Selected == null)
			return false;

		Entity.AccountId = AccountPicker.Selected.Id;
		Entity.ProjectId = ProjectPicker.Selected.Id;
		Entity.IncomeArticleId = ArticlePicker.Selected.Id;
		repo.Save(Entity);
		return true;
	}
}
