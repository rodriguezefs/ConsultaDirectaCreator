using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConsultaDirectaManager.Cmds {
    public static class Cmds {

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
                new KeyGesture(Key.G, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand Salir = new RoutedUICommand(
            "Salir",
            "cmdSalir",
            typeof(Cmds),
            new InputGestureCollection() {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
        });
    }
}
