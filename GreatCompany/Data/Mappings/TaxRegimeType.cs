using System.Data.Common;
using GreatCompany.Data.Models;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace GreatCompany.Data.Mappings;

public class TaxRegimeType : IUserType {
	public SqlType[] SqlTypes => new[] { NHibernateUtil.String.SqlType };
	public Type ReturnedType => typeof(TaxRegime);
	public bool IsMutable => false;

	public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
		var stored = (string?)NHibernateUtil.String.NullSafeGet(rs, names[0], session)
			?? throw new InvalidOperationException($"Налоговый режим не заполнен в базе, колонка \"{names[0]}\"");

		return Enum.Parse<TaxRegime>(stored, ignoreCase: true);
	}

	public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
		var regime = (TaxRegime)(value ?? throw new ArgumentNullException(nameof(value)));
		NHibernateUtil.String.NullSafeSet(cmd, regime.ToString().ToLowerInvariant(), index, session);
	}

	public new bool Equals(object x, object y) => object.Equals(x, y);
	public int GetHashCode(object x) => x?.GetHashCode() ?? 0;
	public object DeepCopy(object value) => value;
	public object Replace(object original, object target, object owner) => original;
	public object Assemble(object cached, object owner) => cached;
	public object Disassemble(object value) => value;
}
