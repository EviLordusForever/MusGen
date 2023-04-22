using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace MusGen
{
    public static class BMP
    {
        public static PixelFormat pf = PixelFormats.Bgra32;

        public static WriteableBitmap Create(int width, int height)
        {
            return new WriteableBitmap(width, height, HardwareParams._dpiX, HardwareParams._dpiY, pf, null);
        }

        public static void CopyPixels(
            WriteableBitmap source,
            WriteableBitmap destination,
            int sourceX,
            int sourceY,
            int destinationX,
            int destinationY,
            int width,
            int height)
        {
            // Получаем информацию о формате пикселя и размере буфера
            PixelFormat format = PixelFormats.Bgra32;
            int stride = (int)source.Width * format.BitsPerPixel / 8;

            // Создаем буфер для части изображения
            byte[] buffer = new byte[stride * height];
            source.CopyPixels(new Int32Rect(sourceX, sourceY, width, height), buffer, stride, 0);

            // Записываем пиксели в целевой WriteableBitmap
            destination.WritePixels(new Int32Rect(destinationX, destinationY, width, height), buffer, stride, 0);
        }

        public static void CopyPixelsWithTransparency(
            WriteableBitmap source,
            WriteableBitmap destination,
            int sourceX,
            int sourceY,
            int destinationX,
            int destinationY,
            int width,
            int height,
            double opacity)
        {
            // Получаем информацию о формате пикселя и размере буфера
            PixelFormat format = PixelFormats.Bgra32;
            int stride = (int)source.Width * format.BitsPerPixel / 8;

            // Создаем буфер для части изображения
            byte[] buffer = new byte[stride * height];
            source.CopyPixels(new Int32Rect(sourceX, sourceY, width, height), buffer, stride, 0);

            // Изменяем альфа-канал каждого пикселя в буфере
            for (int i = 0; i < buffer.Length; i += 4)
            {
                byte alpha = buffer[i + 3];
                buffer[i + 3] = (byte)(alpha * opacity);
            }

            // Записываем пиксели в целевой WriteableBitmap
            destination.WritePixels(new Int32Rect(destinationX, destinationY, width, height), buffer, stride, 0);
        }
    }
}
