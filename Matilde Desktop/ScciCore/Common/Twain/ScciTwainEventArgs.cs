using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore
{

    public class ImageCapturedEventArgs : EventArgs
    {
        public ImageCapturedEventArgs(Image img)
        {
            this.Image = img;
        }

        public Image Image { get; private set; }
    }


    public class TwainStateChangedEventArgs : EventArgs
    {
        public TwainStateChangedEventArgs(int state)
        {
            this.State = state;
        }

        public int State { get; private set; }

        public en_Twain_State TwainState
        {
            get
            {
                return (en_Twain_State)this.State;
            }
        }


    }



}
