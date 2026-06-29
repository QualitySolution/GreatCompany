using System.Reactive;
using GreatCompany.Data.Models;
using QS.Navigation;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels;

// Справочный журнал умеет работать в режиме выбора: возвращает выбранную строку через ItemSelected
public abstract class ReferenceJournalViewModelBase<TRow> : JournalViewModelBase<TRow>, IReferenceJournal
	where TRow : class, IReferenceRow {
	protected ReferenceJournalViewModelBase(INavigationManager navigation) : base(navigation) {
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
}
