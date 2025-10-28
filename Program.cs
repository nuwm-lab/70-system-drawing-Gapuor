using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabWork
{
    public class FunctionGraph : UserControl
    {
        private float minX = 2.3f;
        private float maxX = 5.4f;
        private float stepX = 0.8f;
        private List<PointF> points;

        public FunctionGraph()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                    ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.UserPaint, true);
            
            this.Resize += (s, e) => Invalidate();
            CalculatePoints();
        }

        private void ValidateParameters()
        {
            if (minX >= maxX)
                throw new ArgumentException("minX must be less than maxX");
            if (stepX <= 0)
                throw new ArgumentException("stepX must be greater than 0");
        }

        private void CalculatePoints()
        {
            ValidateParameters();
            points = new List<PointF>();
            int steps = (int)((maxX - minX) / stepX) + 1;
            
            for (int i = 0; i < steps; i++)
            {
                float x = minX + i * stepX;
                if (Math.Abs(x) < float.Epsilon) // Захист від ділення на нуль
                    continue;
                
                float y = (x + (float)Math.Cos(2 * x)) / (3 * x);
                points.Add(new PointF(x, y));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            float width = ClientSize.Width;
            float height = ClientSize.Height;
            float margin = 40;
            float graphWidth = width - 2 * margin;
            float graphHeight = height - 2 * margin;

            // Знаходимо мінімальне та максимальне значення Y
            float minY = points.Min(p => p.Y);
            float maxY = points.Max(p => p.Y);

            // Малюємо осі
            using (Pen axisPen = new Pen(Color.Black, 1))
            using (Pen graphPen = new Pen(Color.Blue, 2))
            using (Font font = new Font("Arial", 10))
            {
                // Малюємо осі координат
                g.DrawLine(axisPen, margin, height - margin, width - margin, height - margin); // Ось X
                g.DrawLine(axisPen, margin, margin, margin, height - margin); // Ось Y

                // Малюємо графік
                PointF[] screenPoints = points.Select(p => new PointF(
                    margin + (p.X - minX) * graphWidth / (maxX - minX),
                    height - margin - (p.Y - minY) * graphHeight / (maxY - minY)
                )).ToArray();

                g.DrawLines(graphPen, screenPoints);

                // Підписи осей з урахуванням розміру тексту
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    
                    // Вимірюємо розмір тексту
                    SizeF xAxisSize = g.MeasureString("X", font);
                    SizeF yAxisSize = g.MeasureString("Y", font);
                    
                    // Підписуємо осі
                    g.DrawString("X", font, Brushes.Black, width - margin, height - margin + 5, format);
                    g.DrawString("Y", font, Brushes.Black, margin - 20, margin - yAxisSize.Height, format);

                    // Підписуємо значення
                    format.Alignment = StringAlignment.Far;
                    g.DrawString(minX.ToString("F1"), font, Brushes.Black, margin, height - margin + 5);
                    g.DrawString(maxX.ToString("F1"), font, Brushes.Black, width - margin, height - margin + 5);
                    g.DrawString(minY.ToString("F2"), font, Brushes.Black, margin - 5, height - margin, format);
                    g.DrawString(maxY.ToString("F2"), font, Brushes.Black, margin - 5, margin, format);
                }
            }
        }
    }

    public class MainForm : Form
    {
        private FunctionGraph graph;

        public MainForm()
        {
            this.Text = "Графік функції";
            this.Size = new Size(800, 600);

            graph = new FunctionGraph();
            graph.Dock = DockStyle.Fill;
            this.Controls.Add(graph);
        }

        static void Main()
        {
            Application.Run(new MainForm());
        }
    }
}
