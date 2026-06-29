using GreatCompany.Controls;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Navigation;
using QS.Navigation;

namespace GreatCompany.ViewModels.CashFlow;

public class ActualIncomeViewModel : FormViewModelBase {
	readonly Repository repo;

	public ActualIncomeViewModel(int id, Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Entity = id == 0 ? new ActualIncome() : repo.Get<ActualIncome>(id) ?? new ActualIncome();
		Title = id == 0 ? "Новый факт прихода" : $"Факт прихода №{Entity.Id}";

		AccountPicker = new ReferencePickerViewModel(repo.References<Account>(), onChosen => NavigationManager.OpenReferenceSelect<AccountJournalViewModel>(this, onChosen));
		ProjectPicker = new ReferencePickerViewModel(repo.References<Project>(), onChosen => NavigationManager.OpenReferenceSelect<ProjectJournalViewModel>(this, onChosen));
		ArticlePicker = new ReferencePickerViewModel(repo.References<IncomeArticle>(), onChosen => NavigationManager.OpenReferenceSelect<IncomeArticleJournalViewModel>(this, onChosen));

		AccountPicker.SelectById(Entity.AccountId);
		ProjectPicker.SelectById(Entity.ProjectId);
		ArticlePicker.SelectById(Entity.IncomeArticleId);
	}

	public ActualIncome Entity { get; }
	public ReferencePickerViewModel AccountPicker { get; }
	public ReferencePickerViewModel ProjectPicker { get; }
	public ReferencePickerViewModel ArticlePicker { get; }

	public void ApplyTemplate(int templateId) {
		var t = repo.Get<AccrualTemplate>(templateId);
		if(t == null)
			return;
		Entity.Purpose = t.Purpose;
		Entity.Amount = t.Amount;
		Entity.VatAmount = t.VatAmount;
		Entity.AccountId = t.AccountId;
		Entity.ProjectId = t.ProjectId;
		Entity.IncomeArticleId = t.IncomeArticleId;
		AccountPicker.SelectById(t.AccountId);
		ProjectPicker.SelectById(t.ProjectId);
		ArticlePicker.SelectById(t.IncomeArticleId);
	}

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
