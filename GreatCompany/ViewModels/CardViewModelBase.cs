using System.ComponentModel;
using System.Reactive;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Project.Domain;
using QS.ViewModels.Dialog;
using ReactiveUI;

namespace GreatCompany.ViewModels;

public abstract class CardViewModelBase<TEntity> : EntityDialogViewModelBase<TEntity>
	where TEntity : class, IDomainObject, new() {
	protected CardDependencies Deps { get; }

	protected CardViewModelBase(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps.UnitOfWork, deps.Navigation, deps.Validator, deps.ChangeWatcher) {
		Deps = deps;

		SaveCommand = ReactiveCommand.Create(() => { SaveAndClose(); });
		CancelCommand = ReactiveCommand.Create(() => Close(true, CloseSource.Cancel));

		if(Entity is INotifyPropertyChanged entity)
			entity.PropertyChanged += (_, _) => HasChanges = true;
	}

	// в библиотечном диалоге Entity - поле, а вьюхи биндятся на свойства
	public new TEntity Entity => base.Entity;

	public ReactiveCommand<Unit, Unit> SaveCommand { get; }
	public ReactiveCommand<Unit, Unit> CancelCommand { get; }

	public override bool Save() {
		if(!base.Save())
			return false;

		HasChanges = false;
		return true;
	}

	protected override bool Validate() {
		if(base.Validate())
			return true;

		var errors = string.Join("\n• ", validator.Results.Select(r => r.ErrorMessage));
		Deps.Interactive.ShowMessage(ImportanceLevel.Warning, "Проверьте заполнение:\n• " + errors, "Не сохранено");
		return false;
	}
}
