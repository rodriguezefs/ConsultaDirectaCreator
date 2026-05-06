using ConsultaDirectaManager.Util;
using System.Collections.ObjectModel;
using System.Windows;

namespace ConsultaDirectaManager
{
    /// <summary>
    /// Interaction logic for DlgPmt.xaml
    /// </summary>
    public partial class DlgPmt : Window
    {

        public ObservableCollection<Pmts> Pmts { get; set; } = new ObservableCollection<Pmts>();
        public bool EsOk { get; set; }

        public DlgPmt(string cfgIni)
        {
            InitializeComponent();
            var lxMainWindow = (MainWindow)Application.Current.MainWindow;

            Pmts = PmtsCrg(cfgIni);
            gridPmts.ItemsSource = Pmts;
        }

        private ObservableCollection<Pmts> PmtsCrg(string cfgIni)
        {
            var Pmts = new ObservableCollection<Pmts>();
            int lxNumPmt = 1;
            while (true)
            {
                string pmt = IniRead.ValorObtenerDesdeTexto(cfgIni, "Pmt", $"Pmt{lxNumPmt}");

                if (string.IsNullOrEmpty(pmt))
                    break;
                var lxPmtParts = pmt.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string lxPmtDsc = lxPmtParts.Length > 0 ? lxPmtParts[0].Trim() : string.Empty;
                string lxPmtName = lxPmtParts.Length > 1 ? lxPmtParts[1].Trim() : string.Empty;
                string lxPmtVlr = lxPmtParts.Length > 3 ? lxPmtParts[3].Trim() : string.Empty;

                Pmts.Add(new Pmts
                {
                    PmtDsc = lxPmtDsc,
                    PmtName = lxPmtName,
                    PmtVlr = lxPmtVlr
                });

                lxNumPmt++;
            }


            return Pmts;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            EsOk = true;
            Close();
        }

        private void btnCnc_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
