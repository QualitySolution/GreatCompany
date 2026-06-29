using GreatCompany.Data;
using GreatCompany.Data.Models;
using QS.Navigation;

namespace GreatCompany.ViewModels.Reference;

public class AccountViewModel : FormViewModelBase {
	readonly Repository repo;

	public AccountViewModel(int id, Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Entity = id == 0 ? new Account() : repo.Get<Account>(id) ?? new Account();
		Title = id == 0 ? "Новый счёт" : Entity.Name;
		selectedTaxRegime = TaxRegimes.All.First(o => o.Value == Entity.TaxRegime);
	}

	public Account Entity { get; }

	public IReadOnlyList<TaxRegimeOption> TaxRegimeOptions => TaxRegimes.All;

	TaxRegimeOption selectedTaxRegime;
	public TaxRegimeOption SelectedTaxRegime {
		get => selectedTaxRegime;
		set => SetField(ref selectedTaxRegime, value);
	}

	protected override bool Save() {
		if(string.IsNullOrWhiteSpace(Entity.Name))
			return false;
		Entity.TaxRegime = SelectedTaxRegime.Value;
		repo.Save(Entity);
		return true;
	}
}
