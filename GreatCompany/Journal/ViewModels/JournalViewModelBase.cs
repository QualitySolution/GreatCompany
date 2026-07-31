using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using GreatCompany.ViewModels;
using QS.Dialog;
using QS.Navigation;
using QS.ViewModels.Dialog;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels;

public abstract class JournalViewModelBase<TRow> : DialogViewModelBase, IDisposable where TRow : class {
	readonly IInteractiveMessage interactive;
	readonly IDisposable searchSubscription;

	protected JournalViewModelBase(INavigationManager navigation, IInteractiveMessage interactive) : base(navigation) {
		this.interactive = interactive;
		var hasSelection = this.WhenAnyValue(x => x.Selected).Select(s => s != null);

		CreateCommand = ReactiveCommand.Create(Create);
		EditCommand = ReactiveCommand.Create(() => Edit(Selected!), hasSelection);
		DeleteCommand = ReactiveCommand.Create(() => Delete(Selected!), hasSelection);
		RefreshCommand = ReactiveCommand.Create(Reload);

		// Поиск перечитывает список не на каждый символ, а после паузы в наборе — меньше запросов к серверу
		searchSubscription = this.WhenAnyValue(x => x.Search)
			.Skip(1)
			.Throttle(TimeSpan.FromMilliseconds(400), RxApp.MainThreadScheduler)
			.Subscribe(_ => Reload());
	}

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Подписку на поиск снимаем при закрытии вкладки: Dispose журналу вызывает Autofac,
	// когда закрывает скоуп вкладки
	protected virtual void Dispose(bool disposing) {
		if(disposing)
			searchSubscription.Dispose();
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
		set => SetField(ref search, value);
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

	// Сервер не дал удалить запись — на неё ссылаются другие документы
	protected void NotifyDeleteBlocked() {
		interactive.ShowMessage(ImportanceLevel.Warning,
			"Запись используется в других документах, поэтому удалить её нельзя.", "Не удалено");
	}

	// Открывает карточку и перечитывает список после сохранения
	protected void OpenCard<TCard>(int id) where TCard : FormViewModelBase {
		var page = OpenCardPage<TCard>(id);
		page.ViewModel.EntitySaved += (_, _) => Reload();
	}

	protected virtual IPage<TCard> OpenCardPage<TCard>(int id) where TCard : FormViewModelBase =>
		NavigationManager.OpenViewModel<TCard, int>(this, id);

	public virtual void RowActivated() {
		if(Selected != null)
			Edit(Selected);
	}
}
