namespace Rabbitminers.Stardew.WheresMyStuff.Core;

internal class Matching
{
    private static int _LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a))
        {
             return b.Length;
        }
        
        if (string.IsNullOrEmpty(b))
        {
             return a.Length;
        }

        var distance = new int[a.Length + 1, b.Length + 1];

        for (var i = 0; i <= a.Length; i++)
        {
            distance[i, 0] = i;
        }

        for (var j = 0; j <= b.Length; j++)
        {
            
            distance[0, j] = j;
        }

        for (var i = 1; i <= a.Length; i++)
        {
            for (var j = 1; j <= b.Length; j++)
            {
                var isSameChar = char.ToLower(a[i - 1]) == char.ToLower(b[j - 1]);
                var substitutionCost = isSameChar ? 0 : 1;

                var deletion = distance[i - 1, j] + 1;
                var insertion = distance[i, j - 1] + 1;
                var substitution = distance[i - 1, j - 1] + substitutionCost;

                distance[i, j] = Math.Min(Math.Min(deletion, insertion), substitution);
            }
        }

        return distance[a.Length, b.Length];
    }

    public static bool IsFuzzyMatch(string a, string b, int threshold = 3)
    {
        return _LevenshteinDistance(a, b) <= threshold;
    }
}