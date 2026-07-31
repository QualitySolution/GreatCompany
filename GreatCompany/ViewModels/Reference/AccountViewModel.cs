using GreatCompany.Data;
using GreatCompany.Data.Models;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Validation;

namespace GreatCompany.ViewModels.Reference;

public class AccountViewModel : FormViewModelBase {
	readonly Repository repo;

	public AccountViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new Account() : repo.Get<Account>(id) ?? new Account();
		Title = Entity.Id == 0 ? "Новый счёт" : Entity.Name;
		selectedTaxRegime = TaxRegimes.All.First(o => o.Value == Entity.TaxRegime);

		// this — потому что выбранный налоговый режим живет на VM (SelectedTaxRegime), а не на сущности
		TrackChanges(Entity, this);
	}

	public Account Entity { get; }

	public IReadOnlyList<TaxRegimeOption> TaxRegimeOptions => TaxRegimes.All;

	TaxRegimeOption selectedTaxRegime;
	public TaxRegimeOption SelectedTaxRegime {
		get => selectedTaxRegime;
		set => SetField(ref selectedTaxRegime, value);
	}

	protected override IDomainObject? SaveEntity() {
		Entity.TaxRegime = SelectedTaxRegime.Value;
		if(!Validate(Entity))
			return null;
		repo.Save(Entity);
		return Entity;
	}
}
