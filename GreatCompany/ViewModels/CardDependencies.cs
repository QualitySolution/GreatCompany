using Autofac;
using QS.Dialog;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.Validation;

namespace GreatCompany.ViewModels;

// зависимости, одинаковые у всех карточек
public record CardDependencies(
	IUnitOfWork UnitOfWork,
	INavigationManager Navigation,
	IValidator Validator,
	IInteractiveMessage Interactive,
	IEntityChangeWatcher ChangeWatcher,
	ILifetimeScope Scope);
