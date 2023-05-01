using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace Extensions
{
    public static class ScreenE
    {
        public static Bitmap TakeScreen()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            Bitmap bmp = new Bitmap(w, h);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(w, h));
            return bmp;
        }

        public static WriteableBitmap TakeScreenWbmp()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            RenderTargetBitmap wbmp = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new VisualBrush(System.Windows.Application.Current.MainWindow), null, new Rect(new System.Windows.Point(), new System.Windows.Size(w, h)));
            }
            wbmp.Render(drawingVisual);
            WriteableBitmap wbmp2 = new WriteableBitmap(wbmp.PixelWidth, wbmp.PixelHeight, wbmp.DpiX, wbmp.DpiY, wbmp.Format, null);
            wbmp.CopyPixels(new Int32Rect(0, 0, wbmp.PixelWidth, wbmp.PixelHeight), wbmp2.BackBuffer, wbmp2.BackBufferStride * wbmp.PixelHeight, wbmp2.BackBufferStride);
            wbmp2.Lock();
            wbmp2.AddDirtyRect(new Int32Rect(0, 0, w, h));
            wbmp2.Unlock();
            return wbmp2;
        }
    }
}
