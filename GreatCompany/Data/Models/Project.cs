using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;
using QS.Validation;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "проект", NominativePlural = "проекты")]
public class Project : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[RequiredField]
	[MaxText(255)]
	public virtual string Name { get => name; set => SetField(ref name, value); }

	Division division = null!;
	[Display(Name = "Подразделение")]
	[RequiredField]
	public virtual Division Division { get => division; set => SetField(ref division, value); }

	bool isArchived;
	[Display(Name = "Архивный")]
	public virtual bool IsArchived { get => isArchived; set => SetField(ref isArchived, value); }
}
