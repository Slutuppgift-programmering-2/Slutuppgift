using System.Windows;
using System.Windows.Controls;
using LabShortestRouteFinder.ViewModel;

namespace LabShortestRouteFinder.View
{
    public partial class ListViewControl : UserControl
    {
        private ListViewModel? ViewModel => DataContext as ListViewModel;

        public ListViewControl()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                // Ensure the current edit is committed
                routesGrid.CommitEdit();
                ViewModel.SaveRoutesToFile();
            }
            else
            {
                MessageBox.Show("ViewModel is not properly initialized.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                routesGrid.CommitEdit();
            }
        }
    }
}