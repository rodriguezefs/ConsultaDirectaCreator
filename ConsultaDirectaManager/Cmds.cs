using System.Windows.Input;

namespace ConsultaDirectaManager.Cmds
{
    public static class Cmds
    {
        public static readonly RoutedUICommand Abrir = new(
            "Abrir",
            "cmdAbrir",
            typeof(Cmds),
            [
                new KeyGesture(Key.A, ModifierKeys.Control)
            ]);

        public static readonly RoutedUICommand Nuevo = new(
            "Nuevo",
            "cmdNuevo",
            typeof(Cmds),
            [
                new KeyGesture(Key.N, ModifierKeys.Control)
            ]);

        public static readonly RoutedUICommand Guardar = new(
            "Guadar",
            "cmdGuardar",
            typeof(Cmds),
            [
                new KeyGesture(Key.S, ModifierKeys.Control)
            ]);

        public static readonly RoutedCommand GuardarComo = new RoutedUICommand(
            "Guardar como...",
            "cmdGuardarComo",
            typeof(Cmds),
            [
                new KeyGesture(Key.S, ModifierKeys.Control  | ModifierKeys.Shift)
            ]);

        public static readonly RoutedUICommand Salir = new(
            "Salir",
            "cmdSalir",
            typeof(Cmds),
            [
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            ]);

        public static readonly RoutedCommand ConectarASQLServer = new RoutedUICommand(
            "Conectar a SQL Server",
            "cmdConectarASqlServer",
            typeof(Cmds),
            [
                new KeyGesture(Key.T, ModifierKeys.Control)
            ]);

        public static readonly RoutedCommand EjecutarSQL = new RoutedUICommand(
            "Ejecutar SQL",
            "cmdEjecutarSQL",
            typeof(Cmds),
            [
                new KeyGesture(Key.F5, ModifierKeys.None)
            ]);
    }
}
