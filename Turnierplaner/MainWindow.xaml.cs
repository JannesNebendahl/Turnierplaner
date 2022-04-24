using DemoLibary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TurnierLibrary;
using TurnierLibrary.DbAccess;

namespace Turnierplaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Mannschaft> ddlMannschaftenList;
        List<Position> ddlPositionList;
        List<Spieler> ddlKapitaenList;

        public MainWindow()
        {
            InitializeComponent();
            PopulatePositionCheckbox();
            PopulateMannschaften();
            PopulateKapitaen();
        }

        #region DB Befüllen

        #region Mannschaften

        private bool missingMannschaftInput()
        {
            bool ret = false;

            if (String.IsNullOrEmpty(tbxMannschaftenName.Text))
            {
                tbxMannschaftenName.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxMannschaftenName.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");

            if (String.IsNullOrEmpty(tbxMannschaftenKürzel.Text))
            {
                tbxMannschaftenKürzel.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxMannschaftenKürzel.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");

            return ret;
        }

        private void btnAddMannschaft_Click(object sender, RoutedEventArgs e)
        {
            if (missingMannschaftInput())
                return;

            Mannschaft mannschaft = new Mannschaft();

            mannschaft.Name = tbxMannschaftenName.Text;
            mannschaft.Kuerzel = tbxMannschaftenKürzel.Text;

            if(dpEntstehungsjahr.SelectedDate != null)
                mannschaft.Entstehungsjahr = dpEntstehungsjahr.DisplayDate;

            if (ddlMannschaftenKapitaen.Text != null)
            {
                foreach(Spieler spieler in ddlKapitaenList)
                {
                    if (string.Equals(spieler.Name, ddlMannschaftenKapitaen.Text))
                    {
                        mannschaft.Kapitan = spieler.Id;
                    }
                }
            }

            AccessMannschaften.StoreMannschaft(mannschaft);

            tbxMannschaftenName.Text = "";
            tbxMannschaftenKürzel.Text = "";
            dpEntstehungsjahr.SelectedDate = null;
            ddlMannschaftenKapitaen.Text = "";

            PopulateMannschaften();
        }

        private void BindKapitaenDropDown()
        {
            ddlMannschaftenKapitaen.ItemsSource = ddlKapitaenList;
        }

        private void PopulateKapitaen()
        {
            try
            {
                ddlKapitaenList = AccessSpieler.LoadSpielerAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindKapitaenDropDown();
        }

        #endregion Mannschaften

        #region Spieler

        private bool missingSpielerInput()
        {
            bool ret = false;

            if (String.IsNullOrEmpty(tbxSpielerVorname.Text))
            {
                tbxSpielerVorname.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxSpielerVorname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");

            if (String.IsNullOrEmpty(tbxSpielerNachname.Text))
            {
                tbxSpielerNachname.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxSpielerNachname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");

            return ret;
        }

        private void btnAddSpieler_Click(object sender, RoutedEventArgs e)
        {
            if (missingSpielerInput())
                return;

            Spieler spieler = new Spieler();

            spieler.Vorname = tbxSpielerVorname.Text;
            spieler.Nachname = tbxSpielerNachname.Text;
            if ( ! String.IsNullOrEmpty(tbxSpielerTrikotnummer.Text))
            {
                spieler.Trikotnummer = int.Parse(tbxSpielerTrikotnummer.Text);
            }
            if (!String.IsNullOrEmpty(ddlSpielerMannschaften.Text))
            {
                foreach(Mannschaft mannschaft in ddlMannschaftenList)
                {
                    if(string.Equals(mannschaft.Name, ddlSpielerMannschaften.Text))
                    {
                        spieler.MannschaftsId = mannschaft.Id;
                    }
                }
            }
                

            try
            {
                spieler.Id = AccessSpieler.StoreSpieler(spieler);

                if(spieler.Id != null)
                {
                    foreach(Position pos in ddlPositionList)
                    {
                        if (pos.Check_Status)
                        {
                            AccessSpieltAuf.StoreSpieltAuf((int)spieler.Id, (int)pos.Id);
                        }
                    }
                }
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            tbxSpielerVorname.Text = "";
            tbxSpielerNachname.Text = "";
            tbxSpielerTrikotnummer.Text = "";
            PopulatePositionCheckbox();
            ddlSpielerMannschaften.Text = "";

            PopulateKapitaen();
        }

        private void tbxSpielerTrikotnummer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PopulatePositionCheckbox()
        {
            try
            {
                List<Position> dbPositions = AccessPosition.LoadPositionen();

                ddlPositionList = new List<Position>();

                foreach (Position pos in dbPositions)
                {
                    pos.Check_Status = false;
                    ddlPositionList.Add(pos);
                }
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindPositionDropDown();
        }

        private void BindPositionDropDown()
        {
            ddlPosition.ItemsSource = ddlPositionList;
        }

        private void BindMannschaftenDropDown()
        {
            ddlSpielerMannschaften.ItemsSource = ddlMannschaftenList;
        }

        private void PopulateMannschaften()
        {
            try
            {
                ddlMannschaftenList = AccessMannschaften.LoadMannschaftenAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindMannschaftenDropDown();
        }
        #endregion Spieler

        #endregion DB Befüllen

    }
}
