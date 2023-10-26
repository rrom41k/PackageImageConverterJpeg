using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PackageImageConverterJpeg;

public static class ConvertToJPEG
{
    public static bool Convert(string sourceImagePath, string destinationImagePath, int quality = 90)
    {
        try
        {
            // Открываем исходное изображение
            BitmapImage sourceImage = new BitmapImage(new Uri(sourceImagePath));

            // Создаем кодер JPEG
            JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
            jpegEncoder.QualityLevel = quality; // Устанавливаем качество сжатия (0-100)

            // Конвертируем изображение в JPEG
            jpegEncoder.Frames.Add(BitmapFrame.Create(sourceImage));

            // Сохраняем конвертированное изображение
            using (FileStream stream = new FileStream(destinationImagePath, FileMode.Create))
            {
                jpegEncoder.Save(stream);
            }

            return true;
        }
        catch (Exception)
        {
            // Если произошла ошибка при конвертации, возвращаем false
            return false;
        }
    }
}