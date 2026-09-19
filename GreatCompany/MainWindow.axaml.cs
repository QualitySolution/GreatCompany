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
		menuItems.Add(item, () => navigationManager?.OpenViewModel<TViewModel>(null));
		menuItemsByViewModel.Add(typeof(TViewModel), item);
	}

	// слушаем клик, а не смену выбора, потому что закрытие вкладки не снимает выделение с пункта меню.
	// по SelectionChanged тот же журнал повторно не открыть, выбор не меняется
	private void OnNavViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e) {
		if(e.InvokedItemContainer is NavigationViewItem item && menuItems.TryGetValue(item, out var action))
			action();
	}

	// вкладку переключают и мышью по самой вкладке, и её закрытием.
	// без этого в меню продолжает гореть пункт журнала, который уже не показан
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

		// обходим по снимку, закрытие вкладки меняет саму коллекцию Pages
		foreach(var page in navigationManager.Pages.ToList()) {
			// подчинённую вкладку уже закрыла вместе с собой хозяйская
			if(!navigationManager.Pages.Contains(page) || navigationManager.AskClosePage(page, CloseSource.AppQuit))
				continue;

			// пользователь отменил закрытие вкладки с несохранёнными изменениями, не выходим
			e.Cancel = true;
			return;
		}
	}
}
