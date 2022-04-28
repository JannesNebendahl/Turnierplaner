using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TurnierLibrary;
using TurnierLibrary.DbAccess;
using TurnierLibrary.TabelModel;

namespace Turnierplaner
{
    public partial class Window2 : Window
    {
        Spiel spiel = new Spiel();
        List<Spieler> spielerList = new List<Spieler>();
        List<Fairnesstabelle> kartenList = new List<Fairnesstabelle>();

        public Window2(Spiel ergebnisSpiel, List<Fairnesstabelle> kartenListe)
        {
            kartenList = kartenListe;
            spiel = ergebnisSpiel;
            InitializeComponent();
        }

        private void btnAddKarte_Click(object sender, RoutedEventArgs e)
        {
            if (ddlErgebnisTorMannschaft.Text != "" && ddlErgebnisTorSpieler.Text != "")
            {
                MainWindow form1 = new MainWindow();
                Fairnesstabelle fairnesstabelle = new Fairnesstabelle();
                fairnesstabelle.SpielID = spiel.Id;

                foreach (Spieler spieler in spielerList)
                {
                    if (spieler.Name == ddlErgebnisTorSpieler.Text)
                    {
                        fairnesstabelle.SpielerId = spieler.Id;
                        fairnesstabelle.SpielerString = spieler.Name;
                        fairnesstabelle.Karte = ddlErgebnisKartenTyp.Text;
                    }
                }
                if (ddlErgebnisTorMannschaft.Text == "Heim")
                {
                    fairnesstabelle.MannschaftString = spiel.Heim;
                }
                else
                {
                    fairnesstabelle.MannschaftString = spiel.Gast;
                }
                kartenList.Add(fairnesstabelle);
                form1.addKarteToList(kartenList);
                Close();
            }
        }

        private void ddlErgebnisMannschaft_Changed(object sender, EventArgs e)
        {
            ddlErgebnisTorSpieler.Items.Clear();
            List<Spieler> spielerListe = new List<Spieler>();

            string test = ddlErgebnisTorMannschaft.Text;
            if (ddlErgebnisTorMannschaft.Text == "Heim")
            {
                spielerListe = AccessSpieler.LoadSpielerAlphabeticalFromMannschaft(spiel.HeimmannschaftsID);

                foreach (Spieler spieleritem in spielerListe)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = spieleritem.Name;
                    ddlErgebnisTorSpieler.Items.Add(item);
                }
            }
            else if (ddlErgebnisTorMannschaft.Text == "Gast")
            {
                spielerListe = AccessSpieler.LoadSpielerAlphabeticalFromMannschaft(spiel.AuswaertsmannschaftsID);
                foreach (Spieler spieleritem in spielerListe)
                {
                    ddlErgebnisTorSpieler.Items.Add(spieleritem.Name);
                }
            }
            spielerList = spielerListe;
        }
    }
}
