using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LabShortestRouteFinder.Model;

public partial class Route : INotifyPropertyChanged
{
    public required CityNode Start { get; set; }
    public required CityNode Destination { get; set; }
    public int Distance { get; set; }

    private bool _isHighlighted;
    public bool IsHighlighted
    {
        get => _isHighlighted;
        set
        {
            if (_isHighlighted != value)
            {
                _isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }
    }

    private List<string> _highlightColours = []; 
    public List<string> HighlightColours
    {
        get => _highlightColours;
        set
        {
            _highlightColours = value;
            OnPropertyChanged(nameof(HighlightColours));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}