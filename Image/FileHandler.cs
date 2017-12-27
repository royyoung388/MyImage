using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyImage
{
    static class FileHandler
    {
        //打开图片
        public static string openImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择一张图片";
            openFileDialog.Filter = @"图片文件(*.png, *.jpg, *.jpeg, *.bmp) | *.png; *.jpg; *.jpeg; *.bmp";
            openFileDialog.Multiselect = false;

            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        //另存为文件
        public static bool saveImage(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = @"图片文件(*.png, *.jpg, *.bmp) | *.png; *.jpg; *.bmp";
                saveFileDialog.FilterIndex = 3;
                saveFileDialog.RestoreDirectory = true;

                DialogResult result = saveFileDialog.ShowDialog();
                if (DialogResult.OK == result)
                {
                    ImageFormat format = ImageFormat.Jpeg;
                    switch (Path.GetExtension(saveFileDialog.FileName).ToLower())
                    {
                        case ".jpg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case ".png":
                            format = ImageFormat.Png;
                            break;
                    }

                    try
                    {
                        if (saveImageTo(bitmap, saveFileDialog.FileName) != null)
                            return true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("save image error");
                        return false;
                    }
                }
                else if (DialogResult.Cancel == result)
                {
                    return true;
                }

            }
            return false;
        }

        //保存文件
        public static Bitmap saveImageTo(Bitmap bitmap, string fileName)
        {
            ImageFormat format = bitmap.RawFormat;

            //新建一个个bitmap类型的bmp2变量
            Bitmap bmp2 = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            //将第一个bmp拷贝到bmp2中
            Graphics draw = Graphics.FromImage(bmp2);
            draw.DrawImage(bitmap, 0, 0, new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);

            //释放资源
            draw.Dispose();
            //图片资源不在这里释放
            //bitmap.Dispose();

            try
            {
            }
            catch (Exception)
            {
                Console.WriteLine("save image to error");
                return null;
            }
            return bmp2;
        }
    }
}
