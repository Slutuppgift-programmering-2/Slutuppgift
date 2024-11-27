using System.ComponentModel;

namespace LabShortestRouteFinder.Model;

public class Route : INotifyPropertyChanged
{
    public required CityNode Start { get; set; }
    public required CityNode Destination { get; set; }
    public int Distance { get; set; }
    public int Cost { get; set; }

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

    private List<string> _highlightedColours = new();
    public List<string> HighlightedColours
    {
        get => _highlightedColours;
        set
        {
            _highlightedColours = value;
            OnPropertyChanged(nameof(HighlightedColours));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}