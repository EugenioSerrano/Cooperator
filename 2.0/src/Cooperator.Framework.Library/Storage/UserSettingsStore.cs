using System;

namespace Cooperator.Framework.Library.Storage;

/// <summary>
/// El <b>punto de fijación</b> del almacén de configuración de usuario: el host fija <b>una vez</b> la
/// implementación (el sustituto del Bolt 010) y el resto del producto la consume por acá. Mismo patrón de
/// fijación al arranque que la configuración portable (ADR-004 §3.2) y que el hook de la carga de providers
/// (ADR-006 §3.1).
/// <para>
/// <b>Sin implementación no hay almacén, y se dice.</b> Consumirlo sin haberlo fijado lanza
/// <see cref="InvalidOperationException"/> nombrándolo: el framework no tiene un almacén "por defecto" y no
/// adivina uno (nunca un valor por defecto silencioso).
/// </para>
/// <para>
/// <b>La implementación no vive acá.</b> El sustituto del registro del 1.4 —almacén de usuario en
/// <c>%APPDATA%</c> o equivalente— es del <b>Bolt 010</b>, que lo implementa y lo fija al arrancar; este Bolt
/// declara el contrato (<see cref="IUserSettingsStore"/>) y su derivación de clave
/// (<see cref="UserSettingsKey"/>).
/// </para>
/// </summary>
public static class UserSettingsStore
{
    private static readonly object Gate = new object();
    private static IUserSettingsStore? current;

    /// <summary>El almacén vigente. Fijarlo es una operación del arranque del host.</summary>
    public static IUserSettingsStore Current
    {
        get
        {
            lock (Gate)
            {
                return current ?? throw new InvalidOperationException(
                    "No hay almacén de configuración de usuario fijado: el host debe fijar su implementación "
                    + "(el sustituto del Bolt 010) en UserSettingsStore.Current al arrancar. El framework no tiene "
                    + "un almacén por defecto ni adivina uno (SPEC-261016-0037 §6/C).");
            }
        }

        set
        {
            lock (Gate)
            {
                current = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }

    /// <summary>Si el host ya fijó su almacén.</summary>
    public static bool IsFixed
    {
        get
        {
            lock (Gate)
            {
                return current is not null;
            }
        }
    }

    /// <summary>Quita el almacén fijado (para pruebas y para hosts que recargan su configuración).</summary>
    public static void Reset()
    {
        lock (Gate)
        {
            current = null;
        }
    }
}
