using LabShortestRouteFinder.ViewModel;
using System.Windows.Controls;

namespace LabShortestRouteFinder.View
{
    /// <summary>
    /// Interaction logic for GraphViewControl.xaml
    /// </summary>
    public partial class GraphViewControl : UserControl
    {
        public GraphViewControl()
        {
            InitializeComponent();
        }

        public GraphViewControl(MainViewModel mainViewModel)
        {
            InitializeComponent();
        }
    }
}