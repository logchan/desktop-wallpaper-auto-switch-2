using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWAS2.Components
{
    public class PicChangedEventArgs : EventArgs
    {
        public PicChangedEventArgs(string picPath)
        {
            newPicPath = picPath;
        }

        private string newPicPath;
        public string NewPicPath { get { return newPicPath; } }
    }

    public class PicPosChangedEventArgs : EventArgs
    {
        public PicPosChangedEventArgs(PicPos picPos)
        {
            newPicPos = picPos;
        }

        private PicPos newPicPos;
        public PicPos NewPicPos { get { return newPicPos; } }
    }
}
