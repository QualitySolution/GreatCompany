using GreatCompany.Data;
using GreatCompany.Data.Models;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Validation;

namespace GreatCompany.ViewModels.Reference;

public class IncomeArticleViewModel : FormViewModelBase {
	readonly Repository repo;

	public IncomeArticleViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new IncomeArticle() : repo.Get<IncomeArticle>(id) ?? new IncomeArticle();
		Title = Entity.Id == 0 ? "Новая статья дохода" : Entity.Name;

		TrackChanges(Entity);
	}

	public IncomeArticle Entity { get; }

	protected override IDomainObject? SaveEntity() {
		if(!Validate(Entity))
			return null;
		repo.Save(Entity);
		return Entity;
	}
}
