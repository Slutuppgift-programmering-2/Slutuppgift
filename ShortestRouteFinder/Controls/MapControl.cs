using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ShortestRouteFinder.Models;

namespace ShortestRouteFinder.Controls
{
    public class MapControl : Canvas
    {
        private readonly Dictionary<string, Ellipse> cityCircles = new Dictionary<string, Ellipse>();
        private readonly List<Line> pathLines = new List<Line>();
        private readonly List<City> cities;
        public const double CANVAS_WIDTH = 800;
        public const double CANVAS_HEIGHT = 600;

        public MapControl(List<City> cities)
        {
            Debug.WriteLine($"MapControl constructor - Received {cities?.Count ?? 0} cities");
            this.cities = cities;
            Background = new SolidColorBrush(Colors.White);
            Margin = new Thickness(10);
            Width = CANVAS_WIDTH;
            Height = CANVAS_HEIGHT;
            
            Loaded += (s, e) =>
            {
                Debug.WriteLine("MapControl Loaded event fired");
                DrawMap();
            };
        }

        private void DrawMap()
        {
            Debug.WriteLine("Drawing map...");
            
            // Draw connections first
            foreach (var city in cities)
            {
                foreach (var connectedCityName in city.ConnectedTo)
                {
                    var connectedCity = cities.Find(c => c.Name == connectedCityName);
                    if (connectedCity != null)
                    {
                        var line = new Line
                        {
                            X1 = city.X * CANVAS_WIDTH,
                            Y1 = city.Y * CANVAS_HEIGHT,
                            X2 = connectedCity.X * CANVAS_WIDTH,
                            Y2 = connectedCity.Y * CANVAS_HEIGHT,
                            Stroke = new SolidColorBrush(Colors.Gray),
                            StrokeThickness = 1
                        };
                        Children.Add(line);
                        Debug.WriteLine($"Drew connection: {city.Name} -> {connectedCity.Name}");
                    }
                }
            }

            // Draw cities
            foreach (var city in cities)
            {
                var cityCircle = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = new SolidColorBrush(Colors.Blue),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };

                SetLeft(cityCircle, city.X * CANVAS_WIDTH - cityCircle.Width / 2);
                SetTop(cityCircle, city.Y * CANVAS_HEIGHT - cityCircle.Height / 2);
                
                var cityLabel = new TextBlock
                {
                    Text = city.Name,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                SetLeft(cityLabel, city.X * CANVAS_WIDTH + 8);
                SetTop(cityLabel, city.Y * CANVAS_HEIGHT - 8);

                Children.Add(cityCircle);
                Children.Add(cityLabel);
                cityCircles[city.Name] = cityCircle;
                Debug.WriteLine($"Drew city: {city.Name} at ({city.X * CANVAS_WIDTH}, {city.Y * CANVAS_HEIGHT})");
            }
        }

        public void ClearPath()
        {
            Debug.WriteLine("Clearing path");
            foreach (var line in pathLines)
            {
                Children.Remove(line);
            }
            pathLines.Clear();

            foreach (var circle in cityCircles.Values)
            {
                circle.Fill = new SolidColorBrush(Colors.Blue);
            }
        }

        public void DrawPath(List<string> path)
        {
            Debug.WriteLine($"Drawing path with {path?.Count ?? 0} cities");
            if (path == null) return;

            for (int i = 0; i < path.Count - 1; i++)
            {
                var city1 = cities.Find(c => c.Name == path[i]);
                var city2 = cities.Find(c => c.Name == path[i + 1]);

                if (city1 != null && city2 != null)
                {
                    var pathLine = new Line
                    {
                        X1 = city1.X * CANVAS_WIDTH,
                        Y1 = city1.Y * CANVAS_HEIGHT,
                        X2 = city2.X * CANVAS_WIDTH,
                        Y2 = city2.Y * CANVAS_HEIGHT,
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeThickness = 3
                    };

                    Children.Add(pathLine);
                    pathLines.Add(pathLine);
                    
                    cityCircles[city1.Name].Fill = new SolidColorBrush(Colors.Green);
                    cityCircles[city2.Name].Fill = new SolidColorBrush(Colors.Green);
                    Debug.WriteLine($"Drew path segment: {city1.Name} -> {city2.Name}");
                }
            }
        }
    }
}