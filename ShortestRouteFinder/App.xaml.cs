using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ShortestRouteFinder.Models;
using ShortestRouteFinder.Services;
using ShortestRouteFinder.ViewModel;
using ShortestRouteFinder.Controls;
using ShortestRouteFinder.Converters;

namespace ShortestRouteFinder
{
    public partial class App : Application
    {
        private IServiceProvider? ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register configuration
            services.AddSingleton(LoadCitiesData());

            // Register services
            services.AddSingleton<PathFinder>(sp =>
            {
                var cities = sp.GetRequiredService<List<City>>();
                return new PathFinder(cities, MapControl.CanvasWidth, MapControl.CanvasHeight);
            });

            // Register controls
            services.AddTransient<MapControl>(sp =>
            {
                var cities = sp.GetRequiredService<List<City>>();
                return new MapControl(cities);
            });

            // Register view models
            services.AddTransient<MainViewModel>(sp =>
            {
                var cities = sp.GetRequiredService<List<City>>();
                return new MainViewModel(cities, MapControl.CanvasWidth, MapControl.CanvasHeight);
            });

            // Register converter
            services.AddSingleton<NullToVisibilityConverter>();

            // Register windows
            services.AddTransient<MainWindow>();
        }

        private List<City>? LoadCitiesData()
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "cities.json");
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    return JsonSerializer.Deserialize<List<City>>(jsonContent);
                }
                else
                {
                    MessageBox.Show("Cities data file not found. Please ensure 'cities.json' exists in the Data folder.",
                                  "Data Loading Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                    Shutdown();
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cities data: {ex.Message}",
                              "Data Loading Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                Shutdown();
                return null;
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}",
                          "Error",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}