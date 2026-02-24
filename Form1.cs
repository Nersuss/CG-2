using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphics_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        Object File_OBJ = new Object();

        double Y = 0, X = 0, Z = 0;
        double XKG = 240, YKG = 210;
        double scale = 80;
        double LX = -60, LY = 30, LZ = 40;
        List<int> list = new List<int>();


        public bool Roberts(double v1x, double v1y, double v1z, double v2x, double v2y, double v2z, double v3x, double v3y, double v3z, double cx, double cy, double cz)
        {
            double A = (v2y - v1y) * (v3z - v1z) - (v2z - v1z) * (v3y - v1y);
            double B = (v2z - v1z) * (v3x - v1x) - (v2x - v1x) * (v3z - v1z);
            double C = (v2x - v1x) * (v3y - v1y) - (v2y - v1y) * (v3x - v1x);
            double D = -1 * (A * v1x + B * v1y + C * v1z);

            if ((A * cx + B * cy + C * cz + D) < 0)
            {
                A = A * -1;
                B = B * -1;
                C = C * -1;
                D = D * -1;
            }
            if ((C * -1) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PaintOBJ()
        {
            if (!File_OBJ.loaded) return;
            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush b = new SolidBrush(Color.White);
            g.FillRectangle(b, 0, 0, pictureBox1.Width, pictureBox1.Height);
            Pen p = new Pen(Color.Green, 2);

            File_OBJ.offset(LX, LY, LZ, XKG, YKG, scale);

            for (int i = 0; i < File_OBJ.Surfaces.Count; i++)
            {
                if (Roberts(File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[0]].x,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[0]].y,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[0]].z,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[1]].x,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[1]].y,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[1]].z,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[2]].x,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[2]].y,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[i].NumPoint3d[2]].z,
                   File_OBJ.centr.x, File_OBJ.centr.y, File_OBJ.centr.z))
                { continue; }
                for (int j = 0; j < File_OBJ.Surfaces[i].NumPoint3d.Count - 1; j++)
                {
                    g.DrawLine(p, Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[j]].x),
                Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[j]].y),
                Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[j + 1]].x),
                Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[j + 1]].y));


                    g.DrawLine(p, Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[File_OBJ.Surfaces[i].NumPoint3d.Count - 1]].x),
                     Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[File_OBJ.Surfaces[i].NumPoint3d.Count - 1]].y),
                     Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[0]].x),
                     Convert.ToInt32(File_OBJ.VertexsN[File_OBJ.Surfaces[i].NumPoint3d[0]].y));

                }
            }
        }

        class Point3d
        {
            public double x = 0;
            public double y = 0;
            public double z = 0;
            public Point3d(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        };

        class Surface
        {
            public List<int> NumPoint3d = new List<int>();
        };

        class Object
        {
            public List<Point3d> Vertexs = new List<Point3d>();
            public List<Surface> Surfaces = new List<Surface>();
            public List<Point3d> VertexsN = new List<Point3d>();
            public List<Point3d> VertexsM = new List<Point3d>();
            public Point3d pointw = new Point3d(0, 0, 0);
            public Point3d centr = new Point3d(0, 0, 0);
            public bool loaded = false;

            public Object()
            {
            }

            public Object(string file_name)
            {
                string[] lines = File.ReadAllLines(file_name);
                {
                    foreach (string line in lines)
                        if (line.ToLower().StartsWith("v "))
                        {
                            var vx = line.Split(' ').Skip(1).Select(v => Double.Parse(v.Replace('.', ','))).ToArray();
                            Vertexs.Add(new Point3d((vx[0]), (vx[1]), (vx[2])));
                            VertexsN.Add(new Point3d((vx[0]), (vx[1]), 0));
                            VertexsM.Add(new Point3d((vx[0]), (vx[1]), (vx[2])));
                        }
                        else if (line.ToLower().StartsWith("f"))
                        {
                            string str;
                            bool key = true;
                            if (line[line.Length - 1] == ' ')
                            {
                                str = line.Remove(line.Length - 1, 1);
                            }
                            else
                            {
                                str = line;
                            }
                            while (key)
                            {
                                int h = 0, k = 0;
                                for (int i = 0; i < str.Length; i++)
                                {
                                    if (str[i] == '/')
                                    {
                                        h = i;
                                        int j = i;
                                        while (str[j] != ' ' && j < str.Length - 1)
                                        {
                                            k++;
                                            j++;
                                        }
                                        if (j == str.Length - 1)
                                        {
                                            k++;
                                        }
                                        str = str.Remove(h, k);
                                        break;
                                    }
                                    if (i == str.Length - 1)
                                    {
                                        key = false;
                                    }
                                }
                            }
                            var vx = str.Split(' ').Skip(1).Select(v => int.Parse(v)).ToArray();
                            Surface temp = new Surface();
                            foreach (int d in vx)
                            {
                                temp.NumPoint3d.Add(d - 1);
                            }
                            Surfaces.Add(temp);
                        }
                }
                loaded = true;
            }

            public void offset(double LX, double LY, double LZ, double XKG, double YKG, double scale)
            {
                double X1, Y1, Z1, X2, Y2, Z2, X3, Y3, Z3, XC = 0, YC = 0, ZC = 0;
                double COS;
                double SIN;
                for (int i = 0; i < Vertexs.Count; i++)
                {
                    X1 = Vertexs[i].x;
                    Y1 = Vertexs[i].y;
                    Z1 = Vertexs[i].z;
                    COS = Math.Cos(LX * Math.PI / 180);
                    SIN = Math.Sin(LX * Math.PI / 180);
                    Y2 = Math.Round(Y1 * COS - Z1 * SIN, 5);
                    Z2 = Math.Round(Y1 * SIN + Z1 * COS, 5);
                    COS = Math.Cos(LY * Math.PI / 180);
                    SIN = Math.Sin(LY * Math.PI / 180);
                    X2 = Math.Round(X1 * COS - Z2 * SIN, 5);
                    Z3 = Math.Round(X1 * SIN + Z2 * COS, 5);
                    COS = Math.Cos(LZ * Math.PI / 180);
                    SIN = Math.Sin(LZ * Math.PI / 180);
                    X3 = Math.Round(X2 * COS - Y2 * SIN, 5);
                    Y3 = Math.Round(X2 * SIN + Y2 * COS, 5);
                    X3 *= scale;
                    Y3 *= scale;
                    Z3 *= scale;

                    Point3d NN = new Point3d(X3 + XKG, Y3 + YKG, Z3);

                    VertexsM[i] = NN;
                    VertexsN[i].x = X3;
                    VertexsN[i].y = Y3;
                    VertexsN[i].x += XKG;
                    VertexsN[i].y += YKG;
                    XC += VertexsN[i].x;
                    YC += VertexsN[i].y;
                    ZC += X3;
                }

                centr = new Point3d((XC / Vertexs.Count), (YC / Vertexs.Count), (ZC / Vertexs.Count));

                X1 = 0; Y1 = 0 * Math.Cos(LX) + 0 * Math.Sin(LX);
                Z1 = 0 * Math.Sin(LX) + 0 * Math.Cos(LX);
                X2 = X1 * Math.Cos(LY) - Z1 * Math.Sin(LY); Y2 = Y1;
                Z2 = X1 * Math.Sin(LY) + Z1 * Math.Cos(LY);
                X3 = X2 * Math.Cos(LZ) + Y2 * Math.Sin(LZ);
                Y3 = -X2 * Math.Sin(LZ) + Y2 * Math.Cos(LZ); Z3 = Z2;
                X3 *= scale; Y3 *= scale; Z3 *= scale;
                X3 += XKG; Y3 += YKG;

                Point3d N = new Point3d(X3, Y3, Z3);
                pointw = N;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File_OBJ = new Object("icaso.obj");
            PaintOBJ();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File_OBJ = new Object("cube.obj");
            PaintOBJ();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            File_OBJ = new Object("dod.obj");
            PaintOBJ();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            scale += 5;
            XKG += 0.08 * scale / 80;
            YKG -= 0.08 * scale / 80;
            PaintOBJ();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            scale -= 5;
            XKG -= 0.08 * scale / 80;
            YKG += 0.08 * scale / 80;
            PaintOBJ();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LX += 10;
            PaintOBJ();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LY += 10;
            PaintOBJ();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LZ += 10;
            PaintOBJ();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            XKG += 30;
            PaintOBJ();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            XKG -= 30;
            PaintOBJ();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            YKG -= 30;
            PaintOBJ();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            YKG += 30;
            PaintOBJ();
        }
    }
}
