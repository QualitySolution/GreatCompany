using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using GreatCompany.Journal.ViewModels.CashFlow;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Journal.ViewModels.Templates;
using QS.Navigation;
using QS.Project.Versioning.ViewModels;

namespace GreatCompany;

public partial class MainWindow : Window {
	private readonly AvaloniaNavigationManager? navigationManager;
	private readonly Dictionary<NavigationViewItem, Action> menuItems = [];

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
		menuItems.Add(plannedIncomesMenuItem, Open<PlannedIncomeJournalViewModel>);
		menuItems.Add(actualIncomesMenuItem, Open<ActualIncomeJournalViewModel>);
		menuItems.Add(plannedExpensesMenuItem, Open<PlannedExpenseJournalViewModel>);
		menuItems.Add(actualExpensesMenuItem, Open<ActualExpenseJournalViewModel>);

		menuItems.Add(accrualTemplatesMenuItem, Open<AccrualTemplateJournalViewModel>);
		menuItems.Add(paymentTemplatesMenuItem, Open<PaymentTemplateJournalViewModel>);

		menuItems.Add(projectsMenuItem, Open<ProjectJournalViewModel>);
		menuItems.Add(divisionsMenuItem, Open<DivisionJournalViewModel>);
		menuItems.Add(accountsMenuItem, Open<AccountJournalViewModel>);
		menuItems.Add(incomeArticlesMenuItem, Open<IncomeArticleJournalViewModel>);
		menuItems.Add(expenseArticlesMenuItem, Open<ExpenseArticleJournalViewModel>);

		menuItems.Add(changeLogMenuItem, Open<ChangeLogViewModel>);
	}

	// Открывает (или переключает на уже открытую) вкладку. Повторный выбор активной вкладки в
	// TabView иногда не применяется сразу, поэтому дополнительно форсим её через диспетчер.
	private void Open<TViewModel>() where TViewModel : class, IDialogViewModel {
		var page = navigationManager?.OpenViewModel<TViewModel>(null);
		if(page != null)
			Dispatcher.UIThread.Post(() => navigationManager!.CurrentPage = page);
	}

	private void OnNavViewSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e) {
		if(e.SelectedItem is NavigationViewItem item && menuItems.TryGetValue(item, out var action))
			action();
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
