using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entry_test
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Переменная для работы с графикой
        /// </summary>
        private Graphics g;

        /// <summary>
        /// Обрабатываемое изображение
        /// </summary>
        private Bitmap bmp;

        private Image im;

        /// <summary>
        /// Массивы цветовых оттенков
        /// </summary>
        private List<int> red, green, blue;

        /// <summary>
        /// Количество цветовых оттенков
        /// </summary>
        private const int Capacity = 256;

        /// <summary>
        /// Пустая точка
        /// </summary>
        private Point Empty = new Point(-1, -1);

        /// <summary>
        /// Точки выделенной области
        /// </summary>
        private Point First, Second;

        public Form1()
        {
            InitializeComponent();
            StartSettings();
        }

        /// <summary>
        /// Начальные настройки программы
        /// </summary>
        private void StartSettings()
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);

            red = new List<int>();
            green = new List<int>();
            blue = new List<int>();

            for (var i = 0; i < Capacity; ++i)
            {
                red.Add(0);
                green.Add(0);
                blue.Add(0);
            }

            First = Second = Empty;

            RenderImage();
        }

        /// <summary>
        /// Рендеринг текущего изображения
        /// </summary>
        private void RenderImage()
        {
            pictureBox1.Image = bmp;
            pictureBox1.Refresh();
        }

        /// <summary>
        /// Событие нажатие кнопки "Загрузить изображение"
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            var fileName = openFileDialog1.FileName;
            bmp = (Bitmap)Bitmap.FromFile(fileName);
            im = Bitmap.FromFile(fileName);
            g = Graphics.FromImage(bmp);
            RenderImage();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (First == Empty || Second != Empty)
            {
                First = e.Location;
                Second = Empty;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (First != Empty && Second == Empty)
            {
                Second = e.Location;
                DrawField();
            }
        }

        private void ResetSettings()
        {
            red.Clear();
            green.Clear();
            blue.Clear();

            for (var i = 0; i < Capacity; ++i)
            {
                red.Add(0);
                green.Add(0);
                blue.Add(0);
            }
        }

        /// <summary>
        /// Событие "Построить гистограмму выделенной области"
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            ResetSettings();
            DrawColorChannel(First.X, Second.X, First.Y, Second.Y);
        }

        /// <summary>
        /// Очистка изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            StartSettings();
        }

        private void DrawField()
        {
            ResetSettings();
            g.Clear(pictureBox1.BackColor);
            bmp = new Bitmap(im);
            g = Graphics.FromImage(bmp);
            g.DrawLines(new Pen(Brushes.Black), new Point[5] {
                First, new Point(First.X, Second.Y), Second, new Point(Second.X, First.Y), First });
            RenderImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           DrawColorChannel(0, pictureBox1.Width - 1, 0, pictureBox1.Height - 1);
        }

        private void DrawColorChannel(int x1, int x2, int y1, int y2)
        {
            var Width = pictureBox2.Width;
            var Height = pictureBox2.Height;

            for (var x = x1; x <= x2; ++x)
                for (var y = y1; y <= y2; ++y)
                {
                    var color = bmp.GetPixel(x, y);
                    ++red[color.R];
                    ++green[color.G];
                    ++blue[color.B];
                }

            var maxValue = 0;

            var bmpR = new Bitmap(Width, Height);
            var bmpG = new Bitmap(Width, Height);
            var bmpB = new Bitmap(Width, Height);
            
            var gR = Graphics.FromImage(bmpR);
            var gG = Graphics.FromImage(bmpG);
            var gB = Graphics.FromImage(bmpB);

            gR.ScaleTransform(1, -1);
            gR.TranslateTransform(0, -Height);

            gG.ScaleTransform(1, -1);
            gG.TranslateTransform(0, -Height);

            gB.ScaleTransform(1, -1);
            gB.TranslateTransform(0, -Height);

            for (var i = 1; i < Capacity; ++i)
                maxValue = Math.Max(maxValue, Math.Max(red[i], 
                    Math.Max(green[i], blue[i])));

            double k = Height / (double) maxValue;

            for (var i = 0; i < Capacity; ++i)
            {
                gR.DrawLine(new Pen(Brushes.Red), i, 0, i, (int)(k * red[i]));
                gG.DrawLine(new Pen(Brushes.Green), i, 0, i, (int)(k * green[i]));
                gB.DrawLine(new Pen(Brushes.Blue), i, 0, i, (int)(k * blue[i]));
            }

            pictureBox2.Image = bmpR;
            pictureBox3.Image = bmpG;
            pictureBox4.Image = bmpB;
        }
    }
}
