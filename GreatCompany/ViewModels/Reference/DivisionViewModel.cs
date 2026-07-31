using GreatCompany.Controls;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Navigation;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Validation;

namespace GreatCompany.ViewModels.Reference;

public class DivisionViewModel : FormViewModelBase {
	readonly Repository repo;

	public DivisionViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new Division() : repo.Get<Division>(id) ?? new Division();
		Title = Entity.Id == 0 ? "Новое подразделение" : Entity.Name;

		ParentPicker = new ReferencePickerViewModel(repo.References<Division>(), onChosen => NavigationManager.OpenReferenceSelect<DivisionJournalViewModel>(this, onChosen));
		if(Entity.ParentDivisionId is int parentId)
			ParentPicker.SelectById(parentId);

		TrackChanges(Entity, ParentPicker);
	}

	public Division Entity { get; }
	public ReferencePickerViewModel ParentPicker { get; }

	protected override IDomainObject? SaveEntity() {
		Entity.ParentDivisionId = ParentPicker.Selected?.Id;
		if(!Validate(Entity))
			return null;
		repo.Save(Entity);
		return Entity;
	}
}
