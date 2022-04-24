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
using TurnierLibrary.TabelModel;

namespace Turnierplaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SolidColorBrush colorNormal = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");
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
            else tbxMannschaftenName.BorderBrush = colorNormal;

            if (String.IsNullOrEmpty(tbxMannschaftenKürzel.Text))
            {
                tbxMannschaftenKürzel.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxMannschaftenKürzel.BorderBrush = colorNormal;

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
            else tbxSpielerVorname.BorderBrush = colorNormal;

            if (String.IsNullOrEmpty(tbxSpielerNachname.Text))
            {
                tbxSpielerNachname.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxSpielerNachname.BorderBrush = colorNormal;

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
            BindSpielMannschaftenDropDown();
        }
        #endregion Spieler

        #region Schiedsrichter

        private bool missingSchiedsrichterInput()
        {
            bool ret = false;

            if (String.IsNullOrEmpty(tbxSchiedsrichterVorname.Text))
            {
                tbxSchiedsrichterVorname.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxSchiedsrichterVorname.BorderBrush = colorNormal;

            if (String.IsNullOrEmpty(tbxSchiedsrichterNachname.Text))
            {
                tbxSchiedsrichterNachname.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxSchiedsrichterNachname.BorderBrush = colorNormal;

            return ret;
        }

        private void btnAddSchiedsrichter_Click(object sender, RoutedEventArgs e)
        {
            if (missingSchiedsrichterInput())
                return;

            Schiedsrichter schiedsrichter = new Schiedsrichter();

            schiedsrichter.Vorname = tbxSchiedsrichterVorname.Text;
            schiedsrichter.Nachname = tbxSchiedsrichterNachname.Text;



            try
            {
                AccessSchiedsrichter.StoreSchiedsrichter(schiedsrichter);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            tbxSchiedsrichterVorname.Text = "";
            tbxSchiedsrichterNachname.Text = "";
        }
        #endregion

        #endregion DB Befüllen

        #region Spiel erstellen

        #region Spiel

        private bool missingOrWrongSpielInput()
        {
            bool ret = false;

            if (String.IsNullOrEmpty(tbxSpieltag.Text) || !int.TryParse(tbxSpieltag.Text, out _))
            {
                tbxSpieltag.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxSpieltag.BorderBrush = colorNormal;

            if (!String.IsNullOrEmpty(tbxZuschaueranzahl.Text)){
                if (!int.TryParse(tbxZuschaueranzahl.Text, out _))
                {
                    tbxZuschaueranzahl.BorderBrush = Brushes.Red;
                    ret = true;
                }
                else tbxZuschaueranzahl.BorderBrush = colorNormal; }

            if (String.IsNullOrEmpty(ddlSpielHeimMannschaften.Text))
            {
                ddlSpielHeimMannschaften.BorderBrush = Brushes.Red;
                ret = true;
            }
            else ddlSpielHeimMannschaften.BorderBrush = colorNormal;

            if (String.IsNullOrEmpty(ddlSpielAuswaertsMannschaften.Text))
            {
                ddlSpielAuswaertsMannschaften.BorderBrush = Brushes.Red;
                ret = true;
            }
            else ddlSpielAuswaertsMannschaften.BorderBrush = colorNormal;

            if (string.Equals(ddlSpielAuswaertsMannschaften.Text, ddlSpielHeimMannschaften.Text))
            {
                ddlSpielAuswaertsMannschaften.BorderBrush = Brushes.Red;
                ddlSpielHeimMannschaften.BorderBrush = Brushes.Red;
                ret = true;
            }
            else
            {
                ddlSpielAuswaertsMannschaften.BorderBrush = colorNormal;
                ddlSpielHeimMannschaften.BorderBrush = colorNormal;
            };

            return ret;
        }

        private void BtnAddSpiel_Click(object sender, RoutedEventArgs e)
        {
            if (missingOrWrongSpielInput())
                return;

            Spiel spiel = new Spiel();

            spiel.Spieltag = int.Parse(tbxSpieltag.Text);
            try { spiel.Zuschaueranzahl = int.Parse(tbxZuschaueranzahl.Text); } catch { }

            foreach (Mannschaft mannschaft in ddlMannschaftenList)
            {
                if (string.Equals(mannschaft.Name, ddlSpielHeimMannschaften.Text))
                {
                    spiel.Heimmanschaft = mannschaft.Id;
                }
                else if (string.Equals(mannschaft.Name, ddlSpielAuswaertsMannschaften.Text))
                {
                    spiel.Auswaertsmannschaft = mannschaft.Id;
                }
            }

            if(spiel.Heimmanschaft == null)
            {
                ddlSpielHeimMannschaften.BorderBrush = Brushes.Red;
                return;
            }
            if(spiel.Auswaertsmannschaft == null)
            {
                ddlSpielAuswaertsMannschaften.BorderBrush = Brushes.Red;
                return;
            }

            AccessSpiel.StoreSpiel(spiel);
            tbxSpieltag.Text = "";
            dpDatum.SelectedDate = null;
            tbxZuschaueranzahl.Text = "";
            tbxSpielerTrikotnummer.Text = "";
            ddlSpielHeimMannschaften.Text = "";
            ddlSpielAuswaertsMannschaften.Text = "";
        }

        private void BindSpielMannschaftenDropDown()
        {
            ddlSpielHeimMannschaften.ItemsSource = ddlMannschaftenList;
            ddlSpielAuswaertsMannschaften.ItemsSource = ddlMannschaftenList;
        }

        #endregion Spiel

        #endregion Spiel erstellen
    }
}
