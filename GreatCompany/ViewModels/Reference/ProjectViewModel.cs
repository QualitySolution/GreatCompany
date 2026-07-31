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

public class ProjectViewModel : FormViewModelBase {
	readonly Repository repo;

	public ProjectViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new Project() : repo.Get<Project>(id) ?? new Project();
		Title = Entity.Id == 0 ? "Новый проект" : Entity.Name;

		DivisionPicker = new ReferencePickerViewModel(repo.References<Division>(), onChosen => NavigationManager.OpenReferenceSelect<DivisionJournalViewModel>(this, onChosen));
		DivisionPicker.SelectById(Entity.DivisionId);

		TrackChanges(Entity, DivisionPicker);
	}

	public Project Entity { get; }
	public ReferencePickerViewModel DivisionPicker { get; }

	protected override IDomainObject? SaveEntity() {
		Entity.DivisionId = DivisionPicker.Selected?.Id ?? 0;
		if(!Validate(Entity))
			return null;
		repo.Save(Entity);
		return Entity;
	}
}
