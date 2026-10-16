using System;
using System.Collections.Generic;
using System.Data;
using Cooperator.Framework.Core;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.Mapping.Tests;

/// <summary>
/// AC-5 del SPEC-261016-0034 §7: el diagnóstico de la **propiedad** es accionable en **los dos** puntos del camino
/// —el comparador de orden de <c>ObjectList&lt;T&gt;</c> y su **gemelo** del llenado del <c>DataSet</c> en
/// <c>ObjectListHelper&lt;T, T2&gt;</c>— y el <c>catch (Exception)</c> genérico ya **no** oculta la causa.
/// </summary>
public sealed class PropertyLookupDiagnosticsTests
{
    [Fact]
    public void The_order_comparer_names_the_property_the_type_and_the_point()
    {
        ObjectList<MappingSample> list = new ObjectList<MappingSample>();

        // Antes: ArgumentException("<{0}> property not found.") — sin el tipo ni el punto.
        TypeResolutionException exception = Assert.Throws<TypeResolutionException>(
            () => list.Sort("propiedadQueNoExiste", true));

        Assert.Equal("propiedadQueNoExiste", exception.Value);
        Assert.Equal("BaseObjectList:GenericComparer", exception.Origin);
        Assert.Contains("BaseObjectList:GenericComparer", exception.Message, StringComparison.Ordinal);
        Assert.Contains("propiedadQueNoExiste", exception.Message, StringComparison.Ordinal);

        // Y una propiedad que existe ordena sin excepción (no se rompió el camino válido).
        list.Add(new MappingSample { Id = 2, Name = "b" });
        list.Add(new MappingSample { Id = 1, Name = "a" });
        list.Sort(nameof(MappingSample.Id), true);
        Assert.Equal(1, list[0].Id);
    }

    [Fact]
    public void The_dataset_fill_twin_propagates_the_cause_instead_of_the_generic_error()
    {
        DataSet dataSet = new DataSet();
        DataTable table = dataSet.Tables.Add("Sample");
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("columnaQueNoEsPropiedad", typeof(string));

        ObjectListHelper<MappingSample, ObjectList<MappingSample>> helper =
            new ObjectListHelper<MappingSample, ObjectList<MappingSample>>();
        ObjectList<MappingSample> rows = new ObjectList<MappingSample> { new MappingSample { Name = "a" } };

        // Antes: InvalidDataSetException("The Dataset schema do not match with RowSet") — genérico, sin causa.
        TypeResolutionException exception = Assert.Throws<TypeResolutionException>(() => helper.FillDataSet(dataSet, rows));

        Assert.Equal("columnaQueNoEsPropiedad", exception.Value);
        Assert.Equal("ObjectListHelper:FillDataSet", exception.Origin);
        Assert.Contains("columnaQueNoEsPropiedad", exception.Message, StringComparison.Ordinal);
        Assert.Contains(typeof(MappingSample).FullName!, exception.Message, StringComparison.Ordinal);

        // El error genérico de esquema sigue existiendo para lo que realmente es un desajuste de esquema.
        DataSet wrongShape = new DataSet();
        Assert.Throws<InvalidDataSetException>(() => helper.FillDataSet(wrongShape, rows));
    }
}
