﻿using DemoLibary;
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
        public MainWindow()
        {
            InitializeComponent();
            PopulatePositionCheckbox();
            PopulateMannschaften();
            PopulateKapitaen();
            PopulateTransferSpieler();
            PopulateTransferMannschaften();
        }

        #region DB Befüllen

        #region Mannschaften

        List<Spieler> ddlMannschaftKapitaen;

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

            if (cbMannschaftenKapitan.Text != null)
            {
                foreach(Spieler spieler in ddlMannschaftKapitaen)
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
            PopulateTransferMannschaften();
        }

        private void BindKapitaenDropDown()
        {
            cbMannschaftenKapitan.ItemsSource = ddlMannschaftKapitaen;
        }

        private void PopulateKapitaen()
        {
            try
            {
                ddlMannschaftKapitaen = AccessSpieler.LoadSpielerAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindKapitaenDropDown();
        }

        #endregion Mannschaften

        #region Spieler

        List<Mannschaft> ddlSpielerMannschaften;
        List<Position> ddlSpielerPosition;

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
            if (!String.IsNullOrEmpty(cbSpielerMannschaften.Text))
            {
                foreach(Mannschaft mannschaft in ddlSpielerMannschaften)
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
                    foreach(Position pos in ddlSpielerPosition)
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
            cbSpielerMannschaften.Text = "";

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

                ddlSpielerPosition = new List<Position>();

                foreach (Position pos in dbPositions)
                {
                    pos.Check_Status = false;
                    ddlSpielerPosition.Add(pos);
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
            cbSpielerPosition.ItemsSource = ddlSpielerPosition;
        }

        private void BindMannschaftenDropDown()
        {
            cbSpielerMannschaften.ItemsSource = ddlSpielerMannschaften;
        }

        private void PopulateMannschaften()
        {
            try
            {
                ddlSpielerMannschaften = AccessMannschaften.LoadMannschaftenAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindMannschaftenDropDown();
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
            else tbxSchiedsrichterVorname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");

            if (String.IsNullOrEmpty(tbxSchiedsrichterNachname.Text))
            {
                tbxSchiedsrichterNachname.BorderBrush = Brushes.Red;
                ret = true;
            }
            else tbxSchiedsrichterNachname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffabadb3");

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

        List<Mannschaft> ddlTransferSpielerMannschaften;
        List<Spieler> ddlTransferSpielerSpieler;

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
            PopulateTransferSpieler();
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

        private void BindTransferSpielerDropDown()
        {
            cbTransferSpieler.ItemsSource = ddlTransferSpielerSpieler;
        }

        private void PopulateTransferSpieler()
        {
            try
            {
                ddlTransferSpielerSpieler = AccessSpieler.LoadSpielerAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindTransferSpielerDropDown();
        }

        private void BindTransferMannschaftenDropDown()
        {
            cbTransferMannschaften.ItemsSource = ddlTransferSpielerMannschaften;
        }

        private void PopulateTransferMannschaften()
        {
            try
            {
                ddlTransferSpielerMannschaften = AccessMannschaften.LoadMannschaftenAlphabetical();
            }
            catch (Exception exep)
            {
                MessageBox.Show(exep.Message);
            }
            BindTransferMannschaftenDropDown();
        }


        #endregion Spieler transferieren

        #endregion BD Ändern

    }
}
