using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;

namespace DICOMDump
{
    public partial class Form1 : Form
    {
        public String file_path;
        int columns, rows;
        double c, w;
        Bitmap bitmap;

        int bits_allocated;
        int high_bits;
        int bits_stored;

        public Form1()
        {
            InitializeComponent();
            columns = rows = 0;
            c = w = 0;

            MainFunction();

            //using System.Drawing;
            //pictureBox1.Image = bitmap;

            Bitmap img = new Bitmap(1024, 1024);
            Graphics g = Graphics.FromImage(img);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            g.DrawImage(bitmap, 0, 0, 1024, 1024);
            img = new Bitmap(img);
            
            pictureBox1.Image = img;
        }

        private Bitmap CreateBitmap(byte[] source, int width, int height, int wc, int ww)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                                    ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            byte[] rgb = new byte[width * height * 3];

            
            for (int i = 0; i < width * height; i++)
            {
                int value = source[2 * i] + source[2 * i + 1] * 256;
                //value >>= 4;//(bits_stored - high_bits);

                value >>= 2;
                
                rgb[3 * i] = (byte)value;
                rgb[3 * i + 1] = (byte)value;
                rgb[3 * i + 2] = (byte)value;
            }
            System.Runtime.InteropServices.Marshal.Copy(rgb, 0, ptr, width * height * 3);
            bitmap.UnlockBits(bmpData);
            bitmap.Save("sampleDicom.bmp");

            return bitmap;
        }

        private void Close_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private int getDataValueInt32(byte[] obyte, byte[] ibyte, int start)
        {
            obyte = new byte[32];
            bufferCopy(obyte, ibyte, start, 2);
            var vlLen = BitConverter.ToInt32(obyte, 0);
            start += 2;
            obyte = new byte[32];
            bufferCopy(obyte, ibyte, start, vlLen);
            var value = BitConverter.ToInt32(obyte, 0);

            return value;
        }

        private double getDataValueDouble(byte[] obyte, byte[] ibyte, int start)
        {
            obyte = new byte[32];
            bufferCopy(obyte, ibyte, start, 2);
            var vlLen = BitConverter.ToInt32(obyte, 0);
            start += 2;
            obyte = new byte[32];
            bufferCopy(obyte, ibyte, start, vlLen);
            var value = BitConverter.ToDouble(obyte, 0);

            return value;
        }

        /*
        
        mainルーチン
        
        */


        private void MainFunction()
        {
            string filename = null;

            String[] arg = Environment.GetCommandLineArgs();

            foreach (var s in arg)
                filename = s;

            filename = "temp.dcm";
            //filename = "mr0052111.dcm";

            FileInfo info = new FileInfo(filename);

            long size = info.Length;

            byte[] buffer = new byte[size];

            FileStream file = new FileStream(filename, FileMode.Open);

            file.Read(buffer, 0, (int)size);

            byte[] bytes = null;

            int i = 0;
            while (i < size)
            {
                if (buffer[i] == 0x44 && buffer[i + 1] == 0x49 && buffer[i + 2] == 0x43 && buffer[i + 3] == 0x4D)
                {
                    //DICOMかどうかの判定
                    //Console.WriteLine(i.ToString());
                    //writeBufferStrings(bytes, buffer, i, 4);
                }

                //0028,0100
                if (buffer[i] == 0x28 && buffer[i + 1] == 0x00)
                {
                    if (buffer[i + 2] == 0x00 && buffer[i + 3] == 0x10)
                    {
                        //画像データのタグの開始
                        Console.WriteLine("ポインタ<0028><0100> = {0}", i.ToString());

                        //readBufferHex(bytes, buffer, i, 4);

                        i += 4;

                        i += 2;

                        bits_allocated = getDataValueInt32(bytes, buffer, i);

                        i += 4;
                    }
                }

                //0028,0101
                if (buffer[i] == 0x28 && buffer[i + 1] == 0x00)
                {
                    if (buffer[i + 2] == 0x01 && buffer[i + 3] == 0x10)
                    {
                        //画像データのタグの開始
                        Console.WriteLine("ポインタ<0028><0010> = {0}", i.ToString());

                        //readBufferHex(bytes, buffer, i, 4);

                        i += 4;

                        i += 2;

                        high_bits = getDataValueInt32(bytes, buffer, i);

                        i += 4;
                    }
                }

                //0028,0102
                if (buffer[i] == 0x28 && buffer[i + 1] == 0x00)
                {
                    if (buffer[i + 2] == 0x02 && buffer[i + 3] == 0x10)
                    {
                        //画像データのタグの開始
                        Console.WriteLine("ポインタ<0028><0010> = {0}", i.ToString());

                        //readBufferHex(bytes, buffer, i, 4);

                        i += 4;

                        i += 2;

                        bits_stored = getDataValueInt32(bytes, buffer, i);

                        i += 4;
                    }
                }

                //0028,0010
                if (buffer[i] == 0x28 && buffer[i + 1] == 0x00)
                {
                    if (buffer[i + 2] == 0x10 && buffer[i + 3] == 0x00)
                    {
                        //画像データのタグの開始
                        Console.WriteLine("ポインタ<0028><0010> = {0}", i.ToString());

                        //readBufferHex(bytes, buffer, i, 4);

                        i += 4;

                        i += 2;

                        rows = getDataValueInt32(bytes, buffer, i);

                        i += 4;
                    }
                }


                //0028,0011
                if (buffer[i] == 0x28 && buffer[i + 1] == 0x00)
                {
                    if (buffer[i + 2] == 0x11 && buffer[i + 3] == 0x00)
                    {
                        //画像データのタグの開始
                        Console.WriteLine("ポインタ<0028><0011> = {0}", i.ToString());

                        //readBufferHex(bytes, buffer, i, 4);

                        i += 4;

                        i += 2;

                        columns = getDataValueInt32(bytes, buffer, i);

                        i += 4;
                    }
                }


                //0028,1050
                if (buffer[i] == 0x28 && buffer[i + 1] == 0x00)
                {
                    if (buffer[i + 2] == 0x50 && buffer[i + 3] == 0x10)
                    {
                        //画像データのタグの開始
                        Console.WriteLine("ポインタ<0028><1050> = {0}", i.ToString());

                        //readBufferHex(bytes, buffer, i, 4);

                        i += 4;

                        i += 2;

                        c = getDataValueDouble(bytes, buffer, i);

                        i += 4;
                    }
                }


                //0028,1051
                if (buffer[i] == 0x28 && buffer[i + 1] == 0x00)
                {
                    if (buffer[i + 2] == 0x51 && buffer[i + 3] == 0x10)
                    {
                        //画像データのタグの開始
                        Console.WriteLine("ポインタ<0028><1051> = {0}", i.ToString());

                        //readBufferHex(bytes, buffer, i, 4);

                        i += 4;

                        i += 2;

                        w = getDataValueDouble(bytes, buffer, i);

                        i += 4;
                    }
                }



                //7FE0,0010
                if (buffer[i] == 0xE0 && buffer[i + 1] == 0x7F)
                {
                    if (buffer[i + 2] == 0x10 && buffer[i + 3] == 0x00)
                    {
                        i = i + 12;
                        //画像データのタグの開始
                        Console.WriteLine(i.ToString());
                        //readBufferHex(bytes, buffer, i, 4);
                        //bytes = new byte[size - i];
                        bytes = new byte[size-i];
                        bufferCopy(bytes, buffer, i, (int)size- i);
                        bitmap = CreateBitmap(bytes, columns, rows, (int)c, (int)w);

                        i += 4;
                        
                        Console.WriteLine("Row = " + rows);
                        Console.WriteLine("Colomn = " + columns);
                        Console.WriteLine("C = " + c);
                        Console.WriteLine("W = " + w);

                        break;
                    }
                }

                i++;
            }
        }

        static void bufferCopy(byte[] obyte, byte[] ibyte, int istart, int length)
        {
            for (int k = 0; k < length; k++)
            {
                obyte[k] = ibyte[istart + k];
            }

        }
    }
}
