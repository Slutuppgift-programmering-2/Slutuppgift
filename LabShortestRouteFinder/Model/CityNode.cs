using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LabShortestRouteFinder.Model
{
    public class CityNode : INotifyPropertyChanged
    {
        private string _name;
        public int X { get; set; }
        public int Y { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool _isPartOfCycle { get; set; }
        private bool _isPartOfPath { get; set; }
        
        public required string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                    OnPropertyChanged(nameof(CenterX));
                }
            }
        }

        private int _y;
        public int Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                    OnPropertyChanged(nameof(CenterY));
                }
            }
        }

        // Center coordinates for the lines to connect to
        public int CenterX => X + 10; // Assuming node width is 20
        public int CenterY => Y + 10; // Assuming node height is 20

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
