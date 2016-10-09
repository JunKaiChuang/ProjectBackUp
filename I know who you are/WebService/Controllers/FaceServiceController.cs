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

            return View(_faceService.GetPersonInfo(userId));
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
        public ActionResult Submit(HttpPostedFileBase files)
        {
            _faceService = new FaceDP(Server.MapPath("~"));

            ViewBag.NullFile = false;
            ViewBag.WrongFileType = false;

            if (files == null)
            {
                ViewBag.NullFile = true;
                return View();
            }
            if (files.ContentType != "image/jpeg")
            {
                ViewBag.WrongFileType = true;
                return View();
            }

            //傳入影像做臉部偵測
            Image<Bgr, byte> newImage = _faceService.FaceDetection(ConvertFileToImage(files));

            //偵測完的人臉做辨識
            int userId = _faceService.RecognizeFace();
            var userNameLabel = _faceService.GetUserNameByUserId(userId);

            //轉換成bitmap格式，加上辨識後結果於左上角
            Bitmap bitmap = newImage.ToBitmap();
            Graphics draw = Graphics.FromImage(bitmap);
            draw.SmoothingMode = SmoothingMode.AntiAlias;
            draw.InterpolationMode = InterpolationMode.HighQualityBicubic;
            draw.PixelOffsetMode = PixelOffsetMode.HighQuality;
            draw.FillRectangle(Brushes.Black, new Rectangle(0, 0, 150, 35));
            draw.DrawString(userNameLabel, new Font("微軟正黑體", 18, FontStyle.Bold), Brushes.Red, new RectangleF(0, 0, 150, 35));
            draw.Flush();

            //轉換影像成Stream
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);

            //to base64
            // Convert byte[] to Base64 String
            var imageBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);

            ViewBag.Image = base64String;
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
    }
}