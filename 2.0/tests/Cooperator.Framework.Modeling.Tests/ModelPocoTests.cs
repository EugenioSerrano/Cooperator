using System.Reflection;
using System.Runtime.Serialization;
using Cooperator.Framework.Modeling.Exceptions;
using Cooperator.Framework.Modeling.Model;
using Cooperator.Framework.Modeling.Tests.Support;

namespace Cooperator.Framework.Modeling.Tests;

/// <summary>
/// SPEC-261016-0004 §6/B.1 (AC-2): el modelo del grafo es un <b>POCO</b>.
/// <list type="bullet">
///   <item>no lleva atributos de editor/diseñador ni tipos de UI en su superficie;</item>
///   <item>no hay <c>[Serializable]</c>, <c>ISerializable</c>, <c>GetObjectData</c> ni
///         constructores <c>(SerializationInfo, StreamingContext)</c> en los tipos que
///         migran —incluidas las excepciones del grafo—: su persistencia es el `.coop2`
///         (ADR-008 §3.2);</item>
///   <item>y la base de las excepciones del grafo es <c>System.Exception</c>, porque
///         `Modeling` no tiene aristas salientes hacia `Library` (ADR-001 §3.1).</item>
/// </list>
/// <para>
/// Las dos primeras se verifican por <b>reflexión sobre el ensamblado construido</b>
/// (la superficie pública real, no la fuente) y la tercera también; la ausencia de
/// <b>tipos de UI</b> en las sentencias del modelo se verifica además por lectura de
/// las fuentes del paquete, que es lo que AC-2 pide literalmente.
/// </para>
/// </summary>
public sealed class ModelPocoTests
{
    /// <summary>Atributos de editor/diseñador que el modelo de la 2.0 no puede llevar.</summary>
    private static readonly string[] DesignerAttributes =
    [
        "EditorAttribute",
        "DesignerSerializationVisibilityAttribute",
        "DescriptionAttribute",
        "BrowsableAttribute",
        "DefaultValueAttribute",
        "CategoryAttribute",
    ];

    private static readonly Assembly Package = typeof(EntityNode).Assembly;

    [Fact]
    public void The_model_types_carry_no_designer_or_ui_attributes()
    {
        foreach (var type in Package.GetTypes())
        {
            foreach (var attribute in type.GetCustomAttributesData())
            {
                AssertNotDesigner(attribute, type.FullName ?? type.Name);
            }

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                foreach (var attribute in member.GetCustomAttributesData())
                {
                    AssertNotDesigner(attribute, $"{type.FullName}.{member.Name}");
                }
            }
        }
    }

    [Fact]
    public void No_type_in_the_package_is_serializable_and_there_is_no_serialization_surface()
    {
        foreach (var type in Package.GetTypes())
        {
            // `System.Exception` implementa ISerializable en la BCL: lo que ADR-008 §3.2
            // retira es la DECLARACIÓN propia de serialización en los tipos que migran,
            // no lo que la BCL hereda. Por eso el caso mira la superficie declarada.
            Assert.DoesNotContain(
                type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                method => method.Name is "GetObjectData" or "OnSerializing" or "OnDeserialized" or "OnSerialized" or "OnDeserializing");

            Assert.DoesNotContain(
                type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                constructor => constructor.GetParameters() is [{ ParameterType: var first }, { ParameterType: var second }]
                    && first.FullName == "System.Runtime.Serialization.SerializationInfo"
                    && second.FullName == "System.Runtime.Serialization.StreamingContext");

            Assert.DoesNotContain(
                type.GetCustomAttributesData(),
                attribute => attribute.AttributeType.FullName == "System.SerializableAttribute"
                    // Los tipos generados por el compilador (cierres de expresiones) no
                    // son tipos del port: ADR-008 §3.2 es sobre los tipos que migran.
                    && !type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false));
        }
    }

    [Fact]
    public void The_graph_exceptions_derive_from_System_Exception()
    {
        Assert.Equal(typeof(Exception), typeof(UtilityException).BaseType);
        Assert.Equal(typeof(UtilityException), typeof(UnknownDBTypeExeception).BaseType);
        Assert.Equal(typeof(UtilityException), typeof(NodeAlreadyExistException).BaseType);
        Assert.Equal(typeof(UtilityException), typeof(CannotAddOrRemoveItemException).BaseType);

        Assert.True(typeof(UtilityException).IsAbstract);
    }

    [Fact]
    public void No_statement_of_the_model_names_a_type_of_the_editor_or_of_the_ui()
    {
        // AC-2, tercera cláusula: el acoplamiento al editor retirado (DomainTree.cs L1621
        // del 1.4) desaparece del modelo, y con él los using de UI/diseñador.
        string[] forbidden =
        [
            "System.Windows.Forms",
            "System.Drawing",
            "System.ComponentModel.Design",
            "PropertyEditors",
            "DropDownListEditor",
            "MSSQLReverseFactory",
            "MSSQLProvider",
            // Se ENSAMBLA: el marcador del proveedor de archivos de Microsoft no puede
            // figurar literal en el código propio (AC-3 del SPEC-006, RetiredSurfaceTests).
            "System.Data." + "Ole" + "Db",
        ];

        var files = Directory
            .EnumerateFiles(Repo.InTree("src", "Cooperator.Framework.Modeling"), "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"))
            .ToArray();

        Assert.NotEmpty(files);

        var offenders = files
            .SelectMany(file => File.ReadAllLines(file)
                .Select((line, index) => (File: Path.GetRelativePath(Repo.InTree(), file), Line: index + 1, Text: line))
                .Where(entry => forbidden.Any(token => entry.Text.Contains(token, StringComparison.Ordinal)))
                // Los comentarios que documentan el RETIRO no son acoplamiento: la
                // cláusula es sobre las sentencias del modelo.
                .Where(entry => !entry.Text.TrimStart().StartsWith("//", StringComparison.Ordinal) && !entry.Text.TrimStart().StartsWith("*", StringComparison.Ordinal)))
            .ToArray();

        Assert.Empty(offenders);
    }

    private static void AssertNotDesigner(CustomAttributeData attribute, string where)
    {
        var name = attribute.AttributeType.Name;
        Assert.False(
            DesignerAttributes.Contains(name, StringComparer.Ordinal),
            $"{where} lleva [{name}]: el modelo de la 2.0 es POCO (ADR-001 §3.1, ADR-002 §3.1).");

        Assert.False(
            (attribute.AttributeType.Namespace ?? string.Empty).StartsWith("System.ComponentModel.Design", StringComparison.Ordinal)
            || (attribute.AttributeType.Namespace ?? string.Empty).StartsWith("System.Windows.Forms", StringComparison.Ordinal)
            || (attribute.AttributeType.Namespace ?? string.Empty).StartsWith("System.Drawing", StringComparison.Ordinal),
            $"{where} lleva un atributo del editor/diseñador ({attribute.AttributeType.FullName}).");
    }
}
