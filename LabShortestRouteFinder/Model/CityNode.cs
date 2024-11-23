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
        private bool isPartOfCycle;
        private bool isPartOfPath;
        public required string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsPartOfCycle
        {
            get => isPartOfCycle;
            set
            {
                if (isPartOfCycle != value)
                {
                    isPartOfCycle = value;
                    OnPropertyChanged(nameof(IsPartOfCycle));
                }
            }
        }

        public bool IsPartOfPath
        {
            get => isPartOfPath;
            set
            {
                if (isPartOfPath != value)
                {
                    isPartOfPath = value;
                    OnPropertyChanged(nameof(IsPartOfPath));
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
