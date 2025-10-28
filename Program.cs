using System;
using System.Drawing;
using System.Windows.Forms;

namespace LabWork
{
    public class GraphForm : Form
    {
        private float minX = 2.3f;
        private float maxX = 5.4f;
        private float stepX = 0.8f;

        public GraphForm()
        {
            this.Text = "График функции";
            this.Size = new Size(800, 600);
            this.Paint += GraphForm_Paint;
            this.Resize += GraphForm_Resize;
        }

        private void GraphForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            // Получаем размеры клиентской области
            float width = ClientSize.Width;
            float height = ClientSize.Height;

            // Отступы от краев формы
            float margin = 40;
            float graphWidth = width - 2 * margin;
            float graphHeight = height - 2 * margin;

            // Находим минимальное и максимальное значение функции
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            for (float x = minX; x <= maxX; x += stepX)
            {
                float y = (x + (float)Math.Cos(2 * x)) / (3 * x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
            }

            // Создаем перо для рисования
            using (Pen axisPen = new Pen(Color.Black, 1))
            using (Pen graphPen = new Pen(Color.Blue, 2))
            {
                // Рисуем оси координат
                g.DrawLine(axisPen, margin, height - margin, width - margin, height - margin); // Ось X
                g.DrawLine(axisPen, margin, margin, margin, height - margin); // Ось Y

                // Рисуем график
                bool isFirst = true;
                float prevScreenX = 0, prevScreenY = 0;

                for (float x = minX; x <= maxX; x += stepX)
                {
                    float y = (x + (float)Math.Cos(2 * x)) / (3 * x);

                    // Преобразуем координаты в экранные
                    float screenX = margin + (x - minX) * graphWidth / (maxX - minX);
                    float screenY = height - margin - (y - minY) * graphHeight / (maxY - minY);

                    if (!isFirst)
                    {
                        g.DrawLine(graphPen, prevScreenX, prevScreenY, screenX, screenY);
                    }

                    prevScreenX = screenX;
                    prevScreenY = screenY;
                    isFirst = false;
                }

                // Подписываем оси
                using (Font font = new Font("Arial", 10))
                {
                    g.DrawString("X", font, Brushes.Black, width - margin, height - margin);
                    g.DrawString("Y", font, Brushes.Black, margin, margin);

                    // Подписываем значения на осях
                    g.DrawString(minX.ToString("F1"), font, Brushes.Black, margin, height - margin + 5);
                    g.DrawString(maxX.ToString("F1"), font, Brushes.Black, width - margin, height - margin + 5);
                    g.DrawString(minY.ToString("F2"), font, Brushes.Black, margin - 35, height - margin);
                    g.DrawString(maxY.ToString("F2"), font, Brushes.Black, margin - 35, margin);
                }
            }
        }

        private void GraphForm_Resize(object sender, EventArgs e)
        {
            Invalidate(); // Перерисовываем форму при изменении размера
        }

        static void Main()
        {
            Application.Run(new GraphForm());
        }
    }
}
