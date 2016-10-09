namespace ManageForm
{
    partial class ManageForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._trainButton = new System.Windows.Forms.Button();
            this._userNameListBox = new System.Windows.Forms.ListBox();
            this._usernameTextBox = new System.Windows.Forms.TextBox();
            this._saveFaceButton = new System.Windows.Forms.Button();
            this._cropFaceImageBox = new Emgu.CV.UI.ImageBox();
            this._byCam = new System.Windows.Forms.Button();
            this._byPhoto = new System.Windows.Forms.Button();
            this._mainImageBox = new Emgu.CV.UI.ImageBox();
            this._Borwse = new System.Windows.Forms.Button();
            this._continuedOrCapture = new System.Windows.Forms.Button();
            this._checkIsDetectedTimer = new System.Windows.Forms.Timer(this.components);
            this._recognizeButton = new System.Windows.Forms.Button();
            this._userNameLabel = new System.Windows.Forms.Label();
            this._deleteUserButton = new System.Windows.Forms.Button();
            this.facesGroupBox = new System.Windows.Forms.GroupBox();
            this._deleteFaceButton = new System.Windows.Forms.Button();
            this._userFaceImageBox = new Emgu.CV.UI.ImageBox();
            this._facesListBox = new System.Windows.Forms.ListBox();
            this._ContinueStateLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._cropFaceImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._mainImageBox)).BeginInit();
            this.facesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._userFaceImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _trainButton
            // 
            this._trainButton.Location = new System.Drawing.Point(1109, 190);
            this._trainButton.Margin = new System.Windows.Forms.Padding(4);
            this._trainButton.Name = "_trainButton";
            this._trainButton.Size = new System.Drawing.Size(100, 29);
            this._trainButton.TabIndex = 17;
            this._trainButton.Text = "Train";
            this._trainButton.UseVisualStyleBackColor = true;
            this._trainButton.Click += new System.EventHandler(this._trainButton_Click);
            // 
            // _userNameListBox
            // 
            this._userNameListBox.FormattingEnabled = true;
            this._userNameListBox.ItemHeight = 15;
            this._userNameListBox.Location = new System.Drawing.Point(1109, 13);
            this._userNameListBox.Margin = new System.Windows.Forms.Padding(4);
            this._userNameListBox.Name = "_userNameListBox";
            this._userNameListBox.Size = new System.Drawing.Size(159, 169);
            this._userNameListBox.TabIndex = 16;
            this._userNameListBox.SelectedIndexChanged += new System.EventHandler(this._userNameListBox_SelectedIndexChanged);
            // 
            // _usernameTextBox
            // 
            this._usernameTextBox.Location = new System.Drawing.Point(950, 49);
            this._usernameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._usernameTextBox.Name = "_usernameTextBox";
            this._usernameTextBox.Size = new System.Drawing.Size(132, 25);
            this._usernameTextBox.TabIndex = 15;
            this._usernameTextBox.Text = "請輸入姓名";
            this._usernameTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this._usernameTextBox_MouseClick);
            // 
            // _saveFaceButton
            // 
            this._saveFaceButton.Location = new System.Drawing.Point(950, 13);
            this._saveFaceButton.Margin = new System.Windows.Forms.Padding(4);
            this._saveFaceButton.Name = "_saveFaceButton";
            this._saveFaceButton.Size = new System.Drawing.Size(100, 29);
            this._saveFaceButton.TabIndex = 14;
            this._saveFaceButton.Text = "Save";
            this._saveFaceButton.UseVisualStyleBackColor = true;
            this._saveFaceButton.Click += new System.EventHandler(this._saveFaceButton_Click);
            // 
            // _cropFaceImageBox
            // 
            this._cropFaceImageBox.Location = new System.Drawing.Point(782, 13);
            this._cropFaceImageBox.Margin = new System.Windows.Forms.Padding(4);
            this._cropFaceImageBox.Name = "_cropFaceImageBox";
            this._cropFaceImageBox.Size = new System.Drawing.Size(160, 145);
            this._cropFaceImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._cropFaceImageBox.TabIndex = 10;
            this._cropFaceImageBox.TabStop = false;
            // 
            // _byCam
            // 
            this._byCam.Location = new System.Drawing.Point(13, 524);
            this._byCam.Margin = new System.Windows.Forms.Padding(4);
            this._byCam.Name = "_byCam";
            this._byCam.Size = new System.Drawing.Size(100, 29);
            this._byCam.TabIndex = 13;
            this._byCam.Text = "Camera";
            this._byCam.UseVisualStyleBackColor = true;
            this._byCam.Click += new System.EventHandler(this._byCam_Click);
            // 
            // _byPhoto
            // 
            this._byPhoto.Location = new System.Drawing.Point(13, 488);
            this._byPhoto.Margin = new System.Windows.Forms.Padding(4);
            this._byPhoto.Name = "_byPhoto";
            this._byPhoto.Size = new System.Drawing.Size(100, 29);
            this._byPhoto.TabIndex = 12;
            this._byPhoto.Text = "Photo";
            this._byPhoto.UseVisualStyleBackColor = true;
            this._byPhoto.Click += new System.EventHandler(this._byPhoto_Click);
            // 
            // _mainImageBox
            // 
            this._mainImageBox.Location = new System.Drawing.Point(13, 13);
            this._mainImageBox.Margin = new System.Windows.Forms.Padding(4);
            this._mainImageBox.Name = "_mainImageBox";
            this._mainImageBox.Size = new System.Drawing.Size(761, 456);
            this._mainImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._mainImageBox.TabIndex = 11;
            this._mainImageBox.TabStop = false;
            // 
            // _Borwse
            // 
            this._Borwse.Location = new System.Drawing.Point(121, 488);
            this._Borwse.Margin = new System.Windows.Forms.Padding(4);
            this._Borwse.Name = "_Borwse";
            this._Borwse.Size = new System.Drawing.Size(97, 29);
            this._Borwse.TabIndex = 9;
            this._Borwse.Text = "Browse";
            this._Borwse.UseVisualStyleBackColor = true;
            this._Borwse.Click += new System.EventHandler(this._Borwse_Click);
            // 
            // _continuedOrCapture
            // 
            this._continuedOrCapture.Location = new System.Drawing.Point(121, 524);
            this._continuedOrCapture.Name = "_continuedOrCapture";
            this._continuedOrCapture.Size = new System.Drawing.Size(146, 29);
            this._continuedOrCapture.TabIndex = 18;
            this._continuedOrCapture.Text = "Continued/Capture";
            this._continuedOrCapture.UseVisualStyleBackColor = true;
            this._continuedOrCapture.Click += new System.EventHandler(this._continuedOrCapture_Click);
            // 
            // _checkIsDetectedTimer
            // 
            this._checkIsDetectedTimer.Interval = 500;
            this._checkIsDetectedTimer.Tick += new System.EventHandler(this._checkIsDetectedTimer_Tick);
            // 
            // _recognizeButton
            // 
            this._recognizeButton.Location = new System.Drawing.Point(782, 165);
            this._recognizeButton.Name = "_recognizeButton";
            this._recognizeButton.Size = new System.Drawing.Size(103, 36);
            this._recognizeButton.TabIndex = 19;
            this._recognizeButton.Text = "Recognize";
            this._recognizeButton.UseVisualStyleBackColor = true;
            this._recognizeButton.Click += new System.EventHandler(this._recognizeButton_Click);
            // 
            // _userNameLabel
            // 
            this._userNameLabel.AutoSize = true;
            this._userNameLabel.Location = new System.Drawing.Point(781, 204);
            this._userNameLabel.Name = "_userNameLabel";
            this._userNameLabel.Size = new System.Drawing.Size(40, 15);
            this._userNameLabel.TabIndex = 20;
            this._userNameLabel.Text = "Name";
            // 
            // _deleteUserButton
            // 
            this._deleteUserButton.Location = new System.Drawing.Point(1109, 226);
            this._deleteUserButton.Name = "_deleteUserButton";
            this._deleteUserButton.Size = new System.Drawing.Size(100, 29);
            this._deleteUserButton.TabIndex = 21;
            this._deleteUserButton.Text = "Delete";
            this._deleteUserButton.UseVisualStyleBackColor = true;
            this._deleteUserButton.Click += new System.EventHandler(this._deleteUserButton_Click);
            // 
            // facesGroupBox
            // 
            this.facesGroupBox.Controls.Add(this._deleteFaceButton);
            this.facesGroupBox.Controls.Add(this._userFaceImageBox);
            this.facesGroupBox.Controls.Add(this._facesListBox);
            this.facesGroupBox.Location = new System.Drawing.Point(784, 261);
            this.facesGroupBox.Name = "facesGroupBox";
            this.facesGroupBox.Size = new System.Drawing.Size(484, 292);
            this.facesGroupBox.TabIndex = 22;
            this.facesGroupBox.TabStop = false;
            this.facesGroupBox.Text = "groupBox1";
            // 
            // _deleteFaceButton
            // 
            this._deleteFaceButton.Location = new System.Drawing.Point(325, 184);
            this._deleteFaceButton.Name = "_deleteFaceButton";
            this._deleteFaceButton.Size = new System.Drawing.Size(75, 23);
            this._deleteFaceButton.TabIndex = 12;
            this._deleteFaceButton.Text = "Delete";
            this._deleteFaceButton.UseVisualStyleBackColor = true;
            this._deleteFaceButton.Click += new System.EventHandler(this._deleteFaceButton_Click);
            // 
            // _userFaceImageBox
            // 
            this._userFaceImageBox.Location = new System.Drawing.Point(7, 25);
            this._userFaceImageBox.Margin = new System.Windows.Forms.Padding(4);
            this._userFaceImageBox.Name = "_userFaceImageBox";
            this._userFaceImageBox.Size = new System.Drawing.Size(200, 200);
            this._userFaceImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._userFaceImageBox.TabIndex = 11;
            this._userFaceImageBox.TabStop = false;
            // 
            // _facesListBox
            // 
            this._facesListBox.FormattingEnabled = true;
            this._facesListBox.ItemHeight = 15;
            this._facesListBox.Location = new System.Drawing.Point(325, 24);
            this._facesListBox.Name = "_facesListBox";
            this._facesListBox.Size = new System.Drawing.Size(153, 154);
            this._facesListBox.TabIndex = 0;
            this._facesListBox.SelectedIndexChanged += new System.EventHandler(this._facesListBox_SelectedIndexChanged);
            // 
            // _ContinueStateLabel
            // 
            this._ContinueStateLabel.AutoSize = true;
            this._ContinueStateLabel.Location = new System.Drawing.Point(273, 531);
            this._ContinueStateLabel.Name = "_ContinueStateLabel";
            this._ContinueStateLabel.Size = new System.Drawing.Size(41, 15);
            this._ContinueStateLabel.TabIndex = 23;
            this._ContinueStateLabel.Text = "label1";
            // 
            // ManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1354, 567);
            this.Controls.Add(this._ContinueStateLabel);
            this.Controls.Add(this.facesGroupBox);
            this.Controls.Add(this._deleteUserButton);
            this.Controls.Add(this._userNameLabel);
            this.Controls.Add(this._recognizeButton);
            this.Controls.Add(this._continuedOrCapture);
            this.Controls.Add(this._trainButton);
            this.Controls.Add(this._userNameListBox);
            this.Controls.Add(this._usernameTextBox);
            this.Controls.Add(this._saveFaceButton);
            this.Controls.Add(this._cropFaceImageBox);
            this.Controls.Add(this._byCam);
            this.Controls.Add(this._byPhoto);
            this.Controls.Add(this._mainImageBox);
            this.Controls.Add(this._Borwse);
            this.Name = "ManageForm";
            this.Text = "I know who you are Manage Form";
            ((System.ComponentModel.ISupportInitialize)(this._cropFaceImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._mainImageBox)).EndInit();
            this.facesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._userFaceImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _trainButton;
        private System.Windows.Forms.ListBox _userNameListBox;
        private System.Windows.Forms.TextBox _usernameTextBox;
        private System.Windows.Forms.Button _saveFaceButton;
        private Emgu.CV.UI.ImageBox _cropFaceImageBox;
        private System.Windows.Forms.Button _byCam;
        private System.Windows.Forms.Button _byPhoto;
        private Emgu.CV.UI.ImageBox _mainImageBox;
        private System.Windows.Forms.Button _Borwse;
        private System.Windows.Forms.Button _continuedOrCapture;
        private System.Windows.Forms.Timer _checkIsDetectedTimer;
        private System.Windows.Forms.Button _recognizeButton;
        private System.Windows.Forms.Label _userNameLabel;
        private System.Windows.Forms.Button _deleteUserButton;
        private System.Windows.Forms.GroupBox facesGroupBox;
        private Emgu.CV.UI.ImageBox _userFaceImageBox;
        private System.Windows.Forms.ListBox _facesListBox;
        private System.Windows.Forms.Label _ContinueStateLabel;
        private System.Windows.Forms.Button _deleteFaceButton;
    }
}

