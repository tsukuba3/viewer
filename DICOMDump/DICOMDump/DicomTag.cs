using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace DICOMDump
{
    class DicomTag
    {
        FileStream fs;
        int VRLength;
        BinaryReader r;

        public DicomTag(String path)
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            r = new BinaryReader(fs, Encoding.UTF8);
            VRLength = 0;
        }

        public int samplesPerPixelValue // <0028, 0002> SamplesPerPixel
        {
            set
            {
                this.samplesPerPixelValue = value;
            }
            get
            {
                return this.samplesPerPixelValue;
            }
        }

        public int planarConfigurationValue // <0028, 0006> PlanarConfiguration
        {
            set
            {
                this.planarConfigurationValue = value;
            }
            get
            {
                return this.planarConfigurationValue;
            }
        }

        public int numberOfFramesValue // <0028, 0008> NumberOfFrames        
        {
            set
            {
                this.numberOfFramesValue = value;
            }
            get
            {
                return this.numberOfFramesValue;
            }
        }

        public int columnsValue // <0028, 0010> Columns
        {
            set
            {
                this.columnsValue = value;
            }
            get
            {
                return this.columnsValue;
            }
        }

        public int rowsValue // <0028, 0011> Rows
        {
            set
            {
                this.rowsValue = value;
            }
            get
            {
                return this.rowsValue;
            }
        }

        public byte[] PixelDataValue; // <7FE0, 0010> PixelData

        public int samplesPerPixelOffset // <0028, 0002> SamplesPerPixel
        {
            set
            {
                this.samplesPerPixelOffset = value;
            }
            get
            {
                return this.samplesPerPixelOffset;
            }
        }

        public int planarConfigurationOffset // <0028, 0006> PlanarConfiguration
        {
            set
            {
                this.planarConfigurationOffset = value;
            }
            get
            {
                return this.planarConfigurationOffset;
            }
        }

        public int numberOfFramesOffset // <0028, 0008> NumberOfFrames        
        {
            set
            {
                this.numberOfFramesOffset = value;
            }
            get
            {
                return this.numberOfFramesOffset;
            }
        }

        public int columnsOffset // <0028, 0010> Columns
        {
            set
            {
                this.columnsOffset = value;
            }
            get
            {
                return this.columnsOffset;
            }
        }

        public int rowsOffset // <0028, 0011> Rows
        {
            set
            {
                this.rowsOffset = value;
            }
            get
            {
                return this.rowsOffset;
            }
        }

        public long PixelDataOffset // <7FE0, 0010> PixelData
        {
            set
            {
                this.PixelDataOffset = value;
            }
            get
            {
                return this.PixelDataOffset;
            }
        }

        public void setSamplesPerPixelValue(int arg)
        {
            this.samplesPerPixelValue = arg;
        }

        public void setPlanarConfigurationValue(int arg)
        {
            this.planarConfigurationValue = arg;
        }

        public void setNumberOfFramesValue(int arg)
        {
            this.numberOfFramesValue = arg;
        }

        public void setColumnsValue(int arg)
        {
            this.columnsValue = arg;
        }

        public void setRowsValue(int arg)
        {
            this.rowsValue = arg;
        }

        public void setPixelDataValue(int arg)
        {
            byte[] ptrSource  = r.ReadBytes(arg);
            this.PixelDataValue = ptrSource;
        }

        public void setSamplesPerPixelOffset(int arg)
        {
            this.samplesPerPixelOffset = arg;
        }

        public void setPlanarConfigurationOffset(int arg)
        {
            this.planarConfigurationOffset = arg;
        }

        public void setNumberOfFramesOffset(int arg)
        {
            this.numberOfFramesOffset = arg;
        }

        public void setColumnsOffset(int arg)
        {
            this.columnsOffset = arg;
        }

        public void setRowsOffset(int arg)
        {
            this.rowsOffset = arg;
        }

        public void setPixelDataOffset(int arg)
        {
            this.PixelDataOffset = arg;
        }

        public byte[] getPixelDataValue()
        {
            return PixelDataValue;
        }

        public void getElementFromDicomFile(String path)
        {
            long fsize = fs.Length;
            r.ReadBytes(128);
            String prefix = Encoding.UTF8.GetString(r.ReadBytes(4));
            if (!(prefix.Equals("DICM")))
            {
                r.Close();
                fs.Close();
                return;
            }
            fsize -= 132;
            while (fsize > 0)
            {
                string Tag = string.Format("{0:X4}", r.ReadUInt16()) + "," + string.Format("{0:X4}", r.ReadUInt16());
                string VR = Encoding.UTF8.GetString(r.ReadBytes(2));
                fsize -= 6;
                
                if ((VR == "OB") || (VR == "OW") || (VR == "OF") || (VR == "SQ") || (VR == "UT") || (VR == "OR") || (VR == "UN"))
                {
                    r.ReadBytes(2);
                    VRLength = (int)r.ReadUInt32();
                    fsize -= 6;
                }
                else
                {
                    VRLength = (int)r.ReadUInt16();
                    fsize -= 2;
                }
                
                switch (Tag)
                {
                    case "0028,0002":
                        setSamplesPerPixelOffset((int)fsize);
                        setSamplesPerPixelValue(BitConverter.ToInt32(r.ReadBytes(VRLength), 0));
                        break;

                    case "0028,0006":
                        setPlanarConfigurationOffset((int)fsize);
                        setPlanarConfigurationValue(BitConverter.ToInt32(r.ReadBytes(VRLength), 0));
                        break;

                    case "0028,0008":
                        setNumberOfFramesOffset((int)fsize);
                        setNumberOfFramesValue(BitConverter.ToInt32(r.ReadBytes(VRLength), 0));
                        break;

                    case "0028,0010":
                        setColumnsOffset((int)fsize);
                        setColumnsValue(BitConverter.ToInt32(r.ReadBytes(VRLength), 0));
                        break;

                    case "0028,0011":
                        setRowsOffset((int)fsize);
                        setRowsValue(BitConverter.ToInt32(r.ReadBytes(VRLength), 0));
                        continue;

                    case "7FE0, 0010":
                        setPixelDataOffset((int)fsize);
                        setPixelDataValue(BitConverter.ToInt32(r.ReadBytes(VRLength), 0));
                        break;
                }

                r.ReadBytes(VRLength);
                fsize -= VRLength;
            }
            r.Close();
            fs.Close();
        }

        public Bitmap drawImage()
        {
            return makeBitmap(getPixelDataValue(), columnsValue, rowsValue);
        }


        private Bitmap makeBitmap(byte[] source, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                                    ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            byte[] rgb = new byte[width * height * 3];
            for (int i = 0; i < width * height; i++)
            {
                int value = source[2 * i] + source[2 * i + 1] * 256;
                value >>= 4;
                rgb[3 * i] = (byte)value;
                rgb[3 * i + 1] = (byte)value;
                rgb[3 * i + 2] = (byte)value;
            }
            System.Runtime.InteropServices.Marshal.Copy(rgb, 0, ptr, width * height * 3);
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }
    }
}
