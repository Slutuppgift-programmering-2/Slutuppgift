using LabShortestRouteFinder.ViewModel;
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

namespace LabShortestRouteFinder.View
{
    /// <summary>
    /// Interaction logic for CityListViewControl.xaml
    /// </summary>
    public partial class CityListViewControl : UserControl
    {
        public CityListViewControl()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CityViewModel viewModel = (CityViewModel)DataContext;
            viewModel.SaveCitiesToFile();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            CityViewModel viewModel = (CityViewModel)DataContext;
            if (viewModel != null)
            {

                viewModel.Cities.Remove(viewModel.SelectedCity);
            }
        }
    }

}