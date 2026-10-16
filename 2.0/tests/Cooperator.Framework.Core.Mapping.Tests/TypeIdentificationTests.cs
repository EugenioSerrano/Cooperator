using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Cooperator.Framework.Core;
using Cooperator.Framework.Core.Exceptions;
using Cooperator.Framework.Core.Mapping;

namespace Cooperator.Framework.Core.Mapping.Tests;

/// <summary>
/// AC-3 y AC-4 del SPEC-261016-0034 §7 (ADR-006 §3.1, F-47): la identificación es <b>inequívoca</b> —donde el tipo
/// está disponible se usa el tipo (y un <c>Nullable</c> se resuelve <b>por API</b>, no por <c>Substring</c> del
/// <c>ToString()</c>), donde llega un nombre se exige <b>calificado</b>— y un valor que no resuelve produce el
/// <b>error tipado</b> que nombra el valor y el punto, nunca un <c>null</c> silencioso.
/// </summary>
public sealed class TypeIdentificationTests
{
    private const string Origin = "prueba:TypeIdentificationTests";

    [Fact]
    public void A_nullable_of_a_type_of_another_assembly_resolves_by_api_not_by_name()
    {
        // El tipo vive en ESTE ensamblado (el de pruebas): es "otro ensamblado" para Core, que es exactamente el
        // caso que el nombre simple no podía resolver (Type.GetType devuelve null para él).
        Assert.Null(Type.GetType("SampleKind", throwOnError: false));
        Assert.Equal(typeof(SampleKind), TypeIdentification.UnwrapNullable(typeof(SampleKind?), Origin));

        // Y la evidencia del defecto que se corrige: el ToString() del Nullable NO es un nombre de tipo; el
        // Substring(18, len-19) del 1.4 producía un nombre simple que no resolvía.
        Assert.EndsWith("SampleKind", typeof(SampleKind).ToString(), StringComparison.Ordinal);
        Assert.Contains("Nullable", typeof(SampleKind?).ToString(), StringComparison.Ordinal);
        Assert.Equal(typeof(SampleKind), TypeIdentification.UnwrapNullable(typeof(SampleKind?), Origin));

        // Un tipo que no es Nullable se devuelve tal cual, y un tipo ausente es un error tipado.
        Assert.Equal(typeof(int), TypeIdentification.UnwrapNullable(typeof(int), Origin));
        Assert.Throws<TypeResolutionException>(() => TypeIdentification.UnwrapNullable(null, Origin));
    }

    [Fact]
    public void A_qualified_name_resolves_and_a_simple_name_is_rejected_without_trying()
    {
        string qualified = typeof(SampleKind).AssemblyQualifiedName!;

        Assert.Equal(typeof(SampleKind), TypeIdentification.ResolveQualifiedName(qualified, Origin));

        // AC-2/AC-3: un nombre simple se RECHAZA sin intentar resolverlo (0 resolución implícita).
        TypeResolutionException simple = Assert.Throws<TypeResolutionException>(
            () => TypeIdentification.ResolveQualifiedName("SampleKind", Origin));
        Assert.Equal("SampleKind", simple.Value);
        Assert.Equal(Origin, simple.Origin);
        Assert.Contains("SampleKind", simple.Message, StringComparison.Ordinal);
        Assert.Contains("calificad", simple.Message, StringComparison.OrdinalIgnoreCase);

        // Los dos casos negativos del Bolt: un tipo inexistente y un tipo de otro ensamblado nombrado de forma simple.
        TypeResolutionException inexistent = Assert.Throws<TypeResolutionException>(
            () => TypeIdentification.ResolveQualifiedName("Ventas.NoExiste.EsteTipo, Ventas.NoExiste", Origin));
        Assert.Equal("Ventas.NoExiste.EsteTipo, Ventas.NoExiste", inexistent.Value);
        Assert.Throws<TypeResolutionException>(() => TypeIdentification.ResolveQualifiedName(typeof(SampleKind).Name, Origin));
        Assert.Throws<TypeResolutionException>(() => TypeIdentification.ResolveQualifiedName("   ", Origin));
        Assert.Throws<TypeResolutionException>(() => TypeIdentification.ResolveQualifiedName(null, Origin));
    }

    [Fact]
    public void The_property_diagnostic_names_the_property_the_type_and_the_origin()
    {
        PropertyInfo found = TypeIdentification.RequireProperty(typeof(MappingSample), nameof(MappingSample.Name), Origin);
        Assert.Equal(nameof(MappingSample.Name), found.Name);

        TypeResolutionException missing = Assert.Throws<TypeResolutionException>(
            () => TypeIdentification.RequireProperty(typeof(MappingSample), "propiedadQueNoExiste", Origin));

        Assert.Equal("propiedadQueNoExiste", missing.Value);
        Assert.Equal(typeof(MappingSample).FullName, missing.TypeName);
        Assert.Equal(Origin, missing.Origin);
        Assert.Contains("propiedadQueNoExiste", missing.Message, StringComparison.Ordinal);
        Assert.Contains(typeof(MappingSample).FullName!, missing.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(MappingSample.Name), missing.Message, StringComparison.Ordinal);

        Assert.Throws<TypeResolutionException>(() => TypeIdentification.RequireProperty(null, "X", Origin));
        Assert.Throws<TypeResolutionException>(() => TypeIdentification.RequireProperty(typeof(MappingSample), "  ", Origin));
    }

    [Fact]
    public void The_exception_is_a_poco_of_the_core_hierarchy()
    {
        Assert.Equal(typeof(CoreException), typeof(TypeResolutionException).BaseType);

        // POCO: sin atributo de serialización y sin constructor de serialización (ADR-008 §3.4, Bolt 007).
        Assert.Empty(typeof(TypeResolutionException).GetCustomAttributes(typeof(SerializableAttribute), inherit: false));
        Assert.Null(typeof(TypeResolutionException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            modifiers: null));

        // Y conserva los constructores canónicos del framework.
        Assert.NotNull(typeof(TypeResolutionException).GetConstructor(Type.EmptyTypes));
        Assert.NotNull(typeof(TypeResolutionException).GetConstructor([typeof(string)]));
        Assert.NotNull(typeof(TypeResolutionException).GetConstructor([typeof(string), typeof(Exception)]));
    }
}

/// <summary>Tipo de prueba: vive en el ensamblado de pruebas (otro ensamblado para Core) e implementa el contrato del mapeo.</summary>
internal sealed class MappingSample : IObject
{
    public ObjectState State { get; set; }

    public int Id { get; set; }

    /// <summary>Un <c>Nullable</c> de un enum de ESTE ensamblado: el caso que el nombre simple no resolvía.</summary>
    public SampleKind? Kind { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>El contrato de fila pide el valor original: en el doble de prueba, la propia instancia.</summary>
    public IObject OriginalValue() => this;

    public void BeginEdit()
    {
    }

    public void CancelEdit()
    {
    }

    public void EndEdit()
    {
    }
}

/// <summary>Enum del ensamblado de pruebas, usado como tipo subyacente de un <c>Nullable</c>.</summary>
internal enum SampleKind
{
    One = 1,
    Two = 2,
}
