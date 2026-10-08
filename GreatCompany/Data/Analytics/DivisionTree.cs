using GreatCompany.Data.Models;

namespace GreatCompany.Data.Analytics;

/// <param name="Depth">уровень вложенности, у корня 0</param>
/// <param name="Ids">подразделение вместе со всеми дочерними на любую глубину</param>
public record DivisionNode(Division Division, int Depth, IReadOnlyCollection<int> Ids);

public static class DivisionTree {
	/// <summary>подразделения в порядке дерева: родитель, за ним его дочерние, внутри уровня по названию</summary>
	public static IReadOnlyList<DivisionNode> Build(IEnumerable<Division> all) {
		var children = all.ToLookup(x => x.ParentDivision?.Id);
		var result = new List<DivisionNode>();

		foreach(var root in children[null].OrderBy(x => x.Name))
			Add(root, 0);
		return result;

		List<int> Add(Division division, int depth) {
			var ids = new List<int> { division.Id };
			result.Add(new DivisionNode(division, depth, ids));
			foreach(var child in children[division.Id].OrderBy(x => x.Name))
				ids.AddRange(Add(child, depth + 1));
			return ids;
		}
	}
}
