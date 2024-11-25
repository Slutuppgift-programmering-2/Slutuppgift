using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LabShortestRouteFinder.Model
{
    public class Route : INotifyPropertyChanged
    {
        private bool _isPartOfPath;
        private bool _isPartOfCycle;

        public required CityNode Start { get; set; }
        public required CityNode Destination { get; set; }
        public int Distance { get; set; }
        public double MidpointX => (Start.X + Destination.X) / 2;
        public double MidpointY => (Start.Y + Destination.Y) / 2;

        public bool IsPartOfPath
        {
            get => _isPartOfPath;
            set
            {
                if (_isPartOfPath == value) return;
                _isPartOfPath = value;
                OnPropertyChanged(nameof(IsPartOfPath));
            }
        }

        public bool IsPartOfCycle
        {
            get => _isPartOfCycle;
            set
            {
                if (_isPartOfCycle == value) return;
                _isPartOfCycle = value;
                OnPropertyChanged(nameof(IsPartOfCycle));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
