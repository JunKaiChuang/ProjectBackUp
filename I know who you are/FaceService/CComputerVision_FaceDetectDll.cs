#define Is64bit

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

using Emgu.CV.CvEnum;
using Emgu.Util;

using System.Runtime.InteropServices;
using System.Runtime;


namespace ComputerVisionLib
{

    public enum eDLLDetectType
    {
        EDLL_DTCTYPE_FRONT60 = 0,
        EDLL_DTCTYPE_SIDE90_SLOW,
        EDLL_DTCTYPE_SIDE90_MRSLOW,
        EDLL_DTCTYPE_FRONT60LIGHT,
    }
    /// <summary>
    /// 新C++LIB 人臉偵測方案
    /// </summary>
    unsafe public partial class CComputerVison
    {
#if  Is64bit 
        //正臉+-60度
        [DllImport("libfacedetect-x64.dll", EntryPoint = "?facedetect_frontal@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        public static extern int* facedetect_frontal(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.
        //側臉+-90度，FPS會較慢
        [DllImport("libfacedetect-x64.dll", EntryPoint = "?facedetect_multiview@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        public static extern int* facedetect_multiview(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.
        //側臉+-90度，FPS會更慢
        [DllImport("libfacedetect-x64.dll", EntryPoint = "?facedetect_multiview_reinforce@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        public static extern int* facedetect_multiview_reinforce(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.

        //正臉+-60度，改善光線影響
        [DllImport("libfacedetect-x64.dll", EntryPoint = "?facedetect_frontal_tmp@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        public static extern int* facedetect_frontal_tmp(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.
#else
        //正臉+-60度
        //[DllImport("libfacedetect.dll", EntryPoint = "?facedetect_frontal@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        [DllImport("libfacedetect.dll", EntryPoint = "?facedetect_frontal@@YAPAHPAEHHHMHHH@Z", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int* facedetect_frontal(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.
        //側臉+-90度，FPS會較慢
        //[DllImport("libfacedetect.dll", EntryPoint = "?facedetect_multiview@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        [DllImport("libfacedetect.dll", EntryPoint = "?facedetect_multiview@@YAPAHPAEHHHMHHH@Z", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int* facedetect_multiview(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.
        //側臉+-90度，FPS會更慢
        //[DllImport("libfacedetect.dll", EntryPoint = "?facedetect_multiview_reinforce@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        [DllImport("libfacedetect.dll", EntryPoint = "?facedetect_multiview_reinforce@@YAPAHPAEHHHMHHH@Z", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int* facedetect_multiview_reinforce(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.

        //正臉+-60度，改善光線影響
        //[DllImport("libfacedetect.dll", EntryPoint = "?facedetect_frontal_tmp@@YAPEAHPEAEHHHMHHH@Z", CharSet = CharSet.Unicode)]
        [DllImport("libfacedetect.dll", EntryPoint = "?facedetect_frontal_tmp@@YAPAHPAEHHHMHHH@Z", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int* facedetect_frontal_tmp(IntPtr gray_image_data, int width, int height, int step, //input image, it must be gray (single-channel) image!
                           float scale, //scale factor for scan windows
                           int min_neighbors, //how many neighbors each candidate rectangle should have to retain it
                           int min_object_width, //Minimum possible face size. Faces smaller than that are ignored.
                           int max_object_width = 0); //Maximum possible face size. Faces larger than that are ignored. It is the largest posible when max_object_width=0.

#endif




        public Rectangle[] FaceDetectByDLL(Image<Gray, Byte> grayImage, eDLLDetectType eDetectType = eDLLDetectType.EDLL_DTCTYPE_SIDE90_MRSLOW)
        {
            Rectangle[] recNewFaces;
            try
            {
                Mat img = new Mat();
                img = grayImage.Mat;
                IntPtr intPtrImg = grayImage.MIplImage.ImageData;
                int* pResults = null;
                switch ((int)eDetectType)
                {
                    case (int)eDLLDetectType.EDLL_DTCTYPE_FRONT60://+-60度正臉 
                        pResults = facedetect_frontal(intPtrImg, grayImage.Width, grayImage.Height, img.Step, 1.1f, 5, 24);
                        break;
                    case (int)eDLLDetectType.EDLL_DTCTYPE_SIDE90_SLOW: //+-90度側臉 FPS較低
                        pResults = facedetect_multiview_reinforce(intPtrImg, grayImage.Width, grayImage.Height, img.Step, 1.1f, 5, 24);
                        break;
                    case (int)eDLLDetectType.EDLL_DTCTYPE_SIDE90_MRSLOW://+-90度側臉 FPS更低
                        pResults = facedetect_multiview_reinforce(intPtrImg, grayImage.Width, grayImage.Height, img.Step, 1.1f, 5, 24);

                        break;
                    case (int)eDLLDetectType.EDLL_DTCTYPE_FRONT60LIGHT://+-60度正臉 改善光線
                        pResults = facedetect_frontal_tmp(intPtrImg, grayImage.Width, grayImage.Height, img.Step, 1.1f, 5, 24);

                        break;
                }
                if (pResults == null)
                {
                    return null;
                }
                int faceCount = *pResults;
                recNewFaces = new Rectangle[faceCount];
                for (int i = 0; i < faceCount; i++)
                {
                    short* p = ((short*)(pResults + 1)) + 6 * i;
                    int x = p[0];
                    int y = p[1];
                    int w = p[2];
                    int h = p[3];
                    int neighbors = p[4];
                    recNewFaces[i] = new Rectangle(x, y, w, h);
                }
                return recNewFaces;
            }
            catch (Exception )
            {
                //CDebug.jmsgEx("[EventPorc]Exception!!{0}", ex.Message);
                //例外的處理方法，如秀出警告
            }
            return null;

        }


    }
}
