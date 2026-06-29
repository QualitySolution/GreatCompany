using GreatCompany.Controls;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Navigation;
using QS.Navigation;

namespace GreatCompany.ViewModels.Reference;

public class DivisionViewModel : FormViewModelBase {
	readonly Repository repo;

	public DivisionViewModel(int id, Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Entity = id == 0 ? new Division() : repo.Get<Division>(id) ?? new Division();
		Title = id == 0 ? "Новый дивизион" : Entity.Name;

		ParentPicker = new ReferencePickerViewModel(repo.References<Division>(), onChosen => NavigationManager.OpenReferenceSelect<DivisionJournalViewModel>(this, onChosen));
		if(Entity.ParentDivisionId is int parentId)
			ParentPicker.SelectById(parentId);
	}

	public Division Entity { get; }
	public ReferencePickerViewModel ParentPicker { get; }

	protected override bool Save() {
		if(string.IsNullOrWhiteSpace(Entity.Name))
			return false;
		Entity.ParentDivisionId = ParentPicker.Selected?.Id;
		repo.Save(Entity);
		return true;
	}
}
