using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ShortestRouteFinder.Controls;
using ShortestRouteFinder.Models;
using ShortestRouteFinder.ViewModel;

namespace ShortestRouteFinder
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            var cities = LoadCitiesData();
            var mapControl = new MapControl(cities);
            _viewModel = new MainViewModel(cities, MapControl.CanvasWidth, MapControl.CanvasHeight);
            
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentPath))
                {
                    if (_viewModel.HasPath)
                    {
                        mapControl.DrawPath(_viewModel.CurrentPath);
                    }
                    else
                    {
                        mapControl.ClearPath();
                    }
                }
            };

            DataContext = _viewModel;
            
            // Add the map control to the window
            var mapContainer = (Border?)FindName("MapContainer");
            if (mapContainer != null) mapContainer.Child = mapControl;
        }

        private List<City>? LoadCitiesData()
        {
            const string jsonData = "nodes.json";
            return JsonSerializer.Deserialize<List<City>>(jsonData);
        }
    }
}