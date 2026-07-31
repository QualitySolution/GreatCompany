using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Feminine, Nominative = "статья дохода", NominativePlural = "статьи дохода")]
public class IncomeArticle : PropertyChangedBase, IDomainObject, IReferenceRow {
	public virtual int Id { get; set; }

	string name = "";
	[Display(Name = "Название")]
	[Required(ErrorMessage = "Заполните название")]
	[StringLength(255, ErrorMessage = "Название должно быть не длиннее 255 символов")]
	public virtual string Name { get => name; set => SetField(ref name, value); }
}
