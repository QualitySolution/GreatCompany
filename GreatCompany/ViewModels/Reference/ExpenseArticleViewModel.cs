using GreatCompany.Data.Models;
using QS.Project.Domain;

namespace GreatCompany.ViewModels.Reference;

public class ExpenseArticleViewModel : CardViewModelBase<ExpenseArticle> {
	public ExpenseArticleViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
	}
}
