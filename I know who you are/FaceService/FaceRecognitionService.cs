using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Microsoft.SqlServer.Server;

namespace FaceService
{
    /// <summary>
    /// 功能名稱 : 臉部辨識
    /// </summary>
    class FaceRecognitionService
    {
        private FaceRecognizer _faceRecognizer;
        private LBPHFaceRecognizer _LBPHFaceRecognizer;
        private DataStoreAccess _dataStoreAccess;
        private String _recognizerFilePath;

        internal class Face
        {
            public byte[] Image { get; set; }
            public int Id { get; set; }
            public String Label { get; set; }
            public int UserId { get; set; }
        }

        public FaceRecognitionService(String databasePath, String recognizerFilePath)
        {
            _recognizerFilePath = recognizerFilePath;
            _dataStoreAccess = new DataStoreAccess(databasePath);
            _faceRecognizer = new EigenFaceRecognizer(80, 1.1);
            _LBPHFaceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
        }

        public bool TrainRecognizer()
        {
            var allFaces = _dataStoreAccess.CallFaces("ALL_USERS");
            if (allFaces != null)
            {
                var faceImages = new Image<Gray, byte>[allFaces.Count];
                var faceLabels = new int[allFaces.Count];
                for (int i = 0; i < allFaces.Count; i++)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(allFaces[i].Image, 0, allFaces[i].Image.Length);
                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages[i] = faceImage.Resize(200, 200, Inter.Cubic);
                    faceLabels[i] = allFaces[i].UserId;
                }

                //LBPHFaceRecognizer--------------------------
                _LBPHFaceRecognizer.Train(faceImages, faceLabels);
                _LBPHFaceRecognizer.Save(_recognizerFilePath);

                ////EigenFaceRecognizer-------------------------
                //_faceRecognizer.Train(faceImages, faceLabels);
                //_faceRecognizer.Save(_recognizerFilePath);
            }
            return true;

        }

        public void LoadRecognizerData()
        {
            _LBPHFaceRecognizer.Load(_recognizerFilePath);
            //_faceRecognizer.Load(_recognizerFilePath);
        }

        public int RecognizeUser(Image<Gray, byte> userImage)
        {
            /*  Stream stream = new MemoryStream();
              stream.Write(userImage, 0, userImage.Length);
              var faceImage = new Image<Gray, byte>(new Bitmap(stream));*/

            //_faceRecognizer.Load(_recognizerFilePath);
            //var result = _faceRecognizer.Predict(userImage.Resize(200, 200, Inter.Cubic));

            _LBPHFaceRecognizer.Load(_recognizerFilePath);
            var result = _LBPHFaceRecognizer.Predict(userImage);

            return result.Label;
        }
    }
}
