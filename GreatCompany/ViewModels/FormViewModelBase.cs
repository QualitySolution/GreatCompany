using System.Reactive;
using QS.Navigation;
using QS.ViewModels.Dialog;
using ReactiveUI;

namespace GreatCompany.ViewModels;

public abstract class FormViewModelBase : DialogViewModelBase {
	protected FormViewModelBase(INavigationManager navigation) : base(navigation) {
		SaveCommand = ReactiveCommand.Create(OnSave);
		CancelCommand = ReactiveCommand.Create(() => Close(false, CloseSource.Cancel));
	}

	public event Action? Saved;
	public ReactiveCommand<Unit, Unit> SaveCommand { get; }
	public ReactiveCommand<Unit, Unit> CancelCommand { get; }

	// Возвращает false, если сохранять нельзя
	protected abstract bool Save();

	void OnSave() {
		if(!Save())
			return;
		Saved?.Invoke();
		Close(false, CloseSource.Save);
	}
}
