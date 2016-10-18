using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using ComputerVisionLib;
using System.Data;

namespace FaceService
{
    /// <summary>
    /// 功能名稱 : 臉部數據處理
    /// </summary>
    public class FaceDP
    {
        private CascadeClassifier _cascadeClassifier;
        private DataStoreAccess _dataStoreAccess;
        private Bitmap _cropFaces;
        private Image<Bgr, byte> _unknowFace;
        private FaceRecognitionService _recognizer;
        private CComputerVison _computerVison;

        /// <summary>
        /// face's data processing initialize
        /// </summary>
        public FaceDP()
        {
            //"/haarcascade_frontalface_alt_tree.xml"
            //"/haarcascade_frontalface_default.xml"
            string str = System.Environment.CurrentDirectory;
            _cascadeClassifier = new CascadeClassifier("./Classifier/haarcascade_frontalface_default.xml");
            _dataStoreAccess = new DataStoreAccess("../DB/faceDB.db3");
            _cropFaces = null;
            _unknowFace = new Image<Bgr, byte>("./unknown.bmp");
            _recognizer = new FaceRecognitionService("../DB/faceDB.db3", "./TrainedFaces.yaml");
            _computerVison = new CComputerVison();
        }

        public FaceDP(string webPath)
        {
            _cascadeClassifier = new CascadeClassifier(webPath + "\\Classifier\\haarcascade_frontalface_default.xml");
            _dataStoreAccess = new DataStoreAccess(webPath + "\\DB\\faceDB.db3");
            _cropFaces = null;
            _unknowFace = new Image<Bgr, byte>( webPath + "\\Images\\unknown.bmp");
            _recognizer = new FaceRecognitionService(webPath + "\\DB\\faceDB.db3", webPath + "\\bin\\TrainedFaces.yaml");
            _computerVison = new CComputerVison();
        }

        /// <summary>
        /// 人臉偵測
        /// </summary>
        /// <param name="Image">來源影像</param>
        /// <returns>處理後的影像</returns>
        public Image<Bgr, byte> FaceDetection(Image<Bgr, byte> Image)
        {
            var grayframe = Image.Convert<Gray, byte>();
            Size minSize = new Size(30, 30);
            Size maxSize = new Size(180, 180);//90,90
            //var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, minSize,maxSize); //the actual face detection happens here 1.1,10
            var faces = _computerVison.FaceDetectByDLL(grayframe);
            if (faces.Count() == 0 && _cropFaces != null) _cropFaces = null;
            foreach (var face in faces)
            {
                _cropFaces = Image.ToBitmap().Clone(face, Image.ToBitmap().PixelFormat);//擷取臉部
                Image.Draw(face, new Bgr(Color.Red), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them                
                //_dataStoreAccess.SaveFace("test", );
            }
            grayframe.Dispose();
            return Image;
        }

        /// <summary>
        /// 人臉辨識
        /// </summary>
        /// <returns></returns>
        public int RecognizeFace()
        {
            if(_cropFaces == null)
            {
                return 0;
            }
            return _recognizer.RecognizeUser(new Image<Gray, byte>((Bitmap)(_cropFaces)));
        }

        /// <summary>
        /// 取得截取後的人臉影像
        /// </summary>
        /// <returns></returns>
        public Image<Bgr, byte> GetFaceImage()
        {
            if (_cropFaces == null) return _unknowFace;
            Image<Bgr, byte> convertImage = new Image<Bgr, byte>((Bitmap)(_cropFaces));
            return convertImage.Resize(200, 200, Emgu.CV.CvEnum.Inter.Linear);
        }

        /// <summary>
        /// 是否偵測到人臉
        /// </summary>
        /// <returns></returns>
        public bool IsFaceDetected()
        {
            if (_cropFaces == null) return false;
            else return true;
        }

        /// <summary>
        /// 初始化截取的人臉
        /// </summary>
        public void InitCropFace()
        {
            _cropFaces = null;
        }

        /// <summary>
        /// 儲存偵測到的人臉到資料庫
        /// </summary>
        /// <param name="username">人臉對應的人名</param>
        /// <returns></returns>
        public string SaveCropFace(string username)
        {
            ImageConverter converter = new ImageConverter();
            byte[] faceBlob = (byte[])converter.ConvertTo(_cropFaces, typeof(byte[]));
            return _dataStoreAccess.SaveFace(username, "M", faceBlob);
        }

        /// <summary>
        /// 將人名、臉部圖像傳入，並存入資料庫
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="faceBlob"></param>
        public void SaveCropFace(string userName, string gender, Bitmap faceImg)
        {
            ImageConverter converter = new ImageConverter();
            byte[] faceBlob = (byte[])converter.ConvertTo(faceImg, typeof(byte[]));

            try
            {
                _dataStoreAccess.SaveFace(userName, gender, faceBlob);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// 取得資料庫中所有已建檔人名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllUsernames()
        {
            return _dataStoreAccess.GetAllUsernames();
        }

        /// <summary>
        /// 訓練資料庫中的人臉資料
        /// </summary>
        /// <returns></returns>
        public bool TrainRecognizer()
        {
            return _recognizer.TrainRecognizer();
        }

        /// <summary>
        /// 由UserId取得UserName
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserNameByUserId(int userId)
        {
            var userName = _dataStoreAccess.GetUsername(userId);

            if(userName == "")
            {
                return "查無此人";
            }
            return userName;
        }

        /// <summary>
        /// 刪除該人名以及其所有臉部資料
        /// </summary>
        /// <param name="userName">User Name</param>
        public void DeleteUserByName(string userName)
        {
            _dataStoreAccess.DeleteUser(userName);
        }

        /// <summary>
        /// 刪除人臉資料By ID
        /// </summary>
        /// <param name="id">人臉ID</param>
        /// <returns></returns>
        public bool DeleteFaceById(string id)
        {
           return _dataStoreAccess.DeleteFace(id);
        }

        /// <summary>
        /// 取得臉部資料清單
        /// </summary>
        /// <param name="userName">人名</param>
        /// <returns></returns>
        public List<string> GetFacesByUserName(string userName)
        {
            return _dataStoreAccess.GetFacesByUserName(userName);
        }

        /// <summary>
        /// 依userID取得資料庫中臉部所有臉部樣本ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetFacesListByUserId(int userID)
        {
            return _dataStoreAccess.GetFacesListByUserID(userID);
        }

        /// <summary>
        /// 取得faceSample
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Image<Bgr, byte> GetFaceById(string id)
        {
            byte[] face = _dataStoreAccess.GetFaceById(id);
            Stream stream = new MemoryStream();
            stream.Write(face, 0, face.Length);
            var faceImage = new Image<Bgr, byte>(new Bitmap(stream));
            return faceImage;
        }

        public DataTable GetAllPersonInDB() {
            DataTable dt = new DataTable();

            dt = _dataStoreAccess.GetAllPersonInfo();

            return dt;
        }

        /// <summary>
        /// 取得性別代碼DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetGenderCode() {
            DataTable dt = _dataStoreAccess.GetGenderCode();
            return dt;
        }

        /// <summary>
        /// 建立個人資訊
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public string CreatePerson(PersonInfo person)
        {
            person.userId = _dataStoreAccess.GenerateUserId();
            return _dataStoreAccess.CreatePersonInfo(person);
        }

        /// <summary>
        /// 取得個人資料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetPersonInfo(int userId)
        {
            return _dataStoreAccess.GetPersonInfo(userId);
        }

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="person"></param>
        public void UpdatePersonInfo(PersonInfo person)
        {
            _dataStoreAccess.UpdatePersonInfo(person);
        }

        /// <summary>
        /// 刪除個人資料
        /// </summary>
        /// <param name="userId"></param>
        public void DeletePerson(int userId)
        {
            _dataStoreAccess.DeletePerson(userId);
        }
    }
}
