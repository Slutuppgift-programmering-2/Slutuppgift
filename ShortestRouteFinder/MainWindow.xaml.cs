using System.Collections.Generic;
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

        public MainWindow(List<City> cities)
        {
            InitializeComponent();
            
            Debug.WriteLine($"MainWindow constructor - Received {cities?.Count ?? 0} cities");
            
            if (cities == null || cities.Count == 0)
            {
                MessageBox.Show("No cities data available!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mapControl = new MapControl(cities);
            Debug.WriteLine("MapControl created");
            
            viewModel = new MainViewModel(cities, MapControl.CANVAS_WIDTH, MapControl.CANVAS_HEIGHT);
            Debug.WriteLine("ViewModel created");
            
            
            viewModel.PropertyChanged += (s, e) =>
            {
                Debug.WriteLine($"Property changed: {e.PropertyName}");
                if (e.PropertyName == nameof(MainViewModel.CurrentPath))
                {
                    if (viewModel.HasPath)
                    {
                        Debug.WriteLine($"Drawing path with {viewModel.CurrentPath?.Count ?? 0} cities");
                        mapControl.DrawPath(viewModel.CurrentPath);
                    }
                    else
                    {
                        Debug.WriteLine("Clearing path");
                        mapControl.ClearPath();
                    }
                }
            };

            DataContext = viewModel;
            Debug.WriteLine("DataContext set");

            // Add the map control to the window
            if (MapContainer != null)
            {
                MapContainer.Child = mapControl;
                Debug.WriteLine("MapControl added to container");
            }
            else
            {
                Debug.WriteLine("ERROR: MapContainer not found!");
                MessageBox.Show("Map container not found in XAML", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Log initial state
            Debug.WriteLine($"Initial state: Start={viewModel.SelectedStartCity}, End={viewModel.SelectedEndCity}");
        }
    }
}