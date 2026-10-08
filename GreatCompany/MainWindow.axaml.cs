using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using GreatCompany.Journal.ViewModels.CashFlow;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Journal.ViewModels.Templates;
using GreatCompany.ViewModels.Analytics;
using QS.Navigation;
using QS.Project.Versioning.ViewModels;
using System.ComponentModel;

namespace GreatCompany;

public partial class MainWindow : FAAppWindow {
	private readonly AvaloniaNavigationManager? navigationManager;
	private readonly Dictionary<FANavigationViewItem, Action> menuItems = [];
	// по нему подсветка в меню следует за активной вкладкой
	private readonly Dictionary<Type, FANavigationViewItem> menuItemsByViewModel = [];

	public MainWindow() {
		InitializeComponent();
		ConfigureWindowChrome();
		ConfigureTitleBar();
	}

	public MainWindow(
		AvaloniaNavigationManager navigationManager,
		string? login,
		string? sessionId,
		string? baseTitle) {
		this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));

		InitializeComponent();
		ConfigureWindowChrome();
		ConfigureTitleBar();
		navigationManagerView.DataContext = navigationManager;
		Title = MakeTitle(login, baseTitle);

		RegMenuItemActions();
		navigationManager.PropertyChanged += OnNavigationPropertyChanged;
		Closing += OnClosing;
	}


	private void ConfigureWindowChrome() {
		if(!OperatingSystem.IsLinux())
			return;

		// На Linux FluentAvalonia оставляет системный заголовок. Заменяем его своим,
		// сохраняя системную рамку для изменения размеров окна.
		WindowDecorations = Avalonia.Controls.WindowDecorations.BorderOnly;
		customTitleBar.IsVisible = true;
	}

	private void ConfigureTitleBar() {
		var titleBar = TitleBar ?? throw new InvalidOperationException("FAAppWindow не создал панель заголовка.");
		var background = GetPaletteColor("QsBrushChrome");
		var hoverBackground = GetPaletteColor("QsBrushChromeHover");
		var pressedBackground = GetPaletteColor("QsBrushChromePressed");
		var foreground = GetPaletteColor("QsBrushChromeText");
		var inactiveForeground = GetPaletteColor("QsBrushChromeTextMuted");

		titleBar.BackgroundColor = background;
		titleBar.ForegroundColor = foreground;
		titleBar.InactiveBackgroundColor = background;
		titleBar.InactiveForegroundColor = inactiveForeground;
		titleBar.ButtonBackgroundColor = background;
		titleBar.ButtonForegroundColor = foreground;
		titleBar.ButtonHoverBackgroundColor = hoverBackground;
		titleBar.ButtonHoverForegroundColor = foreground;
		titleBar.ButtonPressedBackgroundColor = pressedBackground;
		titleBar.ButtonPressedForegroundColor = foreground;
		titleBar.ButtonInactiveBackgroundColor = background;
		titleBar.ButtonInactiveForegroundColor = inactiveForeground;
	}

	private void OnCustomTitleBarPointerPressed(object? sender, PointerPressedEventArgs e) {
		if(!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			return;

		if(e.ClickCount == 2) {
			ToggleMaximized();
			e.Handled = true;
			return;
		}

		BeginMoveDrag(e);
	}

	private void OnMinimizeButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
		WindowState = WindowState.Minimized;

	private void OnMaximizeButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
		ToggleMaximized();

	private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

	private void ToggleMaximized() =>
		WindowState = WindowState == WindowState.Maximized
			? WindowState.Normal
			: WindowState.Maximized;

	private Color GetPaletteColor(string resourceKey) {
		if(this.TryFindResource(resourceKey, out var resource)
			&& resource is ISolidColorBrush brush)
			return brush.Color;

		throw new InvalidOperationException($"Ресурс палитры '{resourceKey}' не найден или не является сплошной кистью.");
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
		RegMenuItem<ActualTransferJournalViewModel>(actualTransfersMenuItem);

		RegMenuItem<IncomeExpenseViewModel>(incomeExpenseMenuItem);
		RegMenuItem<DivisionDetailsViewModel>(divisionDetailsMenuItem);

		RegMenuItem<AccrualTemplateJournalViewModel>(accrualTemplatesMenuItem);
		RegMenuItem<PaymentTemplateJournalViewModel>(paymentTemplatesMenuItem);

		RegMenuItem<ProjectJournalViewModel>(projectsMenuItem);
		RegMenuItem<DivisionJournalViewModel>(divisionsMenuItem);
		RegMenuItem<AccountJournalViewModel>(accountsMenuItem);
		RegMenuItem<IncomeArticleJournalViewModel>(incomeArticlesMenuItem);
		RegMenuItem<ExpenseArticleJournalViewModel>(expenseArticlesMenuItem);

		RegMenuItem<ChangeLogViewModel>(changeLogMenuItem);
	}

	private void RegMenuItem<TViewModel>(FANavigationViewItem item) where TViewModel : class, IDialogViewModel {
		menuItems.Add(item, () => navigationManager?.OpenViewModel<TViewModel>(null));
		menuItemsByViewModel.Add(typeof(TViewModel), item);
	}

	// слушаем клик, а не смену выбора, потому что закрытие вкладки не снимает выделение с пункта меню.
	// по SelectionChanged тот же журнал повторно не открыть, выбор не меняется
	private void OnNavViewItemInvoked(object? sender, FANavigationViewItemInvokedEventArgs e) {
		if(e.InvokedItemContainer is FANavigationViewItem item && menuItems.TryGetValue(item, out var action))
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
