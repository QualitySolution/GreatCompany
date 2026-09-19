using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using GreatCompany.Data.Models;
using NHibernate.Cfg;
using QS.DomainModel.Entity;
using QS.Project.DB;

namespace GreatCompany.Data.Mappings;

/// <summary>
/// Маппинг сущностей выводится из их имён - свойство → колонка, ссылка → колонка с суффиксом _id
/// </summary>
public class EntityAutoMapping : IDatabaseConfigurationExposer {
	public void ExposeConfiguration(NHibernate.Cfg.Configuration config) => CreateModel().Configure(config);

	public static AutoPersistenceModel CreateModel() {
		var model = AutoMap.AssemblyOf<Account>(new DomainEntities());
		model.Conventions.Add(new SnakeCaseNames());
		// налоговый режим хранится строкой в enum-колонке, состав которой задаёт само перечисление
		model.Override<Account>(map => map.Map(x => x.TaxRegime)
			.CustomType<TaxRegimeType>()
			.CustomSqlType(EnumColumnType<TaxRegime>()));
		return model;
	}

	private static string EnumColumnType<TEnum>() where TEnum : struct, Enum =>
		"enum(" + String.Join(",", Enum.GetNames<TEnum>().Select(name => $"'{name.ToLowerInvariant()}'")) + ")";

	/// <summary>
	/// сохраняем доменные объекты из Models
	/// без сеттера свойства — нет
	/// Абстрактные предки сущностью не становятся, раскладыватся в свойства по таблицам наследников
	/// </summary>
	private sealed class DomainEntities : DefaultAutomappingConfiguration {
		public override bool ShouldMap(Type type) => base.ShouldMap(type)
			&& typeof(IDomainObject).IsAssignableFrom(type) && !type.IsAbstract
			&& type.Namespace == typeof(Account).Namespace;

		public override bool ShouldMap(Member member) => base.ShouldMap(member) && member.CanWrite;
	}

	private sealed class SnakeCaseNames : IClassConvention, IIdConvention, IPropertyConvention, IReferenceConvention {
		public void Apply(IClassInstance instance) => instance.Table(ToSnakeCase(instance.EntityType.Name) + "s");

		public void Apply(IIdentityInstance instance) {
			instance.Column(ToSnakeCase(instance.Name));
			instance.GeneratedBy.Native();
		}

		public void Apply(IPropertyInstance instance) {
			instance.Column(ToSnakeCase(instance.Name));
			// загрузка пишет в поле напрямую, логика сеттеров при ней не срабатывает
			instance.Access.CamelCaseField();
			ApplyColumnType(instance);
		}

		public void Apply(IManyToOneInstance instance) {
			instance.Column(ToSnakeCase(instance.Name) + "_id");
			instance.Access.CamelCaseField();
		}

		/// <summary>
		/// строка с ограничением длины ([MaxText] или [StringLength]) — varchar(n)
		/// без ограничения — text
		/// деньги — decimal(19,2)
		/// </summary>
		private static void ApplyColumnType(IPropertyInstance instance) {
			var propertyType = instance.Property.PropertyType;

			if(propertyType == typeof(decimal) || propertyType == typeof(decimal?)) {
				instance.Precision(19);
				instance.Scale(2);
				return;
			}

			if(propertyType != typeof(string))
				return;

			var lengthLimit = instance.Property.MemberInfo.GetCustomAttribute<StringLengthAttribute>();
			if(lengthLimit != null)
				instance.Length(lengthLimit.MaximumLength);
			else
				instance.CustomSqlType("text");
		}

		private static string ToSnakeCase(string name) {
			var result = new StringBuilder(name.Length + 4);
			foreach(var symbol in name) {
				if(char.IsUpper(symbol) && result.Length > 0)
					result.Append('_');
				result.Append(char.ToLowerInvariant(symbol));
			}
			return result.ToString();
		}
	}
}
