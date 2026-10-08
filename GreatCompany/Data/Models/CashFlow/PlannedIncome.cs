using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "план прихода", NominativePlural = "план - приход")]
public class PlannedIncome : IncomeOperationBase {
	AccrualTemplate? accrualTemplate;
	/// <summary>шаблон, по которому создан план. По нему повторное заполнение месяца не создаёт дубль</summary>
	[Display(Name = "Шаблон начисления")]
	public virtual AccrualTemplate? AccrualTemplate { get => accrualTemplate; set => SetField(ref accrualTemplate, value); }
}
