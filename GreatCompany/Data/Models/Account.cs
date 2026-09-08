using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "счёт", NominativePlural = "счета")]
public class Account : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[RequiredField]
	[MaxText(255)]
	public virtual string Name { get => name; set => SetField(ref name, value); }

	TaxRegime taxRegime;
	[Display(Name = "Налоговый режим")]
	public virtual TaxRegime TaxRegime { get => taxRegime; set => SetField(ref taxRegime, value); }
}
