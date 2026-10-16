namespace Cooperator.Framework.Library.Storage;

/// <summary>
/// Destino del almacén de configuración de usuario. El contrato del 1.4 distinguía dos: el usuario que ejecuta
/// (<c>CurrentUser</c>) y la máquina (<c>LocalMachine</c>); se preservan ambos como parte de la semántica del
/// almacén (SPEC-261016-0037 §6/D.1).
/// </summary>
public enum UserSettingsStoreDestination
{
    /// <summary>Configuración del usuario que ejecuta la aplicación (el destino por defecto del 1.4).</summary>
    CurrentUser = 0,

    /// <summary>Configuración de la máquina (compartida por todos los usuarios).</summary>
    LocalMachine = 1,
}

/// <summary>
/// <b>El contrato portable del almacén de configuración de usuario</b> (ADR-001 §3.5): la capacidad que
/// sobrevivió a la evaluación de uso real —tiene uso verificado en el consumidor— detrás de una
/// <b>abstracción sin dependencias</b>, nunca detrás de un TFM Windows.
/// <para>
/// <b>Qué resuelve.</b> Leer, escribir y borrar valores por <b>dueño</b> (el nombre de la aplicación o de la
/// sección) y <b>parámetro</b>, con el <b>tipo</b> del valor (texto, decimal, fecha o booleano).
/// </para>
/// <para>
/// <b>Dónde vive la implementación.</b> Fuera de la librería: el framework sólo declara el contrato y el punto
/// de fijación (<see cref="UserSettingsStore"/>); el <b>sustituto</b> del almacén del registro del 1.4 —el
/// almacén de usuario en <c>%APPDATA%</c> o su equivalente— es del <b>Bolt 010</b>, que lo implementa y lo fija.
/// </para>
/// <para>
/// <b>El contrato de claves es DATO, no implementación.</b> La derivación de la clave está en
/// <see cref="UserSettingsKey"/> y es <b>byte a byte</b> la del 1.4 (incluido el separador): si cambiara un solo
/// byte, las claves ya persistidas dejarían de encontrarse y el fallo sería <b>silencioso</b> del lado del
/// cliente (REV-001 F-48). Una implementación que persista el almacén <b>tiene</b> que usar esa derivación.
/// </para>
/// <para>
/// <b>El camino de ausencia es explícito.</b> Leer un valor que no existe devuelve <c>null</c> (nunca un valor
/// por defecto silencioso) y <see cref="Exists"/> lo dice sin ambigüedad.
/// </para>
/// </summary>
public interface IUserSettingsStore
{
    /// <summary>Destino de este almacén (usuario o máquina).</summary>
    UserSettingsStoreDestination Destination { get; }

    /// <summary>Si el valor del dueño y parámetro indicados existe en el almacén.</summary>
    bool Exists(string owner, string parameter);

    /// <summary>Lee un valor de texto, o <c>null</c> si no existe (camino de ausencia explícito).</summary>
    string? ReadText(string owner, string parameter);

    /// <summary>Lee un valor decimal, o <c>null</c> si no existe.</summary>
    decimal? ReadDecimal(string owner, string parameter);

    /// <summary>Lee un valor de fecha, o <c>null</c> si no existe.</summary>
    DateTime? ReadDate(string owner, string parameter);

    /// <summary>Lee un valor booleano, o <c>null</c> si no existe.</summary>
    bool? ReadBoolean(string owner, string parameter);

    /// <summary>Escribe un valor de texto.</summary>
    void WriteText(string owner, string parameter, string value);

    /// <summary>Escribe un valor decimal.</summary>
    void WriteDecimal(string owner, string parameter, decimal value);

    /// <summary>Escribe un valor de fecha.</summary>
    void WriteDate(string owner, string parameter, DateTime value);

    /// <summary>Escribe un valor booleano.</summary>
    void WriteBoolean(string owner, string parameter, bool value);

    /// <summary>Borra el valor del dueño y parámetro indicados.</summary>
    void Delete(string owner, string parameter);

    /// <summary>Borra todos los valores de un dueño.</summary>
    void DeleteOwner(string owner);
}
