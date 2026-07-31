using System.ComponentModel;
using System.Reactive;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Tdi;
using QS.Validation;
using QS.ViewModels.Dialog;
using ReactiveUI;

namespace GreatCompany.ViewModels;

public abstract class FormViewModelBase : DialogViewModelBase, IHasChanges, ISaveable {
	readonly IValidator validator;
	readonly IInteractiveMessage interactive;
	bool changed;

	protected FormViewModelBase(INavigationManager navigation, IValidator validator, IInteractiveMessage interactive) : base(navigation) {
		this.validator = validator;
		this.interactive = interactive;
		SaveCommand = ReactiveCommand.Create(SaveAndClose);
		CancelCommand = ReactiveCommand.Create(() => Close(false, CloseSource.Cancel));
	}

	// Из ISaveable: по нему журнал перечитывает список после сохранения карточки
	public event EventHandler<EntitySavedEventArgs>? EntitySaved;

	public ReactiveCommand<Unit, Unit> SaveCommand { get; }
	public ReactiveCommand<Unit, Unit> CancelCommand { get; }

	// Есть ли несохраненные правки — навигация смотрит сюда при закрытии крестиком
	public bool HasChanges => changed;

	// Карточка в конце конструктора отдает сюда сущность и пикеры:
	// любое их изменение после этого делает вкладку «грязной»
	protected void TrackChanges(params INotifyPropertyChanged[] sources) {
		foreach(var source in sources)
			source.PropertyChanged += (_, _) => changed = true;
	}

	// Проверяет сущность по её атрибутам ([Required], [Range], IValidatableObject).
	// Если что-то не так — показывает список ошибок и возвращает false.
	protected bool Validate(object entity) {
		if(validator.Validate(entity, null, showValidationResults: false))
			return true;
		var errors = string.Join("\n• ", validator.Results.Select(r => r.ErrorMessage));
		interactive.ShowMessage(ImportanceLevel.Warning, "Проверьте заполнение:\n• " + errors, "Не сохранено");
		return false;
	}

	// Сохраняет сущность карточки и возвращает её. null — сохранять нельзя (не прошла проверку)
	protected abstract IDomainObject? SaveEntity();

	// ISaveable — этим навигация сохраняет карточку, когда пользователь на вопрос
	// «Сохранить изменения перед закрытием?» ответил «Сохранить»
	public bool Save() {
		var entity = SaveEntity();
		if(entity == null)
			return false;
		changed = false;
		EntitySaved?.Invoke(this, new EntitySavedEventArgs(entity));
		return true;
	}

	public void SaveAndClose() {
		if(Save())
			Close(false, CloseSource.Save);
	}
}
