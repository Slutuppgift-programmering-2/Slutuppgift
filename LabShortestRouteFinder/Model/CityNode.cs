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
        public required string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }   

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
