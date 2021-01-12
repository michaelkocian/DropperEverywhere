using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DropperEverywhere
{
    public partial class Form1 : Form
    {

        public const UInt32 SPI_SETMOUSESPEED = 0x0071;
        public const UInt32 SPI_GETMOUSESPEED = 0x0070;

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, ref UInt32 pvParam, UInt32 fWinIni);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);


        Timer t = new Timer();

        public Form1()
        {
            InitializeComponent();
            this.TopMost = true;
            t.Interval = 16;
            t.Tick += MouseMoveTimer_Tick;
            t.Start();
            
        }


        private bool init = true;

        public bool Init
        {
            get {
                var b = init;
                init = false;
                return b;
            }
        }


        private void ChangeMouseSensitivity(bool slowdown )
        {
            UInt32 val = 0;
            if (slowdown)
                val = 10;
            else
                val = 1;
            SystemParametersInfo(SPI_SETMOUSESPEED, 0, ref val, 0);
        }

        uint mousespeed = 0;
        private void SetOriginalSpeed()
        {
            uint speed = 0;
            SystemParametersInfo(
                SPI_GETMOUSESPEED,
                0,
                ref speed,
                0);
            mousespeed = speed;
        }


        private void MouseMoveTimer_Tick(object sender, EventArgs e)
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);           

            var c = GetColorAt(cursor);
            var surr = GetColorSurround(cursor);


            pictureBox1.Image = surr;

            // this.BackColor = c;
            if (Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || Keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt) || Init)
            {
                this.panel1.BackColor = c;
                this.textBox1.Text = c.R.ToString();
                this.textBox2.Text = c.G.ToString();
                this.textBox3.Text = c.B.ToString();
                
                this.textBox4.Text = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
                pictureBox2.Image = surr;
            }
        }

        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        public Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
            return screenPixel.GetPixel(0, 0);
        }

        Bitmap screenPixelSurroundings = new Bitmap(11, 11, PixelFormat.Format32bppArgb);
        Bitmap bmp = new Bitmap(110, 110);

        public Bitmap GetColorSurround(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixelSurroundings))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval2 = BitBlt(hDC, 0, 0, 11, 11, hSrcDC, location.X-5, location.Y-5, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
            using (Graphics g = Graphics.FromImage(bmp))
            {                                                                   
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(screenPixelSurroundings, new Rectangle(0, 0, 110, 110));

                g.DrawLine(new Pen(Brushes.Red), 45, 0, 45, 105);
                g.DrawLine(new Pen(Brushes.Red), 55, 0, 55, 105);

                g.DrawLine(new Pen(Brushes.Red), 0, 45, 105, 45);
                g.DrawLine(new Pen(Brushes.Red), 0, 55, 105, 55);
            }

            return bmp;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(textBox4.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Stop();
        }
    }
}
