using System.ComponentModel.DataAnnotations;

namespace GreatCompany.Data.Models;

/// <summary>
/// Обязательное свойство
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredFieldAttribute : RequiredAttribute {
	public RequiredFieldAttribute() => ErrorMessage = "Заполните \"{0}\"";
}

/// <summary>
/// Ограничение длины строки
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MaxTextAttribute : StringLengthAttribute {
	public MaxTextAttribute(int maximumLength) : base(maximumLength) =>
		ErrorMessage = "\"{0}\" не должно быть длиннее {1} символов";
}
