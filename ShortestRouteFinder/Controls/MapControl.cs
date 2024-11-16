using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ShortestRouteFinder.Models;
using System.Diagnostics;

namespace ShortestRouteFinder.Controls
{
    public class MapControl : Canvas
    {
        private readonly Dictionary<string, Ellipse> cityCircles = new();
        private readonly List<Line> pathLines = new();
        private readonly List<City> cities;
        public const double CANVAS_WIDTH = 800;
        public const double CANVAS_HEIGHT = 600;

        public MapControl(List<City> cities)
        {
            this.cities = cities ?? throw new ArgumentNullException(nameof(cities));
            Background = new SolidColorBrush(Colors.White);
            Margin = new Thickness(10);
            Width = CANVAS_WIDTH;
            Height = CANVAS_HEIGHT;
            
            Loaded += (s, e) =>
            {
                Debug.WriteLine("MapControl Loaded event fired");
                DrawMap();
            };

            SizeChanged += (s, e) =>
            {
                Debug.WriteLine($"MapControl size changed to {e.NewSize}");
                InvalidateVisual();
            };
        }

        private void DrawMap()
        {
            Debug.WriteLine("Drawing map...");
            Children.Clear();
            cityCircles.Clear();

            // Draw background grid for debugging
            for (int i = 0; i < CANVAS_WIDTH; i += 50)
            {
                var line = new Line
                {
                    X1 = i,
                    Y1 = 0,
                    X2 = i,
                    Y2 = CANVAS_HEIGHT,
                    Stroke = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)),
                    StrokeThickness = 0.5
                };
                Children.Add(line);
            }
            for (int i = 0; i < CANVAS_HEIGHT; i += 50)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = i,
                    X2 = CANVAS_WIDTH,
                    Y2 = i,
                    Stroke = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)),
                    StrokeThickness = 0.5
                };
                Children.Add(line);
            }

            // Draw connections first
            foreach (var city in cities)
            {
                if (city.ConnectedTo == null) continue;
                
                foreach (var connectedCityName in city.ConnectedTo)
                {
                    if (string.IsNullOrEmpty(connectedCityName)) continue;
                    
                    var connectedCity = cities.Find(c => c.Name == connectedCityName);
                    if (connectedCity == null) continue;

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

            // Draw cities
            foreach (var city in cities)
            {
                if (string.IsNullOrEmpty(city.Name)) continue;

                var cityCircle = new Ellipse
                {
                    Width = 15,
                    Height = 15,
                    Fill = new SolidColorBrush(Colors.Blue),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2
                };

                SetLeft(cityCircle, city.X * CANVAS_WIDTH - cityCircle.Width / 2);
                SetTop(cityCircle, city.Y * CANVAS_HEIGHT - cityCircle.Height / 2);
                
                var cityLabel = new TextBlock
                {
                    Text = city.Name,
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Background = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(2)
                };

                SetLeft(cityLabel, city.X * CANVAS_WIDTH + 10);
                SetTop(cityLabel, city.Y * CANVAS_HEIGHT - 10);

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

            foreach (var pair in cityCircles)
            {
                if (pair.Value != null)
                {
                    pair.Value.Fill = new SolidColorBrush(Colors.Blue);
                }
            }
        }

        public void DrawPath(List<string> path)
        {
            Debug.WriteLine($"Drawing path with {path?.Count ?? 0} cities");
            if (path == null) return;

            ClearPath();

            for (int i = 0; i < path.Count - 1; i++)
            {
                var city1 = cities.Find(c => c.Name == path[i]);
                var city2 = cities.Find(c => c.Name == path[i + 1]);

                if (city1 == null || city2 == null) continue;

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

                if (cityCircles.TryGetValue(city1.Name, out var circle1) && circle1 != null)
                {
                    circle1.Fill = new SolidColorBrush(Colors.Green);
                }
                if (cityCircles.TryGetValue(city2.Name, out var circle2) && circle2 != null)
                {
                    circle2.Fill = new SolidColorBrush(Colors.Green);
                }
                
                Debug.WriteLine($"Drew path segment: {city1.Name} -> {city2.Name}");
            }
        }
    }
}