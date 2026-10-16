using System.Windows;

namespace ConnectionStringEncriptor
{
    /// <summary>
    /// Arranque de la aplicación (SPEC-261016-0043 §6/A.1). <b>Sin lógica de protección</b>: la protección es del
    /// motor del Bolt 022 y el paso único vive en <c>Protection/ProtectValueStep</c>. La utilidad no elige algoritmo,
    /// no administra claves y no persiste nada.
    /// </summary>
    public partial class App : Application
    {
    }
}
