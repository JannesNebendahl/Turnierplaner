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
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Spiel spiel = new Spiel();
        List<Tor> torListe = new List<Tor>();
        List<Spieler> spielerList = new List<Spieler>();

        public Window1(Spiel ergebnisSpiel, List<Tor> torList)
        {
            spiel = ergebnisSpiel;
            torListe = torList;
            InitializeComponent();
            fillddlBox();
        }
    
    private void fillddlBox()
    {
            for(int i = 1; i <= 90; i++)
            {
                ddlErgebnisZeit.Items.Add(i.ToString());
            }
            ddlErgebnisZeit.SelectedIndex = 0;
     }

        private void btnAddTor_Click(object sender, RoutedEventArgs e)
        {
            if (ddlErgebnisZeit.Text != "" && ddlErgebnisTorMannschaft.Text != "" && ddlErgebnisTorSpieler.Text != "") 
            {
                MainWindow form1 = ((MainWindow)Application.Current.MainWindow);
                Tor tor = new Tor();
                tor.Zeitstempel = Int32.Parse(ddlErgebnisZeit.Text);
                switch (ddlErgebnisTorTyp.Text)
                {
                    case "normales Tor":
                        tor.Typ = 1;
                        break;
                    case "Eigentor":
                        tor.Typ = 2;
                        break;
                    case "Elfmeter":
                        tor.Typ = 3;
                        break;
                    case "Kopfballtor":
                        tor.Typ = 4;
                        break;
                    default:
                        break;
                }
                tor.SpielID = spiel.Id;

                foreach (Spieler spieler in spielerList)
                {
                    if (spieler.Name == ddlErgebnisTorSpieler.Text)
                    {
                        tor.Spieler = spieler.Id;
                        tor.SpielerString = spieler.Name;
                    }
                }
                if (ddlErgebnisTorMannschaft.Text == "Heim")
                {
                    tor.Mannschaft = spiel.HeimmannschaftsId;
                    tor.MannschaftString = spiel.Heim;
                }
                else
                {
                    tor.Mannschaft = spiel.AuswaertsmannschaftsId;
                    tor.MannschaftString = spiel.Gast;
                }
                torListe.Add(tor);
                form1.addTorToList(torListe);
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
                spielerListe = AccessSpieler.LoadSpielerAlphabeticalFromMannschaft(spiel.HeimmannschaftsId);

                foreach (Spieler spieleritem in spielerListe)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = spieleritem.Name;
                    ddlErgebnisTorSpieler.Items.Add(item);
                }
            }
            else if (ddlErgebnisTorMannschaft.Text == "Gast")
            {
                spielerListe = AccessSpieler.LoadSpielerAlphabeticalFromMannschaft(spiel.AuswaertsmannschaftsId);
                foreach (Spieler spieleritem in spielerListe)
                {
                    ddlErgebnisTorSpieler.Items.Add(spieleritem.Name);
                }
            }
            spielerList = spielerListe;
        }
    }
}
