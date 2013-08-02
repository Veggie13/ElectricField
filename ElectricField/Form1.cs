using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ElectricField
{
    public partial class Form1 : Form
    {
        private Timer _timer = new Timer();

        class Charge
        {
            public Charge(double x, double y, double q) { X = x; Y = y; Q = q; }
            public double X;
            public double Y;
            public double Q;
        }

        double oldX, oldY, X, Y;
        
        Charge[] _charges = new Charge[14];
        RectangleF _screen;
        object _locker = new object();

        public Form1()
        {
            InitializeComponent();

            Random r = new Random();
            for (int i = 0; i < _charges.Length; i++)
            {
                _charges[i] = new Charge(
                    2 * r.NextDouble() - 1,
                    2 * r.NextDouble() - 1,
                    r.NextDouble() - 0.5 );
            }

            X = 2 * r.NextDouble() - 1;
            Y = 2 * r.NextDouble() - 1;
            oldX = X;
            oldY = Y;

            Activated += new EventHandler(Form1_Activated);
            Paint += new PaintEventHandler(Form1_Paint);
            KeyDown += new KeyEventHandler(Form1_KeyDown);

            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = 1000;

            this.DoubleBuffered = true;
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Space)
            //    Invalidate();
        }

        Point Transform(double x, double y)
        {
            float fx = (float)x;
            float fy = -(float)y;
            float rx = (float)DisplayRectangle.Width * ((fx - _screen.Left) / _screen.Width);
            float ry = (float)DisplayRectangle.Height * ((fy - _screen.Top) / _screen.Height);
            return new Point((int)rx, (int)ry);
        }

        Bitmap bm;
        bool first = true;
        void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = Graphics.FromImage(bm);

            for (int r = 0; r < DisplayRectangle.Height; r++)
                for (int c = 0; c < DisplayRectangle.Width; c++)
                {
                    double mX = ((double)c / DisplayRectangle.Width) * 4 - 2;
                    double mY = ((double)r / DisplayRectangle.Height) * 4 - 2;
                    double val = 0;
                    for (int i = 0; i < _charges.Length; i++)
                    {
                        double dX = _charges[i].X - mX;
                        double dY = _charges[i].Y - mY;
                        double denom = dX * dX + dY * dY;
                        val += _charges[i].Q / denom;
                    }
                    int v = (int)(0.127 * val) + 127;
                    Color clr = Color.FromArgb((val >= 0) ? v : 0, (val == 0) ? v : 0, (val <= 0) ? v : 0);
                    Brush b = new SolidBrush(clr);
                    g.FillRectangle(b, c, r, 1, 1);
                }
            /*if (first)
            {
                g.Clear(Color.Black);
                Pen p1 = new Pen(Brushes.Red);
                Pen p2 = new Pen(Brushes.Blue);
                for (int i = 0; i < _charges.Length; i++)
                {
                    Pen p = (_charges[i].Q < 0) ? p2 : p1;
                    Point l1 = Transform(_charges[i].X, _charges[i].Y);
                    Point l2 = Transform(_charges[i].X + 0.005, _charges[i].Y);
                    g.DrawLine(p, l1, l2);
                }
                first = false;
            }
            Pen p0 = new Pen(Brushes.White);
            Point l = Transform(X, Y);
            Point ol = Transform(oldX, oldY);
            g.DrawLine(p0, l, ol);
             * */

            e.Graphics.DrawImage(bm, 0, 0);
        }

        void Form1_Activated(object sender, EventArgs e)
        {
            FindNext();
            /*double width = _moonP.X + 3 * (_earthR + _moonR);
            double ratio = (double)DisplayRectangle.Height / (double)DisplayRectangle.Width;
            double height = ratio * width;
            _screen = new RectangleF(-3f * (float)_earthR, -0.5f * (float)height, (float)width, (float)height);*/
            _screen = new RectangleF(-2f, -2f, 4f, 4f);
            bm = new Bitmap(DisplayRectangle.Width, DisplayRectangle.Height);

            _timer.Start();
        }

        void FindNext()
        {
            oldX = X;
            oldY = Y;

            double tX = 0, tY = 0;
            for (int i = 0; i < _charges.Length; i++)
            {
                double dX = _charges[i].X - oldX;
                double dY = _charges[i].Y - oldY;
                double denom = dX * dX + dY * dY;
                denom *= denom;
                tX += _charges[i].Q * dX / denom;
                tY += _charges[i].Q * dY / denom;
            }
            X -= 0.001 * tX;
            Y -= 0.001 * tY;
        }

        int _counter = 0;
        void _timer_Tick(object sender, EventArgs e)
        {
            lock (_locker)
            {
                FindNext();
            }

            //if (_counter++ % 4 == 0)
            {
                //Invalidate();
            }
        }
    }
}
