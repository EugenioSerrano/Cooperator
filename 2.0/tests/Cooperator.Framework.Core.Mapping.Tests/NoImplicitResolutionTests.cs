using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cooperator.Framework.Core.Mapping;

namespace Cooperator.Framework.Core.Mapping.Tests;

/// <summary>
/// AC-2, AC-6 y AC-1 del SPEC-261016-0034 §7 (ADR-006 §3.1): el camino de mapeo **no** contiene ninguna forma de
/// resolución implícita —**0** identificación por nombre simple, **0** reconstrucción de nombres por manipulación
/// de cadenas de reflexión, **0** cargas de ensamblado— y la regla de nombres aplicada es **una sola** (la
/// calificada), la misma del resolutor de providers del Bolt 005; el **inventario** la declara (AC-1).
/// </summary>
public sealed class NoImplicitResolutionTests
{
    /// <summary>Los archivos del camino de mapeo (el aporte y su gemelo), relativos a la raíz del árbol 2.0.</summary>
    private static readonly string[] MappingPathFiles =
    [
        @"src\Cooperator.Framework.Core\BaseObjectList.cs",
        @"src\Cooperator.Framework.Core\ObjectListHelper.cs",
        @"src\Cooperator.Framework.Core\Mapping\TypeIdentification.cs",
    ];

    [Fact]
    public void The_mapping_path_has_zero_implicit_resolution()
    {
        foreach (string relative in MappingPathFiles)
        {
            string path = TreePath(relative);
            Assert.True(File.Exists(path), $"Falta un archivo del camino de mapeo: {path}");
            string source = File.ReadAllText(path);

            // 0 resolución de tipos por nombre (la única que existe es la calificada y vive en TypeIdentification).
            if (!relative.Contains(@"Mapping\", StringComparison.Ordinal))
            {
                Assert.DoesNotContain("Type.GetType", source, StringComparison.Ordinal);
                Assert.DoesNotContain("GetType(strPropType", source, StringComparison.Ordinal);
                // 0 reconstrucción de nombres por manipulación de cadenas de reflexión.
                Assert.DoesNotContain("Substring(18", source, StringComparison.Ordinal);
                Assert.DoesNotContain("PropertyType.ToString()", source, StringComparison.Ordinal);
                // 0 búsqueda cruda de propiedades por nombre: la verificación es la compartida.
                Assert.DoesNotContain("GetProperty(", source, StringComparison.Ordinal);
            }

            // 0 cargas de ensamblado en TODO el camino (esa capacidad vive en el Bolt 005 con su contrato).
            Assert.DoesNotContain("Assembly.Load", source, StringComparison.Ordinal);
            Assert.DoesNotContain("LoadFrom", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_single_name_rule_rejects_an_unqualified_name_behaviourally()
    {
        // El control no es sólo textual: la resolución por nombre del camino RECHAZA lo no calificado.
        Assert.Throws<Exceptions.TypeResolutionException>(
            () => TypeIdentification.ResolveQualifiedName("System.String", "prueba:NoImplicitResolutionTests"));
        Assert.Throws<Exceptions.TypeResolutionException>(
            () => TypeIdentification.ResolveQualifiedName("MappingSample", "prueba:NoImplicitResolutionTests"));

        // Y el nombre calificado sí resuelve (la forma admitida es una sola y funciona).
        Assert.Equal(
            typeof(MappingSample),
            TypeIdentification.ResolveQualifiedName(typeof(MappingSample).AssemblyQualifiedName!, "prueba:NoImplicitResolutionTests"));
    }

    [Fact]
    public void The_inventory_declares_every_point_and_the_single_name_rule()
    {
        string inventory = TreePath(@"docs\core-type-resolution-inventory.md");
        Assert.True(File.Exists(inventory), $"Falta el inventario de puntos de resolución: {inventory}");
        string text = File.ReadAllText(inventory);

        // AC-1: los tres puntos (los dos del Bolt y el gemelo) y su resolución declarada.
        Assert.Contains("BaseObjectList.cs:409", text, StringComparison.Ordinal);
        Assert.Contains("BaseObjectList.cs:342", text, StringComparison.Ordinal);
        Assert.Contains("ObjectListHelper.cs:64", text, StringComparison.Ordinal);
        Assert.Contains("calificad", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ADR-006", text, StringComparison.Ordinal);
        Assert.Contains("F-47", text, StringComparison.Ordinal);

        // La frontera declarada (el filtro de las vistas queda fuera, con su dueño).
        Assert.Contains("BaseObjectListView.cs:68", text, StringComparison.Ordinal);
    }

    private static string TreePath(string relative)
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            string candidate = Path.Combine(directory.FullName, "2.0", "Cooperator2.sln");
            if (File.Exists(candidate))
            {
                return Path.Combine(directory.FullName, "2.0", relative);
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No se encontró la raíz del repositorio (2.0/Cooperator2.sln).");
    }
}
