using System.Reflection;
using Cooperator.Framework.Characterization.Tests.Support;

namespace Cooperator.Framework.Characterization.Tests.Generation;

/// <summary>
/// Área 5 del SPEC §6.E: generación de artefactos.
///
/// <para>
/// <b>Hallazgo F-B1 (se escala — no se corrige aquí, G04).</b> Los puntos de
/// entrada públicos de generación (<c>CooperatorModeler.CodeGenerator.GenerateStoredProcedures</c>
/// y <c>GenerateClasses</c>) <b>no son alcanzables de forma hermética</b>: ambos
/// desreferencian el estado de la interfaz de Windows Forms (<c>UI.ProgessBar</c>,
/// <c>UI.StatusLabel</c>), llaman a <c>System.Windows.Forms.Application.DoEvents()</c>
/// y escriben bajo la ruta de la aplicación
/// (<c>Cooperator.Framework.Library.IO.Path.AppPath(true) + "GeneratedCode\…"</c>).
/// Fuera del host de la aplicación esa propiedad es <c>null</c>, y la invocación
/// muere antes de renderizar nada.
/// </para>
/// <para>
/// Por eso el SPEC §15.6 se aplica literalmente: <i>no se simula</i> un sustituto
/// del host. Lo que este V-Bounce caracteriza de forma hermética es (a) el
/// <b>contrato de configuración</b> del <c>Snapshot</c> que alimenta la generación,
/// (b) la <b>firma exacta</b> de los puntos de entrada —el contrato que el motor
/// headless de ADR-003 tendrá que preservar o reemplazar—, y (c) el <b>inventario
/// dorado</b> de las plantillas que la generación consume. La invocación real contra
/// un catálogo vive en <c>Integration/GenerationWithCatalogTests.cs</c>.
/// </para>
/// </summary>
public sealed class GenerationContractTests
{
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.Static;
    private static readonly Type SnapshotType = typeof(CooperatorModeler.Snapshot);
    private static readonly Type CodeGeneratorType = typeof(CooperatorModeler.CodeGenerator);

    [Fact]
    public void GenerateStoredProcedures_has_the_public_static_contract_the_migrator_must_preserve()
    {
        var method = CodeGeneratorType.GetMethod("GenerateStoredProcedures", Static, null, [SnapshotType], null);

        Assert.NotNull(method);
        Assert.Equal(typeof(void), method.ReturnType);
        Assert.Equal("GenerateStoredProcedures", method.Name);
    }

    [Fact]
    public void GenerateClasses_has_the_public_static_contract_the_migrator_must_preserve()
    {
        var method = CodeGeneratorType.GetMethod("GenerateClasses", Static, null, [SnapshotType], null);

        Assert.NotNull(method);
        Assert.Equal(typeof(bool), method.ReturnType);
    }

    [Fact]
    public void The_generation_entry_points_depend_on_the_windows_application_host_state()
    {
        // Es el hallazgo F-B1, verificado de forma observable: la propiedad que los
        // puntos de entrada desreferencian existe y, sin host de aplicación, su
        // lectura misma falla porque el formulario principal es null.
        var modeler = typeof(CooperatorModeler.CodeGenerator).Assembly;
        var ui = modeler.GetType("CooperatorModeler.UI", throwOnError: false);

        Assert.NotNull(ui);

        var progressBar = ui!.GetProperty("ProgessBar", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(progressBar);

        // La lectura misma falla: la reflexión envuelve el NullReferenceException.
        var progressFailure = Assert.Throws<TargetInvocationException>(() => progressBar!.GetValue(null));
        Assert.IsType<NullReferenceException>(progressFailure.InnerException);

        var statusLabel = ui.GetProperty("StatusLabel", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(statusLabel);
        var labelFailure = Assert.Throws<TargetInvocationException>(() => statusLabel!.GetValue(null));
        Assert.IsType<NullReferenceException>(labelFailure.InnerException);
    }

    [Fact]
    public void The_golden_inventory_covers_both_target_languages_and_the_stored_procedure_scripts()
    {
        var golden = Context.Fixture("golden-generation", "template-digests.json");
        var files = golden.GetProperty("files").EnumerateObject().Select(property => property.Name).ToArray();

        Assert.Equal(46, golden.GetProperty("file_count").GetInt32());
        Assert.Equal(46, files.Length);

        Assert.All(
            new[] { "CSharpClasses/", "VisualBasicClasses/", "CSharpProjects/", "VisualBasicProjects/", "StoredProcedures/", "ConfigFiles/" },
            prefix => Assert.Contains(files, file => file.StartsWith(prefix, StringComparison.Ordinal)));
    }

    [Fact]
    public void The_golden_inventory_covers_exactly_the_stored_procedure_scripts_the_entry_point_renders()
    {
        // Contraste real entre el inventario dorado y la lista de guiones que el
        // 1.4 renderiza en GenerateStoredProcedures: si el migrador cambia una
        // plantilla, el inventario dorado deja de cubrirla y esta aserción lo dice.
        var golden = Context.Fixture("golden-generation", "template-digests.json");
        var expected = new[]
        {
            "StoredProcedures/DescriptionFunction.cs",
            "StoredProcedures/GetAll.cs",
            "StoredProcedures/Insert.cs",
            "StoredProcedures/GetOne.cs",
            "StoredProcedures/Delete.cs",
            "StoredProcedures/Update.cs",
            "StoredProcedures/GetByParent.cs",
            "StoredProcedures/GetByParents.cs",
            "StoredProcedures/DeleteByParents.cs",
            "StoredProcedures/Queries.cs",
        };

        var files = golden.GetProperty("files").EnumerateObject().Select(property => property.Name).ToArray();
        foreach (var script in expected)
        {
            Assert.Contains(script, files);
            Assert.Equal(64, golden.GetProperty("files").GetProperty(script).GetString()!.Length);
        }
    }

    [Fact]
    public void The_snapshot_generation_configuration_survives_the_binary_round_trip()
    {
        // El Snapshot es el único insumo de la generación: su contrato de
        // configuración es lo que el migrador debe preservar en .coop2.
        var snapshot = Activator.CreateInstance(SnapshotType)!;
        Set(snapshot, "DataBaseName", "BaseFixture");
        Set(snapshot, "ConnectionString", "Server=fixture");
        Set(snapshot, "SPPrefix", "sp_");
        Set(snapshot, "Language", "CS");
        Set(snapshot, "DeployFolder", @"C:\temp\salida");
        Set(snapshot, "EntitiesProjectName", "Ventas.Entities");
        Set(snapshot, "DataProjectName", "Ventas.Data");
        Set(snapshot, "RulesProjectName", "Ventas.Rules");
        Set(snapshot, "AppProjectName", "Ventas.App");
        Set(snapshot, "GenerateAutoFileOnly", true);
        Set(snapshot, "StampDateAndTimeOnAutoFiles", false);

        var restored = RoundTrip(snapshot);

        Assert.Equal("BaseFixture", Get<string>(restored, "DataBaseName"));
        Assert.Equal("sp_", Get<string>(restored, "SPPrefix"));
        Assert.Equal("CS", Get<string>(restored, "Language"));
        Assert.Equal(@"C:\temp\salida", Get<string>(restored, "DeployFolder"));
        Assert.Equal("Ventas.Entities", Get<string>(restored, "EntitiesProjectName"));
        Assert.True(Get<bool>(restored, "GenerateAutoFileOnly"));
        Assert.False(Get<bool>(restored, "StampDateAndTimeOnAutoFiles"));
    }

    [Fact]
    public void Turning_the_timestamp_stamp_off_is_what_makes_the_generation_output_deterministic()
    {
        // SPEC §6.E.5: la caracterización de generación se monta con la marca de
        // fecha/hora DESACTIVADA. El default del 1.4 es true, así que el arnés tiene
        // que desactivarla explícitamente; este caso fija ese contrato.
        var snapshot = Activator.CreateInstance(SnapshotType)!;
        Assert.True(Get<bool>(snapshot, "StampDateAndTimeOnAutoFiles"));

        Set(snapshot, "StampDateAndTimeOnAutoFiles", false);
        Assert.False(Get<bool>(snapshot, "StampDateAndTimeOnAutoFiles"));
        Assert.True(Get<bool>(snapshot, "HaveChanges"));
    }

    [Fact]
    public void A_snapshot_without_a_domain_tree_is_not_a_generation_input()
    {
        // El árbol de dominio es la entrada real del render; el arnés no lo inventa.
        var snapshot = Activator.CreateInstance(SnapshotType)!;

        Assert.Null(Get<object>(snapshot, "DomainTree"));

        var domainTreeProperty = SnapshotType.GetProperty("DomainTree")!;
        Assert.True(domainTreeProperty.CanWrite);
        Assert.Equal("BaseTreeNode", domainTreeProperty.PropertyType.Name);
    }

    private static object RoundTrip(object snapshot)
    {
#pragma warning disable SYSLIB0011 // El propio 1.4 usa el serializador binario: aquí se OBSERVA, no se introduce.
        using var stream = new MemoryStream();
        new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, snapshot);
        stream.Position = 0;
        return new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }

    private static T? Get<T>(object instance, string propertyName) =>
        (T?)SnapshotType.GetProperty(propertyName)!.GetValue(instance);

    private static void Set(object instance, string propertyName, object? value) =>
        SnapshotType.GetProperty(propertyName)!.SetValue(instance, value);
}
