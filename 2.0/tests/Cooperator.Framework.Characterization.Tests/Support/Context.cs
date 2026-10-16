using System.Reflection;
using System.Text.Json;
using Cooperator.Framework.Core;

namespace Cooperator.Framework.Characterization.Tests.Support;

/// <summary>
/// Localización del árbol del repositorio y lectura de los fixtures versionados.
/// El proyecto corre sobre la superficie 1.4 congelada: todo lo que se observa
/// sale de los ensamblados del 1.4 (por <c>ProjectReference</c>, sin editar una
/// línea de <c>1.4/**</c>) o de los fixtures producidos por el build 1.4.
/// </summary>
internal static class Context
{
    public static string RepoRoot { get; } = LocateRoot();

    public static string InTree(params string[] parts) => Path.Combine([RepoRoot, "2.0", .. parts]);

    public static string InRepo(params string[] parts) => Path.Combine([RepoRoot, .. parts]);

    public static JsonElement Fixture(string area, string fileName)
    {
        var path = InTree("tests", "fixtures", area, fileName);
        Assert.True(File.Exists(path), $"Falta el fixture versionado {path}.");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.Clone();
    }

    /// <summary>
    /// Carga el ensamblado 1.4 construido por el build reproducible cuando existe,
    /// y si no el que copió el build de este proyecto a su salida. Nunca se edita
    /// el árbol congelado: sólo se lee.
    /// </summary>
    public static Assembly LoadFrozenAssembly(string assemblyName)
    {
        var local = Path.Combine(AppContext.BaseDirectory, assemblyName + ".dll");
        if (File.Exists(local))
        {
            return Assembly.LoadFrom(local);
        }

        var exe = Path.Combine(AppContext.BaseDirectory, assemblyName + ".exe");
        if (File.Exists(exe))
        {
            return Assembly.LoadFrom(exe);
        }

        throw new FileNotFoundException(
            $"No se encontró {assemblyName} junto a este ensamblado de tests ni en el build del 1.4.", local);
    }

    private static string LocateRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "2.0", "Cooperator2.sln")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException(
            $"No se encontró la raíz del repositorio (2.0/Cooperator2.sln) partiendo de '{AppContext.BaseDirectory}'.");
    }
}

/// <summary>
/// Doble de <see cref="IObject"/> para ejercitar el comportamiento observable de
/// <c>ObjectStateHelper</c> sin inventar tipos de producto: la interfaz pública
/// expone el estado y el valor original, y hereda el contrato de edición de
/// <see cref="System.ComponentModel.IEditableObject"/>.
/// </summary>
internal sealed class FakeRow : IObject
{
    public ObjectState State { get; set; }

    public bool BeginEditCalled { get; private set; }

    public bool EndEditCalled { get; private set; }

    public bool CancelEditCalled { get; private set; }

    public IObject OriginalValue() => this;

    public void BeginEdit() => BeginEditCalled = true;

    public void EndEdit() => EndEditCalled = true;

    public void CancelEdit() => CancelEditCalled = true;
}

/// <summary>Doble de <see cref="IUniqueIdentifiable"/> con una clave fija.</summary>
internal sealed class FakeIdentifiable : IUniqueIdentifiable
{
    private readonly object[] _identifier;

    public FakeIdentifiable(params object[] identifier) => _identifier = identifier;

    public object[] Identifier() => _identifier;
}

/// <summary>Doble de un identificable con clave nula, para los caminos de error.</summary>
internal sealed class NullKeyIdentifiable : IUniqueIdentifiable
{
    private static readonly object[] Nulls = [null!, null!];

    public object[] Identifier() => Nulls;
}
