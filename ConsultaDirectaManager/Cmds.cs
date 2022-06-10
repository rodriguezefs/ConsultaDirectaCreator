using System.Windows.Input;

namespace ConsultaDirectaManager.Cmds
{
    public static class Cmds
    {

        public static readonly RoutedUICommand Abrir = new RoutedUICommand(
            "Abrir",
            "cmdAbrir",
            typeof(Cmds),
            new InputGestureCollection() {
                new KeyGesture(Key.A, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand Nuevo = new RoutedUICommand(
            "Nuevo",
            "cmdNuevo",
            typeof(Cmds),
            new InputGestureCollection() {
                new KeyGesture(Key.N, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand Guardar = new RoutedUICommand(
            "Guadar",
            "cmdGuardar",
            typeof(Cmds),
            new InputGestureCollection() {
                new KeyGesture(Key.S, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand Salir = new RoutedUICommand(
            "Salir",
            "cmdSalir",
            typeof(Cmds),
            new InputGestureCollection() {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
        });

        public static readonly RoutedCommand ConectarASQLServer = new RoutedUICommand(
            "Conectar a SQL Server",
            "cmdConectarASqlServer",
            typeof(Cmds),
            new InputGestureCollection()
            {
                new KeyGesture(Key.T, ModifierKeys.Control)
            });

        public static readonly RoutedCommand EjecutarSQL = new RoutedUICommand(
            "Ejecutar SQL",
            "cmdEjecutarSQL",
            typeof(Cmds),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F5, ModifierKeys.None)
            });
    }
}
