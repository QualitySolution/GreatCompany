using System.Globalization;
using GreatCompany.Data.Planning;
using QS.Dialog;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.ViewModels.Dialog;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace GreatCompany.ViewModels.Templates;

/// <summary>окно выбора месяца, на который заполняется план по шаблонам</summary>
public class FillPlansViewModel : WindowDialogViewModelBase {
	readonly IUnitOfWorkFactory uowFactory;
	readonly IInteractiveMessage interactive;
	readonly PlanKind kind;

	public FillPlansViewModel(
		INavigationManager navigation,
		IUnitOfWorkFactory uowFactory,
		IInteractiveMessage interactive,
		PlanKind kind) : base(navigation) {
		this.uowFactory = uowFactory;
		this.interactive = interactive;
		this.kind = kind;

		Title = kind == PlanKind.Income ? "Заполнить план прихода" : "Заполнить план расхода";
		Resizable = false;

		// план обычно заполняют наперёд
		var next = DateTime.Today.AddMonths(1);
		monthIndex = next.Month - 1;
		year = next.Year;

		FillCommand = ReactiveCommand.Create(Fill);
		CancelCommand = ReactiveCommand.Create(() => Close(false, CloseSource.Cancel));
	}

	public static IReadOnlyList<string> Months { get; } =
		CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.MonthNames[..12];

	int monthIndex;
	public int MonthIndex { get => monthIndex; set => SetField(ref monthIndex, value); }

	int year;
	public int Year { get => year; set => SetField(ref year, value); }

	public ReactiveCommand<RxVoid, RxVoid> FillCommand { get; }
	public ReactiveCommand<RxVoid, RxVoid> CancelCommand { get; }

	void Fill() {
		var month = new DateTime(Year, MonthIndex + 1, 1);
		PlanFillResult result;
		using(var uow = uowFactory.Create(Title))
			result = PlansFromTemplates.Fill(uow, kind, month);

		var message = $"{Months[MonthIndex]} {Year}: создано планов {result.Created}.";
		if(result.Existing > 0)
			message += $"\nПо {result.Existing} шаблонам план на этот месяц уже был, его не трогали.";
		interactive.ShowMessage(ImportanceLevel.Info, message, Title);

		Close(false, CloseSource.Save);
	}
}
