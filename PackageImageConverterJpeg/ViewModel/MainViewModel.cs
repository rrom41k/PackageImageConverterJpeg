using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using PackageImageConverterJpeg.Model;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ListView = System.Windows.Controls.ListView;

namespace PackageImageConverterJpeg.ViewModel;

public sealed class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Photo> Photos { get; set; } = new ObservableCollection<Photo>();

    private RelayCommand _addPhotos;
    private RelayCommand _convertPhotos;
    private RelayCommand _deletePhotos;

    public RelayCommand AddPhotos => _addPhotos ??= new RelayCommand(btAdd_OnClick);
    public RelayCommand ConvertPhotos => _convertPhotos ??= new RelayCommand(btConvert_OnClick);
    public RelayCommand DeletePhotos => _deletePhotos ??= new RelayCommand(btDelete_OnClick);

    private void btAdd_OnClick(object o)
    {
        // запуск формы для выбора файлов
        OpenFileDialog openFileDialog = new OpenFileDialog(); // создание объекта диалоговой формы
        openFileDialog.Multiselect = true; // свойство для выбора нескольких файлов
        openFileDialog.Title = "Открыть файлы изображения:"; // изменение свойства заголовка
        openFileDialog.Filter =
            "Формат файлов(*.BMP; *.JPG; *.PNG; *.GIF) | *.BMP; *.JPG; *.PNG; *.GIF| All files (*.*)|*.*"; // фильтрация файлов

        try
        {
            if (openFileDialog.ShowDialog() == true) // если была нажата кнопка ОК
                foreach (string file_path in openFileDialog.FileNames) // Проверяем каждый выбранный файл по пути
                    if (!Photos.Any(photo => photo.Path == file_path)) // Проверяем, существует ли файл в списке объектов Photo
                        Photos.Add(new Photo(Path.GetFileName(file_path), new BitmapImage(new Uri(file_path)), file_path)); // Добавляем фото, если ещё его нет
        }
        catch
        {
            MessageBox.Show("Невозможно открыть данный файл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btConvert_OnClick(object o)
    {
        try
        {
            using var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                int i = 0;
                string savePath = dialog.SelectedPath; // Получаем выбранную папку
                foreach (var item in Photos)
                {
                    string filePath = Path.Combine(savePath, $"New image{++i}.jpg");
                    ConvertToJPEG.Convert(item.Path, filePath);
                }
            
                MessageBox.Show($"Файлы были сохранены по пути {savePath}");
            }
            else
            {
                MessageBox.Show("Отменено");
            }
        }
        catch
        {
            MessageBox.Show("Операция конвертации сорвалась", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btDelete_OnClick(object o)
    {
        ListView photos = (ListView)o;
        List<Photo> photosToRemove = new List<Photo>(photos.SelectedItems.Cast<Photo>());
        if (photosToRemove != null)
            foreach (Photo photo in photosToRemove)
                Photos.Remove(photo);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}