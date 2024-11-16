using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ShortestRouteFinder.Models;
using ShortestRouteFinder.Services;
using ShortestRouteFinder.Controls;

namespace ShortestRouteFinder.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly PathFinder pathFinder;
    private readonly List<City> cities;

    private string? selectedStartCity;
    private string? selectedEndCity;
    private List<string>? currentPath;
    private double currentDistance;
    private bool hasPath;

    public ObservableCollection<string> CityNames { get; }

    public string? SelectedStartCity
    {
        get => selectedStartCity;
        set
        {
            if (selectedStartCity != value)
            {
                selectedStartCity = value;
                OnPropertyChanged();
                FindPathCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? SelectedEndCity
    {
        get => selectedEndCity;
        set
        {
            if (selectedEndCity != value)
            {
                selectedEndCity = value;
                OnPropertyChanged();
                FindPathCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public List<string>? CurrentPath
    {
        get => currentPath;
        private set
        {
            if (currentPath != value)
            {
                currentPath = value;
                OnPropertyChanged();
            }
        }
    }

    public double CurrentDistance
    {
        get => currentDistance;
        private set
        {
            if (currentDistance != value)
            {
                currentDistance = value;
                OnPropertyChanged();
            }
        }
    }

    public bool HasPath
    {
        get => hasPath;
        private set
        {
            if (hasPath != value)
            {
                hasPath = value;
                OnPropertyChanged();
            }
        }
    }

    public RelayCommand FindPathCommand { get; }
    public RelayCommand ClearPathCommand { get; }

    public MainViewModel(List<City> cities)
    {
        this.cities = cities ?? throw new ArgumentNullException(nameof(cities));
        this.pathFinder = new PathFinder(cities, MapControl.CANVAS_WIDTH, MapControl.CANVAS_HEIGHT);
            
        CityNames = new ObservableCollection<string>(
            cities.Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(c => c.Name!)
                .OrderBy(n => n));

        FindPathCommand = new RelayCommand(ExecuteFindPath, CanExecuteFindPath);
        ClearPathCommand = new RelayCommand(ExecuteClearPath, () => HasPath);
    }

    private bool CanExecuteFindPath()
    {
        return !string.IsNullOrEmpty(selectedStartCity) && 
               !string.IsNullOrEmpty(selectedEndCity) && 
               selectedStartCity != selectedEndCity;
    }

    private void ExecuteFindPath()
    {
        if (selectedStartCity == null || selectedEndCity == null) return;
            
        var (path, distance) = pathFinder.FindShortestPath(selectedStartCity, selectedEndCity);
            
        CurrentPath = path;
        CurrentDistance = distance;
        HasPath = path != null && path.Count > 0;

        if (!HasPath)
        {
            ErrorMessage = "No path found between selected cities.";
        }
        else
        {
            ErrorMessage = null;
        }
    }

    private void ExecuteClearPath()
    {
        CurrentPath = null;
        CurrentDistance = 0;
        HasPath = false;
        ErrorMessage = null;
    }

    private string? errorMessage;
    public string? ErrorMessage
    {
        get => errorMessage;
        private set
        {
            if (errorMessage != value)
            {
                errorMessage = value;
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

public class RelayCommand : ICommand
{
    private readonly Action execute;
    private readonly Func<bool>? canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        execute();
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}