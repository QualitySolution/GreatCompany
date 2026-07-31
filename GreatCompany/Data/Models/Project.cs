using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "проект", NominativePlural = "проекты")]
public class Project : PropertyChangedBase, IDomainObject, IReferenceRow {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[Required(ErrorMessage = "Заполните название")]
	[StringLength(255, ErrorMessage = "Название должно быть не длиннее 255 символов")]
	public virtual string Name { get => name; set => SetField(ref name, value); }

	int divisionId;
	[Display(Name = "Подразделение")]
	[Range(1, int.MaxValue, ErrorMessage = "Выберите подразделение")]
	public virtual int DivisionId { get => divisionId; set => SetField(ref divisionId, value); }
}

public class ProjectRow : IReferenceRow {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public string DivisionName { get; set; } = "";
}
