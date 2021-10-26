using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Framework.IPC;

namespace UnicodeSrl.ScciCore.ViewController
{

    [Serializable]
    public class ViewControllerData : I_IpcData
    {

        public string Channel { get; set; }

        public EnumCommandListener CommandListener { get; set; }
        public EnumMaschere Maschera { get; set; }
        public Sessione Sessione { get; set; }
        public IViewController ViewController { get; set; }

    }


    [Serializable]
    public class ViewControllerBase : IViewController
    {

        public Maschera Maschera { get; set; }

        public void InitViewController(IViewController viewcontroller)
        {
            throw new NotImplementedException();
        }

        public void LoadViewController()
        {
            throw new NotImplementedException();
        }

        public IViewController SaveViewController()
        {
            throw new NotImplementedException();
        }

    }

    [Serializable]
    public class ViewControllerTopNonModale : ViewControllerBase
    {

        public ViewControllerTopNonModale()
        {

        }

        public Paziente Paziente { get; set; }
        public Episodio Episodio { get; set; }

    }

    [Serializable]
    public class ViewControllerBottomNonModale : ViewControllerBase
    {

        public ViewControllerBottomNonModale()
        {

        }

    }

    [Serializable]
    public class ViewControllerMultiTaskInfermieristico : ViewControllerBase
    {

        public ViewControllerMultiTaskInfermieristico()
        {
            this.DialogResultReturn = DialogResult.Cancel;
            this.CodTipoProtocollo = string.Empty;
        }

        public DialogResult DialogResultReturn { get; set; }
        public string CodTipoProtocollo { get; set; }
        public Paziente Paziente { get; set; }
        public Episodio Episodio { get; set; }
        public Cartella Cartella { get; set; }
        public Trasferimento Trasferimento { get; set; }
        public MovTaskInfermieristico MovTaskInfermieristico { get; set; }

    }

    [Serializable]
    public class ViewControllerDiarioClinico : ViewControllerBase
    {

        public ViewControllerDiarioClinico()
        {
            this.DialogResultReturn = DialogResult.Cancel;
        }

        public DialogResult DialogResultReturn { get; set; }
        public Paziente Paziente { get; set; }
        public Episodio Episodio { get; set; }
        public Cartella Cartella { get; set; }
        public Trasferimento Trasferimento { get; set; }
        public MovDiarioClinico MovDiarioClinico { get; set; }

    }

    [Serializable]
    public class ViewControllerScheda : ViewControllerBase
    {

        public ViewControllerScheda()
        {
            this.DialogResultReturn = DialogResult.Cancel;
        }

        public DialogResult DialogResultReturn { get; set; }
        public Paziente Paziente { get; set; }
        public Episodio Episodio { get; set; }
        public Cartella Cartella { get; set; }
        public Trasferimento Trasferimento { get; set; }
        public MovScheda MovScheda { get; set; }

    }

}
