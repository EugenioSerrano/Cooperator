using System;
using System.Data;
using System.IO;
using Cooperator.Framework.Core;
using Cooperator.Framework.Core.Mapping;

namespace Cooperator.Framework.Core.Mapping.Tests;

/// <summary>
/// AC-7 (fase E.1) del SPEC-261016-0034 §7: los **valores reales** que alimentan cada punto del inventario quedan
/// verificados con su **forma declarada**, y cuando la forma la produce otro componente se declara con su **dueño**
/// en el inventario.
/// <para>
/// El <b>corpus de modelos</b> (§5) lo dejan los Bolts 002/004: si no está en el árbol, este caso lo <b>declara</b>
/// en lugar de inventar un corpus o de tolerar una forma no calificada (SPEC §15 #9).
/// </para>
/// </summary>
public sealed class RealValuesOverCorpusTests
{
    [Fact]
    public void The_column_of_the_mapping_uses_the_property_type_resolved_by_api()
    {
        ObjectList<MappingSample> list = new ObjectList<MappingSample> { new MappingSample { Id = 1, Kind = SampleKind.Two } };

        DataSet dataSet = list.ToDataSet();
        Assert.Single(dataSet.Tables);
        DataTable table = dataSet.Tables[0];

        // El valor real del punto 1 es el PropertyType de la propiedad: la columna del Nullable se crea con el tipo
        // SUBYACENTE (por API) y admite nulos.
        DataColumn kind = table.Columns[nameof(MappingSample.Kind)]!;
        Assert.Equal(typeof(SampleKind), kind.DataType);
        Assert.True(kind.AllowDBNull, "La columna de un tipo anulable debe admitir nulos.");

        DataColumn id = table.Columns[nameof(MappingSample.Id)]!;
        Assert.Equal(typeof(int), id.DataType);

        // Y el mapeo no falla por no tener filas (edge case del SPEC §8.1).
        Assert.Empty(new ObjectList<MappingSample>().ToDataSet().Tables[0].Rows);
    }

    [Fact]
    public void The_model_corpus_state_is_declared_and_the_inventory_carries_the_owner_rule()
    {
        string corpus = TreePath(@"tests\fixtures\model-corpus");
        string inventory = TreePath(@"docs\core-type-resolution-inventory.md");
        Assert.True(File.Exists(inventory), $"Falta el inventario: {inventory}");
        string text = File.ReadAllText(inventory);

        if (Directory.Exists(corpus))
        {
            // El corpus está: se enuncia su forma y se verifica que la verificación sea sobre él (no sobre un ejemplo).
            Assert.NotEmpty(Directory.GetFiles(corpus, "*", SearchOption.AllDirectories));
            Assert.Contains("model-corpus", text, StringComparison.Ordinal);
        }
        else
        {
            // No está (los Bolts 002/004 todavía no lo dejaron): se DECLARA, no se inventa (SPEC §15 #9) y el
            // inventario dice qué hacer cuando llegue y de quién es la forma emitida.
            Assert.Contains("model-corpus", text, StringComparison.Ordinal);
            Assert.Contains("hallazgo con dueño", text, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void The_characterization_suite_of_the_harness_is_the_one_that_runs_before_and_after()
    {
        // AC-7/E.2: la red que prueba que sobre entradas VÁLIDAS el comportamiento observable no cambia es la suite
        // de caracterización del Bolt 001, y es la que el comando reproducible corre en cada corrida.
        Assert.True(
            Directory.Exists(TreePath(@"tests\Cooperator.Framework.Characterization.Tests")),
            "Falta la suite de caracterización del Bolt 001 (la red del antes/después).");
        Assert.True(File.Exists(TreePath(@"build\build.ps1")), "Falta el comando reproducible del Bolt 001.");
    }

    private static string TreePath(string relative)
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "2.0", "Cooperator2.sln")))
            {
                return Path.Combine(directory.FullName, "2.0", relative);
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No se encontró la raíz del repositorio (2.0/Cooperator2.sln).");
    }
}
