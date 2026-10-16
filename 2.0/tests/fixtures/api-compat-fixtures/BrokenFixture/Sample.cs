namespace Cooperator.ApiCompatFixture;

/// <summary>
/// Fixture sintético del lado IMPLEMENTATION del par de pruebas del gate
/// (SPEC §6.D.2, corrida roja). Respecto de <c>BaselineFixture/Sample.cs</c>:
/// <list type="bullet">
///   <item>retira el miembro público <c>RemovedMethod(string)</c>;</item>
///   <item>altera la firma de <c>ChangedSignature(int)</c> → <c>ChangedSignature(string)</c>.</item>
/// </list>
/// La ruptura es intencional y es lo único que distingue a ambos lados.
/// </summary>
public sealed class Sample
{
    /// <summary>Miembro que se conserva en ambos lados del par.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Miembro que se conserva en ambos lados del par.</summary>
    public static int KeptMethod(int input) => input + 1;

    // RemovedMethod(string) — RETIRADO a propósito: ruptura de tipo 1.

    /// <summary>Firma ALTERADA a propósito: ruptura de tipo 2.</summary>
    public static int ChangedSignature(string argument) => argument.Length;
}
