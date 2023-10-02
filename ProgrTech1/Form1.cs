
namespace ProgrTech1
{
    public partial class Form1 : Form
    {
        public float[][] MatA { get; set; }
        public float[][] MatB { get; set; }

        public float[] VerificationFloats = new float[Operation.MaxThreads + 1];
        public int[] Milliseconds = new int[Operation.MaxThreads + 1];
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            MatA = Operation.Create();
            MatB = Operation.Create();
            Operation operation = new Operation(this);
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.Gray);
            operation.Execute(MatA, MatB, VerificationFloats, Milliseconds);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            Pen pen = new Pen(Color.Black, 3f);
            Point[] points = new Point[Operation.MaxThreads+1];
            bool up = false;
            g.DrawString("Format: (threads ; time (in seconds))",new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(10,10));
            for (int i = 0; i < Operation.MaxThreads+1; i++)
            {
                points[i] = new Point(i * (int)(panel1.Width / (Operation.MaxThreads + 1)),
                    (panel1.Height - Milliseconds[i]/100));
                g.FillEllipse(new SolidBrush(Color.Black), points[i].X-4, points[i].Y-4, 8, 8);
                //if (up)
                //{
                //    g.DrawString($"({i} ; {Milliseconds[i]/1000})", new Font("Arial", 10), new SolidBrush(Color.Black), points[i].X+10, points[i].Y+10);
                //}
                //else
                //{
                    g.DrawString($"({i} ; {Milliseconds[i] / 1000})", new Font("Arial", 10), new SolidBrush(Color.Black), points[i].X + 10, points[i].Y - 30);

                //}

                //up = !up;
            }
            g.DrawLines(pen, points);
        }
    }
}