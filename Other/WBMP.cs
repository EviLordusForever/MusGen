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
    public static class WBMP
    {
        public static PixelFormat pf = PixelFormats.Bgra32;

        public static WriteableBitmap Create(int width, int height)
        {
            return new WriteableBitmap(width, height, HardwareParams._dpiX, HardwareParams._dpiY, pf, null);
        }

        public static void CopyPixels(WriteableBitmap source, WriteableBitmap destination,
            int sourceX, int sourceY, int destinationX, int destinationY, int width, int height)
        {
            PixelFormat format = PixelFormats.Bgra32;
            int stride = (int)source.Width * format.BitsPerPixel / 8;

            byte[] buffer = new byte[stride * height];

            source.CopyPixels(new Int32Rect(sourceX, sourceY, width, height), buffer, stride, 0);

            destination.WritePixels(new Int32Rect(destinationX, destinationY, width, height), buffer, stride, 0);
        }

        public static void CopyPixels(BitmapImage source, WriteableBitmap destination,
            int sourceX, int sourceY, int destinationX, int destinationY, int width, int height)
        {
            PixelFormat format = PixelFormats.Bgra32;
            int stride = (int)source.Width * format.BitsPerPixel / 8;

            byte[] buffer = new byte[stride * height];

            source.CopyPixels(new Int32Rect(sourceX, sourceY, width, height), buffer, stride, 0);

            destination.WritePixels(new Int32Rect(destinationX, destinationY, width, height), buffer, stride, 0);
        }

        public static void MultiplyAlpha(WriteableBitmap bitmap, float value, int x, int y, int width, int height)
        {
            PixelFormat format = PixelFormats.Bgra32;
            int bytesPerPixel = format.BitsPerPixel / 8;
            int stride = (int)bitmap.Width * bytesPerPixel;
            int offset = y * stride + x * bytesPerPixel;

            byte[] buffer = new byte[stride * height];

            bitmap.CopyPixels(new Int32Rect(x, y, width, height), buffer, stride, offset); // копируем пиксели из зоны в буфер

            for (int i = 0; i < buffer.Length; i += bytesPerPixel) // изменяем прозрачность для каждого пикселя
            {
                byte _alpha = buffer[i + 3];
                _alpha = (byte)(_alpha * value);
                buffer[i + 3] = _alpha;
            }

            bitmap.WritePixels(new Int32Rect(x, y, width, height), buffer, stride, offset); // записываем измененные пиксели обратно в зону
        }
    }
}
