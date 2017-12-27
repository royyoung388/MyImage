using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyImage
{
    class Matrix
    {
        //平移
        public void translation(ref int x, ref int y, int transX, int transY)
        {
            double[,] matrix1 = { { x }, { y }, { 1 } };
            double[,] matrix2 = { { 1, 0, transX }, { 0, 1, transY }, { 0, 0, 1 } };
            double[,] result = matixMultiply(matrix1, matrix2);
            x = (int)result[0, 0];
            y = (int)result[1, 0];
        }

        //旋转
        public void rotation(ref int x, ref int y, double angle)
        {
            double[,] matrix1 = { { x }, { y }, { 1 } };
            double[,] matrix2 = { { Math.Cos(angle), -Math.Sin(angle), 0 }, { Math.Sin(angle), Math.Cos(angle), 0 }, { 0, 0, 1 } };
            double[,] result = matixMultiply(matrix1, matrix2);
            x = (int)result[0, 0];
            y = (int)result[1, 0];
        }

        //缩放
        public void scale(ref int x, ref int y, int scaleX, int scaleY)
        {
            double[,] matrix1 = { { x }, { y }, { 1 } };
            double[,] matrix2 = { { scaleX, 0, 0 }, { 0, scaleY, 0 }, { 0, 0, 1 } };
            double[,] result = matixMultiply(matrix1, matrix2);
            x = (int)result[0, 0];
            y = (int)result[1, 0];
        }

        //错切
        public void shear(ref int x, ref int y, int shearX, int shearY)
        {
            double[,] matrix1 = { { x }, { y }, { 1 } };
            double[,] matrix2 = { { 1, shearX, 0 }, { shearY, 1, 0 }, { 0, 0, 1 } };
            double[,] result = matixMultiply(matrix1, matrix2);
            x = (int)result[0, 0];
            y = (int)result[1, 0];
        }

        //矩阵乘法计算
        private double[,] matixMultiply(double[,] matrix1, double[,] matrix2)
        {
            Console.WriteLine("matrix multiply : matrix1: {0}, matrix2: {1}", matrix1.ToString(), matrix2.ToString());
            double[,] result = new double[matrix1.GetLength(0), matrix2.GetLength(1)];
            //1的行数
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                //2的列数
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    //乘积求和
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                    {
                        // 1的行，2的列
                        result[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
            }
            Console.WriteLine("matrix multiply result : " + result.ToString());
            return result;
        }
    }
}
