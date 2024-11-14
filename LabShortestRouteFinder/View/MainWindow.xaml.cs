using LabShortestRouteFinder.ViewModel;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; }

        public RouteViewModel RouteViewModel { get; }
        public GraphViewModel GraphViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            MainViewModel = new MainViewModel();
            RouteViewModel = new RouteViewModel(MainViewModel);
            GraphViewModel = new GraphViewModel(MainViewModel);

            DataContext = MainViewModel;
        }

        private void OnNavigationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationListBox.SelectedItem is ListBoxItem selectedItem)
            {
                string? tabName = selectedItem.Tag as string;

                foreach (TabItem tab in MainTabControl.Items)
                {
                    if (tab.Name == tabName)
                    {
                        MainTabControl.SelectedItem = tab;
                        break;
                    }
                }
            }
        }
    }
}