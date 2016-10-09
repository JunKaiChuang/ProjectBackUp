using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageForm
{
    class view
    {
        private bool browseButton;
        private bool byPhoto;
        private bool byCam;
        private bool stopCapture;
        private bool isContinued;

        /// <summary>
        /// Form View initialize
        /// </summary>
        public view()
        {
            byCam = true;
            byPhoto = true;
            browseButton = false;
            stopCapture = false;
            isContinued = true;
        }

        public void ClickByPhoto()
        {
            byPhoto = false;
            byCam = true;
            browseButton = true;
            stopCapture = false;
        }

        public void ClickStop()
        {
            stopCapture = false;
            byPhoto = true;
            byCam = true;
            browseButton = false;
        }

        public void ClickByCam()
        {
            byPhoto = true;
            byCam = false;
            browseButton = false;
            stopCapture = true;
        }

        public void ClickContinuedOrCapture()
        {
            isContinued = !isContinued;
        }

        public string GetContinuedOrCaptureState()
        {
            if(isContinued)
            {
                return "Continued";
            }
            return "Capture";
        }

        public void SetContinued()
        {
            isContinued = true;
        }

        public bool EnableByPhoto()
        {
            return byPhoto;
        }

        public bool EnableByCam()
        {
            return byCam;
        }

        public bool EnableBrowse()
        {
            return browseButton;
        }

        public bool StopCapture()
        {
            return stopCapture;
        }

        public bool IsContinued()
        {
            return isContinued;
        }
    }
}
