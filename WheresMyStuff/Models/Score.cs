namespace Rabbitminers.Stardew.WheresMyStuff.Models;

public record ComparisonScore<T>(T Value, double Score)
{
    public override string ToString()
    {
        return $"{{ value = {Value}, score = {Score} }}";
    }
}