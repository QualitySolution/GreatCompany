using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Neuter, Nominative = "подразделение", NominativePlural = "подразделения")]
public class Division : PropertyChangedBase, IDomainObject, IValidatableObject {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[RequiredField]
	[MaxText(255)]
	public virtual string Name { get => name; set => SetField(ref name, value); }

	Division? parentDivision;
	[Display(Name = "Головное подразделение")]
	public virtual Division? ParentDivision { get => parentDivision; set => SetField(ref parentDivision, value); }

	public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
		if(Id != 0 && ParentDivision?.Id == Id)
			yield return new ValidationResult("подразделение не может быть родителем самому себе", new[] { nameof(ParentDivision) });
	}
}
