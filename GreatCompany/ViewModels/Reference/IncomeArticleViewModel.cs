using GreatCompany.Data.Models;
using QS.Project.Domain;

namespace GreatCompany.ViewModels.Reference;

public class IncomeArticleViewModel : CardViewModelBase<IncomeArticle> {
	public IncomeArticleViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
	}
}
