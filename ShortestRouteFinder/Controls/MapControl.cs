using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ShortestRouteFinder.Models;

namespace ShortestRouteFinder.Controls
{
    public class MapControl : Canvas
    {
        private readonly Dictionary<string, Ellipse> _cityCircles = new Dictionary<string, Ellipse>();
        private readonly List<Line> _pathLines = new List<Line>();
        private readonly List<City>? _cities;
        public const double CanvasWidth = 800;
        public const double CanvasHeight = 600;

        public MapControl(List<City>? cities)
        {
            this._cities = cities;
            Background = new SolidColorBrush(Colors.White);
            Margin = new Thickness(10);
            DrawMap();
        }

        private void DrawMap()
        {
            // Draw connections first
            if (_cities == null) return;
            foreach (var city in _cities)
            {
                foreach (var connectedCityName in city.ConnectedTo)
                {
                    var connectedCity = _cities.Find(c => c.Name == connectedCityName);
                    if (connectedCity == null) continue;
                    var line = new Line
                    {
                        X1 = city.X * CanvasWidth,
                        Y1 = city.Y * CanvasHeight,
                        X2 = connectedCity.X * CanvasWidth,
                        Y2 = connectedCity.Y * CanvasHeight,
                        Stroke = new SolidColorBrush(Colors.Gray),
                        StrokeThickness = 1
                    };
                    Children.Add(line);
                }
            }

            // Draw cities
            foreach (var city in _cities)
            {
                var cityCircle = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = new SolidColorBrush(Colors.Blue),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };

                SetLeft(cityCircle, city.X * CanvasWidth - cityCircle.Width / 2);
                SetTop(cityCircle, city.Y * CanvasHeight - cityCircle.Height / 2);

                var cityLabel = new TextBlock
                {
                    Text = city.Name,
                    FontSize = 12
                };

                SetLeft(cityLabel, city.X * CanvasWidth + 8);
                SetTop(cityLabel, city.Y * CanvasHeight - 8);

                Children.Add(cityCircle);
                Children.Add(cityLabel);
                if (city.Name != null) _cityCircles[city.Name] = cityCircle;
            }
        }

        public void ClearPath()
        {
            foreach (var line in _pathLines)
            {
                Children.Remove(line);
            }
            _pathLines.Clear();

            foreach (var circle in _cityCircles.Values)
            {
                circle.Fill = new SolidColorBrush(Colors.Blue);
            }
        }

        public void DrawPath(List<string?>? path)
        {
            if (path == null) return;

            for (int i = 0; i < path.Count - 1; i++)
            {
                var city1 = _cities?.Find(c => c.Name == path[i]);
                var city2 = _cities?.Find(c => c.Name == path[i + 1]);

                if (city1 != null)
                {
                    if (city2 != null)
                    {
                        var pathLine = new Line
                        {
                            X1 = city1.X * CanvasWidth,
                            Y1 = city1.Y * CanvasHeight,
                            X2 = city2.X * CanvasWidth,
                            Y2 = city2.Y * CanvasHeight,
                            Stroke = new SolidColorBrush(Colors.Red),
                            StrokeThickness = 3
                        };

                        Children.Add(pathLine);
                        _pathLines.Add(pathLine);
                    }
                }

                _cityCircles[city1.Name].Fill = new SolidColorBrush(Colors.Green);
                _cityCircles[city2.Name].Fill = new SolidColorBrush(Colors.Green);
            }
        }
    }
}