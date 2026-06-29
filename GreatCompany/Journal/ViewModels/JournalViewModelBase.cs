using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using GreatCompany.ViewModels;
using QS.Navigation;
using QS.ViewModels.Dialog;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels;

public abstract class JournalViewModelBase<TRow> : DialogViewModelBase where TRow : class {
	protected JournalViewModelBase(INavigationManager navigation) : base(navigation) {
		var hasSelection = this.WhenAnyValue(x => x.Selected).Select(s => s != null);

		CreateCommand = ReactiveCommand.Create(Create);
		EditCommand = ReactiveCommand.Create(() => Edit(Selected!), hasSelection);
		DeleteCommand = ReactiveCommand.Create(() => Delete(Selected!), hasSelection);
		RefreshCommand = ReactiveCommand.Create(Reload);
	}

	public ObservableCollection<TRow> Items { get; } = new();

	TRow? selected;
	public TRow? Selected {
		get => selected;
		set => SetField(ref selected, value);
	}

	string search = "";
	public string Search {
		get => search;
		set { if(SetField(ref search, value)) Reload(); }
	}

	public ReactiveCommand<Unit, Unit> CreateCommand { get; }
	public ReactiveCommand<Unit, Unit> EditCommand { get; }
	public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
	public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

	protected abstract IEnumerable<TRow> Load(string search);
	protected abstract void Create();
	protected abstract void Edit(TRow row);
	protected abstract void Delete(TRow row);

	public void Reload() {
		Items.Clear();
		foreach(var row in Load(search))
			Items.Add(row);
	}

	// Открывает карточку и перечитывает список после сохранения
	protected void OpenCard<TCard>(int id) where TCard : FormViewModelBase {
		var page = NavigationManager.OpenViewModel<TCard, int>(this, id);
		page.ViewModel.Saved += Reload;
	}

	public virtual void RowActivated() {
		if(Selected != null)
			Edit(Selected);
	}
}
