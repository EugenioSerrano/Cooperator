using Xunit;

namespace CooperatorModeler.Reverse.Tests;

/// <summary>
/// Los casos que tocan <c>CooperatorSettings.Current</c> comparten estado <b>de proceso</b>: sin colección, xUnit
/// los paraleliza por clase y el verde sería flaky (mismo criterio que el arnés y que el Bolt 013).
/// </summary>
[CollectionDefinition("ReverseTooling", DisableParallelization = true)]
public sealed class ReverseToolingCollection
{
}
