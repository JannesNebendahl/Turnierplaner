using DemoLibary;
using System;
using System.Collections.Generic;
using System.Linq;
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
using TurnierLibrary;

namespace Turnierplaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MannschaftModel> mannschaften = new List<MannschaftModel>();

        public MainWindow()
        {
            InitializeComponent();

            loadMannschaften();
        }

        private void loadMannschaften()
        {
            mannschaften = SqliteDataAccess.LoadMannschaften();

            WireUpPeopleList();
        }

        private void WireUpPeopleList()
        {
            lbMannschaften.ItemsSource = null;
            lbMannschaften.ItemsSource = mannschaften;
        }

        private void btnAddMannschaft_Click(object sender, RoutedEventArgs e)
        {
            MannschaftModel p = new MannschaftModel();

            p.Name = tbxMannschaftenName.Text;

            SqliteDataAccess.SaveMannschaft(p);

            tbxMannschaftenName.Text = "";
        }

        private void btnRefreshList_Click(object sender, RoutedEventArgs e)
        {
            loadMannschaften();
        }

    }
}
