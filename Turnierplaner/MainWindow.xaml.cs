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
            ddlTrainerMannschaft.ItemsSource = ddlMannschaften;
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

        #region Trainer

        private void btnAddTrainer_Click(object sender, RoutedEventArgs e)
        {

            Trainer trainer = new Trainer();

            if (checkTrainerInput())
            {
                try
                {
                    trainer.Vorname = tbxTrainerVorname.Text;
                    trainer.Nachname = tbxTrainerNachname.Text;
                    foreach (Mannschaft mannschaft in ddlMannschaften)
                    {
                        if (string.Equals(mannschaft.Name, ddlTrainerMannschaft.Text))
                        {
                            trainer.Mannschaft = mannschaft.Id;
                        }
                    }
                    trainer.Amtsantritt = dpTrainerAmtsantritt.DisplayDate;
                    AccessTrainer.StoreTrainer(trainer);

                    tbxTrainerVorname.Text = "";
                    tbxTrainerNachname.Text = "";
                    ddlTrainerMannschaft.Text = "";
                    dpTrainerAmtsantritt.SelectedDate = null;
                }
                catch
                {
                    MessageBox.Show("Fehlerhafte Eingabe.");
                }

            }
        }

        private bool checkTrainerInput()
        {
            bool returnValue = true;

            if (!String.IsNullOrEmpty(tbxTrainerVorname.Text)) tbxTrainerVorname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");
            else
            {
                tbxTrainerVorname.BorderBrush = Brushes.Red;
                returnValue = false;
            }
            if (!String.IsNullOrEmpty(tbxTrainerNachname.Text)) tbxTrainerNachname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");
            else
            {
                tbxTrainerNachname.BorderBrush = Brushes.Red;
                returnValue = false;
            }

            if (!String.IsNullOrEmpty(ddlTrainerMannschaft.Text)) ddlTrainerMannschaft.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");
            else
            {
                ddlTrainerMannschaft.BorderBrush = Brushes.Red;
                returnValue = false;
            }
            return returnValue;
        }

        #endregion

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
            else
            {
                tbxSpieltag.BorderBrush = colorNormal;
            }

            if (!String.IsNullOrEmpty(tbxZuschaueranzahl.Text)){
                if (!int.TryParse(tbxZuschaueranzahl.Text, out _))
                {
                    tbxZuschaueranzahl.BorderBrush = Brushes.Red;
                    ret = true;
                }
                else
                {
                    tbxZuschaueranzahl.BorderBrush = colorNormal;
                }
            }

            if (String.IsNullOrEmpty(ddlSpielHeimMannschaften.Text))
            {
                bSpielHeimMannschaftenBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else bSpielHeimMannschaftenBorder.BorderBrush = Brushes.Transparent;

            if (String.IsNullOrEmpty(ddlSpielAuswaertsMannschaften.Text))
            {
                bSpielAuswaertsMannschaftenBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else bSpielAuswaertsMannschaftenBorder.BorderBrush = Brushes.Transparent;

            if (!String.IsNullOrEmpty(ddlSpielAuswaertsMannschaften.Text) && !String.IsNullOrEmpty(ddlSpielHeimMannschaften.Text) && string.Equals(ddlSpielAuswaertsMannschaften.Text, ddlSpielHeimMannschaften.Text) )
            {
                bSpielHeimMannschaftenBorder.BorderBrush = Brushes.Red;
                bSpielAuswaertsMannschaftenBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else if(!String.IsNullOrEmpty(ddlSpielAuswaertsMannschaften.Text) && !String.IsNullOrEmpty(ddlSpielHeimMannschaften.Text) && !string.Equals(ddlSpielAuswaertsMannschaften.Text, ddlSpielHeimMannschaften.Text))
            {
                bSpielHeimMannschaftenBorder.BorderBrush = Brushes.Transparent;
                bSpielAuswaertsMannschaftenBorder.BorderBrush = Brushes.Transparent;
            };

            return ret;
        }

        private void BtnAddSpiel_Click(object sender, RoutedEventArgs e)
        {
            if (missingOrWrongSpielInput())
                return;

            Spiel spiel = new Spiel();

            spiel.Spieltag = int.Parse(tbxSpieltag.Text);
            try { spiel.Zuschauerzahl = int.Parse(tbxZuschaueranzahl.Text); } catch { }

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
                bSpielHeimMannschaftenBorder.BorderBrush = Brushes.Red;
                return;
            }
            if(spiel.Auswaertsmannschaft == null)
            {
                bSpielAuswaertsMannschaftenBorder.BorderBrush = Brushes.Red;
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

        private void tbxZuschaueranzahl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void tbxSpieltag_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        #endregion Spiel

        #endregion Spiel erstellen

        #region DB Zeigen
        private void btnZeigenTabelle_Click(object sender, RoutedEventArgs e)
        {
            string sql = "";

            switch (cbZeigenTabelle.Text)
            {
                case "Mannschaften":
                    sql = "SELECT * " +
                          "FROM Mannschaften";
                    break;
                case "Positionen":
                    sql = "SELECT * " +
                          "FROM Positionen";
                    break;
                case "Schiedsrichter":
                    sql = "SELECT * " +
                          "FROM Schiedsrichter";
                    break;
                case "Spiele":
                    sql = "SELECT * " +
                          "FROM Spiel";
                    break;
                case "Spieler":
                    sql = "SELECT * " +
                          "FROM Spieler";
                    break;
                case "SpieltAuf":
                    sql = "SELECT * " +
                          "FROM SpieltAuf";
                    break;
                default:
                    return;
            }

            try
            {
                SqliteDataAccess.LoadTableInDataGrid(dgZeigenTabelle, sql);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
        }
        #endregion DB Zeigen

        #region Turnier erstellen
        private void btnCreateTurnierJederVsJeden_Click(object sender, RoutedEventArgs e)
        {
            if (KeepExistingTournament())
                return;

            List<Mannschaft> teams = AccessMannschaften.LoadMannschaften();

            if (teams.Count < 2)
            {
                MessageBox.Show("Es werden mindestens 2 Mannschaften benötigt.");
                return;
            }

            Mannschaft? dummy = null;
            List<Spiel> spielplan = new List<Spiel>();
            DateTime? firstSpieltag = dpCreateTrunier1Spieltag.SelectedDate;
            TimeSpan? timeBetweenSpieltagen = CalcutlateTimespanBetweenSpieltag(firstSpieltag, dpCreateTrunierLastSpieltag.SelectedDate, teams.Count);

            AddDummyIfNeeded(ref teams, ref dummy);

            CreateSpielplan(ref teams, ref spielplan, ref firstSpieltag, ref timeBetweenSpieltagen);

            RemoveSpieleMitDummy(dummy, ref spielplan, spielplan);

            PushSpielplanToDb(ref spielplan);

            dpCreateTrunier1Spieltag.SelectedDate = null;
            dpCreateTrunierLastSpieltag.SelectedDate = null;
        }

        private static bool KeepExistingTournament()
        {
            bool ret = false;

            try
            {
                int? countSpiele = AccessSpiel.CountSpiele();
                if (countSpiele == null) 
                    throw new Exception("Unexpected behavior: CountSpiele returned null");

                if (countSpiele > 0)
                {
                    if (MessageBox.Show("Es existiert bereits ein Turnier. Wollen Sie dieses löschen und ein neues Turnier generieren lassen?", "Existens eines Turnieres", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                    {
                        return true;
                    }

                    int? countDeleted = AccessSpiel.CleanSpiele();
                    if (countSpiele == null)
                        throw new Exception("Unexpected behavior: CleanSpiele returned null");

                    if (countDeleted != countSpiele)
                    {
                        if (countDeleted > countSpiele)
                        {
                            throw new Exception("Unexpected behavior: Deleted more Spiele than exist");
                        }
                        else
                        {
                            throw new Exception("Didn't deleted all Spiele. [ExistingSpiele: " + countSpiele + "; DeletedSpiele: " + countDeleted + "]");
                        }
                    }
                }
                   
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            return ret;
        }

        private TimeSpan? CalcutlateTimespanBetweenSpieltag(DateTime? firstDay, DateTime? lastDay, int teamsCount)
        {
            TimeSpan? timeBetweenSpieltagen = null;
            if (firstDay != null && lastDay != null)
            {
                timeBetweenSpieltagen = lastDay - firstDay;
                timeBetweenSpieltagen = timeBetweenSpieltagen / teamsCount;
            }
            else if (dpCreateTrunier1Spieltag.SelectedDate != null)
            {
                timeBetweenSpieltagen = TimeSpan.FromDays(7);
            }
            return timeBetweenSpieltagen;
        }

        private void AddDummyIfNeeded(ref List<Mannschaft> teams, ref Mannschaft? dummy)
        {
            if (teams.Count % 2 == 1)
            {
                dummy = new Mannschaft();
                bool didntfoundFreeId = true;
                int tryId = 0;
                try
                {
                    do
                    {
                        if ((bool)AccessSpiel.IdExist(tryId))
                        {
                            tryId++;
                        }
                        else
                        {
                            didntfoundFreeId = false;
                        }
                    } while (didntfoundFreeId);
                }
                catch (Exception exep)
                {
                    MessageBox.Show(exep.Message);
                }
                dummy.Id = tryId;
                teams.Add(dummy);
            }
        }

        private void CreateSpielplan(ref List<Mannschaft> teams, ref List<Spiel> spielplan, ref DateTime? firstSpieltag, ref TimeSpan? timeBetweenSpieltagen)
        {
            int[] sideA = new int[teams.Count / 2];
            int[] sideB = new int[teams.Count / 2];

            int a = 0;
            int b = 0;
            foreach(Mannschaft mannschaft in teams)
            {
                if(a < teams.Count / 2)
                {
                    sideA[a] = (int)mannschaft.Id;
                    a++;
                }
                else
                {
                    sideB[b] = (int)mannschaft.Id;
                    b++;
                }
            }

            for (int spieltag = 1; spieltag < teams.Count; spieltag++)
            {
                DateTime? spieltagDate = firstSpieltag + timeBetweenSpieltagen * (spieltag - 1);
                CreateSpieltag(teams.Count, sideA, sideB, ref spielplan, spieltag, spieltagDate);
                if (teams.Count > 2)
                    ReArrangeSides(teams.Count / 2, ref sideA, ref sideB);
            }
        }

        private void CreateSpieltag(int teamsCount, int[] sideA, int[] sideB, ref List<Spiel> spielplan, int spieltag, DateTime? spieltagDate)
        {
            for (int i = 0; i < (teamsCount / 2); i++)
            {
                Spiel spiel = new Spiel();
                spiel.Heimmanschaft = sideA[i];
                spiel.Auswaertsmannschaft = sideB[i];
                spiel.Spieltag = spieltag;
                if (spieltagDate != null)
                {
                    spiel.Datum = spieltagDate;
                }
                spielplan.Add(spiel);
            }
        }

        private void ReArrangeSides(int size, ref int[] sideA, ref int[] sideB)
        {
            int temp = sideA[size - 1];
            for (int i = size - 1; i > 1; i--)
            {
                sideA[i] = sideA[i - 1];
            }
            sideA[1] = sideB[0];
            for (int i = 0; i < size - 1; i++)
            {
                sideB[i] = sideB[i + 1];
            }
            sideB[size - 1] = temp;
        }

        private void RemoveSpieleMitDummy(Mannschaft? dummy, ref List<Spiel> spielplan, List<Spiel> tempSpielplan)
        {
            if (dummy == null)
                return;

            spielplan.RemoveAll(spiel => spiel.Heimmanschaft == dummy.Id || spiel.Auswaertsmannschaft == dummy.Id);
            
        }

        private void PushSpielplanToDb(ref List<Spiel> spielplan)
        {
            try
            {
                foreach(Spiel spiel in spielplan)
                {
                    AccessSpiel.StoreSpiel(spiel);
                }
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
        }
        #endregion

        #region Spielplan zeigen
        private void btnRefreshSpielplan_Click(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT S.Spieltag, S.Datum, H.Name AS Heim, A.Name AS Gast " +
                         "FROM Spiel S, Mannschaften H, Mannschaften A " +
                         "WHERE S.HeimmannschaftsID == H.Id AND S.AuswaertsmannschaftsID == A.Id " +
                         "ORDER BY S.Spieltag;";
            SqliteDataAccess.LoadTableInDataGrid(dgSpielplan, sql);
        }
        #endregion Spielplan zeigen

    }
}
