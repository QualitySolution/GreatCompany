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
		int index = rs.GetOrdinal(names[0]);
		if(rs.IsDBNull(index))
			return TaxRegime.Vat;
		return Enum.Parse<TaxRegime>(Convert.ToString(rs.GetValue(index))!, ignoreCase: true);
	}

	public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
		var regime = value is TaxRegime r ? r : TaxRegime.Vat;
		NHibernateUtil.String.NullSafeSet(cmd, regime.ToString().ToLowerInvariant(), index, session);
	}

	public new bool Equals(object x, object y) => object.Equals(x, y);
	public int GetHashCode(object x) => x?.GetHashCode() ?? 0;
	public object DeepCopy(object value) => value;
	public object Replace(object original, object target, object owner) => original;
	public object Assemble(object cached, object owner) => cached;
	public object Disassemble(object value) => value;
}
