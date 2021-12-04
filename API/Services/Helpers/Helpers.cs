using Eltin_Buchard_Keller_Algorithm;
namespace WildRiftWebAPI;

public interface IName
{
    string Name { get; set; }
}

public static class Helpers
{
    /// <summary>
    /// Approximate name of entity, based on the Levenshtein metric.
    /// </summary>
    /// <typeparam name="T">The db set type that implements the IName interface</typeparam>
    /// <param name="name">String to be approximated</param>
    /// <param name="dbSet">The collection where the approximation is done</param>
    /// <returns></returns>
    public static string ApproximateName<T>(string name, DbSet<T> dbSet) where T : class, IName
    {
        var itemNames = dbSet.Select(item => item.Name).ToList();
        List<string> itemNamesTree = new();

        foreach (var element in itemNames)
            itemNamesTree.AddRange(element.Split(new char[] { ' ', '\'' }));

        itemNamesTree.AddRange(itemNames);

        BKTree tree = new(new(name));
        tree.AddMultiple(itemNamesTree);
        return tree.FindBestNodeWithDistance(name);
    }

    /// <summary>
    /// Capitalizes the given string input. First letter will be capitalized and the following ones will be lowered.
    /// </summary>
    /// <param name="input">The input to be capitalized</param>
    public static void Capitalize(ref string input)
    {
        input = $"{input[0].ToString().ToUpper()}{input[1..].ToLower()}";
    }
}
