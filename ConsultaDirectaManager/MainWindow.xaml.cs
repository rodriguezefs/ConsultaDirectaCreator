using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using SharpCompress.Readers;
using SharpCompress.Common;
using Path = System.IO.Path;

namespace ConsultaDirectaManager {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void AbrirArchivo() {
            OpenFileDialog lxDlg = new OpenFileDialog();
            lxDlg.Filter = "Todos (*.*)|*.*|Latis (*.latis)|*.latis|Zip (*.zip)|*.zip";

            if (lxDlg.ShowDialog() == true) {
                string lxNomArch = lxDlg.FileName;
                txtArchLatis.Text = lxNomArch;

                string lxFldrDst = Path.Combine(@"C:\temp\", Path.GetRandomFileName());

                ExtraerTodoslosArchivos(lxNomArch, lxFldrDst);

                string lxCfgFile = Path.Combine(lxFldrDst, "cfg.ini");
                string lxCfgText = "";
                if (File.Exists(lxCfgFile)) {
                    lxCfgText = File.ReadAllText(lxCfgFile);
                    txtCfg.Text = lxCfgText;
                }

                string lxNomScript = lxCfgText.Split('\r')
                                              .FirstOrDefault(x => x.ToLower().Contains("script"))
                                              .ToString()
                                              .Split('=')[1];

                string lxNomScriptFile = Path.Combine(lxFldrDst, lxNomScript);
                if (File.Exists(lxNomScriptFile)) {
                    string lxScriptText = File.ReadAllText(lxNomScriptFile);
                    txtSQL.Text = lxScriptText;
                }
            }
        }

        private void ExtraerTodoslosArchivos(string NomArch, string FldrDst) {

            if (!Directory.Exists(FldrDst)) { Directory.CreateDirectory(FldrDst); }

            using (Stream stream = File.OpenRead(NomArch)) {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry()) {
                    if (!reader.Entry.IsDirectory) {
                        Console.WriteLine(reader.Entry.Key);

                        reader.WriteEntryToDirectory(FldrDst, new ExtractionOptions() { ExtractFullPath = false, Overwrite = true });
                    }
                }
            }
        }

        private void CmdAbir_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CmdAbir_Executed(object sender, ExecutedRoutedEventArgs e) {
            AbrirArchivo();
        }

        private void CmdGuardar_CanExecute(object sender, CanExecuteRoutedEventArgs e) {

        }

        private void CmdGuardar_Executed(object sender, ExecutedRoutedEventArgs e) {

        }

        private void CmdSalir_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CmdSalir_Executed(object sender, ExecutedRoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsultaDirectaManager.Resources.sql.xshd")) {
                using (var reader = new System.Xml.XmlTextReader(stream)) {
                    txtSQL.SyntaxHighlighting =
                        HighlightingLoader.Load(reader,
                        HighlightingManager.Instance);
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsultaDirectaManager.Resources.ini.xshd")) {
                using (var reader = new System.Xml.XmlTextReader(stream)) {
                    txtCfg.SyntaxHighlighting =
                        HighlightingLoader.Load(reader,
                        HighlightingManager.Instance);
                }
            }
        }
    }
}
