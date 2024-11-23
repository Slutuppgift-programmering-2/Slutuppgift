using LabShortestRouteFinder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabShortestRouteFinder.ViewModel
{
    public class CityViewModel
    {
        public ObservableCollection<CityNode> Cities { get; }

        public CityViewModel(MainViewModel mainViewModel)
        {
            // Reference the shared Cities collection
            Cities = mainViewModel.Cities;
        }
    }
}
