using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using FaceService;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
/*Json.NET相關的命名空間*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebService.Models;
using System.Text;
using Goheer.EXIF;

namespace WebService.Controllers
{
    public class FaceServiceController : Controller
    {
        private FaceDP _faceService;
        private PersonInfo _personInfoModel;
        private WebModel _webModel = new WebModel();

        // GET: FaceService
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 管理頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult ManagementPage()
        {
            return View();
        }


        /// <summary>
        /// 辨識人臉，回傳資訊
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult Recognize(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Content("null");
            }
            _faceService = new FaceDP(Server.MapPath("~"));

            
            //傳入影像做臉部偵測
            Image<Bgr, byte> newImage = _faceService.FaceDetection(ConvertFileToImage(file));
            //偵測完的人臉做辨識
            int userId = _faceService.RecognizeFace();
            //var userNameLabel = _faceService.GetUserNameByUserId(userId);
            //PersonInfo personInfo = new PersonInfo();

            //查無此人
            if (userId == 0) {
                return Content("null");
            }

            ////將資料轉成陣列
            //personInfo = _faceService.GetPersonInfo(userId);
            //List<string> info = new List<string>();
            //info.Add(personInfo.userName);
            //info.Add(personInfo.gender);
            //info.Add(personInfo.birthDate.Date.ToString("yyyy/MM/dd"));
            //info.Add(personInfo.tag);
            var info = _faceService.GetPersonInfo(userId);
            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            json = json.Substring(1, json.Length - 3);
            //var temp = Content(json, "application/json");
            //temp.ContentEncoding = Encoding.UTF8;

            return Content(json);
        }

        /// <summary>
        /// 取得所有資料庫中的人名
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllPersonName()
        {
            _faceService = new FaceDP(Server.MapPath("~"));
            List<string> userNames = _faceService.GetAllUsernames();
            string json = JsonConvert.SerializeObject(userNames);
            var temp = Content(json, "application/json");
            temp.ContentEncoding = Encoding.UTF8;

            return temp;
        }

        /// <summary>
        /// 依照userID取得資料庫中的人臉樣本list
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult GetFacesListByUserID(string userID)
        {        
            try
            {
                _faceService = new FaceDP(Server.MapPath("~"));
                DataTable faces = _faceService.GetFacesListByUserId(Convert.ToInt32(userID));
                string json = JsonConvert.SerializeObject(faces);
                return this.Json(json);
            }

            catch (Exception ex)
            {
                return this.Json(new { success = false, message = "取得失敗! \n" + ex.Message });
            }
        }

        /// <summary>
        /// 由臉部樣本ID取得圖像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetFaceByItID(string id)
        {
            
            try
            {
                _faceService = new FaceDP(Server.MapPath("~"));
                var face = _faceService.GetFaceById(id).ToBitmap();
                //轉換影像成Stream
                var stream = new MemoryStream();
                face.Save(stream, ImageFormat.Jpeg);
                var faceStream = new MemoryStream();
                face.Save(faceStream, ImageFormat.Jpeg);

                //to base64
                // Convert byte[] to Base64 String
                var imageBytes = stream.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                return Json(base64String);
            }
            catch (Exception ex)
            {

                return this.Json(new { success = false, message = "取得失敗! \n" + ex.Message });
            }
        }

        /// <summary>
        /// 新增人臉
        /// </summary>
        /// <param name="base64Image"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult AddPersonImage(string userName,string gender, string base64Image)
        {
            try
            {
                _faceService = new FaceDP(Server.MapPath("~"));

                // Convert Base64 String to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64Image);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                var faceImage = new Image<Bgr, byte>(new Bitmap(ms));
                _faceService.SaveCropFace(userName, gender, faceImage.ToBitmap());

                return this.Json(new { success = true, message = string.Empty });
            }

            catch (Exception ex)
            {
                return this.Json(new { success = false, message = userName + gender +  "新增失敗! \n" + ex.Message + "\n" + base64Image});
            }
        }

        /// <summary>
        /// 新增個人資料(初始)
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult AddNewPerson()
        {
            _faceService = new FaceDP(Server.MapPath("~"));
            _personInfoModel = new PersonInfo();

            DataTable dt = _faceService.GetGenderCode();
            ViewBag.GenderCode = _webModel.MapCodeData(dt);

            return View(_personInfoModel);
        }

        /// <summary>
        /// 新增個人資料(驗證)
        /// </summary>
        /// <param name="tempPerson"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult AddNewPerson(PersonInfo tempPerson)
        {
            _faceService = new FaceDP(Server.MapPath("~"));
            _personInfoModel = new PersonInfo();

            DataTable dt = _faceService.GetGenderCode();
            ViewBag.GenderCode = _webModel.MapCodeData(dt);

            if (ModelState.IsValid)
            {
                TempData["notice"] = _faceService.CreatePerson(tempPerson);
                return View("ManagementPage");
            }
            return View(_personInfoModel);
        }

        /// <summary>
        /// 修改個人資料(初始)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult EditPerson(int userId)
        {
            _faceService = new FaceDP(Server.MapPath("~"));
            DataTable dt = _faceService.GetGenderCode();            
            ViewBag.GenderCode = _webModel.MapCodeData(dt);

            DataTable personInfo = _faceService.GetPersonInfo(userId);

            var tempPerson = new PersonInfo
            {
                userId = Convert.ToInt64(personInfo.Rows[0]["userId"]),
                userName = personInfo.Rows[0]["userName"].ToString(),
                gender = personInfo.Rows[0]["gender"].ToString(),
                birthDate = Convert.ToDateTime(personInfo.Rows[0]["birthDate"].ToString()),
                tag = personInfo.Rows[0]["tag"].ToString()
            };

            return View(tempPerson);
        }

        /// <summary>
        /// 修改個人資料(編輯送出)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult EditPerson(PersonInfo personinfo)
        {
            _faceService = new FaceDP(Server.MapPath("~"));
            DataTable dt = _faceService.GetGenderCode();
            ViewBag.GenderCode = _webModel.MapCodeData(dt);
            _faceService.UpdatePersonInfo(personinfo);

            return View("ManagementPage");
        }

        /// <summary>
        /// 取得個人資料列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllPersonInfo()
        {
            _faceService = new FaceDP(Server.MapPath("~"));
            DataTable dt = _faceService.GetAllPersonInDB();

            string json = JsonConvert.SerializeObject(dt);

            return Content(json, "application/json");
        }

        /// <summary>
        /// 刪除個人資料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult DeletePerson(string userId)
        {
            try
            {
                _faceService = new FaceDP(Server.MapPath("~"));
                _faceService.DeletePerson(Convert.ToInt32(userId));
                return this.Json(new { success = true, message = string.Empty });
            }

            catch (Exception ex)
            {
                return this.Json(new { success = false, message = "刪除失敗! \n" +  ex.Message});
            }
        }

        /// <summary>
        /// 刪除臉部資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult DeleteFace(string id)
        {
            try
            {
                _faceService = new FaceDP(Server.MapPath("~"));
                _faceService.DeleteFaceById(id);
                return this.Json(new { success = true, message = string.Empty });
            }

            catch (Exception ex)
            {
                return this.Json(new { success = false, message = "刪除失敗! \n" + ex.Message });
            }
        }

        /// <summary>
        /// 訓練臉部資料
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult TrainFace()
        {
            try
            {
                _faceService = new FaceDP(Server.MapPath("~"));
                _faceService.TrainRecognizer();
                return this.Json(new { success = true, message = string.Empty });
            }

            catch (Exception ex)
            {
                return this.Json(new { success = false, message = "訓練失敗! \n" + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Submit(HttpPostedFileBase file)
        {
            _faceService = new FaceDP(Server.MapPath("~"));

            ViewBag.NullFile = false;
            ViewBag.WrongFileType = false;

            if (file == null)
            {
                ViewBag.NullFile = true;
                return View();
            }
            if (file.ContentType != "image/jpeg")
            {
                ViewBag.WrongFileType = true;
                return View();
            }

            //翻轉處理
            //Bitmap originImg = ConvertFileToImage(files).ToBitmap();
             var originImg = FileToImageRotate(file);


            //傳入影像做臉部偵測
            //Image<Bgr, byte> newImage = _faceService.FaceDetection(ConvertFileToImage(files));
            Bitmap bitmap = _faceService.FaceDetection(originImg).Resize(450,800,Emgu.CV.CvEnum.Inter.Linear).ToBitmap();
            Bitmap cropFace = _faceService.GetFaceImage().ToBitmap();

            //偵測完的人臉做辨識
            int userId = _faceService.RecognizeFace();
            DataTable user = _faceService.GetPersonInfo(userId);

            //轉換成bitmap格式，加上辨識後結果於左上角
            //Bitmap bitmap = newImage.ToBitmap();
            //Bitmap cropFace = _faceService.GetFaceImage().ToBitmap();

            //繪製辨識資訊
            //Graphics draw = Graphics.FromImage(bitmap);
            //draw.SmoothingMode = SmoothingMode.AntiAlias;
            //draw.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //draw.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //draw.FillRectangle(Brushes.Black, new Rectangle(0, 0, 150, 35));
            //draw.DrawString(userNameLabel, new Font("微軟正黑體", 18, FontStyle.Bold), Brushes.Red, new RectangleF(0, 0, 150, 35));
            //draw.Flush();

            //轉換影像成Stream
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            var faceStream = new MemoryStream();
            cropFace.Save(faceStream, ImageFormat.Jpeg);

            //to base64
            // Convert byte[] to Base64 String
            var imageBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);

            ViewBag.Image = base64String;

            imageBytes = faceStream.ToArray();
            base64String = Convert.ToBase64String(imageBytes);

            ViewBag.FaceImage = base64String;

            //info
            ViewBag.UserName = user.Rows[0]["userName"].ToString();
            ViewBag.UserGender = user.Rows[0]["gender"].ToString();
            ViewBag.UserBirthDate = Convert.ToDateTime(user.Rows[0]["birthDate"]).ToString("yyyy/MM/dd");
            ViewBag.UserTag = user.Rows[0]["tag"].ToString();
            DataTable dt = _faceService.GetGenderCode();
            ViewBag.GenderCode = _webModel.MapCodeData(dt);
            return View(); 
        }

        private string GetFileInfo(HttpPostedFileBase files)
        {
            return string.Format("{0} ({1} bytes)", Path.GetFileName(files.FileName), files.ContentLength);
        }

        /// <summary>
        /// 將檔案轉換成CV.Image類型
        /// </summary>
        /// <param name="file">來源檔案</param>
        /// <returns></returns>
        private Image<Bgr, byte> ConvertFileToImage(HttpPostedFileBase file)
        {
            // Read bytes from http input stream
            BinaryReader binReader = new BinaryReader(file.InputStream);
            byte[] binData = binReader.ReadBytes(file.ContentLength);

            Stream stream = new MemoryStream();
            stream.Write(binData, 0, binData.Length);
            var faceImage = new Image<Bgr, byte>(new Bitmap(stream));
            return faceImage;
        }

        /// <summary>
        /// 轉換成Image並旋轉
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private Image<Bgr, byte> FileToImageRotate(HttpPostedFileBase file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var theImage = Image.FromStream(file.InputStream);
                Bitmap bitmap = new Bitmap(theImage);
                MemoryStream stream = new MemoryStream();
                foreach (var item in theImage.PropertyItems)
                    bitmap.SetPropertyItem(item);

                var exif = new EXIFextractor(ref bitmap, "n");
                RotateFlipType flip = _webModel.GetOrientationToFlipType(exif["Orientation"] == null ? "0" : exif["Orientation"].ToString());

                if (flip != RotateFlipType.Rotate180FlipNone)
                {
                    foreach (var item in theImage.PropertyItems)
                        bitmap.SetPropertyItem(item);
                }

                bitmap.RotateFlip(flip);
                //先不做回存tag動作(有BUG)
                //exif.setTag(0x112, "1"); // Optional: reset orientation tag

                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Position = 0;
                byte[] image = new byte[stream.Length + 1];
                stream.Read(image, 0, image.Length);

                var faceImage = new Image<Bgr, byte>(new Bitmap(stream));
                return faceImage;
            }
        }
    }
}