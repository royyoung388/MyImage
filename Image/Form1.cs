using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyImage
{
    public partial class Form1 : Form
    {
        private Bitmap originBmp;
        private Bitmap currentBmp;
        //打开的文件
        private string fileName;

        //灰度化的选项
        enum GrayMode
        {
            COMPONENT_RED,  //最大分量法—红
            COMPONENT_GREEN,    //最大分量法-绿
            COMPONENT_BLUE,     //最大分量法—蓝
            MAX_RGB,        //最大值法
            AVERAGE_RGB,        //平均值法
            WEIGHT_RGB,     //加权平均法
        }

        enum Mirror
        {
            HRIZONTAL,  //水平
            VERTICAL    //垂直
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void bt_reset_Click(object sender, EventArgs e)
        {
            currentBmp = originBmp;
            this.picture.Image = currentBmp;
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileName = FileHandler.openImage();

            if (fileName == null)
                return;

            //这样打开文件处理会有错
            //originBmp = Bitmap.FromFile(fileName, true) as Bitmap;
            FileStream fs = File.OpenRead(fileName);
            Image img = Image.FromStream(fs);
            fs.Close();
            originBmp = new Bitmap(img);
            currentBmp = originBmp.Clone() as Bitmap;
            this.picture.Image = currentBmp;
            Console.WriteLine("bitmap format " + currentBmp.PixelFormat);
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName == null)
                return;

            Bitmap bmp = FileHandler.saveImageTo(currentBmp, fileName);
            if (bmp == null)
                MessageBox.Show(this, "Save Image To File Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                currentBmp.Dispose();
                currentBmp = bmp;
                this.picture.Image = currentBmp;
            }
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isSaved = FileHandler.saveImage(currentBmp);
            if (!isSaved)
                MessageBox.Show(this, "Save Image Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void 红色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapGray(currentBmp, GrayMode.COMPONENT_RED);
            this.picture.Image = currentBmp;
        }

        private void 绿色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapGray(currentBmp, GrayMode.COMPONENT_GREEN);
            this.picture.Image = currentBmp;
        }

        private void 蓝色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapGray(currentBmp, GrayMode.COMPONENT_BLUE);
            this.picture.Image = currentBmp;
        }

        private void 最大值法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapGray(currentBmp, GrayMode.MAX_RGB);
            this.picture.Image = currentBmp;
        }

        private void 平均值法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapGray(currentBmp, GrayMode.AVERAGE_RGB);
            this.picture.Image = currentBmp;
        }

        private void 加权平均值法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapGray(currentBmp, GrayMode.WEIGHT_RGB);
            this.picture.Image = currentBmp;
        }

        private void 水平镜像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapMirror(currentBmp, Mirror.HRIZONTAL);
            this.picture.Image = currentBmp;
        }

        private void 垂直镜像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBmp = bitmapMirror(currentBmp, Mirror.VERTICAL);
            this.picture.Image = currentBmp;
        }

        //灰度化
        private Bitmap bitmapGray(Bitmap bitmap, GrayMode mode)
        {
            if (bitmap == null)
            {
                return null;
            }

            Bitmap newBitmap = bitmap.Clone() as Bitmap;
            int width = newBitmap.Width;
            int height = newBitmap.Height;
            BitmapData bitmapData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, newBitmap.PixelFormat);
            int stride = bitmapData.Stride;//stride是一整行的宽度 32色图情况下为width*4

            IntPtr ptr = bitmapData.Scan0;
            byte[] src = new byte[stride * height];
            Marshal.Copy(ptr, src, 0, stride * height);

            for (int i = 0; i < height; i++)
                for (int j = 0; j < stride; j += 4)
                {
                    int k = i * stride;
                    switch (mode)
                    {
                        case GrayMode.COMPONENT_RED:
                            src[k + j + 1] = src[k + j + 2] = src[k + j];
                            break;
                        case GrayMode.COMPONENT_GREEN:
                            src[k + j] = src[k + j + 2] = src[k + j + 1];
                            break;
                        case GrayMode.COMPONENT_BLUE:
                            src[k + j] = src[k + j + 1] = src[k + j + 2];
                            break;
                        case GrayMode.AVERAGE_RGB:
                            src[k + j] = src[k + j + 1] = src[k + j + 2] = (byte)((src[k + j] + src[k + j + 1] + src[k + j + 2]) / 3);
                            break;
                        case GrayMode.MAX_RGB:
                            src[k + j] = src[k + j + 1] = src[k + j + 2] = Math.Max(Math.Max(src[k + j], src[k + j + 1]), src[k + j + 2]);
                            break;
                        case GrayMode.WEIGHT_RGB:
                            byte sum = (byte)(src[k + j] * 0.299 + src[k + j + 1] * 0.578 + src[k + j + 2] * 0.114);
                            src[k + j + 1] = src[k + j + 2] = src[k + j] = sum;
                            break;
                    }
                }

            Marshal.Copy(src, 0, ptr, stride * height);
            newBitmap.UnlockBits(bitmapData);
            return newBitmap;
        }

        //镜像
        private Bitmap bitmapMirror(Bitmap bitmap, Mirror mode)
        {
            if (bitmap == null)
            {
                return null;
            }

            Bitmap newBmp = bitmap.Clone() as Bitmap;
            int width = newBmp.Width;
            int height = newBmp.Height;
            BitmapData data = newBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, newBmp.PixelFormat);
            int stride = data.Stride;

            byte[] src = new byte[stride * height];
            IntPtr ptr = data.Scan0;
            Marshal.Copy(ptr, src, 0, stride * height);

            switch (mode)
            {
                //竖直镜像
                case Mirror.VERTICAL:
                    //行数
                    for (int i = 0; i < height / 2; i++)
                    {
                        //列数
                        for (int j = 0; j < stride / 4; j ++)
                        {
                            //第i行第j列的像素点中第s个字节
                            for (int s = 0; s < 4; s++)
                            {
                                swap(ref src[i * stride + j * 4 + s], ref src[(height - i - 1) * stride + j * 4 + s]);
                            }
                        }
                    }
                    break;
                //水平镜像
                case Mirror.HRIZONTAL:
                    //行数
                    for (int i = 0; i < height; i++)
                    {
                        //列数
                        for (int j = 0; j < stride / 4 / 2; j++)
                        {
                            //第i行第j列的像素点中第s个字节
                            for (int s = 0; s < 4; s++)
                            {
                                swap(ref src[i * stride + j * 4 + s], ref src[i * stride + (width - j - 1) * 4 + s]);
                            }
                        }
                    }
                    break;
            }

            Marshal.Copy(src, 0, ptr, stride * height);
            newBmp.UnlockBits(data);

            return newBmp;
        }

        //交换两个byte
        private void swap(ref byte a, ref byte b)
        {
            byte temp = a;
            a = b;
            b = temp;
        }

        private void bar_rotate_Scroll(object sender, EventArgs e)
        {
            if (currentBmp == null)
                return;
            
            
        }
    }
}
