using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices;

namespace MusGen
{
    public static class WBMP
    {
        public static PixelFormat pf = PixelFormats.Bgra32;

        public static WriteableBitmap Create(int width, int height)
        {
            return new WriteableBitmap(width, height, HardwareParams._dpiX, HardwareParams._dpiY, pf, null);
        }

        public static WriteableBitmap Load(string fileName)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(fileName));
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage);

            return writeableBitmap;
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

        public static void WriteByteArrayToColumn(WriteableBitmap wbmp, byte[] byteArray, int columnNumber)
        {
            int width = wbmp.PixelWidth;
            int height = wbmp.PixelHeight;
            int bytesPerPixel = (wbmp.Format.BitsPerPixel + 7) / 8;
            int stride = bytesPerPixel * width;

            byte[] pixels = new byte[byteArray.Length * bytesPerPixel];
            for (int i = 0; i < byteArray.Length; i++)
            {
                pixels[i * bytesPerPixel] = byteArray[i];
                pixels[i * bytesPerPixel + 1] = byteArray[i];
                pixels[i * bytesPerPixel + 2] = byteArray[i];
                pixels[i * bytesPerPixel + 3] = 255;
            }

            wbmp.Lock();
            IntPtr buffer = wbmp.BackBuffer;

            for (int row = height - 1; row >= 0; row--)
            {
                IntPtr pixelPointer = buffer + row * stride + columnNumber * bytesPerPixel;
                int startIndex = (height - 1 - row) * bytesPerPixel;
                Marshal.Copy(pixels, startIndex, pixelPointer, bytesPerPixel);
            }

            wbmp.AddDirtyRect(new Int32Rect(columnNumber, 0, 1, height));
            wbmp.Unlock();
        }

        public static void WriteByteArrayToRow(WriteableBitmap wbmp, byte[] byteArray, int rowNumber)
        {
            int width = wbmp.PixelWidth;
            int height = wbmp.PixelHeight;
            int bytesPerPixel = (wbmp.Format.BitsPerPixel + 7) / 8;
            int stride = bytesPerPixel * width;

            byte[] pixels = new byte[byteArray.Length * bytesPerPixel];
            for (int i = 0; i < byteArray.Length; i++)
            {           
                pixels[i * bytesPerPixel] = byteArray[i];
                pixels[i * bytesPerPixel + 1] = byteArray[i];
                pixels[i * bytesPerPixel + 2] = byteArray[i];
                pixels[i * bytesPerPixel + 3] = 255;
            }

            wbmp.Lock();
            IntPtr buffer = wbmp.BackBuffer;
            IntPtr rowPointer = buffer + rowNumber * stride;

            Marshal.Copy(pixels, 0, rowPointer, stride);

            wbmp.AddDirtyRect(new Int32Rect(0, rowNumber, width, 1));
            wbmp.Unlock();
        }

        public static byte GetGreenValue(WriteableBitmap bitmap, int x, int y)
        {
            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
            {
                throw new ArgumentOutOfRangeException("x or y is out of range");
            }

            int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[bytesPerPixel];

            bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixelData, bytesPerPixel, 0);

            return pixelData[1];
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
