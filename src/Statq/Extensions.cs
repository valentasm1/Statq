namespace Statq;

public static class Extensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var rng = new Random();
        var n = list.Count;

        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static T Second<T>(this IEnumerable<T> source)
    {
        return source.Skip(1).Single();
    }
}