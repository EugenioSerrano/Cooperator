using Xunit;

namespace Cooperator.Framework.Data.Tests;

/// <summary>
/// Los casos que tocan el proveedor de caché del proceso (<c>CacheRegions.Provider</c>) y la configuración
/// ambiental comparten estado <b>de proceso</b>: sin colección, xUnit los paraleliza por clase y el verde sería
/// flaky. Mismo criterio que el Bolt 011 en el arnés.
/// </summary>
[CollectionDefinition("DataCaching", DisableParallelization = true)]
public sealed class DataCachingCollection
{
}
