using System.Reactive;
using GreatCompany.Data.Models;
using QS.Dialog;
using QS.Navigation;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels;

// Справочный журнал умеет работать в режиме выбора: возвращает выбранную строку через ItemSelected
public abstract class ReferenceJournalViewModelBase<TRow> : JournalViewModelBase<TRow>, IReferenceJournal
	where TRow : class, IReferenceRow {
	protected ReferenceJournalViewModelBase(INavigationManager navigation, IInteractiveMessage interactive) : base(navigation, interactive) {
		SelectCommand = ReactiveCommand.Create(RaiseSelected);
	}

	public bool SelectMode { get; private set; }
	public event Action<ReferenceItem>? ItemSelected;
	public ReactiveCommand<Unit, Unit> SelectCommand { get; }

	public void EnableSelect() => SelectMode = true;

	void RaiseSelected() {
		if(Selected != null)
			ItemSelected?.Invoke(new ReferenceItem(Selected.Id, Selected.Name));
	}

	public override void RowActivated() {
		if(SelectMode)
			RaiseSelected();
		else
			base.RowActivated();
	}

	// В режиме выбора журнал — модальное окно, вкладки главного окна недоступны,
	// поэтому карточка открывается окном поверх окна выбора
	protected override IPage<TCard> OpenCardPage<TCard>(int id) =>
		SelectMode
			? ((AvaloniaNavigationManager)NavigationManager).OpenViewModelAsWindow<TCard, int>(this, id, OpenPageOptions.IgnoreHash)
			: base.OpenCardPage<TCard>(id);
}
