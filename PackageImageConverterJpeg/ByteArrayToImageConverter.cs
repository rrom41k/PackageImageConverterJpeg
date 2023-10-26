using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PackageImageConverterJpeg;

public class ByteArrayToImageConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var byteArray = value as byte[];

        if (byteArray == null || byteArray.Length == 0)
            return null;

        var image = new BitmapImage();
        using (var mem = new MemoryStream(byteArray))
        {
            mem.Position = 0;
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = null;
            image.StreamSource = mem;
            image.EndInit();
        }

        image.Freeze();

        return image;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var image = value as BitmapImage;

        if (image == null)
            return null;

        using var mem = new MemoryStream();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(image));
        encoder.Save(mem);
        return mem.ToArray();
    }

    public static byte[] CompressImage(string imagePath, int maxPixels)
    {
        var bitmap = new BitmapImage(new Uri(imagePath));

        var originalWidth = bitmap.PixelWidth;
        var originalHeight = bitmap.PixelHeight;

        if (originalWidth * originalHeight <= maxPixels)
            // Изображение уже удовлетворяет заданному количеству пикселей, возвращаем оригинальные данные
            return File.ReadAllBytes(imagePath);

        var targetWidth = (int)Math.Sqrt(maxPixels * (originalWidth / (double)originalHeight));
        var targetHeight = (int)(maxPixels / (double)targetWidth);

        var transformedBitmap = new TransformedBitmap(bitmap,
            new ScaleTransform(targetWidth / (double)originalWidth, targetHeight / (double)originalHeight));

        BitmapEncoder encoder = new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));

        using var memoryStream = new MemoryStream();
        encoder.Save(memoryStream);
        return memoryStream.ToArray();
    }
}