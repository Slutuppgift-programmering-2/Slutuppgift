using System.Diagnostics;
using System.Windows;
using ShortestRouteFinder.Controls;
using ShortestRouteFinder.Models;
using ShortestRouteFinder.ViewModel;

namespace ShortestRouteFinder
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;
        private readonly MapControl mapControl;

        public MainWindow(List<City> cities, MainViewModel viewModel, MapControl mapControl)
        {
            InitializeComponent();
            
            Debug.WriteLine($"MainWindow constructor - Received {cities?.Count ?? 0} cities");
            
            this.viewModel = viewModel;
            this.mapControl = mapControl;

            // Subscribe to path changes
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentPath))
                {
                    Debug.WriteLine($"Path changed: {viewModel.CurrentPath?.Count ?? 0} cities");
                    if (viewModel is { HasPath: true, CurrentPath: not null })
                    {
                        mapControl.DrawPath(viewModel.CurrentPath);
                    }
                    else
                    {
                        mapControl.ClearPath();
                    }
                }
            };

            DataContext = viewModel;
            MapContainer.Child = mapControl;
            
            Debug.WriteLine("MainWindow initialized");
        }
    }
}