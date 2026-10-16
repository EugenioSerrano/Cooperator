using System;
using System.Windows;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;
using ConnectionStringEncriptor.Protection;

namespace ConnectionStringEncriptor
{
    /// <summary>
    /// La ventana del <b>paso único</b> (SPEC-261016-0043 §6/A.1/§6/B.1): recoge el valor (enmascarado) y la clave de
    /// configuración de destino, invoca <see cref="ProtectValueStep"/> —que delega en el motor del Bolt 022— y
    /// muestra el valor protegido con el <c>kid</c> usado.
    /// <para>
    /// <b>Sin lógica de protección propia:</b> acá no hay algoritmo, ni clave, ni composición del valor. El valor en
    /// claro no se muestra, no se persiste y no se registra; al cerrar la ventana se limpia el estado del paso.
    /// </para>
    /// </summary>
    public partial class ProtectValueWindow : Window
    {
        /// <summary>
        /// Nombre de la variable de entorno de la que la utilidad toma la <b>referencia de la clave activa</b>. El
        /// material lo aporta la provisión del motor (ADR-007 §3.2): la utilidad consume la referencia, nunca la clave.
        /// </summary>
        public const string ActiveKeyReferenceVariable = "Cooperator__ProtectedValues__ActiveKid";

        private ProtectValueStep? _step;

        /// <summary>Inicializa la ventana.</summary>
        public ProtectValueWindow()
        {
            InitializeComponent();
        }

        private void OnProtectClick(object sender, RoutedEventArgs e)
        {
            DiagnosticsText.Text = string.Empty;
            ProtectedValueBox.Text = string.Empty;
            KidText.Text = string.Empty;

            try
            {
                ProtectionResult result = Step().Protect(DestinationKeyBox.Text, ValueBox.Password);
                ProtectedValueBox.Text = result.ProtectedValue;
                KidText.Text = "Protegido con la clave '" + result.Kid + "' (esquema " + result.Scheme + ").";
            }
            catch (Exception exception) when (exception is ProtectedValueException or ArgumentException or InvalidOperationException)
            {
                // El fallo del motor se muestra ACCIONABLE y no hay resultado dudoso (§6/B.2, AC-5).
                DiagnosticsText.Text = ProtectionDiagnostics.Describe(exception);
            }
        }

        /// <summary>
        /// Arma el paso sobre el <b>motor</b> con la provisión de claves por variables de entorno y la referencia de
        /// la clave activa que declara el entorno del operador (no hay selector de clave en la interfaz, AC-4).
        /// </summary>
        /// <returns>El paso único listo para usarse.</returns>
        /// <exception cref="InvalidOperationException">El entorno no declara la referencia de la clave activa.</exception>
        private ProtectValueStep Step()
        {
            if (_step != null)
            {
                return _step;
            }

            EnvironmentValueKeyProvider provider = new EnvironmentValueKeyProvider();
            ProtectedValueKeyRegistry keys = new ProtectedValueKeyRegistry(provider);

            string? activeKid = Environment.GetEnvironmentVariable(ActiveKeyReferenceVariable);
            if (string.IsNullOrWhiteSpace(activeKid))
            {
                throw new InvalidOperationException(
                    "El entorno del operador no declara la referencia de la clave activa: definí la variable " +
                    ActiveKeyReferenceVariable + " con el 'kid' y la variable " +
                    ProtectedValueFormat.EnvironmentKeyPrefix + "<kid> con la clave de 32 bytes en Base64" +
                    " (provisión del motor, ADR-007 §3.2).");
            }

            keys.SetActive(activeKid!);
            _step = new ProtectValueStep(new AesGcmValueResolver(keys));
            return _step;
        }

        /// <summary>
        /// Al cerrarse la ventana se limpia el estado del paso (§6/B.1): el valor en claro no queda retenido y el
        /// paso —con su registro de claves— se descarta.
        /// </summary>
        /// <param name="e">Argumentos del evento.</param>
        protected override void OnClosed(EventArgs e)
        {
            ValueBox.Clear();
            ProtectedValueBox.Clear();
            KidText.Text = string.Empty;
            DiagnosticsText.Text = string.Empty;
            _step = null;

            base.OnClosed(e);
        }
    }
}
