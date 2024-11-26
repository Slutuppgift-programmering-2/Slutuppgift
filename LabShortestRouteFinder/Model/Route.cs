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

    private string _highlightColour; 
    public string HighlightColour
    {
        get => _highlightColour;
        set
        {
            if (!_highlightColour.Equals(value))
            {
            _highlightColour = value;
            OnPropertyChanged(nameof(HighlightColour));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}