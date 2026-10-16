using System.Reflection;
using Cooperator.Framework.Modeling.Model;
using Cooperator.Framework.Modeling.Tests.Support;

namespace Cooperator.Framework.Modeling.Tests;

/// <summary>
/// SPEC-261016-0004 §6/B.2 (AC-6): la partición de <c>DomainTreeHelper</c>.
/// <para>
/// La mitad de <b>árbol</b> —los 11 métodos puros y el caché de namespaces— vive
/// <b>dentro</b> del paquete portable; la mitad <b>reverse</b> —los 3 métodos que
/// instancian la factory concreta, reciben una cadena de conexión o toman tipos del
/// esquema de base de datos— <b>no</b> está en el paquete (se reubica con la fase D,
/// obligación registrada). Y los <b>cuatro</b> métodos que el modelo invoca en sus
/// <b>ocho</b> sitios se resuelven dentro del paquete, que es lo que hace que el
/// paquete no tenga ninguna arista saliente.
/// </para>
/// </summary>
public sealed class DomainTreeHelperPartitionTests
{
    private const string HelperFullName = "Cooperator.Framework.Modeling.Model.DomainTreeHelper";

    private static readonly string[] TreeHalfMethods =
    [
        "GetDescriptionFieldNameForEntity",
        "GetGenerateAsForEntity",
        "GetFullGenerateAsForEntity",
        "GetGenerateAsForProperty",
        "FindPropertyInTree",
        "GetNamespaceCollection",
        "ResetNamespaceCollection",
        "FindEntityInTree",
        "CorrectCLRType",
        "CorrectSelectedCollections",
        "CorrectGenerationNamesForRelatedFields",
    ];

    private static readonly string[] ReverseHalfMethods =
    [
        "UpdateDomainTree",
        "GetDomainTree",
        "ADDNodeFromDBObject",
    ];

    [Fact]
    public void The_tree_half_lives_in_the_package_with_its_eleven_methods()
    {
        var helper = typeof(DomainTreeHelper);
        Assert.Equal(HelperFullName, helper.FullName);
        Assert.True(helper.IsAbstract && helper.IsSealed, "DomainTreeHelper debe seguir siendo una clase estática.");

        var methods = helper.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(method => method.Name)
            .ToArray();

        Assert.Equal(TreeHalfMethods.OrderBy(name => name, StringComparer.Ordinal).ToArray(), methods.OrderBy(name => name, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void The_reverse_half_and_the_concrete_provider_do_not_exist_in_the_package()
    {
        var helper = typeof(DomainTreeHelper);
        Assert.Null(helper.GetMethod(ReverseHalfMethods[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
        Assert.Null(helper.GetMethod(ReverseHalfMethods[1], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
        Assert.Null(helper.GetMethod(ReverseHalfMethods[2], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));

        var typeNames = helper.Assembly.GetTypes().Select(type => type.Name).ToArray();
        foreach (var forbidden in new[] { "MSSQLReverseFactory", "MSSQLProvider", "MSSQLCommon", "SP_HelpTablesEnum", "DataAccess", "PropertyEditors" })
        {
            Assert.DoesNotContain(forbidden, typeNames);
        }

        // La mitad reverse es la que recibe la cadena de conexión: no hay ninguna
        // firma del paquete que la acepte.
        var connectionStringMethods = typeof(DomainTreeHelper).Assembly.GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            .Where(method => method.GetParameters().Any(parameter => parameter.ParameterType == typeof(string) && parameter.Name == "connectionString"))
            .Select(method => method.Name)
            .ToArray();

        Assert.Empty(connectionStringMethods);
    }

    [Fact]
    public void The_eight_model_sites_resolve_inside_the_package_and_recalculate_the_derived_values()
    {
        var (root, entity, collection) = BuildTwoEntityGraph();

        // Sitios 4 a 8 (`GenerateAsType` → `GetFullGenerateAsForEntity` + `FindEntityInTree`):
        // fijar el tipo de colección recalcula el tipo CLR del nodo con la mitad de árbol
        // del helper, que vive en el paquete.
        var relatedFullGenerateAs = DomainTreeHelper.GetFullGenerateAsForEntity(root, "Order");
        Assert.Equal("OrderList", collection.CLRType);
        collection.GenerateAsType = CollectionTypeEnum.EntityList;

        // El setter recalculó el tipo CLR con la mitad de árbol del helper: el valor
        // pasa a ser el "full generate as" de la entidad relacionada + el sufijo del
        // tipo de colección (es el mismo cálculo del 1.4).
        Assert.Equal(relatedFullGenerateAs + "List", collection.CLRType);

        // Sitio 2 (`GenerateAsChildOf` → `CorrectSelectedCollections`): el setter sólo
        // acepta valores que estén entre los candidatos a padre de la entidad, y al
        // fijarlo refresca las colecciones — una colección hacia una entidad que no es
        // hija de la actual deja de generar la propiedad.
        collection.GenerateProperty = true;
        entity.ParentCandidates = ["OtroPadre"];
        entity.GenerateAsChildOf = "OtroPadre";
        Assert.Equal("OtroPadre", entity.GenerateAsChildOf);
        Assert.False(collection.GenerateProperty);

        // Sitios 1 y 3 (`GenerateAs` → `CorrectCLRType`): el setter no lanza, la
        // resolución ocurre dentro del paquete y el nodo queda consistente.
        entity.GenerateAs = "OrderLine";
        Assert.Equal("OrderLine", entity.GenerateAs);
        Assert.Contains("OrderLine", entity.FullGenerateAs, StringComparison.Ordinal);

        // El helper es el del PAQUETE (no hay segunda declaración en otro ensamblado).
        Assert.Null(Type.GetType("Cooperator.Framework.Utility.DBReverseHelper.DomainTreeHelper, Cooperator.Framework.Utility"));
    }

    [Fact]
    public void The_namespace_cache_behaves_as_in_the_14()
    {
        // El caché es estático (estado preexistente del 1.4, heredado y declarado en
        // §B.2): el caso lo aísla reseteándolo primero.
        DomainTreeHelper.ResetNamespaceCollection();

        var (root, entity, _) = BuildTwoEntityGraph();
        entity.Namespace = "VentasModelo";
        entity.GenerateEntity = true;

        // El setter del 1.4 ignora los valores que traen punto (los namespace
        // cualificados NO son un namespace de entidad): el valor simple se conserva.
        Assert.Equal("VentasModelo", entity.Namespace);
        Assert.Equal(".VentasModelo", entity.FormattedNamespace);

        var namespaces = DomainTreeHelper.GetNamespaceCollection(root);
        Assert.Contains("VentasModelo", namespaces);

        // El caché se REBUILDEA a demanda: resetearlo lo vacía y la lectura siguiente lo
        // vuelve a poblar desde el árbol (comportamiento del 1.4, heredado y declarado).
        DomainTreeHelper.ResetNamespaceCollection();
        Assert.Contains("VentasModelo", DomainTreeHelper.GetNamespaceCollection(root));

        // Sin entidades que aporten namespace, la lectura queda vacía.
        entity.Namespace = string.Empty;
        entity.GenerateEntity = false;
        DomainTreeHelper.ResetNamespaceCollection();
        Assert.Empty(DomainTreeHelper.GetNamespaceCollection(root));
    }

    /// <summary>
    /// Grafo mínimo con la forma que los setters del modelo esperan: raíz → entidad →
    /// (propiedad de colección hacia otra entidad) → segunda entidad.
    /// </summary>
    private static (EntityNode Root, EntityNode Entity, PropertyNode Collection) BuildTwoEntityGraph()
    {
        var root = new EntityNode(null!, "Model");
        var entity = new EntityNode(root, "OrderLine");
        root.Add(entity);

        var order = new EntityNode(root, "Order");
        root.Add(order);

        var collection = new PropertyNode(
            entity,
            "Orders",
            "OrderList",
            "int",
            4,
            10,
            0,
            isPrimaryKey: false,
            isForeignKey: false,
            isRowVersion: false,
            isNullable: true,
            isAutoGenerated: false,
            isIdentity: false,
            isRowGuid: false,
            isComputed: false,
            isCollection: true,
            isEntity: false,
            isDescriptionField: false,
            relatedTableName: "Order",
            isOneToOneRelation: false,
            isLazyLoadable: false);

        entity.Add(collection);
        return (root, entity, collection);
    }
}
