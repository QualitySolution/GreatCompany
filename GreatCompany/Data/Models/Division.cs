using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "родразделение", NominativePlural = "родразделения")]
public class Division : PropertyChangedBase, IDomainObject, IReferenceRow, IValidatableObject {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[Required(ErrorMessage = "Заполните название")]
	[StringLength(255, ErrorMessage = "Название должно быть не длиннее 255 символов")]
	public virtual string Name { get => name; set => SetField(ref name, value); }

	int? parentDivisionId;
	[Display(Name = "Головное подразделение")]
	public virtual int? ParentDivisionId { get => parentDivisionId; set => SetField(ref parentDivisionId, value); }

	public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
		if(Id != 0 && ParentDivisionId == Id)
			yield return new ValidationResult("подразделение не может быть родителем самому себе", new[] { nameof(ParentDivisionId) });
	}
}

public class DivisionRow : IReferenceRow {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public string? ParentName { get; set; }
}
