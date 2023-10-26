using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace PackageImageConverterJpeg.Model;

public class Photo : INotifyPropertyChanged
{
    public Photo(string name, BitmapImage imageSource, string path)
    {
        Name = name;
        ImageSource = imageSource;
        Path = path;
    }
    
    private string _name;
    private BitmapImage _imageSource;
    private string _path;

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public BitmapImage ImageSource
    {
        get => _imageSource;
        set
        {
            if (value == _imageSource) return;
            _imageSource = value;
            OnPropertyChanged();
        }
    }

    public string Path
    {
        get => _path;
        set
        {
            if (value == _path) return;
            _path = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}