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
using System.Windows.Shapes;

namespace LabShortestRouteFinder.View
{
    /// <summary>
    /// Interaction logic for ListViewControl.xaml
    /// </summary>
    public partial class ListViewControl : UserControl
    {
        public ListViewControl()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast DataContext to ListViewModel and call the Save method
            ListViewModel viewModel = (ListViewModel)DataContext;
            viewModel.SaveRoutesToFile();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewModel viewModel = (ListViewModel)DataContext;
            if (viewModel != null)
            {

                viewModel.Routes.Remove(viewModel.SelectedRoute);
            }
        }
    }
}
