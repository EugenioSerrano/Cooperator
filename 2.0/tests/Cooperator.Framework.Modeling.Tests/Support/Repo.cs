namespace Cooperator.Framework.Modeling.Tests.Support;

/// <summary>
/// Localización del árbol del repositorio. Mismo criterio que el
/// <c>Support/Repo.cs</c> del arnés del Bolt 001: la raíz se descubre caminando
/// hacia arriba hasta la solución <c>2.0/Cooperator2.sln</c>, nunca por una ruta
/// absoluta codificada.
/// </summary>
internal static class Repo
{
    public static string Root { get; } = LocateRoot();

    public static string TreeRoot => Path.Combine(Root, "2.0");

    public static string InTree(params string[] parts) =>
        parts.Length == 0 ? TreeRoot : Path.Combine([TreeRoot, .. parts]);

    public static string InRepo(params string[] parts) =>
        parts.Length == 0 ? Root : Path.Combine([Root, .. parts]);

    private static string LocateRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "2.0", "Cooperator2.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"No se encontró la raíz del repositorio (2.0/Cooperator2.sln) partiendo de '{AppContext.BaseDirectory}'.");
    }
}
