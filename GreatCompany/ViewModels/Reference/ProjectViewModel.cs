using GreatCompany.Controls;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Navigation;
using QS.Navigation;

namespace GreatCompany.ViewModels.Reference;

public class ProjectViewModel : FormViewModelBase {
	readonly Repository repo;

	public ProjectViewModel(int id, Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Entity = id == 0 ? new Project() : repo.Get<Project>(id) ?? new Project();
		Title = id == 0 ? "Новый проект" : Entity.Name;

		DivisionPicker = new ReferencePickerViewModel(repo.References<Division>(), onChosen => NavigationManager.OpenReferenceSelect<DivisionJournalViewModel>(this, onChosen));
		DivisionPicker.SelectById(Entity.DivisionId);
	}

	public Project Entity { get; }
	public ReferencePickerViewModel DivisionPicker { get; }

	protected override bool Save() {
		if(string.IsNullOrWhiteSpace(Entity.Name) || DivisionPicker.Selected == null)
			return false;
		Entity.DivisionId = DivisionPicker.Selected.Id;
		repo.Save(Entity);
		return true;
	}
}
