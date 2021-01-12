using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropperEverywhere
{
    public partial class ColorRangeForm : Form
    {

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        public ColorRangeForm()
        {
            InitializeComponent();
            b = CaptureScreen();

            Timer t = new Timer();
            t.Interval = 3000;
            t.Tick += T_Tick;
            t.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            b = CaptureScreen();
            this.Invalidate();
        }

        Bitmap b;

        Pen rp = new Pen(Brushes.Red);
        Pen gp = new Pen(Brushes.Green);
        Pen bp = new Pen(Brushes.Blue);
        Pen wp = new Pen(Brushes.White);

        bool init = true; 

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (init)
            {
                base.OnPaintBackground(e);
                init = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);

            Color lastC = Color.Black;
            Color c;
            for (int i = 0; i < b.Width; i++)
            {
                c = b.GetPixel(i, Math.Abs(cursor.Y));

                e.Graphics.DrawLine(wp, i, 300, i, 45);
                e.Graphics.DrawLine(rp, i - 1, 300 - lastC.R, i, 300 - c.R);
                e.Graphics.DrawLine(gp, i - 1, 300 - lastC.G, i, 300 - c.G);
                e.Graphics.DrawLine(bp, i - 1, 300 - lastC.B, i, 300 - c.B);

                lastC = c;
            }           

        }

        private Bitmap CaptureScreen()
        {
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            // Save the screenshot to the specified path that the user has chosen.
            //bmpScreenshot.Save("Screenshot.png", ImageFormat.Png);

            return bmpScreenshot;
        }

    }
}
