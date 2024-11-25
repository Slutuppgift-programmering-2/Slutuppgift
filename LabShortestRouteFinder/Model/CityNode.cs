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


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
