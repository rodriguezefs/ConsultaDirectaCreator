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
using SharpCompress.Archives.Zip;
using SharpCompress.Writers;
using ConsultaDirectaManager.Util;

namespace ConsultaDirectaManager {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private const string CFG_INI = "cfg.ini";

        public MainWindow() {
            InitializeComponent();
        }

        private void ArchivoAbrir() {
            OpenFileDialog lxDlg = new OpenFileDialog();
            lxDlg.Filter = "Todos (*.*)|*.*|Latis (*.latis)|*.latis|Zip (*.zip)|*.zip";

            if (lxDlg.ShowDialog() == true) {
                string lxNomArch = lxDlg.FileName;
                txtArchLatis.Text = lxNomArch;

                string lxFldrDst = Path.Combine(@"C:\temp\", Path.GetRandomFileName());

                ExtraerTodoslosArchivos(lxNomArch, lxFldrDst);

                string lxCfgFile = Path.Combine(lxFldrDst, CFG_INI);
                string lxCfgText = "";
                if (File.Exists(lxCfgFile)) {
                    lxCfgText = File.ReadAllText(lxCfgFile, Encoding.GetEncoding(1252));
                    txtCfg.Text = lxCfgText;
                }

                string lxNomScript = lxCfgText.Split('\r')
                                              .FirstOrDefault(x => x.ToLower().Contains("script"))
                                              .ToString()
                                              .Split('=')[1];

                string lxNomScriptFile = Path.Combine(lxFldrDst, lxNomScript);
                if (File.Exists(lxNomScriptFile)) {
                    string lxScriptText = File.ReadAllText(lxNomScriptFile, Encoding.GetEncoding(1252));
                    txtSQL.Text = lxScriptText;
                }
            }
        }

        private void ArchivoGuardar(string NomArchLatis) {
            //Crear carpteta temporal
            string lxNomCsl = Path.GetFileNameWithoutExtension(NomArchLatis);
            string lxBaseDir = (Path.GetDirectoryName(NomArchLatis));

            string lxDir = Path.Combine(lxBaseDir, lxNomCsl + "_Qry");
            if (!Directory.Exists(lxDir)) { Directory.CreateDirectory(lxDir); }

            //Guardar cfg.ini
            txtCfg.Save(Path.Combine(lxDir, CFG_INI));

            //Guardar script.sql
            txtSQL.Save(Path.Combine(lxDir, lxNomCsl + ".sql"));

            //Comprimir carpeta temporal en formato .zip con extensión .latis
            using (var zip = File.OpenWrite(NomArchLatis)) {
                using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, CompressionType.Deflate)) {
                    string[] lxFileList = Directory.GetFiles(lxDir);
                    foreach (var filePath in lxFileList) {
                        zipWriter.Write(Path.GetFileName(filePath), filePath);
                    }
                }
            }

            //Eliminar directorio temporal
            Directory.Delete(lxDir);
        }

        private void ArchivoNuevo() {
            

            SaveFileDialog lxDlg = new SaveFileDialog();
            lxDlg.DefaultExt = ".latis";
            lxDlg.AddExtension = true;
            lxDlg.Filter = "Todos (*.*)|*.*|Latis (*.latis)|*.latis|Zip (*.zip)|*.zip";

            if (lxDlg.ShowDialog() == true) {
                string lxNomArch = lxDlg.FileName;
                txtArchLatis.Text = lxNomArch;

                string lxNomScript = Path.GetFileNameWithoutExtension(lxNomArch);

                string lxTemplate =
                "[CFG]" + Environment.NewLine +
                "Ver=2" + Environment.NewLine +
                "Dsc=" + Environment.NewLine +
                "Script=" + lxNomScript +".sql" + Environment.NewLine +
                "RtnReg=1" + Environment.NewLine +
                "" + Environment.NewLine +
                "[Ifo]" + Environment.NewLine +
                "Mdl=3200" + Environment.NewLine +
                "NomQry=" + Environment.NewLine +
                "IdQry="+ lxNomScript.Truncate(10) + Environment.NewLine +
                "" + Environment.NewLine +
                "[Pmt]" + Environment.NewLine +
                ";Pmtn=Nombre Publico|Nombre Parametro|Tipo|Default" + Environment.NewLine +
                ";Tipo: Ver cosntantes de ADO" + Environment.NewLine +
                ";      7 = adDate" + Environment.NewLine +
                ";    200 = adVarChar" + Environment.NewLine +
                "Pmt1=Fecha desde|FchDes|7|01/01/2012" + Environment.NewLine +
                "Pmt2=Fecha hasta|FchHas|7|01/01/2012";

                txtCfg.Text = lxTemplate;


                txtSQL.Text = "";
            }
        }

        private void CmdAbir_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CmdAbir_Executed(object sender, ExecutedRoutedEventArgs e) {
            ArchivoAbrir();
        }

        private void CmdGuardar_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = txtCfg.Text.Length > 0 && txtSQL.Text.Length > 0 && txtArchLatis.Text.Length > 0;
        }

        private void CmdGuardar_Executed(object sender, ExecutedRoutedEventArgs e) {
            ArchivoGuardar(txtArchLatis.Text);
        }

        private void CmdNuevo_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CmdNuevo_Executed(object sender, ExecutedRoutedEventArgs e) {
            ArchivoNuevo();
        }

        private void CmdSalir_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CmdSalir_Executed(object sender, ExecutedRoutedEventArgs e) {
            Application.Current.Shutdown();
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
