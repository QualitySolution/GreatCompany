using System.Reactive;
using GreatCompany.Data.Models;
using ReactiveUI;

namespace GreatCompany.Controls;

public class ReferencePickerViewModel : ReactiveObject {
	public ReferencePickerViewModel(IReadOnlyList<ReferenceItem> items, Action<Action<ReferenceItem>> openSelectJournal) {
		Items = items;
		OpenJournalCommand = ReactiveCommand.Create(() => openSelectJournal(item => Selected = item));
	}

	public IReadOnlyList<ReferenceItem> Items { get; }

	ReferenceItem? selected;
	public ReferenceItem? Selected {
		get => selected;
		set => this.RaiseAndSetIfChanged(ref selected, value);
	}

	public ReactiveCommand<Unit, Unit> OpenJournalCommand { get; }

	public void SelectById(int id) => Selected = Items.FirstOrDefault(i => i.Id == id);
}
