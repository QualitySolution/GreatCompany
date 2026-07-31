using GreatCompany.Controls;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Navigation;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Validation;

namespace GreatCompany.ViewModels.CashFlow;

public class PlannedIncomeViewModel : FormViewModelBase {
	readonly Repository repo;

	public PlannedIncomeViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new PlannedIncome() : repo.Get<PlannedIncome>(id) ?? new PlannedIncome();
		Title = Entity.Id == 0 ? "Новый план прихода" : $"План прихода №{Entity.Id}";

		AccountPicker = new ReferencePickerViewModel(repo.References<Account>(), onChosen => NavigationManager.OpenReferenceSelect<AccountJournalViewModel>(this, onChosen));
		ProjectPicker = new ReferencePickerViewModel(repo.References<Project>(), onChosen => NavigationManager.OpenReferenceSelect<ProjectJournalViewModel>(this, onChosen));
		ArticlePicker = new ReferencePickerViewModel(repo.References<IncomeArticle>(), onChosen => NavigationManager.OpenReferenceSelect<IncomeArticleJournalViewModel>(this, onChosen));

		AccountPicker.SelectById(Entity.AccountId);
		ProjectPicker.SelectById(Entity.ProjectId);
		ArticlePicker.SelectById(Entity.IncomeArticleId);

		TrackChanges(Entity, AccountPicker, ProjectPicker, ArticlePicker);
	}

	public PlannedIncome Entity { get; }
	public ReferencePickerViewModel AccountPicker { get; }
	public ReferencePickerViewModel ProjectPicker { get; }
	public ReferencePickerViewModel ArticlePicker { get; }

	// Заполняет новую запись по шаблону начисления
	// Вызывается до показа карточки
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
