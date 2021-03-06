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
            PopulateSchiedsrichter();
            PopulateMinutes();
            BindPunktetabelle();
            PopulateSpieltag();
        }

        #region DataBindings
        List<Spieler> ddlSpieler;
        List<Position> ddlPosition;
        List<Mannschaft> ddlMannschaften;
        List<Schiedsrichter> ddlSchiedsrichter;
        List<int> ddlMinutes;
        List<Spiel> ddlSpiel;
        List<Spieltag> ddlSpieltag;

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            switch (tabItem)
            {
                case "Punktetabelle":
                    BindPunktetabelle();
                    break;
                case "Spielplan":
                    string sql = "SELECT S.Spieltag, S.Datum, H.Name AS Heim, A.Name AS Gast, Sc.Vorname || ' ' || Sc.Nachname AS Schiedsrichter " +
                         "FROM Spiel S, Mannschaften H, Mannschaften A, Pfeift P, Schiedsrichter Sc " +
                         "WHERE S.HeimmannschaftsID == H.Id AND S.AuswaertsmannschaftsID == A.Id AND P.SpielId == S.Id AND P.SchiedsrichterId == Sc.Id " +
                         "ORDER BY S.Spieltag; ";

                    SqliteDataAccess.LoadTableInDataGrid(dgSpielplan, sql);
                    break;
                default:
                    return;
            }
        }

        private void BindPunktetabelle()
        {
            List<Punktetabelle> punktetabelle = AccessPunktetabelle.LoadPunktetabelle();
            int platz = 1;
            foreach (Punktetabelle punktetabelle2 in punktetabelle)
            {
                punktetabelle2.Platzierung = platz.ToString();
                platz++;
            }
            dgZeigePunktetabele.ItemsSource = punktetabelle;
        }

        private void BindSchiedsrichterDropDown()
        {
            cbSpielSchiedsrichter.ItemsSource = ddlSchiedsrichter;
        }

        private void BindSpielerDropDown()
        {
            cbMannschaftenKapitan.ItemsSource = ddlSpieler;
            cbTransferSpieler.ItemsSource = ddlSpieler;
            cbChangePositionSpieler.ItemsSource = ddlSpieler;
        }

        private void BindPositionDropDown()
        {
            cbSpielerPosition.ItemsSource = ddlPosition;
            cbFilterSpielerPosition.ItemsSource = ddlPosition;
        }

        private void BindMannschaftenDropDown()
        {
            cbSpielerMannschaften.ItemsSource = ddlMannschaften;
            cbTransferMannschaften.ItemsSource = ddlMannschaften;
            ddlSpielHeimMannschaften.ItemsSource = ddlMannschaften;
            ddlSpielAuswaertsMannschaften.ItemsSource = ddlMannschaften;
            ddlTrainerMannschaft.ItemsSource = ddlMannschaften;
            cbSpieleFilternMannschaften.ItemsSource = ddlMannschaften;
            cbSpieleFilternMannschaften.ItemsSource = ddlMannschaften;
            cbSpielerFilternMannschaften.ItemsSource = ddlMannschaften;
        }

        private void BindMinutesDropDown()
        {
            ddlToreFilternMinuteBis.ItemsSource = ddlMinutes;
            ddlToreFilternMinuteVon.ItemsSource = ddlMinutes;
        }

        private void BindSpieltag()
        {
            cbSpieleFilternSpieltag.ItemsSource = ddlSpieltag;
        }

        private void PopulateSpieltag()
        {
            List<Spiel> spiele = AccessSpiel.LoadSpiele();
            ddlSpieltag = new List<Spieltag>();
            foreach(Spiel spiel in spiele)
            {
                Spieltag st = new Spieltag();
                st.Check_Status = false;
                st.Tag = (int)spiel.Spieltag;
                bool contains = ddlSpieltag.Any(s => s.Tag == st.Tag);
                if (!contains)
                {
                    ddlSpieltag.Add(st);
                }

            }
            BindSpieltag();
        }

        private void PopulateMannschaften()
        {
            try
            {
                ddlMannschaften = AccessMannschaften.LoadMannschaftenAlphabetical();
                foreach(Mannschaft mannschaft in ddlMannschaften)
                {
                    mannschaft.Check_Status = false;
                }
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

        private void PopulateSchiedsrichter()
        {
            try
            {
                ddlSchiedsrichter = AccessSchiedsrichter.LoadAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindSchiedsrichterDropDown();
        }

        private void PopulateMinutes()
        {
            ddlMinutes = new List<int>();
            for (int i = 1; i <= 90; i++)
            {
                ddlMinutes.Add(i);
            }
            BindMinutesDropDown();
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

            if (dpEntstehungsjahr.SelectedDate != null)
                mannschaft.Entstehungsjahr = dpEntstehungsjahr.DisplayDate;

            if (cbMannschaftenKapitan.Text != null)
            {
                foreach (Spieler spieler in ddlSpieler)
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
            if (!String.IsNullOrEmpty(tbxSpielerTrikotnummer.Text))
            {
                spieler.Trikotnummer = int.Parse(tbxSpielerTrikotnummer.Text);
            }
            if (!String.IsNullOrEmpty(cbSpielerMannschaften.Text))
            {
                foreach (Mannschaft mannschaft in ddlMannschaften)
                {
                    if (string.Equals(mannschaft.Name, cbSpielerMannschaften.Text))
                    {
                        spieler.MannschaftsId = mannschaft.Id;
                    }
                }
            }

            try
            {
                spieler.Id = AccessSpieler.StoreSpieler(spieler);

                if (spieler.Id != null)
                {
                    foreach (Position pos in ddlPosition)
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
                PopulateSchiedsrichter();
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
            {
                tbxTransferAktuelleMannschaft.Text = "";
                return;
            }

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

            for (int i = 0; i < positionsOfPlayer.Count; i++)
            {
                positionsOfPlayer[i].Check_Status = true;
            }

            foreach (Position selPos in selectedPositions)
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

            foreach (Position pos in addToSpieltAuf)
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
            foreach (Position pos in removeFromSpieltAuf)
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
            {
                return;
            }


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

            foreach (Position pos in positionsOfPlayer)
            {
                for (int i = 0; i < ddlPositionsOfPlayer.Count; i++)
                {
                    if (ddlPositionsOfPlayer[i].Id == pos.Id)
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

        #region DB Zeigen
        private void cbZeigenTabelle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sql = "";

            var selectedItem = (ComboBoxItem)cbZeigenTabelle.SelectedItem;

            switch (selectedItem.Content)
            {
                case "Mannschaften":
                    // TODO: Alle Mannschaften in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM Mannschaften";
                    break;
                case "Positionen":
                    // TODO: Alle Positionen in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM Positionen";
                    break;
                case "Schiedsrichter":
                    // TODO: Alle Schiedsrichter in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM Schiedsrichter";
                    break;
                case "Spiele":
                    // TODO: Alle Spiele in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM Spiel";
                    break;
                case "Spieler":
                    // TODO: Alle Spieler in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM Spieler";
                    break;
                case "SpieltAuf":
                    // TODO: Alle SpieltAuf Relationen in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM SpieltAuf";
                    break;
                case "Fairnesstabelle":
                    // TODO: Alle Fairnesstabellen Einträge in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM Fairnesstabelle";
                    break;
                case "Tor":
                    // TODO: Alle Tore in der Datenbank im DataGrid des Tabs Datenbank zeigen
                    sql = "SELECT * " +
                          "FROM Tor";
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

        #region Spielplan

        #region Spielplan erstellen

        private bool missingJederGegenJedenInput()
        {
            bool ret = false;

            if (dpCreateTrunier1Spieltag.SelectedDate == null)
            {
                dpCreateTrunier1Spieltag.BorderBrush = Brushes.Red;
                ret = true;
            }
            else
            {
                dpCreateTrunier1Spieltag.BorderBrush = colorNormal;
            }

            return ret;
        }

        private void btnCreateTurnierJederVsJeden_Click(object sender, RoutedEventArgs e)
        {
            if (missingJederGegenJedenInput())
                return;

            if (KeepExistingTournament())
                return;

            List<Mannschaft> teams = AccessMannschaften.LoadMannschaften();

            if (teams.Count < 2)
            {
                MessageBox.Show("Es werden mindestens 2 Mannschaften benötigt.");
                return;
            }
            if (ddlSchiedsrichter.Count < 1)
            {
                MessageBox.Show("Es wird mindestens 1 Schiedsrichter benötigt.");
                return;
            }

            Mannschaft? dummy = null;
            List<Spiel> spielplan = new List<Spiel>();
            DateTime firstSpieltag = (DateTime)dpCreateTrunier1Spieltag.SelectedDate;
            TimeSpan timeBetweenSpieltagen = CalcutlateTimespanBetweenSpieltag(firstSpieltag, dpCreateTrunierLastSpieltag.SelectedDate, teams.Count);

            SqliteDataAccess.deleteTableEntrys();

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
                    AccessPfeift.CleanPfeift();
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

        private TimeSpan CalcutlateTimespanBetweenSpieltag(DateTime firstDay, DateTime? lastDay, int teamsCount)
        {
            TimeSpan timeBetweenSpieltagen;
            if (lastDay != null)
            {
                timeBetweenSpieltagen = (DateTime)lastDay - firstDay;
                timeBetweenSpieltagen = timeBetweenSpieltagen / teamsCount;
            }
            timeBetweenSpieltagen = TimeSpan.FromDays(7);
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

        private void CreateSpielplan(ref List<Mannschaft> teams, ref List<Spiel> spielplan, ref DateTime firstSpieltag, ref TimeSpan timeBetweenSpieltagen)
        {
            int[] sideA = new int[teams.Count / 2];
            int[] sideB = new int[teams.Count / 2];

            int a = 0;
            int b = 0;
            foreach (Mannschaft mannschaft in teams)
            {
                if (a < teams.Count / 2)
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
                DateTime spieltagDate = firstSpieltag + timeBetweenSpieltagen * (spieltag - 1);
                CreateSpieltag(teams.Count, sideA, sideB, ref spielplan, spieltag, spieltagDate);
                if (teams.Count > 2)
                    ReArrangeSides(teams.Count / 2, ref sideA, ref sideB);
            }
        }

        private void CreateSpieltag(int teamsCount, int[] sideA, int[] sideB, ref List<Spiel> spielplan, int spieltag, DateTime spieltagDate)
        {
            for (int i = 0; i < (teamsCount / 2); i++)
            {
                Spiel spiel = new Spiel();
                spiel.HeimmannschaftsId = sideA[i];
                spiel.AuswaertsmannschaftsId = sideB[i];
                spiel.Spieltag = spieltag;
                spiel.Datum = spieltagDate;
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

            spielplan.RemoveAll(spiel => spiel.HeimmannschaftsId == dummy.Id || spiel.AuswaertsmannschaftsId == dummy.Id);

        }

        private void PushSpielplanToDb(ref List<Spiel> spielplan)
        {
            Schiedsrichter[] schiedsrichtersAnsetzung = new Schiedsrichter[ddlSchiedsrichter.Count];
            int i = 0;
            foreach (Schiedsrichter schiedsrichter in ddlSchiedsrichter)
            {
                schiedsrichtersAnsetzung[i] = schiedsrichter;
                i++;
            }

            try
            {
                foreach (Spiel spiel in spielplan)
                {
                    int? spielId = AccessSpiel.StoreSpiel(spiel);
                    if (spielId != null)
                    {
                        AccessPfeift.AddRelation((int)spielId, (int)schiedsrichtersAnsetzung[0].Id);
                        RotateSchiedsrichterAnsetzung(ref schiedsrichtersAnsetzung);
                    }
                }
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            PopulateSpieltag();
        }

        private void RotateSchiedsrichterAnsetzung(ref Schiedsrichter[] schiris)
        {
            Schiedsrichter temp = schiris[0];
            for (int i = 1; i < schiris.Length; i++)
            {
                schiris[i - 1] = schiris[i];
            }
            schiris[schiris.Length - 1] = temp;
        }
        #endregion Spielplan erstellen

        #region Spiel erstellen

        private bool missingOrWrongSpielInput()
        {
            bool ret = false;

            if (dpDatum.SelectedDate == null)
            {
                dpDatum.BorderBrush = Brushes.Red;
                ret = true;
            }
            else
            {
                dpDatum.BorderBrush = colorNormal;
            }

            if (String.IsNullOrEmpty(tbxSpieltag.Text) || !int.TryParse(tbxSpieltag.Text, out _))
            {
                tbxSpieltag.BorderBrush = Brushes.Red;
                ret = true;
            }
            else
            {
                tbxSpieltag.BorderBrush = colorNormal;
            }

            if (!String.IsNullOrEmpty(tbxZuschaueranzahl.Text))
            {
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

            if (String.IsNullOrEmpty(cbSpielSchiedsrichter.Text))
            {
                bSpielSchiedsrichterBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else bSpielSchiedsrichterBorder.BorderBrush = Brushes.Transparent;

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

            if (!String.IsNullOrEmpty(ddlSpielAuswaertsMannschaften.Text) && !String.IsNullOrEmpty(ddlSpielHeimMannschaften.Text) && string.Equals(ddlSpielAuswaertsMannschaften.Text, ddlSpielHeimMannschaften.Text))
            {
                bSpielHeimMannschaftenBorder.BorderBrush = Brushes.Red;
                bSpielAuswaertsMannschaftenBorder.BorderBrush = Brushes.Red;
                ret = true;
            }
            else if (!String.IsNullOrEmpty(ddlSpielAuswaertsMannschaften.Text) && !String.IsNullOrEmpty(ddlSpielHeimMannschaften.Text) && !string.Equals(ddlSpielAuswaertsMannschaften.Text, ddlSpielHeimMannschaften.Text))
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
                    spiel.HeimmannschaftsId = mannschaft.Id;
                }
                else if (string.Equals(mannschaft.Name, ddlSpielAuswaertsMannschaften.Text))
                {
                    spiel.AuswaertsmannschaftsId = mannschaft.Id;
                }
            }

            spiel.Datum = (DateTime)dpDatum.SelectedDate;

            if (spiel.HeimmannschaftsId == null)
            {
                bSpielHeimMannschaftenBorder.BorderBrush = Brushes.Red;
                return;
            }
            if (spiel.AuswaertsmannschaftsId == null)
            {
                bSpielAuswaertsMannschaftenBorder.BorderBrush = Brushes.Red;
                return;
            }

            try
            {
                int? spielId = AccessSpiel.StoreSpiel(spiel);

                if (cbSpielSchiedsrichter.SelectedItem != null && spielId != null)
                {
                    Schiedsrichter schiedsrichter = (Schiedsrichter)cbSpielSchiedsrichter.SelectedItem;
                    if (schiedsrichter.Id != null)
                    {
                        AccessPfeift.AddRelation((int)spielId, (int)schiedsrichter.Id);
                    }
                }
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
            PopulateSpieltag();
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

        #endregion Spiel erstellen

        #region Spielplan zeigen
        private void btnRefreshSpielplan_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Lade alle Spieltage im Format "SpieltagNummer, SpieltagDatum, Heimmannschaft, Gastmannschaft, SchiedsrichterName" in ein DataGrid
            string sql = "SELECT S.Spieltag, S.Datum, H.Name AS Heim, A.Name AS Gast, Sc.Vorname || ' ' || Sc.Nachname AS Schiedsrichter " +
                         "FROM Spiel S, Mannschaften H, Mannschaften A, Pfeift P, Schiedsrichter Sc " +
                         "WHERE S.HeimmannschaftsID == H.Id AND S.AuswaertsmannschaftsID == A.Id AND P.SpielId == S.Id AND P.SchiedsrichterId == Sc.Id " +
                         "ORDER BY S.Spieltag; ";

            try
            {
                SqliteDataAccess.LoadTableInDataGrid(dgSpielplan, sql);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
        }


        #endregion Spielplan zeigen

        #endregion Spielplan

        #region Spielergebnis

        List<Tor> torList = new List<Tor>();
        List<Fairnesstabelle> listKarten = new List<Fairnesstabelle>();
        Spiel ergebnisSpiel = new Spiel();

        private void tbxErgebnis_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void tbxErgebnis2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void loadErgebnisSpiele(object sender, MouseButtonEventArgs e)
        {
            dpErgebnisSpieltag.SelectedDate = DateTime.Now;
            dpErgebnisSpieltag.Text = DateTime.Now.ToString();
        }

        private void btnAddTor_Click(object sender, RoutedEventArgs e)
        {
            if (ddlErgebnisSpiel.Text != "")
            {
                Window1 window1 = new Window1(ergebnisSpiel, torList);
                window1.Show();
            }
        }

        private void GetGames(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ddlSpiel = AccessSpiel.LoadGamesOfDate(dpErgebnisSpieltag.SelectedDate.Value);
                if (ddlErgebnisSpiel != null) ddlErgebnisSpiel.Items.Clear();
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }


            for (int i = 0; i < ddlSpiel.Count; i++)
            {
                ddlErgebnisSpiel.Items.Add("(" + ddlSpiel[i].Id + ") " + ddlSpiel[i].Heim + " : " + ddlSpiel[i].Gast);
            }
        }

        public void addTorToList(List<Tor> torListe)
        {
            switch (torListe[torListe.Count - 1].Typ)
            {
                case 1:
                    torListe[torListe.Count - 1].TypString = "normales Tor";
                    break;
                case 2:
                    torListe[torListe.Count - 1].TypString = "Eigentor";
                    break;
                case 3:
                    torListe[torListe.Count - 1].TypString = "Elfmeter";
                    break;
                case 4:
                    torListe[torListe.Count - 1].TypString = "Kopfballtor";
                    break;
                default:
                    break;
            }
            torList = torListe;
                dgTore.ItemsSource = torListe;
                dgTore.Items.Refresh();
        }

        public void addKarteToList(List<Fairnesstabelle> fairnesstabelle)
        {
            listKarten = fairnesstabelle;
            dgKarten.ItemsSource = listKarten;
            dgKarten.Items.Refresh();
        }

        private void btnErgebnisSave_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(tbxErgebnisHeim.Text) && !String.IsNullOrEmpty(tbxErgebnisGast.Text) && torList.Count != 0)
            {
                try
                {
                    foreach (Tor tor in torList)
                    {
                        AccessTor.StoreTor(tor);
                    }
                    foreach (Fairnesstabelle karte in listKarten)
                    {
                        AccessFairnesstabelle.StoreKarte(karte);
                    }
                }
                catch
                {

                }
                torList.Clear();
                dgTore.ItemsSource = torList;
                dgTore.Items.Refresh();
                listKarten.Clear();
                dgKarten.ItemsSource = listKarten;
                dgKarten.Items.Refresh();
                tbxErgebnisHeim.Text = "";
                tbxErgebnisGast.Text = "";
                dpErgebnisSpieltag.SelectedDate = DateTime.Now;
                dpErgebnisSpieltag.Text = DateTime.Now.ToString();
            }
            else if (!String.IsNullOrEmpty(tbxErgebnisHeim.Text) && !String.IsNullOrEmpty(tbxErgebnisGast.Text))
            {
                fillTorList();

                try
                {
                    foreach (Tor tor in torList)
                    {
                        AccessTor.StoreTor(tor);
                    }
                    foreach (Fairnesstabelle karte in listKarten)
                    {
                        AccessFairnesstabelle.StoreKarte(karte);
                    }
                }
                catch
                {

                }
                torList.Clear();
                dgTore.ItemsSource = torList;
                dgTore.Items.Refresh();
                listKarten.Clear();
                dgKarten.ItemsSource = listKarten;
                dgKarten.Items.Refresh();
                tbxErgebnisHeim.Text = "";
                tbxErgebnisGast.Text = "";
                ddlErgebnisSpiel.Items.Clear();
                dpErgebnisSpieltag.SelectedDate = DateTime.Now;
                dpErgebnisSpieltag.Text = DateTime.Now.ToString();
            }
            else
            {
                MessageBox.Show("Es muss ein Ergebnis eingetragen werden.");
            }
        }

        private void fillTorList()
        {
            torList.Clear();
            Tor torHeim = new Tor();
            torHeim.Mannschaft = ergebnisSpiel.HeimmannschaftsId;
            torHeim.SpielID = ergebnisSpiel.Id;
            torHeim.Typ = 1;

            for (int i = 0; i < Int32.Parse(tbxErgebnisHeim.Text); i++)
            {
                torList.Add(torHeim);
            }
            Tor torGast = new Tor();
            torGast.Mannschaft = ergebnisSpiel.AuswaertsmannschaftsId;
            torGast.SpielID = ergebnisSpiel.Id;
            torGast.Typ = 1;
            for (int i = 0; i < Int32.Parse(tbxErgebnisGast.Text); i++)
            {
                torList.Add(torGast);
            }
        }

        private void ddlErgebnisSpiel_ContextChanged(object sender, EventArgs e)
        {
            if (ddlErgebnisSpiel.SelectedValue != null)
            {
                string spielID = Regex.Match(ddlErgebnisSpiel.SelectedValue.ToString(), @"\d+").Value;
                ergebnisSpiel.Id = Int32.Parse(spielID);

                foreach (Spiel spiel in ddlSpiel)
                {
                    if (ergebnisSpiel.Id == spiel.Id)
                    {
                        ergebnisSpiel.HeimmannschaftsId = spiel.HeimmannschaftsId;
                        ergebnisSpiel.AuswaertsmannschaftsId = spiel.AuswaertsmannschaftsId;
                        ergebnisSpiel.Heim = spiel.Heim;
                        ergebnisSpiel.Gast = spiel.Gast;
                    }
                }
            }
            torList.Clear();
            dgTore.ItemsSource = torList;
            dgTore.Items.Refresh();
            listKarten.Clear();
            dgKarten.ItemsSource = listKarten;
            dgKarten.Items.Refresh();
            tbxErgebnisHeim.Text = "";
            tbxErgebnisGast.Text = "";
        }

        private void btnAddKarte_Click(object sender, RoutedEventArgs e)
        {
            if (ddlErgebnisSpiel.Text != "")
            {
                Window2 window2 = new Window2(ergebnisSpiel, listKarten);
                window2.Show();
            }
        }

        private void btnClearTore_Click(object sender, RoutedEventArgs e)
        {
            torList.Clear();
            dgTore.ItemsSource = torList;
            dgTore.Items.Refresh();
        }

        private void btnClearKarten_Click(object sender, RoutedEventArgs e)
        {
            listKarten.Clear();
            dgKarten.ItemsSource = listKarten;
            dgKarten.Items.Refresh();
        }
        #endregion Spielergebnis

        #region Filtern

        #region Tore
        string[] sqlFilterTore = new string[4];

        private void ddlSpielFilternMinuteVon_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ddlSpielFilternMinuteBis_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ddlSpielFilternMinuteBis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)e.Source;
            if ((int)comboBox.SelectedValue <= 90 && (int)comboBox.SelectedValue >= 1)
            {
                // TODO: Filter, der alle Tore unterhalb der ausgewählten Minute filtert
                sqlFilterTore[1] = " AND T.Zeitstempel <= " + (int)comboBox.SelectedValue;
            }
            else
            {
                sqlFilterTore[1] = null;
            }
        }

        private void ddlSpielFilternMinuteVon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)e.Source;
            if ((int)comboBox.SelectedValue <= 90 && (int)comboBox.SelectedValue >= 1)
            {
                // TODO: Filter, der alle Tore oberhalb der ausgewählten Minute filtert
                sqlFilterTore[0] = " AND T.Zeitstempel >= " + (int)comboBox.SelectedValue;
            }
            else
            {
                sqlFilterTore[0] = null;
            }
        }

        private void ddlErgebnisTorTyp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)e.Source;
            var comboBoxItem = (ComboBoxItem)comboBox.SelectedItem;
            string text = (string)comboBoxItem.Content;
            short typ = 1;

            if (text.Equals("Normales Tor") || text.Equals("Eigentor") || text.Equals("Elfmeter") || text.Equals("Kopfball"))
            {
                switch (text)
                {
                    case "Normales Tor":
                        typ = 1;
                        break;
                    case "Eigentor":
                        typ = 2;
                        break;
                    case "Elfmeter":
                        typ = 3;
                        break;
                    case "Kopfball":
                        typ = 4;
                        break;
                    default:
                        typ = 1;
                        break;
                }
                // TODO: Filter, der alle Tore der ausgewählten Kategorie filtert
                sqlFilterTore[2] = " AND T.Typ == '" + typ + "'";
            }
            else
            {
                sqlFilterTore[2] = null;
            }
        }

        private bool getSpielerID()
        {
            bool ret = false;

            List<Spieler> spielerlist = new List<Spieler>();
            spielerlist = AccessSpieler.LoadSpielerAlphabetical();

            foreach (Spieler spieler in spielerlist)
            {
                if (spieler.Name.Contains(tbxSpieleFilternSpieler.Text))
                {
                    // TODO: Filter, der die Tore aller ausgewählten Spieler filtert
                    sqlFilterTore[3] = " AND Sp.Id == '" + spieler.Id.ToString() + "' ";
                    ret = true;
                }
            }
            return ret;
        }

        private void btnFilterTore_Click(object sender, RoutedEventArgs e)
        {
            if (tbxSpieleFilternSpieler.Text != "")
            {
                if (!getSpielerID())
                {
                    dgFilterTore.ItemsSource = null;
                    dgFilterTore.Items.Refresh();
                    return;
                }
            }

            // TODO: Alle Tore die auf den eingestellten Filter zutreffen in ein DataGrid laden. Das DataGrid besitzt dabei folgende Spalten: Minuten, Spieler (der das Tor erzielt hat), Spiel (in dem das Tor gefallen ist), Datum (des Spiels)
            string sql = "SELECT  T.Zeitstempel AS 'Minute', " +
                                 "SP.Vorname || ' ' || SP.Nachname AS 'Spieler', " +
                                 "T.Typ, " +
                                 "H.Kuerzel || ' vs ' || G.Kuerzel AS 'Spiel', " +
                                 "S.Datum AS 'Datum' " +
                         "From Tor T, Spiel S, Mannschaften H, Mannschaften G, Spieler SP " +
                         "WHERE  T.SpielID == S.Id " +
                            "AND S.HeimmannschaftsId == H.Id " +
                            "AND S.AuswaertsmannschaftsId == G.Id " +
                            "AND Sp.Id == T.Spieler ";

            //Filter zur obigem SQL Befehl hinzufügen
            foreach (string filter in sqlFilterTore)
            {
                if (filter != null)
                {
                    sql += filter;
                }
            }

            sql += ";";

            try
            {
                AccessSpiel.LoadTableInDataGrid(dgFilterTore, sql);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
        }

        #endregion Tore

        #region Spiele

        private void tbxSpieleFilternToranzahl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnFilterSpiele_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Alle Spiele auf der eingestellten Filter zutreffen in ein DataGrid laden. Das DataGrid besitzt dabei folgende Spalten: Spielbegegnung (im Format HeimmanschftsKürzel:GastmannschaftsKürzel), Ergebnis (im Format Heimtore:Gasttore), Datum (des Spiels)
            string sql = "SELECT H.Kuerzel || ':' || G.Kuerzel AS Begegnung," +
                            "sum(CASE  WHEN(T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ != 2) THEN 1 when(T.SpielID == S.Id AND T.Mannschaft == G.Id and T.Typ == 2) then 1 else 0 END) || ':' || " +
                            "sum(CASE  WHEN(T.SpielID == S.Id AND T.Mannschaft == G.Id and T.Typ != 2) THEN 1 when(T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ == 2) then 1 else 0 END) AS Ergebnis, " +
                            "S.Datum " +
                         "FROM Tor T, Spiel S, Mannschaften H, Mannschaften G " +
                         "WHERE S.HeimmannschaftsId == H.Id " +
                           "AND S.AuswaertsmannschaftsId == G.Id ";

            string[] sqlFilterSpiele = new string[4];

            sqlFilterSpiele[2] = "";
            sqlFilterSpiele[3] = "";
            if (tbxSpieleFilternToranzahl.Text != "")
            {
                // TODO: Filter hinzufügen, der nur Spiele mit einer exakten Toranzahl durchlässt
                sqlFilterSpiele[2] = " AND T.SpielID == S.Id ";
                sqlFilterSpiele[3] = " HAVING SUM(T.SpielID == S.Id) == " + tbxSpieleFilternToranzahl.Text;
            }

            sqlFilterSpiele[0] = "";
            List<string> sqlTage = new List<string>();
            foreach (Spieltag st in ddlSpieltag)
            {
                if (st.Check_Status)
                {
                    sqlTage.Add("S.Spieltag == " + st.Tag);
                }
            }
            if (sqlTage.Count > 0)
            {
                // TODO: Filter hinzufügen, der nur Spiele aller ausgewählten Spieltage durchlässt
                sqlFilterSpiele[0] = "AND ( ";
                int index = 1;
                foreach (string s in sqlTage)
                {
                    //alle ausgewählten Spieltage im Filter durch disjunktionen verknüpfen
                    sqlFilterSpiele[0] += s;
                    if (index != sqlTage.Count)
                        sqlFilterSpiele[0] += " OR ";
                    index++;
                }
                sqlFilterSpiele[0] += " ) ";
            }

            sqlFilterSpiele[1] = "";
            List<string> sqlMannschaft = new List<string>();
            foreach (Mannschaft m in ddlMannschaften)
            {
                if (m.Check_Status)
                {
                    // TODO: Filter hinzufügen, der nur Spiele aller ausgewählten Mannschaften durchlässt
                    sqlMannschaft.Add(" S.AuswaertsmannschaftsId == " + m.Id + " OR S.HeimmannschaftsId == " + m.Id + " ");
                }
            }
            if (sqlMannschaft.Count > 0)
            {
                sqlFilterSpiele[1] = " AND ( ";
                int index = 1;
                foreach (string s in sqlMannschaft)
                {
                    //alle ausgewählten Mannschaften im Filter durch disjunktionen verknüpfen
                    sqlFilterSpiele[1] += s;
                    if (index != sqlMannschaft.Count)
                        sqlFilterSpiele[1] += " OR ";
                    index++;
                }
                sqlFilterSpiele[1] += " ) ";
            }

            int i = 0;
            foreach (string filter in sqlFilterSpiele)
            {
                if (i == 3)
                {
                    sql += " GROUP BY S.Id ";
                }
                sql += filter;
                i++;
            }

            sql += ";";

            try
            {
                AccessSpiel.LoadTableInDataGrid(dgFilterTore, sql);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            PopulateMannschaften();
            PopulateSpieltag();
            tbxSpieleFilternToranzahl.Text = "";
        }

        #endregion Spiele

        #region Spieler
        private void btnFilterSpieler_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Alle Spieler die auf den eingestellten Filter zutreffen in ein DataGrid laden. Das DataGrid besitzt dabei folgende Spalten: Name (des Spielers), Vereinsnamen (des Spielers), Positionen (auf denen der Spieler spielen kann)
            string sql = "SELECT SP.Vorname || ' ' || Sp.Nachname AS Name, M.Name AS Mannschaft, GROUP_CONCAT(P.Kuerzel) AS Position " +
                         "FROM Spieler SP, Positionen P, Mannschaften M, SpieltAuf SA " +
                         "WHERE Sp.Id == SA.SpielerId AND M.Id == Sp.MannschaftsId AND P.Id == SA.PositionId ";

            string[] sqlFilterSpieler = new string[3];

            if (tbxSpielerFilternName.Text != "")
            {
                // TODO: Filter hinzufügen, der nur Spieler durchlässt in dessen namen der eingegebene Text ein Teil des Namen ist
                sqlFilterSpieler[2] = " AND SP.Vorname || ' ' || SP.Nachname LIKE '%" + tbxSpielerFilternName.Text + "%' ";
            }

            sqlFilterSpieler[0] = "";
            List<string> sqlMannschaft = new List<string>();
            foreach (Mannschaft m in ddlMannschaften)
            {
                if (m.Check_Status)
                {
                    // TODO: Filter hinzufügen, der nur Spieler der ausgewählten Vereine durchlässt
                    sqlMannschaft.Add(" SP.MannschaftsId == " + m.Id);
                }
            }
            if (sqlMannschaft.Count > 0)
            {
                sqlFilterSpieler[0] = "AND ( ";
                int index = 1;
                foreach (string s in sqlMannschaft)
                {
                    //alle ausgewählten Vereine im Filter durch disjunktionen verknüpfen
                    sqlFilterSpieler[0] += s;
                    if (index != sqlMannschaft.Count)
                        sqlFilterSpieler[0] += " OR ";
                    index++;
                }
                sqlFilterSpieler[0] += " ) ";
            }

            sqlFilterSpieler[1] = "";
            List<string> sqlPosition = new List<string>();
            foreach (Position p in ddlPosition)
            {
                if (p.Check_Status)
                {
                    // TODO: Filter hinzufügen, der nur Spieler durchlässt die in
                    sqlPosition.Add(" SA.PositionId == " + p.Id + " ");
                }
            }
            if (sqlPosition.Count > 0)
            {
                sqlFilterSpieler[1] = " AND ( ";
                int index = 1;
                foreach (string s in sqlPosition)
                {
                    //alle ausgewählten Positionen im Filter durch disjunktionen verknüpfen
                    sqlFilterSpieler[1] += s;
                    if (index != sqlPosition.Count)
                        sqlFilterSpieler[1] += " OR ";
                    index++;
                }
                sqlFilterSpieler[1] += " ) ";
            }

            foreach (string filter in sqlFilterSpieler)
            {
                if (filter != null)
                {
                    sql += filter;
                }
            }

            sql += " GROUP BY SP.Id;";

            try
            {
                AccessSpiel.LoadTableInDataGrid(dgFilterTore, sql);
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }

            PopulatePosition();
            PopulateMannschaften();
            tbxSpielerFilternName.Text = "";
        }

        #endregion Spieler

        #endregion Filtern

        #region Statistiken
        private void loadStatistiken(object sender, MouseButtonEventArgs e)
        {
            loadTorStatistiken();
            loadFairnesstablle();
            loadTabelle();
        }

        #region Tore
        private void loadTorStatistiken()
        {
            try
            {
                List<Tor> torschuetzenkoenig = new List<Tor>();
                torschuetzenkoenig = AccessTor.LoadTorschuetzenliste(true, "");
                if (torschuetzenkoenig.Count != 0)
                {
                    lbTorschuetzenkoenig.Content = torschuetzenkoenig[0].Vorname + " " + torschuetzenkoenig[0].Nachname + ", Tore: " + torschuetzenkoenig[0].Toranzahl;

                    List<Tor> avgToreproSpiel = new List<Tor>();
                    avgToreproSpiel = AccessTor.LoadavgToreproSpiel();
                    lblavgToreSpiel.Content = avgToreproSpiel[0].avgSpiel;

                    List<Tor> torschuetzenliste = new List<Tor>();
                    torschuetzenliste = AccessTor.LoadTorschuetzenliste(false, "");
                    dgTorschuetzenliste.ItemsSource = torschuetzenliste;
                    dgTorschuetzenliste.Items.Refresh();
                }
                else
                {
                    lbTorschuetzenkoenig.Content = "kein Torschütze vorhanden";
                    lblavgToreSpiel.Content = "noch keine Tore vorhanden.";
                }

                List<Tor> meisteElfmetertore = new List<Tor>();
                meisteElfmetertore = AccessTor.LoadTorschuetzenliste(true, "Elfmeter");
                if (meisteElfmetertore.Count != 0)
                {
                    lbmeisteElfmetertore.Content = meisteElfmetertore[0].Vorname + " " + meisteElfmetertore[0].Nachname + ", Elfmetertore: " + meisteElfmetertore[0].Toranzahl;
                }
                else
                {
                    lbmeisteElfmetertore.Content = "kein Torschütze mit Elfmetertoren vorhanden.";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
        }
        #endregion Tore

        #region Fairness
        private void loadFairnesstablle()
        {
            List<Fairnesstabelle> fairnesstablle = new List<Fairnesstabelle>();
            try {
                 fairnesstablle = AccessFairnesstabelle.LoadFairnesstabelle();
                }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
    dgFairnesstablle.ItemsSource = fairnesstablle;
        }
        #endregion Fairness

        #region Tabelle
        private void loadTabelle()
        {

        }


        #endregion Tabelle

        #endregion Statistiken


    }
}