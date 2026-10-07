using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;
using QS.Validation;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "сервис Intra проекта", NominativePlural = "сервисы Intra проекта")]
public class ProjectIntraService : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	Project project = null!;
	[Display(Name = "Проект")]
	[RequiredField]
	public virtual Project Project { get => project; set => SetField(ref project, value); }

	uint intraId;
	[Display(Name = "Intra ID сервиса")]
	[RequiredField]
	public virtual uint IntraId { get => intraId; set => SetField(ref intraId, value); }
}
