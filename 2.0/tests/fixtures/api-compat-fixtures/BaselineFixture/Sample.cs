namespace Cooperator.ApiCompatFixture;

/// <summary>
/// Fixture sintético del lado CONTRACT del par de pruebas del gate
/// (SPEC §6.D.2, corrida roja). No representa ningún tipo real del framework:
/// es una superficie mínima cuyo único propósito es que el comparador tenga
/// algo que comparar.
/// </summary>
public sealed class Sample
{
    /// <summary>Miembro que se conserva en ambos lados del par.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Miembro que se conserva en ambos lados del par.</summary>
    public static int KeptMethod(int input) => input + 1;

    /// <summary>Miembro público que el lado roto RETIRA (ruptura de tipo 1).</summary>
    public static string RemovedMethod(string argument) => argument ?? string.Empty;

    /// <summary>Miembro público cuya FIRMA el lado roto ALTERA (ruptura de tipo 2).</summary>
    public static int ChangedSignature(int argument) => argument;
}
