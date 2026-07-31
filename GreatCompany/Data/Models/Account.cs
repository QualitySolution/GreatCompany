using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "счёт", NominativePlural = "счета")]
public class Account : PropertyChangedBase, IDomainObject, IReferenceRow {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[Required(ErrorMessage = "Заполните название")]
	[StringLength(255, ErrorMessage = "Название должно быть не длиннее 255 символов")]
	public virtual string Name { get => name; set => SetField(ref name, value); }

	TaxRegime taxRegime;
	[Display(Name = "Налоговый режим")]
	public virtual TaxRegime TaxRegime { get => taxRegime; set => SetField(ref taxRegime, value); }
}

public class AccountRow : IReferenceRow {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public TaxRegime TaxRegime { get; set; }
	public string TaxRegimeTitle => TaxRegimes.TitleOf(TaxRegime);
}
