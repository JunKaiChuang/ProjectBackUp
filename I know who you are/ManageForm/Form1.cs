using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FaceService;

namespace ManageForm
{
    public partial class ManageForm : Form
    {
        private FaceDP _faceService;
        private view _view;
        private Capture _capture;
        private Thread _threadVideo;
        private bool _isTraining;
        private BackgroundWorker _worker;

        public ManageForm()
        {
            InitializeComponent();
            _faceService = new FaceDP();
            _view = new view();
            _capture = new Capture();
            _capture.Stop();
            
            RefreshControl();
            _cropFaceImageBox.Image = _faceService.GetFaceImage();
            _isTraining = false;

            //video capture thread
            //StartCaptureThread();

            //backgroundWorker
            _worker = new BackgroundWorker();
            _worker.DoWork += Training;
            _worker.RunWorkerCompleted += TrainComplete;
        }

        /// <summary>
        /// 開始影像人臉偵測&截取
        /// </summary>
        void StartCaptureThread()
        {            
            _threadVideo = new Thread(ThreadFunc);
            _threadVideo.IsBackground = true;
            _threadVideo.Start();
        }

        /// <summary>
        /// 控制項狀態更新
        /// </summary>
        private void RefreshControl()
        {
            _byPhoto.Enabled = _view.EnableByPhoto();
            _byCam.Enabled = _view.EnableByCam();
            _Borwse.Enabled = _view.EnableBrowse();
            _continuedOrCapture.Enabled = _view.StopCapture();
            _trainButton.Enabled = !_isTraining;
            _userNameLabel.Text = "";

            //Save Face
            _saveFaceButton.Enabled = _faceService.IsFaceDetected();
            _usernameTextBox.Enabled = _faceService.IsFaceDetected();
            //username list
            _userNameListBox.DataSource = _faceService.GetAllUsernames();
            //user faces group
            try
            {
                _facesListBox.DataSource = _faceService.GetFacesByUserName(_userNameListBox.SelectedItem.ToString());
                facesGroupBox.Enabled = true;
            }
            catch (Exception)
            {
                _facesListBox.DataSource = null;
                facesGroupBox.Enabled = false;
                //MessageBox.Show(e.Message);
            }
            
            //Capture State
            refreshCaptureState();
        }

        /// <summary>
        /// 抓取影像Thread
        /// </summary>
        private void ThreadFunc()
        {
            while (!_faceService.IsFaceDetected() || _view.IsContinued())
            {
                if (!_view.EnableByCam())
                {
                    using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
                    {
                        if (imageFrame != null)
                        {
                            try
                            {
                                imageFrame.Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);//縮小圖像來源大小
                                _mainImageBox.Image = _faceService.FaceDetection(imageFrame);
                                _cropFaceImageBox.Image = _faceService.GetFaceImage();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "");
                                throw;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Form關閉前執行動作
        /// </summary>
        void whenClosing()
        {
            _capture.Stop();
            if (_threadVideo != null && _threadVideo.IsAlive) _threadVideo.Abort();
        }

        /// <summary>
        /// 進行臉部資料訓練
        /// </summary>
        private void Training(object sender, DoWorkEventArgs e)
        {
            _faceService.TrainRecognizer();
            _isTraining = false;
        }

        /// <summary>
        /// 訓練臉部資料完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("訓練完成", "");
            RefreshControl();
        }

        private void _byPhoto_Click(object sender, EventArgs e)
        {
            _view.ClickByPhoto();
            RefreshControl();
            _mainImageBox.Image = null;
            _capture.Stop();
        }

        private void _Borwse_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                Image<Bgr, byte> My_Image = new Image<Bgr, byte>(Openfile.FileName);
                My_Image = _faceService.FaceDetection(My_Image);
                _mainImageBox.Image = My_Image;
                //My_Image.Dispose();
                _cropFaceImageBox.Image = _faceService.GetFaceImage();
                RefreshControl();
            }
        }

        private void _byCam_Click(object sender, EventArgs e)
        {
            _view.ClickByCam();
            _capture.Start();
            StartCaptureThread();
            _faceService.InitCropFace();
            RefreshControl();
        }

        private void _saveFaceButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_faceService.SaveCropFace(_usernameTextBox.Text));
            RefreshControl();
        }

        private void _trainButton_Click(object sender, EventArgs e)
        {
            _isTraining = true;
            RefreshControl();
            _worker.RunWorkerAsync();
        }

        private void _userNameListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _usernameTextBox.Text = _userNameListBox.SelectedItem.ToString();
            facesGroupBox.Text = _userNameListBox.SelectedItem.ToString() + "'s faces";
            //user faces group
            _facesListBox.DataSource = _faceService.GetFacesByUserName(_userNameListBox.SelectedItem.ToString());
        }
        
        private void _usernameTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            _usernameTextBox.Text = "";
        }

        private void _checkIsDetectedTimer_Tick(object sender, EventArgs e)
        {
            if(_faceService.IsFaceDetected() && !_view.EnableByCam() && !_view.IsContinued())
            {
                _view.SetContinued();
                stopCapture();
            }
        }

        /// <summary>
        /// 停止截取畫面
        /// </summary>
        void stopCapture()
        {
            _view.ClickStop();
            RefreshControl();
            //_mainImageBox.Image = null;
            _capture.Stop();
        }

        private void _recognizeButton_Click(object sender, EventArgs e)
        {
            int userId = _faceService.RecognizeFace();
            _userNameLabel.Text = _faceService.GetUserNameByUserId(userId);
        }

        private void _deleteUserButton_Click(object sender, EventArgs e)
        {
            var userName = _userNameListBox.SelectedItem.ToString();
            try
            {
                _faceService.DeleteUserByName(userName);
                MessageBox.Show(userName + ",刪除成功", "");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "");
                throw;
            }
            RefreshControl();
        }

        private void _facesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = _facesListBox.SelectedItem.ToString();
            _userFaceImageBox.Image = _faceService.GetFaceById(id);
        }

        private void _continuedOrCapture_Click(object sender, EventArgs e)
        {
            _view.ClickContinuedOrCapture();
            refreshCaptureState();
        }

        void refreshCaptureState()
        {
            //Check face is detected timer
            _checkIsDetectedTimer.Enabled = !_view.EnableByCam() && !_view.IsContinued();
            //State label text
            _ContinueStateLabel.Text = _view.GetContinuedOrCaptureState();
        }

        private void _deleteFaceButton_Click(object sender, EventArgs e)
        {
            var id = _facesListBox.SelectedItem.ToString();
            if(_faceService.DeleteFaceById(id))
            {
                MessageBox.Show("人臉資料刪除成功", "");
            }
            else
            {
                MessageBox.Show("Oops,刪除出錯", "");
            }
            RefreshControl();
        }
    }
}
