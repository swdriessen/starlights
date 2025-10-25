namespace Starlights.Modules.Characters.Services.Statistics;

[Serializable]
public class StatisticsCalculatorException : Exception
{
    public StatisticsCalculatorException() { }
    public StatisticsCalculatorException(string message) : base(message) { }
    public StatisticsCalculatorException(string message, Exception inner) : base(message, inner) { }
}
