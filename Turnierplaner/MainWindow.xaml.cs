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
    public partial class MainWindow : Window
    {
        readonly SolidColorBrush colorNormal = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");

        public MainWindow()
        {
            InitializeComponent();
            PopulatePosition();
            PopulateMannschaften();
            PopulateSpieler();
        }

        #region DataBindings
        List<Spieler> ddlSpieler;
        List<Position> ddlPosition;
        List<Mannschaft> ddlMannschaften;

        private void BindSpielerDropDown()
        {
            cbMannschaftenKapitan.ItemsSource = ddlSpieler;
            cbTransferSpieler.ItemsSource = ddlSpieler;
            cbChangePositionSpieler.ItemsSource = ddlSpieler;
        }

        private void BindPositionDropDown()
        {
            cbSpielerPosition.ItemsSource = ddlPosition;
        }
        private void BindMannschaftenDropDown()
        {
            cbSpielerMannschaften.ItemsSource = ddlMannschaften;
            cbTransferMannschaften.ItemsSource = ddlMannschaften;
            ddlSpielHeimMannschaften.ItemsSource = ddlMannschaften;
            ddlSpielAuswaertsMannschaften.ItemsSource = ddlMannschaften;
        }

        private void PopulateMannschaften()
        {
            try
            {
                ddlMannschaften = AccessMannschaften.LoadMannschaftenAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindMannschaftenDropDown();
        }

        private void PopulateSpieler()
        {
            try
            {
                ddlSpieler = AccessSpieler.LoadSpielerAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindSpielerDropDown();
        }

        private void PopulatePosition()
        {
            try
            {
                List<Position> dbPositions = AccessPosition.LoadPositionen();

                ddlPosition = new List<Position>();

                foreach (Position pos in dbPositions)
                {
                    pos.Check_Status = false;
                    ddlPosition.Add(pos);
                }
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindPositionDropDown();
        }

        #endregion DataBindings

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

            if (cbMannschaftenKapitan.Text != null)
            {
                foreach(Spieler spieler in ddlSpieler)
                {
                    if (string.Equals(spieler.Name, cbMannschaftenKapitan.Text))
                    {
                        mannschaft.Kapitan = spieler.Id;
                    }
                }
            }

            AccessMannschaften.StoreMannschaft(mannschaft);

            tbxMannschaftenName.Text = "";
            tbxMannschaftenKürzel.Text = "";
            dpEntstehungsjahr.SelectedDate = null;
            cbMannschaftenKapitan.Text = "";

            PopulateMannschaften();
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
            if (!String.IsNullOrEmpty(cbSpielerMannschaften.Text))
            {
                foreach(Mannschaft mannschaft in ddlMannschaften)
                {
                    if(string.Equals(mannschaft.Name, cbSpielerMannschaften.Text))
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
                    foreach(Position pos in ddlPosition)
                    {
                        if (pos.Check_Status)
                        {
                            AccessSpieltAuf.AddRelation((int)spieler.Id, (int)pos.Id);
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
            PopulatePosition();
            cbSpielerMannschaften.Text = "";

            PopulateSpieler();
        }

        private void tbxSpielerTrikotnummer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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

        #region DB Ändern

        #region Spieler transferieren

        private bool missingTransferInput()
        {
            bool ret = false;

            if (String.IsNullOrEmpty(cbTransferSpieler.Text))
            {
                bTransferSpielerBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else bTransferSpielerBorder.BorderBrush = Brushes.Transparent;

            if (String.IsNullOrEmpty(cbTransferMannschaften.Text))
            {
                bTransferMannschaftenBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else bTransferMannschaftenBorder.BorderBrush = Brushes.Transparent;

            return ret;
        }

        private void btnTransferSpieler_Click(object sender, RoutedEventArgs e)
        {
            if (missingTransferInput())
                return;

            Mannschaft newMannschaft = (Mannschaft)cbTransferMannschaften.SelectedItem;
            Spieler selectedSpieler = (Spieler)cbTransferSpieler.SelectedItem;

            try
            {
                AccessSpieler.ChangeMannschaft((int)selectedSpieler.Id, (int)newMannschaft.Id);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            tbxTransferAktuelleMannschaft.Text = "";
            cbTransferMannschaften.Text = "";
            cbTransferSpieler.Text = "";
            PopulateSpieler();
        }

        private void cbTransferSpieler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTransferSpieler.SelectedItem == null)
                return;

            Spieler selectedSpieler = (Spieler)cbTransferSpieler.SelectedItem;

            if (selectedSpieler.MannschaftsId == null)
            {
                tbxTransferAktuelleMannschaft.Text = "vereinslos";
            }
            else
            {
                try
                {
                    Mannschaft currentMannschaft = AccessMannschaften.QueryById((int)selectedSpieler.MannschaftsId);
                    tbxTransferAktuelleMannschaft.Text = currentMannschaft.Name;
                }
                catch (Exception exep)
                {
                    MessageBox.Show(exep.Message);
                }
            }
        }

        #endregion Spieler transferieren

        #region Spieler position ändern
        private bool missingPositionChangeInput()
        {
            bool ret = false;

            if (String.IsNullOrEmpty(cbChangePositionSpieler.Text))
            {
                bChangePositionSpielerBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else bChangePositionSpielerBorder.BorderBrush = Brushes.Transparent;

            return ret;
        }

        private void btnChangeSpielerPosition_Click(object sender, RoutedEventArgs e)
        {
            if (missingPositionChangeInput())
                return;

            Spieler selectedSpieler = (Spieler)cbChangePositionSpieler.SelectedItem;
            List<Position> positionsOfPlayer = new List<Position>();
            List<Position> selectedPositions = new List<Position>();

            try
            {
                positionsOfPlayer = AccessSpieltAuf.GetPositions(selectedSpieler);
                selectedPositions = (List<Position>)cbChangePosition.ItemsSource;
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            List<Position> addToSpieltAuf = new List<Position>();
            List<Position> removeFromSpieltAuf = new List<Position>();

            for(int i=0; i<positionsOfPlayer.Count; i++)
            {
                positionsOfPlayer[i].Check_Status = true;
            }

            foreach(Position selPos in selectedPositions)
            {
                if (selPos.Check_Status)
                {
                    if (!positionsOfPlayer.Any(p => p.Id == selPos.Id))
                    {
                        addToSpieltAuf.Add(selPos);
                    }
                }
                else
                {
                    if (positionsOfPlayer.Any(p => p.Id == selPos.Id))
                    {
                        removeFromSpieltAuf.Add(selPos);
                    }
                }
            }

            foreach(Position pos in addToSpieltAuf)
            {
                try
                {
                    AccessSpieltAuf.AddRelation((int)selectedSpieler.Id, (int)pos.Id);
                }
                catch (Exception exep)
                {
                    MessageBox.Show(exep.Message);
                }
            }
            foreach(Position pos in removeFromSpieltAuf)
            {
                try
                {
                    AccessSpieltAuf.DeleteRelation((int)selectedSpieler.Id, (int)pos.Id);
                }
                catch (Exception exep)
                {
                    MessageBox.Show(exep.Message);
                }
            }

            cbChangePositionSpieler.Text = "";
            cbChangePosition.ItemsSource = null;
        }

        private void cbChangePositionSpieler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbChangePositionSpieler.SelectedItem == null)
                return;

            Spieler selectedSpieler = (Spieler)cbChangePositionSpieler.SelectedItem;
            List<Position> positionsOfPlayer = new List<Position>();

            try
            {
                positionsOfPlayer = AccessSpieltAuf.GetPositions(selectedSpieler);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            PopulatePosition();
            List<Position> ddlPositionsOfPlayer = ddlPosition;

            foreach(Position pos in positionsOfPlayer)
            {
                for(int i=0; i<ddlPositionsOfPlayer.Count; i++)
                {
                    if(ddlPositionsOfPlayer[i].Id == pos.Id)
                    {
                        ddlPositionsOfPlayer[i].Check_Status = true;
                        break;
                    }
                }
            }

            cbChangePosition.ItemsSource = ddlPositionsOfPlayer;
        }
        #endregion

        #endregion BD Ändern

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

            foreach (Mannschaft mannschaft in ddlMannschaften)
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

            try
            {
                AccessSpiel.StoreSpiel(spiel);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            tbxSpieltag.Text = "";
            dpDatum.SelectedDate = null;
            tbxZuschaueranzahl.Text = "";
            tbxSpielerTrikotnummer.Text = "";
            ddlSpielHeimMannschaften.Text = "";
            ddlSpielAuswaertsMannschaften.Text = "";
        }

        #endregion Spiel

        #endregion Spiel erstellen

        
    }
}
