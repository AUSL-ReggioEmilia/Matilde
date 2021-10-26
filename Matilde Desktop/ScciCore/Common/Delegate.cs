using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciCore
{

    public delegate void ChangeEventHandler(object sender, EventArgs e);

    public delegate void ImmagineTopClickHandler(object sender, ImmagineTopClickEventArgs e);

    public delegate void PulsanteBottomClickHandler(object sender, PulsanteBottomClickEventArgs e);
    public delegate void ImmagineBottomClickHandler(object sender, ImmagineBottomClickEventArgs e);

    public delegate void SegnalibriClickHandler(object sender, SegnalibriClickEventArgs e);

    public delegate void CartelleInVisioneClickHandler(object sender, CartelleInVisioneClickEventArgs e);
    public delegate void PazientiInVisioneClickHandler(object sender, PazientiInVisioneClickEventArgs e);

    public delegate void PazientiSeguitiClickHandler(object sender, PazientiSeguitiClickEventArgs e);

    public class ImmagineTopClickEventArgs : EventArgs
    {

        EnumImmagineTop _tipo;

        public ImmagineTopClickEventArgs(EnumImmagineTop tipo)
        {
            _tipo = tipo;
        }

        public EnumImmagineTop Tipo
        {
            get
            {
                return _tipo;
            }
            set
            {
                _tipo = value;
            }
        }

    }

    public class PulsanteBottomClickEventArgs : EventArgs
    {

        private EnumPulsanteBottom _tipo;

        public PulsanteBottomClickEventArgs(EnumPulsanteBottom tipo)
        {
            _tipo = tipo;
        }

        public EnumPulsanteBottom tipo
        {
            get
            {
                return _tipo;
            }
            set
            {
                _tipo = value;
            }
        }

    }

    public class ImmagineBottomClickEventArgs : EventArgs
    {

        private EnumImmagineBottom _tipo;

        public ImmagineBottomClickEventArgs(EnumImmagineBottom tipo)
        {
            _tipo = tipo;
        }

        public EnumImmagineBottom Tipo
        {
            get
            {
                return _tipo;
            }
            set
            {
                _tipo = value;
            }
        }

    }

    public class CalendarioEventArgs : EventArgs
    {

        private Infragistics.Win.UltraWinSchedule.Owner _Owner = null;

        public CalendarioEventArgs(Infragistics.Win.UltraWinSchedule.Owner Owner)
        {
            this._Owner = Owner;
        }

        public Infragistics.Win.UltraWinSchedule.Owner Owner
        {
            get { return _Owner; }
        }

    }

    public class ScrollbarEventArgs : EventArgs
    {

        #region Declare

        private EnumTypeButton _TypeButton = EnumTypeButton.Su;

        public enum EnumTypeButton
        {
            Su = 0,
            Giu = 1,
        }

        #endregion

        public ScrollbarEventArgs(EnumTypeButton typebutton)
        {
            _TypeButton = typebutton;
        }

        public EnumTypeButton TypeButton
        {
            get { return _TypeButton; }
        }

    }

    public class SegnalibriClickEventArgs : EventArgs
    {

        EnumPulsanteSegnalibri _pulsante;
        string _id;

        public SegnalibriClickEventArgs(EnumPulsanteSegnalibri pulsante, string id)
        {
            _pulsante = pulsante;
            _id = id;
        }

        public EnumPulsanteSegnalibri Pulsante
        {
            get
            {
                return _pulsante;
            }
            set
            {
                _pulsante = value;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

    }

    public class CartelleInVisioneClickEventArgs : EventArgs
    {

        EnumPulsanteCartelleInVisione _pulsante;
        string _id;

        public CartelleInVisioneClickEventArgs(EnumPulsanteCartelleInVisione pulsante, string id)
        {
            _pulsante = pulsante;
            _id = id;
        }

        public EnumPulsanteCartelleInVisione Pulsante
        {
            get
            {
                return _pulsante;
            }
            set
            {
                _pulsante = value;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

    }

    public class PazientiInVisioneClickEventArgs : EventArgs
    {

        EnumPulsanteCartelleInVisione _pulsante;
        string _id;

        public PazientiInVisioneClickEventArgs(EnumPulsanteCartelleInVisione pulsante, string id)
        {
            _pulsante = pulsante;
            _id = id;
        }

        public EnumPulsanteCartelleInVisione Pulsante
        {
            get
            {
                return _pulsante;
            }
            set
            {
                _pulsante = value;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

    }

    public class PazientiSeguitiClickEventArgs : EventArgs
    {

        EnumPulsantePazientiSeguiti _pulsante;
        string _id;

        public PazientiSeguitiClickEventArgs(EnumPulsantePazientiSeguiti pulsante, string id)
        {
            _pulsante = pulsante;
            _id = id;
        }

        public EnumPulsantePazientiSeguiti Pulsante
        {
            get
            {
                return _pulsante;
            }
            set
            {
                _pulsante = value;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

    }

    #region Infragistics

    public class DayNumberClickEventArgs : EventArgs
    {

        private DateTime _day;
        private Infragistics.Win.UltraWinSchedule.Owner _owner;

        public DayNumberClickEventArgs(DateTime day, Infragistics.Win.UltraWinSchedule.Owner owner)
        {
            _day = day;
            _owner = owner;
        }

        public DateTime Day
        {
            get { return _day; }
        }

        public Infragistics.Win.UltraWinSchedule.Owner Owner
        {
            get { return _owner; }
        }

    }

    #endregion

}

