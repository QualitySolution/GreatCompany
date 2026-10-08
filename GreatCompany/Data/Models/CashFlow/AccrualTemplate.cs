using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "шаблон начисления", NominativePlural = "шаблоны начислений")]
public class AccrualTemplate : IncomeDocumentBase {
	int dayOfMonth = 1;
	/// <summary>день, на который создаётся план. Если в месяце дней меньше, план встаёт на последний день</summary>
	[Display(Name = "День месяца")]
	[Range(1, 31, ErrorMessage = "день месяца должен быть от 1 до 31")]
	public virtual int DayOfMonth { get => dayOfMonth; set => SetField(ref dayOfMonth, value); }

	/// <summary>дата плана по шаблону в месяце, к которому относится <paramref name="month"/></summary>
	public virtual DateTime PlanDate(DateTime month) =>
		new(month.Year, month.Month, Math.Min(DayOfMonth, DateTime.DaysInMonth(month.Year, month.Month)));

	// отключённый шаблон не предлагается при создании документа по шаблону
	bool isDisabled;
	[Display(Name = "Отключён")]
	public virtual bool IsDisabled { get => isDisabled; set => SetField(ref isDisabled, value); }
}
