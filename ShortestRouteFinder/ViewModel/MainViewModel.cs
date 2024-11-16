using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ShortestRouteFinder.Models;
using ShortestRouteFinder.Services;

namespace ShortestRouteFinder.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly PathFinder _pathFinder;
        private readonly List<City>? _cities;

        private string? _selectedStartCity;
        private string? _selectedEndCity;
        private List<string?>? _currentPath;
        private double _currentDistance;
        private bool _hasPath;
        private const double Tolerance = 0.03;

        public ObservableCollection<string> CityNames { get; }

        public string? SelectedStartCity
        {
            get => _selectedStartCity;
            set
            {
                if (_selectedStartCity != value)
                {
                    _selectedStartCity = value;
                    OnPropertyChanged();
                    FindPathCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string? SelectedEndCity
        {
            get => _selectedEndCity;
            set
            {
                if (_selectedEndCity != value)
                {
                    _selectedEndCity = value;
                    OnPropertyChanged();
                    FindPathCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<string?>? CurrentPath
        {
            get => _currentPath;
            private set
            {
                if (_currentPath == value) return;
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        public double CurrentDistance
        {
            get => _currentDistance;
            private set
            {
                if (Math.Abs(_currentDistance - value) < Tolerance) return;
                _currentDistance = value;
                OnPropertyChanged();
            }
        }

        public bool HasPath
        {
            get => _hasPath;
            set
            {
                if (_hasPath != value)
                {
                    _hasPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand FindPathCommand { get; }
        public RelayCommand ClearPathCommand { get; }

        public MainViewModel(List<City>? cities, double canvasWidth, double canvasHeight)
        {
            this._cities = cities;
            this._pathFinder = new PathFinder(cities, canvasWidth, canvasHeight);
            
            // Initialize collections
            CityNames = new ObservableCollection<string>(cities.Select(c => c.Name).OrderBy(n => n));

            // Initialize commands
            FindPathCommand = new RelayCommand(ExecuteFindPath, CanExecuteFindPath);
            ClearPathCommand = new RelayCommand(ExecuteClearPath, () => HasPath);
        }

        private bool CanExecuteFindPath()
        {
            return !string.IsNullOrEmpty(SelectedStartCity) && 
                   !string.IsNullOrEmpty(SelectedEndCity) && 
                   SelectedStartCity != SelectedEndCity;
        }

        private void ExecuteFindPath()
        {
            var (path, distance) = _pathFinder.FindShortestPath(SelectedStartCity, SelectedEndCity);
            
            CurrentPath = path;
            CurrentDistance = distance;
            HasPath = true;

            ErrorMessage = null;
        }

        private void ExecuteClearPath()
        {
            CurrentPath = null;
            CurrentDistance = 0;
            HasPath = false;
            ErrorMessage = null;
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}