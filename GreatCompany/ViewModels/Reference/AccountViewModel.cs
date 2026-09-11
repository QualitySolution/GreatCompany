using GreatCompany.Data.Models;
using QS.Project.Domain;

namespace GreatCompany.ViewModels.Reference;

public class AccountViewModel : CardViewModelBase<Account> {
	public AccountViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
	}

	// Карточка биндится на Entity.TaxRegime напрямую, здесь только список
	public IReadOnlyList<TaxRegime> TaxRegimeOptions { get; } = Enum.GetValues<TaxRegime>();
}
