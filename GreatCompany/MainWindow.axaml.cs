using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using GreatCompany.Journal.ViewModels.CashFlow;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Journal.ViewModels.Templates;
using QS.Navigation;
using QS.Project.Versioning.ViewModels;
using System.ComponentModel;

namespace GreatCompany;

public partial class MainWindow : Window {
	private readonly AvaloniaNavigationManager? navigationManager;
	private readonly Dictionary<NavigationViewItem, Action> menuItems = [];
	// по нему подсветка в меню следует за активной вкладкой
	private readonly Dictionary<Type, NavigationViewItem> menuItemsByViewModel = [];

	public MainWindow() {
		InitializeComponent();
	}

	public MainWindow(
		AvaloniaNavigationManager navigationManager,
		string? login,
		string? sessionId,
		string? baseTitle) {
		this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));

		InitializeComponent();
		navigationManagerView.DataContext = navigationManager;
		Title = MakeTitle(login, baseTitle);

		RegMenuItemActions();
		navigationManager.PropertyChanged += OnNavigationPropertyChanged;
		Closing += OnClosing;
	}

	private static string MakeTitle(string? login, string? baseTitle) {
		var title = "QS: Великая компания";
		if(!string.IsNullOrWhiteSpace(baseTitle))
			title += $" (БД: {baseTitle})";
		if(!string.IsNullOrWhiteSpace(login))
			title += $" - {login}";
		return title;
	}

	private void RegMenuItemActions() {
		RegMenuItem<PlannedIncomeJournalViewModel>(plannedIncomesMenuItem);
		RegMenuItem<ActualIncomeJournalViewModel>(actualIncomesMenuItem);
		RegMenuItem<PlannedExpenseJournalViewModel>(plannedExpensesMenuItem);
		RegMenuItem<ActualExpenseJournalViewModel>(actualExpensesMenuItem);

		RegMenuItem<AccrualTemplateJournalViewModel>(accrualTemplatesMenuItem);
		RegMenuItem<PaymentTemplateJournalViewModel>(paymentTemplatesMenuItem);

		RegMenuItem<ProjectJournalViewModel>(projectsMenuItem);
		RegMenuItem<DivisionJournalViewModel>(divisionsMenuItem);
		RegMenuItem<AccountJournalViewModel>(accountsMenuItem);
		RegMenuItem<IncomeArticleJournalViewModel>(incomeArticlesMenuItem);
		RegMenuItem<ExpenseArticleJournalViewModel>(expenseArticlesMenuItem);

		RegMenuItem<ChangeLogViewModel>(changeLogMenuItem);
	}

	private void RegMenuItem<TViewModel>(NavigationViewItem item) where TViewModel : class, IDialogViewModel {
		menuItems.Add(item, Open<TViewModel>);
		menuItemsByViewModel.Add(typeof(TViewModel), item);
	}

	// Открывает вкладку; если она уже открыта, навигация сама переключается на неё
	private void Open<TViewModel>() where TViewModel : class, IDialogViewModel =>
		navigationManager?.OpenViewModel<TViewModel>(null);

	// Слушаем именно клик, а не смену выбора: закрытие вкладки не снимает выделение с пункта меню,
	// и по SelectionChanged повторно открыть тот же журнал было бы нельзя — выбор не меняется
	private void OnNavViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e) {
		if(e.InvokedItemContainer is NavigationViewItem item && menuItems.TryGetValue(item, out var action))
			action();
	}

	// Вкладку переключают и мышью по самой вкладке, и её закрытием. Без этого в меню
	// продолжает гореть пункт журнала, который уже не показан
	private void OnNavigationPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if(e.PropertyName != nameof(AvaloniaNavigationManager.CurrentPage))
			return;

		var openedViewModel = navigationManager?.CurrentPage?.ViewModel?.GetType();
		if(openedViewModel == null || !menuItemsByViewModel.TryGetValue(openedViewModel, out var item))
			return;

		if(item.IsLoaded)
			navigationView.SelectedItem = item;
	}

	private void OnClosing(object? sender, WindowClosingEventArgs e) {
		if(navigationManager == null)
			return;

		// обходим по снимку: закрытие вкладки меняет саму коллекцию Pages
		foreach(var page in navigationManager.Pages.ToList()) {
			if(navigationManager.AskClosePage(page, CloseSource.AppQuit))
				continue;

			// пользователь отменил закрытие вкладки с несохраненными изменениями — не выходим
			e.Cancel = true;
			return;
		}
	}
}
