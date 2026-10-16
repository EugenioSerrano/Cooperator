namespace Cooperator.Harness.Tests;

/// <summary>
/// Colección de los casos que tocan el estado ambiental de la configuración (<c>CooperatorSettings.Current</c>) y
/// las variables de entorno: se serializan entre sí para que no compitan por ese estado compartido.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ConfigurationCollection
{
    public const string Name = "cooperator-configuration";
}
