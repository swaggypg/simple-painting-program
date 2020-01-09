using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace CST17011
{
    public enum shape 
    {
        Line,                 //线
        Ellipse,             //椭圆
        Rectangle,          //矩形
        Regular_triangle,   //等腰三角形
        Right_triangle,      //直角三角形
        Straint_line,        //直线
        Arrow,               //箭头
        Diamon,               //菱形
        Star                 //星形
    }
    public enum tool
    {
        Selection,       //选择
        PaintBucket,     //油漆桶
        eraser,          //橡皮擦
        Straw           //吸管
    }
    public partial class Form1 : Form
    {
        private bool mark = false;
        private bool mark_pB2 = false;      //pictureBox2
        private bool mark3 = false;      //用来标识选择功能的状态
        private bool mark_color1 = true;//默认标识颜色一
        private bool mark_color2 = false;
        private Point point;
        private Bitmap bits;
        private Graphics bitG;

        int pen_width = 1;

        Pen pen = new Pen(Color.Black);
        private String pen_func;        //当前选中的功能或图形 

        Point EndPoint;
        Point StartPoint;
        
        int x = 0, y = 0;
        
        private Stack<int> xStack = new Stack<int>();//用于填充功能的实现
        private Stack<int> yStack = new Stack<int>();

        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel2.Text = pictureBox1.Width.ToString() + " x " + pictureBox1.Height.ToString() + "像素";
            bits = new Bitmap(pictureBox1.Width,
            pictureBox1.Height); //建立位图类对象，宽和高为指定值
                                 //得到位图对象的Graphics类的对象方法3
            bitG = Graphics.FromImage(bits);
            bitG.Clear(Color.White);//用白色清除位图对象中的图像
            pictureBox1.Image = bits;//bits记录了pictureBox1显示的图像 

            pen_func = shape.Line.ToString(); //初始形状设定为线
        }


        //######################################  菜单  ############################# 


        private void 新建ToolStripMenuItem_Click
            (object sender, EventArgs e)
        {
            bits = new Bitmap(pictureBox1.Width,
           pictureBox1.Height);
            bitG = Graphics.FromImage(bits);
            bitG.Clear(Color.White);//用白色清空位图对象bitG
            pictureBox1.Image = bits;
        }		//pictureBox1显示用白色清空位图对象bitG 

        private void 打开ToolStripMenuItem_Click
                (object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) ==
                DialogResult.OK)
            {
                bits.Dispose(); //撤销bitG所引用的对象
                                //建立指定文件的新位图对象
                bits = new Bitmap(openFileDialog1.FileName);
                //得到位图对象使用的Graphics类对象
                bitG = Graphics.FromImage(bits); pictureBox1.Image = bits;
            }
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) ==
                        DialogResult.OK)
            {
                string s = saveFileDialog1.FileName + ".jpeg";
                bits.Save(s,
                      System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }




        //#############################  鼠标移动  ###################################


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left &&( pen_func == shape.Line.ToString()||pen_func==tool.eraser.ToString()))//是否是鼠标左键按下且形状为线
            {
                point.X = e.X;
                point.Y = e.Y;      //画线段开始点
                if (pen_func == shape.Line.ToString())
                    pen.Color = color1.BackColor;
                else if (pen_func == tool.eraser.ToString())
                    pen.Color = color2.BackColor;
                mark = true;
            }		//鼠标左键按下标识 
            if (e.Button == MouseButtons.Right && pen_func == shape.Line.ToString())
            {
                point.X = e.X;
                point.Y = e.Y;      //画线段开始点
               
                pen.Color = color2.BackColor;
                mark = true;
            }

            if (e.Button == MouseButtons.Left && (pen_func == shape.Ellipse.ToString() ||
                pen_func == tool.Selection.ToString()|| 
                pen_func ==shape.Right_triangle.ToString() || 
                pen_func == shape.Straint_line.ToString()||
                pen_func == shape.Rectangle.ToString()||
                pen_func == shape.Arrow.ToString()||
                pen_func==shape.Regular_triangle.ToString()||
                pen_func==shape.Diamon.ToString()||
                pen_func==shape.Star.ToString()))//是否是鼠标左键按下and形状
            {
                StartPoint.X = e.X;//以鼠标左键被按下处作为矩形的一个顶点
                StartPoint.Y = e.Y;//StartPoint记录矩形的这个顶点
                EndPoint.X = e.X;//拖动鼠标移动的位置作为矩形另一顶点
                                 //EndPoint记录矩形的这个顶点，两个顶点定义一个矩形
                EndPoint.Y = e.Y;
                mark3 = true;
                mark = true;
            }	//开始拖动鼠标画图标记
            if(pen_func==tool.Selection.ToString()&&mark==true)   //随便点一下去除虚线框
            {
                Bitmap myBitmap = new Bitmap(pictureBox1.Image);
                Graphics g = Graphics.FromImage(myBitmap);
                pictureBox1.Image = myBitmap;
                copy.Enabled = false;
                cut.Enabled = false;        //复制粘贴键切换为不可用
            }
            if (pictureBox2.Image != null && pictureBox2.Visible)
            {
                Bitmap myBitmap = new Bitmap(pictureBox2.Image);
                bits = new Bitmap(pictureBox1.Image);
                Graphics g = Graphics.FromImage(bits);
                g.DrawImage(myBitmap, pictureBox2.Left, pictureBox2.Top);
                pictureBox1.Image = bits;//位图对象在pictureBox1中显示
                pictureBox2.Visible = false;
                mark3 = false;
            }
            if(pen_func==tool.PaintBucket.ToString())  //油漆桶
            {
                Bitmap myBitmap = new Bitmap(pictureBox1.Image);
                Point location = new Point(e.X, e.Y);
                Color fillColor=color1.BackColor;
                if (e.Button == MouseButtons.Right)
                    fillColor = color2.BackColor;
                Color oldColor = myBitmap.GetPixel(e.X, e.Y);

                floodFillScanLineWithStack(myBitmap, e.X,e.Y,fillColor,oldColor);
                pictureBox1.Image = myBitmap;
            }
            if (pen_func == tool.Straw.ToString()) //吸管
            {
                Bitmap bit = new Bitmap(pictureBox1.Image);
                if (e.Button == MouseButtons.Left)   //按左键改颜色一
                {
                    color1.BackColor = bit.GetPixel(e.X, e.Y);
                    pen.Color = color1.BackColor;
                }
                else if (e.Button == MouseButtons.Right)  //按右键改颜色二
                {
                    color2.BackColor = bit.GetPixel(e.X, e.Y);
                }
            }


        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (pen_func == shape.Line.ToString() || pen_func == tool.eraser.ToString())
            {
                mark = false;
                bits = new Bitmap(pictureBox1.Image);

                pictureBox1.Image = bits;     //保存了所画的图形 
            }
            if (pen_func == shape.Ellipse.ToString()|| 
                pen_func == shape.Right_triangle.ToString()||
                pen_func == shape.Straint_line.ToString()||
                pen_func == shape.Rectangle.ToString()||
                pen_func == shape.Arrow.ToString()||
                pen_func==shape.Regular_triangle.ToString()||
                pen_func==shape.Diamon.ToString()||
                pen_func == shape.Star.ToString())
            {
                EndPoint.X = e.X;
                EndPoint.Y = e.Y;
                Rectangle r1 = MakeRectangle(StartPoint, EndPoint);
                //最终椭圆画在pictureBox1属性Image引用的对象中
                Graphics bitG = Graphics.FromImage(pictureBox1.Image);
                if(pen_func == shape.Ellipse.ToString())
                    bitG.DrawEllipse(pen, r1);
                else if(pen_func == shape.Right_triangle.ToString())
                {
                    Point p = new Point(StartPoint.X, EndPoint.Y);
                    bitG.DrawLine(pen, StartPoint, EndPoint);
                    bitG.DrawLine(pen, StartPoint, p);
                    bitG.DrawLine(pen, p, EndPoint);
                }
                else if (pen_func == shape.Straint_line.ToString())
                {
                    bitG.DrawLine(pen, StartPoint, EndPoint);
                }
                else if(pen_func == shape.Rectangle.ToString())
                {
                    bitG.DrawRectangle(pen, r1);
                }
                else if(pen_func == shape.Arrow.ToString())
                {
                    Point p1 = new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y);
                    Point p2 = new Point(EndPoint.X, (StartPoint.Y + EndPoint.Y) / 2);
                    Point p3 = new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y);
                    Point p4 = new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y - (EndPoint.Y - StartPoint.Y) / 4);
                    Point p5 = new Point(StartPoint.X, EndPoint.Y - (EndPoint.Y - StartPoint.Y) / 4);
                    Point p6 = new Point(StartPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) / 4);
                    Point p7 = new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y + (EndPoint.Y - StartPoint.Y) / 4);
                    Point[] points = new Point[7] { p1, p2, p3, p4, p5, p6, p7 };
                    for (int i = 0; i < 6; i++)
                    {
                        bitG.DrawLine(pen, points[i], points[i + 1]);
                    }
                    bitG.DrawLine(pen, points[0], points[6]);
                }
                else if (pen_func == shape.Regular_triangle.ToString())
                {
                    Point p1 = new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y);
                    Point p2 = new Point(StartPoint.X, EndPoint.Y);
                    Point p3 = new Point(EndPoint.X, EndPoint.Y);
                    bitG.DrawLine(pen, p1, p2);
                    bitG.DrawLine(pen, p1, p3);
                    bitG.DrawLine(pen, p2, p3);
                }
                else if (pen_func == shape.Diamon.ToString())
                {
                    Point p1 = new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y);
                    Point p2 = new Point(EndPoint.X, (StartPoint.Y + EndPoint.Y) / 2);
                    Point p3 = new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y);
                    Point p4 = new Point(StartPoint.X, (StartPoint.Y + EndPoint.Y) / 2);
                    bitG.DrawLine(pen, p1, p2);
                    bitG.DrawLine(pen, p2, p3);
                    bitG.DrawLine(pen, p3, p4);
                    bitG.DrawLine(pen, p4, p1);
                }
                else if(pen_func == shape.Star.ToString())
                {
                    bitG.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, StartPoint.Y, EndPoint.X - (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    bitG.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, EndPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    bitG.DrawLine(pen, EndPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, EndPoint.X - (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96);
                    bitG.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96, EndPoint.X - (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y);
                    bitG.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, EndPoint.Y - (EndPoint.Y - StartPoint.Y) / 4);
                    bitG.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, EndPoint.Y - (EndPoint.Y - StartPoint.Y) / 4, StartPoint.X + (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y);
                    bitG.DrawLine(pen, StartPoint.X + (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y, StartPoint.X + (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96);
                    bitG.DrawLine(pen, StartPoint.X + (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96, StartPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    bitG.DrawLine(pen, StartPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, StartPoint.X + (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    bitG.DrawLine(pen, StartPoint.X + (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, StartPoint.Y);

                }
                mark = false;
                
            }
            if (pen_func == tool.Selection.ToString()&&mark3)
            {
                EndPoint.X = e.X;
                EndPoint.Y = e.Y;

                if (EndPoint.X != StartPoint.X && EndPoint.Y != StartPoint.Y)  //所选矩形框体长宽为0时为无效选择
                {
                    copy.Enabled = true;
                    cut.Enabled = true;
                }
                
                Bitmap myBitmap = new Bitmap(pictureBox1.Image);
                Rectangle cloneRect = MakeRectangle(StartPoint, EndPoint);
                System.Drawing.Imaging.PixelFormat format = myBitmap.PixelFormat;
                Bitmap cloneBitmap = myBitmap.Clone(cloneRect, format);
                Clipboard.SetDataObject(cloneBitmap);

                Bitmap bits = new Bitmap(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);   //建立位图对象，宽和高为选中区域大小
                Graphics g = Graphics.FromImage(bits);  //得到位图对象的Graphics类的对象
                g.Clear(Color.White);   //用白色清除位图对象中的图像
                myBitmap = new Bitmap(pictureBox1.Image);
                g = Graphics.FromImage(myBitmap);
                g.DrawImage(bits, StartPoint.X, StartPoint.Y, EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
                pictureBox1.Image = myBitmap;//位图对象显示在pictureBox1中，即清除剪切区域

                IDataObject iData = Clipboard.GetDataObject();

                Bitmap bit = (Bitmap)iData.GetData(DataFormats.Bitmap);
                pictureBox2.Width = bit.Width;//阴影为修改部分
                pictureBox2.Height = bit.Height;
                pictureBox2.Image = bit;
                pictureBox2.Top = StartPoint.Y;
                pictureBox2.Left = StartPoint.X;
                pictureBox2.Parent = pictureBox1;
                pictureBox2.Visible = true;

                
                mark = false;

            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = e.X.ToString() + ", " + e.Y.ToString() + "像素";
            if (mark && (pen_func == shape.Line.ToString() || pen_func == tool.eraser.ToString()))      //如果鼠标左键按下&&线
            {
                Graphics g = pictureBox1.CreateGraphics();
                //图形画在PictureBox表面
                g.DrawLine(pen, point.X, point.Y, e.X, e.Y);
                //图形画在位图对象bits中
                bitG = Graphics.FromImage(bits);//全局变量拿来拿去有点混乱，每次需要用的时候重新赋值一下比较保险
                bitG.DrawLine(pen, point.X, point.Y, e.X, e.Y);
                point.X = e.X;
                point.Y = e.Y;
            }//下次绘制画线段开始点

            if (mark&&mark3 && (pen_func == shape.Ellipse.ToString()|| 
                pen_func == shape.Rectangle.ToString() || 
                pen_func == tool.Selection.ToString()||
                pen_func==shape.Right_triangle.ToString()||
                pen_func == shape.Straint_line.ToString()||
                pen_func == shape.Arrow.ToString()||
                pen_func==shape.Regular_triangle.ToString()||
                pen_func == shape.Diamon.ToString()||
                pen_func==shape.Star.ToString())) //计算重画区域&&shape
            {
                Rectangle r1 = MakeRectangle(StartPoint, EndPoint);
                r1.Height += 2; r1.Width += 2;  //区域增大些
                pictureBox1.Invalidate(new Rectangle(0,0,100000,100000));//擦除上次鼠标移动时画的图形，r1为擦除区域
                pictureBox1.Update();//立即重画，即擦除
                Graphics g = pictureBox1.CreateGraphics();
                EndPoint.X = e.X;
                EndPoint.Y = e.Y;
                r1 = MakeRectangle(StartPoint, EndPoint);//计算椭圆新位置
                if(pen_func == shape.Ellipse.ToString())
                    g.DrawEllipse(pen, r1);//在新位置画椭圆，显示椭圆绘制的新位置
                else if(pen_func == shape.Rectangle.ToString() || pen_func == tool.Selection.ToString())
                    g.DrawRectangle(pen, r1);
                else if (pen_func == shape.Right_triangle.ToString())
                {
                    Point p = new Point(StartPoint.X, EndPoint.Y);
                    g.DrawLine(pen, StartPoint, EndPoint);
                    g.DrawLine(pen, StartPoint, p);
                    g.DrawLine(pen, p, EndPoint);
                }
                else if(pen_func == shape.Straint_line.ToString())
                {
                    g.DrawLine(pen, StartPoint, EndPoint);
                }
                else if(pen_func == shape.Arrow.ToString())
                {
                    Point p1 = new Point((StartPoint.X+EndPoint.X)/2, StartPoint.Y);
                    Point p2 = new Point(EndPoint.X, (StartPoint.Y + EndPoint.Y) / 2);
                    Point p3 = new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y);
                    Point p4 = new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y-(EndPoint.Y-StartPoint.Y)/4);
                    Point p5 = new Point(StartPoint.X, EndPoint.Y - (EndPoint.Y - StartPoint.Y) / 4);
                    Point p6 = new Point(StartPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) / 4);
                    Point p7 = new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y + (EndPoint.Y - StartPoint.Y) / 4);
                    Point[] points = new Point[7] { p1, p2, p3, p4, p5, p6, p7 };
                    for(int i = 0; i < 6; i++)
                    {
                        g.DrawLine(pen, points[i], points[i + 1]);
                    }
                    g.DrawLine(pen, points[0], points[6]);
                }
                else if (pen_func == shape.Regular_triangle.ToString())
                {
                    Point p1 = new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y);
                    Point p2 = new Point(StartPoint.X, EndPoint.Y);
                    Point p3 = new Point(EndPoint.X, EndPoint.Y);
                    g.DrawLine(pen, p1, p2);
                    g.DrawLine(pen, p1, p3);
                    g.DrawLine(pen, p2, p3);
                }
                else if(pen_func == shape.Diamon.ToString())
                {
                    Point p1 = new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y);
                    Point p2 = new Point(EndPoint.X, (StartPoint.Y + EndPoint.Y) / 2);
                    Point p3 = new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y);
                    Point p4 = new Point(StartPoint.X, (StartPoint.Y + EndPoint.Y) / 2);
                    g.DrawLine(pen, p1, p2);
                    g.DrawLine(pen, p2, p3);
                    g.DrawLine(pen, p3, p4);
                    g.DrawLine(pen, p4, p1);
                }
                else if (pen_func == shape.Star.ToString())
                {
                    g.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, StartPoint.Y, EndPoint.X - (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    g.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, EndPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    g.DrawLine(pen, EndPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, EndPoint.X - (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96);
                    g.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96, EndPoint.X - (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y);
                    g.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, EndPoint.Y - (EndPoint.Y - StartPoint.Y) / 4);
                    g.DrawLine(pen, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, EndPoint.Y - (EndPoint.Y - StartPoint.Y) / 4, StartPoint.X + (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y);
                    g.DrawLine(pen, StartPoint.X + (EndPoint.X - StartPoint.X) * 3 / 16, EndPoint.Y, StartPoint.X + (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96);
                    g.DrawLine(pen, StartPoint.X + (EndPoint.X - StartPoint.X) * 5 / 16, EndPoint.Y - (EndPoint.Y - StartPoint.Y) * 37 / 96, StartPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    g.DrawLine(pen, StartPoint.X, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, StartPoint.X + (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160);
                    g.DrawLine(pen, StartPoint.X + (EndPoint.X - StartPoint.X) * 37 / 96, StartPoint.Y + (EndPoint.Y - StartPoint.Y) * 59 / 160, EndPoint.X - (EndPoint.X - StartPoint.X) / 2, StartPoint.Y);

                }
            }
        }


        //#############################  鼠标移动画形状  ###################################


        private void 椭圆_Click(object sender, EventArgs e)
        {
            pen_func = shape.Ellipse.ToString(); //形状变换为椭圆
        }

        private void 直角三角形_Click(object sender, EventArgs e)
        {
            pen_func = shape.Right_triangle.ToString();  
        }

        private void 直线_Click(object sender, EventArgs e)
        {
            pen_func = shape.Straint_line.ToString();
        }
        private void 矩形_Click(object sender, EventArgs e)
        {
            pen_func = shape.Rectangle.ToString();
        }
        private void 箭头_Click(object sender, EventArgs e)
        {
            pen_func = shape.Arrow.ToString();
        }
        private void 正三角形_Click(object sender, EventArgs e)
        {
            pen_func = shape.Regular_triangle.ToString();
        }
        private void 菱形_Click(object sender, EventArgs e)
        {
            pen_func = shape.Diamon.ToString();
        }
        private void 五角星_Click(object sender, EventArgs e)
        {
            pen_func = shape.Star.ToString();
        }












        private Rectangle MakeRectangle(Point p1, Point p2)
        {
            int top, left, bottom, right;
            top = p1.Y <= p2.Y ? p1.Y : p2.Y;   //计算矩形左上角点的y坐标
            if (top < 0)   //防止鼠标脱出框体后松开引起的越界
                top = 0;
            left = p1.X <= p2.X ? p1.X : p2.X;//计算矩形左上角点的x坐标
            if (left <0)
                left = 0;
            bottom = p1.Y > p2.Y ? p1.Y : p2.Y;//计算矩形右下角点的y坐标
            if (bottom > pictureBox1.Height)
                bottom = pictureBox1.Height;
            right = p1.X > p2.X ? p1.X : p2.X;//计算矩形右下角点的x坐标
            if (right > pictureBox1.Width)
                right = pictureBox1.Width;
            return (new Rectangle(left+1, top+1, right - left+1, bottom - top+1));
        }   //返回矩形



        //#############################  区域选择  ###################################
        private void 选择_Click(object sender, EventArgs e)
        {
            toolStripButton4.Checked = true;
            pen_func = tool.Selection.ToString();
            pen = new Pen(Color.Gray, 1);
            float[] dashValues = { 2, 3 };//2是线长 3是间隔的宽度
            pen.DashPattern = dashValues;
        }

        private void 复制_Click(object sender, EventArgs e)
        {
            Bitmap bitOfPic2 = new Bitmap(pictureBox2.Image);
            Bitmap bitOfPic1 = new Bitmap(pictureBox1.Image);
            Graphics bitG = Graphics.FromImage(bitOfPic1);
            bitG.DrawImage(bitOfPic2, pictureBox2.Left, pictureBox2.Top, pictureBox2.Width, pictureBox2.Height);
            pictureBox1.Image = bitOfPic1;
            Clipboard.SetDataObject(bitOfPic2);
            paste.Enabled = true;
            //因为上面写的选择区域直接拖动的功能是选择之后直接剪切到pictureBox2，所以复制时需要在Box2下面重新画一遍
        }

        private void 粘贴_Click(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                Bitmap bit = (Bitmap)iData.GetData(DataFormats.Bitmap);
                pictureBox2.Width = bit.Width;//阴影为修改部分
                pictureBox2.Height = bit.Height;
                pictureBox2.Image = bit;
                pictureBox2.Top = pictureBox1.Top;
                pictureBox2.Left = pictureBox1.Left;
                pictureBox2.Parent = pictureBox1;
                pictureBox2.Visible = true;

                Bitmap myBitmap = new Bitmap(pictureBox1.Image); //粘贴时将现在屏幕上的虚线矩形去除
                Graphics g = Graphics.FromImage(myBitmap);
                pictureBox1.Image = myBitmap;

                copy.Enabled = false;
                cut.Enabled = false;
            }
        }

       

        private void 剪切_Click(object sender, EventArgs e)
        {
            复制_Click(sender, e);//调用复制菜单项单击事件处理函数
            Bitmap bits = new Bitmap(EndPoint.X-StartPoint.X,EndPoint.Y-StartPoint.Y);   //建立位图对象，宽和高为选中区域大小
            Graphics g = Graphics.FromImage(bits);  //得到位图对象的Graphics类的对象
            g.Clear(Color.White);   //用白色清除位图对象中的图像
            Bitmap myBitmap = new Bitmap(pictureBox1.Image);
            g = Graphics.FromImage(myBitmap);
            g.DrawImage(bits, StartPoint.X, StartPoint.Y, EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
            pictureBox1.Image = myBitmap;//位图对象显示在pictureBox1中，即清除剪切区域
            paste.Enabled = true;
            pictureBox2.Visible = false;

        }

      

        //#############################  拖动粘贴图  ###################################

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            mark_pB2 = true;
            pictureBox2.Cursor = Cursors.Hand;
            x = e.X;
            y = e.Y;

        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (mark_pB2)
            {
                int x1, y1;
                x1 = pictureBox2.Left + e.X - x;
                y1 = pictureBox2.Top + e.Y - y;
                pictureBox1.Invalidate();//擦除上次鼠标移动时画的图形
                pictureBox1.Update();//立即重画，即擦除
                pictureBox2.Left = x1;
                pictureBox2.Top = y1;
                
            }

        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            mark_pB2 = false;
            pictureBox2.Cursor = Cursors.Arrow;

        }


        //#############################  旋转图  ###################################
        private void 向右旋转90度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Visible)                            //如果有粘贴出来的picture Box2
            {
                Bitmap bits = new Bitmap(pictureBox2.Image);
                bits.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox2.Image = bits;
            }
            else                                                //没有就转整个pictureBox1
            {
                Bitmap bits = new Bitmap(pictureBox1.Image);
                bits.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox1.Image = bits;

            }
        }

        private void 向左旋转90度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Visible)                            //如果有粘贴出来的picture Box2
            {
                Bitmap bits = new Bitmap(pictureBox2.Image);
                bits.RotateFlip(RotateFlipType.Rotate270FlipNone);
                pictureBox2.Image = bits;
            }
            else                                                //没有就转整个pictureBox1
            {
                Bitmap bits = new Bitmap(pictureBox1.Image);
                bits.RotateFlip(RotateFlipType.Rotate270FlipNone);
                pictureBox1.Image = bits;

            }
        }

        private void 垂直翻转ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (pictureBox2.Visible)                            //如果有粘贴出来的picture Box2
            {
                Bitmap bits = new Bitmap(pictureBox2.Image);
                bits.RotateFlip(RotateFlipType.RotateNoneFlipY);
                pictureBox2.Image = bits;
            }
            else                                                //没有就转整个pictureBox1
            {
                Bitmap bits = new Bitmap(pictureBox1.Image);
                bits.RotateFlip(RotateFlipType.RotateNoneFlipY);
                pictureBox1.Image = bits;

            }
        }

        private void 旋转180度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Visible)                            //如果有粘贴出来的picture Box2
            {
                Bitmap bits = new Bitmap(pictureBox2.Image);
                bits.RotateFlip(RotateFlipType.Rotate180FlipNone);
                pictureBox2.Image = bits;
            }
            else                                                //没有就转整个pictureBox1
            {
                Bitmap bits = new Bitmap(pictureBox1.Image);
                bits.RotateFlip(RotateFlipType.Rotate180FlipNone);
                pictureBox1.Image = bits;

            }
        }


        private void 水平翻转ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Visible)                            //如果有粘贴出来的picture Box2
            {
                Bitmap bits = new Bitmap(pictureBox2.Image);
                bits.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox2.Image = bits;
            }
            else                                                //没有就转整个pictureBox1
            {
                Bitmap bits = new Bitmap(pictureBox1.Image);
                bits.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox1.Image = bits;

            }
        }

        private void 画笔_Click(object sender, EventArgs e)
        {
            pen_func = shape.Line.ToString();
            pen.Width = pen_width;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;//笔画变为实线
            mark = false;                       //mark拿来拿去都不知道到底是哪里赋了true 只好在点画笔之后再赋一次false好了
        }

        

        //#############################  油漆桶  ###################################
        private void 油漆桶_Click(object sender, EventArgs e)
        {
            pen_func = tool.PaintBucket.ToString();
        }

       
        //下面这段代码是网上找来然后修改的
        public void floodFillScanLineWithStack(Bitmap bit,int x, int y, Color newColor, Color oldColor)
        {
            if (oldColor.ToArgb() == newColor.ToArgb())
            {
                return;
            }
            int y1;
            bool spanLeft, spanRight;
            xStack.Push(x);
            yStack.Push(y);

            while (true)
            {
                if (xStack.Count==0||yStack.Count==0)
                    return;
                x = xStack.Pop();
                y = yStack.Pop();
                y1 = y;
                while (y1 >= 0 && bit.GetPixel(x, y1) == oldColor) y1--; //往下找到当前列的边界
                y1++; // 在边界的上一点为起点
                spanLeft = spanRight = false;
                while (y1 < pictureBox1.Height && bit.GetPixel(x, y1) == oldColor)
                {
                    bit.SetPixel(x, y1, newColor);
                    if (!spanLeft && x > 0 && bit.GetPixel(x - 1, y1) == oldColor)// just keep left line once in the stack  
                    {
                        xStack.Push(x - 1);
                        yStack.Push(y1);
                        spanLeft = true;
                    }
                    else if (spanLeft && x > 0 && bit.GetPixel(x - 1, y1) != oldColor)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && x < pictureBox1.Width - 1 && bit.GetPixel(x + 1, y1) == oldColor) // just keep right line once in the stack  
                    {
                        xStack.Push(x + 1);
                        yStack.Push(y1);
                        spanRight = true;
                    }
                    else if (spanRight && x < pictureBox1.Width - 1 && bit.GetPixel(x + 1, y1) != oldColor)
                    {
                        spanRight = false;
                    }
                    y1++;
                }
                if (oldColor.ToArgb() == newColor.ToArgb())
                {
                    return;
                }
            }

        }

      

        //#############################  橡皮擦and xiguan  ###################################

        private void 橡皮擦_Click(object sender, EventArgs e)
        {
            pen_func = tool.eraser.ToString();
        }

        

        private void 吸管_Click(object sender, EventArgs e)
        {
            pen_func = tool.Straw.ToString();
        }

       



        //#############################  粗细  ###################################

        private void px_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            switch (item.Text)
            {
                case "1px":
                    pen_width = 1;
                    break;
                case "3px":
                    pen_width = 3;
                    break;
                case "5px":
                    pen_width = 5;
                    break;
                case "12px":
                    pen_width = 12;
                    break;
                default:
                    break;
            }
            pen.Width = pen_width;
        }

       
        //#############################  颜色  ###################################

        private void color1_Click(object sender, EventArgs e)//标识颜色1
        {
            mark_color1 = true;
            mark_color2 = false;
        }
        private void color2_Click(object sender, EventArgs e)//标识颜色2
        {
            mark_color1 = false;
            mark_color2 = true;
        }

        
        private void colorChange_quick(object sender, EventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;
            if (mark_color1)
            {
                switch (item.Text)
                {
                    case "black":
                        color1.BackColor = Color.Black;
                        break;
                    case "red":
                        color1.BackColor = Color.Red;
                        break;
                    case "yellow":
                        color1.BackColor = Color.Yellow;
                        break;
                    case "oringe":
                        color1.BackColor = Color.Orange;
                        break;
                    case "gray":
                        color1.BackColor = Color.Gray;
                        break;
                    case "blue":
                        color1.BackColor = Color.Blue;
                        break;
                    case "green":
                        color1.BackColor = Color.Green;
                        break;
                    case "white":
                        color1.BackColor = Color.White;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (item.Text)
                {
                    case "black":
                        color2.BackColor = Color.Black;
                        break;
                    case "red":
                        color2.BackColor = Color.Red;
                        break;
                    case "yellow":
                        color2.BackColor = Color.Yellow;
                        break;
                    case "oringe":
                        color2.BackColor = Color.Orange;
                        break;
                    case "gray":
                        color2.BackColor = Color.Gray;
                        break;
                    case "blue":
                        color2.BackColor = Color.Blue;
                        break;
                    case "green":
                        color2.BackColor = Color.Green;
                        break;
                    case "white":
                        color2.BackColor = Color.White;
                        break;
                    default:
                        break;
                }
            }
            pen.Color = color1.BackColor;
        }


        
        private void 颜色编辑_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) ==
               DialogResult.OK)
            {
                if (mark_color1)
                {
                    color1.BackColor = colorDialog1.Color;
                }
                else
                {
                    color2.BackColor = colorDialog1.Color;
                }
            }
        }


        //#############################  图片特效  ###################################

        private void 灰化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color c;
            int i, j, xres, yres, r, g, b;
            xres = pictureBox1.Image.Width;
            yres = pictureBox1.Image.Height;
            Bitmap box1 = new Bitmap(pictureBox1.Image);
            Bitmap box2 = new Bitmap(xres, yres);
            for (i = 0; i < xres; i++)
            {
                for (j = 0; j < yres; j++)
                {
                    c = box1.GetPixel(i, j);//得到此点彩色颜色值
                    r = c.R;
                    g = c.G;
                    b = c.B;
                    r = (r + g + b) / 3;//求红、绿、蓝三基色的平均值
                    c = Color.FromArgb(r, r, r);//彩色变为灰度
                    box2.SetPixel(i, j, c);
                }
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = box2;
        }

        private void 浮雕ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int Height = this.pictureBox1.Image.Height;
            int Width = this.pictureBox1.Image.Width;
            Bitmap newBitmap = new Bitmap(Width, Height);
            Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
            Color pixel1, pixel2;
            for (int x = 0; x < Width - 1; x++)
            {
                for (int y = 0; y < Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    pixel1 = oldBitmap.GetPixel(x, y);
                    pixel2 = oldBitmap.GetPixel(x + 1, y + 1);
                    r = Math.Abs(pixel1.R - pixel2.R + 128);
                    g = Math.Abs(pixel1.G - pixel2.G + 128);
                    b = Math.Abs(pixel1.B - pixel2.B + 128);
                    if (r > 255)
                        r = 255;
                    if (r < 0)
                        r = 0;
                    if (g > 255)
                        g = 255;
                    if (g < 0)
                        g = 0;
                    if (b > 255)
                        b = 255;
                    if (b < 0)
                        b = 0;
                    newBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            this.pictureBox1.Image = newBitmap;
            
        }




    }
}
