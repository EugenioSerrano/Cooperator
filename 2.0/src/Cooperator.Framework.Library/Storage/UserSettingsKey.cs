using System;

namespace Cooperator.Framework.Library.Storage;

/// <summary>
/// <b>La derivación de la clave del almacén de configuración de usuario</b>, byte a byte la del 1.4
/// (SPEC-261016-0037 §6/D.1, REV-001 F-48).
/// <para>
/// <b>Es contrato de datos.</b> En el 1.4 la clave se construía como
/// <c>ownerName.Trim().ToLower() + '¡' + parameterName.Trim().ToLower()</c> —con el carácter <c>U+00A1</c> como
/// separador, que es el <b>único byte no ASCII</b> del archivo que la declaraba— y la lectura del almacén
/// atrapaba el error de E/S y caía al caso «no existe». Si la derivación cambia <b>un solo byte</b>, las claves ya
/// persistidas dejan de encontrarse y el fallo es <b>silencioso</b>: por eso vive acá, como dato portable, y
/// cualquier implementación del almacén la usa tal cual.
/// </para>
/// <para>
/// <b>Limitación heredada y declarada:</b> el plegado a minúsculas usa <c>ToLower()</c> sensible a la cultura
/// del proceso, como el 1.4. Es una <b>limitación del contrato</b>, no un defecto de esta implementación:
/// cambiarla (por <c>ToLowerInvariant</c>, por ejemplo) alteraría las claves ya persistidas en culturas con
/// reglas especiales de plegado, así que se conserva y se documenta.
/// </para>
/// </summary>
public static class UserSettingsKey
{
    /// <summary>El separador del contrato: <c>U+00A1</c> (el único byte no ASCII de la derivación del 1.4).</summary>
    public const char Separator = '\u00A1';

    /// <summary>
    /// Compone la clave de un valor: la forma normalizada del dueño + el separador +
    /// la forma normalizada del parámetro.
    /// </summary>
    /// <param name="owner">Dueño del valor (aplicación o sección).</param>
    /// <param name="parameter">Nombre del parámetro.</param>
    /// <returns>La clave, byte a byte igual a la del 1.4.</returns>
    public static string Compose(string owner, string parameter) =>
        Normalize(owner) + Separator + Normalize(parameter);

    /// <summary>
    /// Normaliza una de las dos partes: recorta los espacios y la pliega a minúsculas — <b>por parte</b>, no
    /// sobre la composición, que es lo que el contrato del 1.4 hace.
    /// </summary>
    /// <param name="part">Parte de la clave (dueño o parámetro).</param>
    /// <returns>La parte normalizada.</returns>
    public static string Normalize(string part) => (part ?? string.Empty).Trim().ToLower();
}
