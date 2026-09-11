using GreatCompany.Journal.ViewModels.CashFlow;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.Journal.ViewModels.Templates;
using QS.Journal.Columns;

namespace GreatCompany.Journal;

/// <summary>
/// Колонки всех журналов приложения
/// </summary>
public static class JournalsColumnsConfigs {
	// Заголовки, встречающиеся больше чем в одном журнале
	private const string Code = "Код";
	private const string Date = "Дата";
	private const string Purpose = "Назначение";
	private const string Amount = "Сумма";
	private const string Vat = "НДС";
	private const string AccountName = "Счёт";
	private const string DivisionName = "Подразделение";
	private const string ProjectName = "Проект";
	private const string ArticleName = "Статья";
	private const string Name = "Название";
	private const string PlannedId = "План";

	// Числовые колонки держим в пикселях
	private const double CodeWidth = 60;
	private const double DateWidth = 95;
	private const double AmountWidth = 105;
	private const double PlannedWidth = 70;

	public static JournalColumnsRegistry Create() {
		var registry = new JournalColumnsRegistry();
		RegisterCashFlow(registry);
		RegisterTemplates(registry);
		RegisterReference(registry);
		return registry;
	}

	private static void RegisterCashFlow(JournalColumnsRegistry registry) => registry
		.Register<PlannedIncomeJournalViewModel>(() => IncomeColumns().Finish())
		// План и факт показывают одно и то же, у факта в конце добавляется номер плана
		.Register<ActualIncomeJournalViewModel>(() => IncomeColumns()
			.AddColumn(PlannedId).Width(PlannedWidth).RightAligned().AddTextRenderer(x => x.PlannedId)
			.Finish())
		.Register<PlannedExpenseJournalViewModel>(() => ExpenseColumns().Finish())
		.Register<ActualExpenseJournalViewModel>(() => ExpenseColumns()
			.AddColumn(PlannedId).Width(PlannedWidth).RightAligned().AddTextRenderer(x => x.PlannedId)
			.Finish());

	private static void RegisterTemplates(JournalColumnsRegistry registry) => registry
		.Register<AccrualTemplateJournalViewModel>(() => ColumnsConfig<AccrualTemplateJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Purpose).StarWidth(3).MinWidth(140).AddTextRenderer(x => x.Purpose)
			.AddColumn(Amount).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.Amount, "N2")
			.AddColumn(Vat).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.VatAmount, "N2")
			.AddColumn(AccountName).StarWidth().MinWidth(80).AddTextRenderer(x => x.AccountName)
			.AddColumn(ProjectName).StarWidth(1.5).MinWidth(90).AddTextRenderer(x => x.ProjectName)
			.AddColumn(ArticleName).StarWidth(2).MinWidth(100).AddTextRenderer(x => x.ArticleName)
			.Finish())
		.Register<PaymentTemplateJournalViewModel>(() => ColumnsConfig<PaymentTemplateJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Purpose).StarWidth(3).MinWidth(140).AddTextRenderer(x => x.Purpose)
			.AddColumn(Amount).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.Amount, "N2")
			.AddColumn(Vat).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.VatAmount, "N2")
			.AddColumn(AccountName).StarWidth().MinWidth(80).AddTextRenderer(x => x.AccountName)
			.AddColumn(DivisionName).StarWidth(1.5).MinWidth(90).AddTextRenderer(x => x.DivisionName)
			.AddColumn(ProjectName).StarWidth(1.5).MinWidth(90).AddTextRenderer(x => x.ProjectName)
			.AddColumn(ArticleName).StarWidth(2).MinWidth(100).AddTextRenderer(x => x.ArticleName)
			.Finish());

	private static void RegisterReference(JournalColumnsRegistry registry) => registry
		.Register<AccountJournalViewModel>(() => ColumnsConfig<AccountJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Name).StarWidth(3).MinWidth(160).AddTextRenderer(x => x.Name)
			.AddColumn("Налоговый режим").StarWidth(1.5).MinWidth(140).AddTextRenderer(x => x.TaxRegimeTitle)
			.Finish())
		.Register<DivisionJournalViewModel>(() => ColumnsConfig<DivisionJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Name).StarWidth(3).MinWidth(160).AddTextRenderer(x => x.Name)
			.AddColumn("Головное подразделение").StarWidth(2).MinWidth(160).AddTextRenderer(x => x.ParentName)
			.Finish())
		.Register<ProjectJournalViewModel>(() => ColumnsConfig<ProjectJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Name).StarWidth(3).MinWidth(160).AddTextRenderer(x => x.Name)
			.AddColumn(DivisionName).StarWidth(2).MinWidth(160).AddTextRenderer(x => x.DivisionName)
			.Finish())
		.Register<IncomeArticleJournalViewModel>(() => ColumnsConfig<IncomeArticleJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Name).StarWidth().AddTextRenderer(x => x.Name)
			.Finish())
		.Register<ExpenseArticleJournalViewModel>(() => ColumnsConfig<ExpenseArticleJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Name).StarWidth().AddTextRenderer(x => x.Name)
			.Finish());

	private static ColumnsConfig<IncomeJournalNode> IncomeColumns() =>
		ColumnsConfig<IncomeJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Date).Width(DateWidth).AddTextRenderer(x => x.Date, "dd.MM.yyyy")
			.AddColumn(Purpose).StarWidth(3).MinWidth(140).AddTextRenderer(x => x.Purpose)
			.AddColumn(Amount).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.Amount, "N2")
			.AddColumn(Vat).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.VatAmount, "N2")
			.AddColumn(AccountName).StarWidth().MinWidth(80).AddTextRenderer(x => x.AccountName)
			.AddColumn(ProjectName).StarWidth(1.5).MinWidth(90).AddTextRenderer(x => x.ProjectName)
			.AddColumn(ArticleName).StarWidth(2).MinWidth(100).AddTextRenderer(x => x.ArticleName);

	private static ColumnsConfig<ExpenseJournalNode> ExpenseColumns() =>
		ColumnsConfig<ExpenseJournalNode>.Create()
			.AddColumn(Code).Width(CodeWidth).RightAligned().AddTextRenderer(x => x.Id)
			.AddColumn(Date).Width(DateWidth).AddTextRenderer(x => x.Date, "dd.MM.yyyy")
			.AddColumn(Purpose).StarWidth(3).MinWidth(140).AddTextRenderer(x => x.Purpose)
			.AddColumn(Amount).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.Amount, "N2")
			.AddColumn(Vat).Width(AmountWidth).RightAligned().AddTextRenderer(x => x.VatAmount, "N2")
			.AddColumn(AccountName).StarWidth().MinWidth(80).AddTextRenderer(x => x.AccountName)
			.AddColumn(DivisionName).StarWidth(1.5).MinWidth(90).AddTextRenderer(x => x.DivisionName)
			.AddColumn(ProjectName).StarWidth(1.5).MinWidth(90).AddTextRenderer(x => x.ProjectName)
			.AddColumn(ArticleName).StarWidth(2).MinWidth(100).AddTextRenderer(x => x.ArticleName);
}
