using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciManagement
{
    public partial class frmManutenzioneDati : Form, Interfacce.IViewFormBase
    {

        private const string C_DUMMY = @"DUMMY";

        private class ChildForm
        {
            internal string id { get; set; }
            internal string description { get; set; }
            internal bool hasChild { get; set; }
        }

        public frmManutenzioneDati()
        {
            InitializeComponent();
        }

        #region Interface

        public Icon ViewIcon
        {
            get
            {
                return this.Icon;
            }
            set
            {
                this.Icon = value;
            }
        }

        public Image ViewImage
        {
            get
            {
                return this.PicImage.Image;
            }
            set
            {
                this.PicImage.Image = value;
            }
        }

        public string ViewText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public void ViewInit()
        {

            this.SuspendLayout();

            MyStatics.SetUltraToolbarsManager(this.UltraToolbarsManager);
            this.InitializeTreeView();
            this.LoadTreeView();
            this.InitializeUltraTab();
            this.InitializeUltraGrid();

            this.ResumeLayout();

        }

        #endregion

        #region TreeView

        private void InitializeTreeView()
        {
            MyStatics.SetUltraTree(this.UltraTreeMenu, new Size(16, 16), true);
            MyStatics.SetUltraTree(this.UltraTreeSchedeFiglie13, new Size(16, 16), true);
            this.UltraTreeSchedeFiglie13.Override.NodeStyle = NodeStyle.CheckBox;
        }

        private void LoadTreeView()
        {

            UltraTreeNode oNode = null;
            UltraTreeNode oNodeParent = null;

            try
            {

                this.UltraTreeMenu.Nodes.Clear();

                oNode = new UltraTreeNode(MyStatics.TV_ROOT, MyStatics.TV_ROOT);
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_AMMINISTRAZIONE, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_ROOT;
                oNode.Expanded = true;
                this.UltraTreeMenu.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_ROOT);
                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOMENU, @"Spostamento Singolo");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOMENU, Enums.EnumImageSize.isz16)));
                oNode.Expanded = true;
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOMENU;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_SPOSTASINGOLOMENU);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOAPPPAZAMB, @"Appuntamento Paziente Ambulatoriale");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOAPPPAZAMB, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOAPPPAZAMB;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOAPPPAZRO, @"Appuntamento Paziente Ricoverato");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOAPPPAZRO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOAPPPAZRO;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLODIARIOCLINICO, @"Diario Clinico");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLODIARIOCLINICO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLODIARIOCLINICO;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA, @"Evidenza Clinica");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE, @"Prescrizione");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI, @"Parametro Vitale");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ, @"Scheda Paziente");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOSCHEDAEPI, @"Scheda Episodio");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOSCHEDAEPI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOSCHEDAEPI;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOTASKINF, @"Task Infermieristico");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOTASKINF, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOTASKINF;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOALERTGENERICO, @"Alert Generico");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOALERTGENERICO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOALERTGENERICO;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOAPPAMBTOEPI, @"Appuntamento da Ambulatoriale a Ricovero");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOAPPAMBTOEPI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOAPPAMBTOEPI;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTASINGOLOAPPEPITOAMB, @"Appuntamento da Ricovero a Ambulatoriale");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOAPPEPITOAMB, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTASINGOLOAPPEPITOAMB;
                oNodeParent.Nodes.Add(oNode);


                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_ROOT);
                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOMENU, @"Spostamento di Gruppo");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOMENU, Enums.EnumImageSize.isz16)));
                oNode.Expanded = true;
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOMENU;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_SPOSTAGRUPPOMENU);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB, @"Appuntamento Paziente Ambulatoriale");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOAPPPAZRO, @"Appuntamento Paziente Ricoverato");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOAPPPAZRO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOAPPPAZRO;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO, @"Diario Clinico");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOEVIDENZACLINICA, @"Evidenza Clinica");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOEVIDENZACLINICA, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOEVIDENZACLINICA;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE, @"Prescrizione");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI, @"Parametro Vitale");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ, @"Scheda Paziente");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI, @"Scheda Episodio");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOTASKINF, @"Task Infermieristico");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOTASKINF, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOTASKINF;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SPOSTAGRUPPOALERTGENRICO, @"Alert Generico");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTAGRUPPOALERTGENRICO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SPOSTAGRUPPOALERTGENRICO;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_ROOT);
                oNode = new UltraTreeNode(MyStatics.TV_SCHEDAGENERARTFGRUPPO, @"Schede");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SPOSTASINGOLOMENU, Enums.EnumImageSize.isz16)));
                oNode.Expanded = true;
                oNode.Tag = MyStatics.TV_SCHEDAGENERARTFGRUPPO;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_SCHEDAGENERARTFGRUPPO);

                oNode = new UltraTreeNode(MyStatics.TV_SCHEDAGENERARTFSINGOLO, @"Scheda Genera RTF Singola");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SCHEDAGENERARTFSINGOLO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SCHEDAGENERARTFSINGOLO;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SCHEDAGENERARTFMULTIPLO, @"Scheda Genera RTF Multiplo");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SCHEDAGENERARTFMULTIPLO, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SCHEDAGENERARTFMULTIPLO;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_SBLOCCOSCHEDA, @"Sblocco Schede");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_SBLOCCOSCHEDA, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_SBLOCCOSCHEDA;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_ANNULLACANCSCHEDA, @"Annulla Cancellazione Scheda");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_ANNULLACANCSCHEDA, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_ANNULLACANCSCHEDA;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_RIPRISTINOREVISIONESCHEDA, @"Ripristino Revisione Scheda");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_RIPRISTINOREVISIONESCHEDA, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_RIPRISTINOREVISIONESCHEDA;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_ROOT);
                oNode = new UltraTreeNode(MyStatics.TV_CARTELLAGRUPPO, @"Cartella");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CARTELLAMENU, Enums.EnumImageSize.isz16)));
                oNode.Expanded = true;
                oNode.Tag = MyStatics.TV_CARTELLAGRUPPO;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_CARTELLAGRUPPO);

                oNode = new UltraTreeNode(MyStatics.TV_MODIFICANUMEROCARTELLA, @"Modifica Numero Cartella");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_CARTELLARIAPRI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_MODIFICANUMEROCARTELLA;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_CARTELLARIAPRI, @"Riapertura Cartella");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_CARTELLARIAPRI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_CARTELLARIAPRI;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_CARTELLACANCELLA, @"Cancellazione Cartella");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_CARTELLACANCELLA, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_CARTELLACANCELLA;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_CARTELLASCOLLEGA, @"Scollega Cartella");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_CARTELLARIAPRI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_CARTELLASCOLLEGA;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_ROOT);
                oNode = new UltraTreeNode(MyStatics.TV_CARTELLAAMBULATORIALEGRUPPO, @"Cartella Ambulatoriale");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_CARTELLAMENU, Enums.EnumImageSize.isz16)));
                oNode.Expanded = true;
                oNode.Tag = MyStatics.TV_CARTELLAAMBULATORIALEGRUPPO;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_CARTELLAAMBULATORIALEGRUPPO);

                oNode = new UltraTreeNode(MyStatics.TV_MODIFICANUMEROCARTELLAAMBULATORIALE, @"Modifica Numero Cartella");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_CARTELLARIAPRI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_MODIFICANUMEROCARTELLAAMBULATORIALE;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_CARTELLAAMBULATORIALERIAPRI, @"Riapertura Cartella");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_CARTELLARIAPRI, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_CARTELLAAMBULATORIALERIAPRI;
                oNodeParent.Nodes.Add(oNode);

                oNodeParent = this.UltraTreeMenu.GetNodeByKey(MyStatics.TV_ROOT);

                oNode = new UltraTreeNode(MyStatics.TV_CANCELLAEVIDENZACLINICA, @"Cancellazione Evidenza Clinica");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_CANCELLAEVIDENZACLINICA, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_CANCELLAEVIDENZACLINICA;
                oNodeParent.Nodes.Add(oNode);

                oNode = new UltraTreeNode(MyStatics.TV_ALLINEAADT, @"Allinea ADT");
                oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.TV_ALLINEAADT, Enums.EnumImageSize.isz16)));
                oNode.Tag = MyStatics.TV_ALLINEAADT;
                oNodeParent.Nodes.Add(oNode);

                this.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadTreeView", this.Text);
            }
        }

        private void LoadTreeViewSchedeFiglie(string parentNodeID, string parentNodeDescription)
        {
            UltraTreeNode oNodeParent = null;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                this.UltraTreeSchedeFiglie13.Nodes.Clear();

                oNodeParent = new UltraTreeNode(parentNodeID, "Schede Figlie Cancellate");
                oNodeParent.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz16)));
                oNodeParent.Tag = parentNodeID;
                oNodeParent.Expanded = true;
                oNodeParent.CheckedState = CheckState.Checked;
                this.UltraTreeSchedeFiglie13.Nodes.Add(oNodeParent);

                AddSchedeFiglie(parentNodeID, oNodeParent);

                this.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "LoadTreeViewSchedeFiglie", this.Text);
            }
        }

        private void AddSchedeFiglie(string parentNodeID, UltraTreeNode parentNode)
        {
            UltraTreeNode oNode = null;
            try
            {
                foreach (ChildForm child in this.GetSchedeFiglie(parentNodeID))
                {
                    oNode = new UltraTreeNode(child.id, child.description);
                    oNode.LeftImages.Add(Risorse.GetImageFromResource(MyStatics.GetNameResource(MyStatics.GC_SCHEDE, Enums.EnumImageSize.isz16)));
                    oNode.Tag = child.id;
                    oNode.Expanded = true;
                    oNode.CheckedState = CheckState.Checked;
                    parentNode.Nodes.Add(oNode);

                    if (child.hasChild) this.AddSchedeFiglie(child.id, oNode);
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "AddSchedeFiglie", this.Text);
            }
        }

        private List<ChildForm> GetSchedeFiglie(string parentID)
        {
            List<ChildForm> retList = new List<ChildForm>();
            string sSql = string.Empty;

            try
            {
                sSql = @"SELECT ID, IDSchedaPadre,
                    S.Descrizione  + CONVERT(varchar(20),M.Numero ) +
                    ' Data Creazione : ' + ISNULL(CONVERT(Varchar(10),DataCreazione,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataCreazione,14),'') + 
                    ', Data Modifica : ' + ISNULL(CONVERT(Varchar(10),DataUltimaModifica,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataUltimaModifica,14),'') +  CHAR(13)+CHAR(10) +
                    'Stato: ' + ISNULL(ST.Descrizione ,'') + 
                    ' Utente: ' + ISNULL(CodUtenteRilevazione,'') +  CHAR(13) + CHAR(10) +
                    'Entita: ' + ISNULL(M.CodEntita,'') + ' Storicizza: ' + Convert(CHAR(1),M.Storicizzata) +  CHAR(13) + CHAR(10) +
                    'Scheda: ' + '(' + ISNULL(M.CodScheda,'') + ') ' + ISNULL(S.Descrizione,'')  +  
                    ' n°: ' + CONVERT(VARCHAR(5),M.Numero) AS Descrizione
                    FROM 
	                    T_MovSchede M WITH (NOLOCK)
		                    LEFT JOIN T_StatoScheda ST WITH (NOLOCK)
			                    ON M.CodStatoScheda = ST.Codice
		                    LEFT JOIN T_Schede S WITH (NOLOCK)
			                    ON M.CodScheda=S.Codice	
                    WHERE 
                    IDSchedaPadre = '" + parentID + "' AND CodStatoScheda = 'CA' AND Storicizzata = 0";

                DataTable dt = DataBase.GetDataTable(sSql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        ChildForm child = new ChildForm();
                        child.id = r["ID"].ToString();
                        child.description = r["Descrizione"].ToString();

                        string childrenCount = DataBase.FindValue("COUNT(*) AS Children", "T_MovSchede", "IDSchedaPadre = '" + child.id + "'", string.Empty);
                        if (childrenCount != string.Empty && int.Parse(childrenCount) > 0)
                            child.hasChild = true;
                        else
                            child.hasChild = false;

                        retList.Add(child);
                    }
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "GetSchedeFiglie", this.Text);
                retList = new List<ChildForm>();
            }

            return retList;
        }

        private List<string> GetSchedeID(UltraTreeNode parentNode)
        {
            List<string> retList = new List<string>();

            try
            {
                foreach (UltraTreeNode childNode in parentNode.Nodes)
                {
                    if (childNode.CheckedState == CheckState.Checked) retList.Add(childNode.Tag.ToString());
                    if (childNode.HasNodes)
                    {
                        retList.AddRange(GetSchedeID(childNode));
                    }
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "GetSchedeID", this.Text);
                retList = new List<string>();
            }

            return retList;
        }
        #endregion

        #region UltraTabControl

        private void InitializeUltraTab()
        {
            this.ShowTabPage(C_DUMMY);
        }

        #endregion

        #region UltraGrid

        private void InitializeUltraGrid()
        {
            MyStatics.SetUltraGridLayout(ref this.ugEsito, false, false);
            this.ugEsito.DisplayLayout.GroupByBox.Hidden = true;
            this.ugEsito.DisplayLayout.Override.AutoResizeColumnWidthOptions = AutoResizeColumnWidthOptions.All;
            this.ugEsito.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            this.ugEsito.Text = string.Format("{0} ({1:#,##0})", "Errori Elaborazione", this.ugEsito.Rows.Count);

            MyStatics.SetUltraGridLayout(ref this.ugGriglia21, false, false);
            this.ugGriglia21.DisplayLayout.GroupByBox.Hidden = true;
            this.ugGriglia21.DisplayLayout.Override.AutoResizeColumnWidthOptions = AutoResizeColumnWidthOptions.All;

        }

        private void LoadGriglia21()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {

                string sSql = "SELECT MS.* FROM T_MovSchede MS WITH (NOLOCK) " + Environment.NewLine;

                if (this.uteCriterio21.Text == string.Empty || this.uteCriterio21.Text.Contains("WHERE") == false)
                {
                    sSql += "WHERE 1=0";
                }
                else
                {
                    sSql += this.uteCriterio21.Text;
                }

                this.ugGriglia21.DataSource = DataBase.GetDataSet(sSql);
                this.ugGriglia21.Refresh();
                this.ugGriglia21.Text = string.Format("{0} ({1:#,##0})", "Schede", this.ugGriglia21.Rows.Count);

            }
            catch (Exception)
            {

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        #region Private

        private void ShowTabPage(string PageName)
        {

            try
            {

                this.ugbEsito.Visible = true;
                this.ubElabora.Visible = true;
                this.utxtEsito.Text = string.Empty;
                this.ugEsito.DataSource = null;
                this.ugEsito.Refresh();
                this.pbEsito.Image = null;

                switch (PageName)
                {
                    case MyStatics.TV_ROOT:
                    case MyStatics.TV_SPOSTASINGOLOMENU:
                    case MyStatics.TV_SPOSTAGRUPPOMENU:
                    case C_DUMMY:
                    default:
                        this.utcManutenzioneDati.Tabs[C_DUMMY].Selected = true;
                        this.ugbEsito.Visible = false;
                        this.ubElabora.Visible = false;
                        break;
                    case MyStatics.TV_SPOSTASINGOLOAPPPAZAMB:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTASINGOLOAPPPAZAMB].Selected = true;
                        this.utxtIDEntitaInizio2.Text = string.Empty;
                        this.utxtIDEntitaFine2.Text = string.Empty;
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPAMBTOEPI:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTASINGOLOAPPAMBTOEPI].Selected = true;
                        this.utxtIDEntitaInizio18.Text = string.Empty;
                        this.ucInfoCartellaFine18.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaFine18.NumeroCartella = string.Empty;
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPEPITOAMB:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTASINGOLOAPPEPITOAMB].Selected = true;
                        this.utxtIDEntitaInizio19.Text = string.Empty;
                        break;

                    case MyStatics.TV_ALLINEAADT:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_ALLINEAADT].Selected = true;
                        this.ucAllineaADT1.Refresh();
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPPAZRO:
                    case MyStatics.TV_SPOSTASINGOLODIARIOCLINICO:
                    case MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE:
                    case MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI:
                    case MyStatics.TV_SPOSTASINGOLOTASKINF:
                    case MyStatics.TV_SPOSTASINGOLOALERTGENERICO:
                        this.utcManutenzioneDati.Tabs["SpostaSingolo"].Selected = true;
                        this.ucInfoCartellaInizio1.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio1.NumeroCartella = string.Empty;
                        this.ucInfoCartellaFine1.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaFine1.NumeroCartella = string.Empty;
                        this.utxtIDEntitaInizio1.Text = string.Empty;
                        this.SetLabelDescription(PageName);
                        break;
                    case MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA].Selected = true;
                        this.utxtIDEntitaInizio11.Text = string.Empty;
                        this.utxtIDEntitaFine11.Text = string.Empty;
                        break;
                    case MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ].Selected = true;
                        this.utxtIDEntitaInizio3.Text = string.Empty;
                        this.utxtIDEntitaFine3.Text = string.Empty;
                        this.utxtIDSchedaPadreFine3.Text = string.Empty;
                        this.umeNumerositaFine3.Text = string.Empty;
                        break;
                    case MyStatics.TV_SPOSTASINGOLOSCHEDAEPI:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTASINGOLOSCHEDAEPI].Selected = true;
                        this.ucInfoCartellaInizio4.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio4.NumeroCartella = string.Empty;
                        this.ucInfoCartellaFine4.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaFine4.NumeroCartella = string.Empty;
                        this.utxtIDEntitaInizio4.Text = string.Empty;
                        this.utxtIDEntitaFine4.Text = string.Empty;
                        this.utxtIDSchedaPadreFine4.Text = string.Empty;
                        this.umeNumerositaFine4.Text = string.Empty;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB].Selected = true;
                        this.utxtIDEntitaInizio6.Text = string.Empty;
                        this.utxtIDEntitaFine6.Text = string.Empty;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZRO:
                    case MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO:
                    case MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE:
                    case MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI:
                    case MyStatics.TV_SPOSTAGRUPPOTASKINF:
                    case MyStatics.TV_SPOSTAGRUPPOALERTGENRICO:
                        this.utcManutenzioneDati.Tabs["SpostaGruppo"].Selected = true;
                        this.ucInfoCartellaInizio5.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio5.NumeroCartella = string.Empty;
                        this.ucInfoCartellaFine5.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaFine5.NumeroCartella = string.Empty;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOEVIDENZACLINICA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTAGRUPPOEVIDENZACLINICA].Selected = true;
                        this.utxtIDEntitaInizio12.Text = string.Empty;
                        this.utxtIDEntitaFine12.Text = string.Empty;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ].Selected = true;
                        this.utxtIDEntitaInizio7.Text = string.Empty;
                        this.utxtIDEntitaFine7.Text = string.Empty;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI].Selected = true;
                        this.ucInfoCartellaInizio8.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio8.NumeroCartella = string.Empty;
                        this.ucInfoCartellaFine8.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaFine8.NumeroCartella = string.Empty;
                        break;
                    case MyStatics.TV_CARTELLACANCELLA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_CARTELLACANCELLA].Selected = true;
                        this.ucInfoCartellaInizio9.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio9.NumeroCartella = string.Empty;
                        break;
                    case MyStatics.TV_CARTELLARIAPRI:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_CARTELLARIAPRI].Selected = true;
                        this.ucInfoCartellaInizio10.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio10.NumeroCartella = string.Empty;
                        break;
                    case MyStatics.TV_SBLOCCOSCHEDA:
                    case MyStatics.TV_ANNULLACANCSCHEDA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SBLOCCOSCHEDA].Selected = true;
                        this.utxtIDSchedaInizio13.Text = string.Empty;

                        if (this.UltraTreeMenu.SelectedNodes[0].Key.ToString() == MyStatics.TV_ANNULLACANCSCHEDA)
                        {
                            this.ugbFiglie13.Visible = true;
                            this.UltraTreeSchedeFiglie13.Nodes.Clear();
                        }
                        else
                            this.ugbFiglie13.Visible = false;
                        break;
                    case MyStatics.TV_RIPRISTINOREVISIONESCHEDA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_RIPRISTINOREVISIONESCHEDA].Selected = true;
                        this.utxtIDSchedaInizio24.Text = string.Empty;
                        this.utxtIDSchedaInizio24.Text = string.Empty;
                        break;
                    case MyStatics.TV_CANCELLAEVIDENZACLINICA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_CANCELLAEVIDENZACLINICA].Selected = true;
                        this.utxtIDSchedaInizio14.Text = string.Empty;
                        break;
                    case MyStatics.TV_SCHEDAGENERARTFSINGOLO:
                        this.ugbEsito.Visible = false;
                        this.ubElabora.Visible = false;
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SCHEDAGENERARTFSINGOLO].Selected = true;
                        this.uteIDScheda16.Text = string.Empty;
                        break;
                    case MyStatics.TV_SCHEDAGENERARTFMULTIPLO:
                        this.ugbEsito.Visible = false;
                        this.ubElabora.Visible = false;
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_SCHEDAGENERARTFMULTIPLO].Selected = true;
                        this.LoadGriglia21();
                        break;
                    case MyStatics.TV_CARTELLASCOLLEGA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_CARTELLASCOLLEGA].Selected = true;
                        this.ucInfoCartellaInizio17.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio17.NumeroCartella = string.Empty;
                        break;
                    case MyStatics.TV_MODIFICANUMEROCARTELLA:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_MODIFICANUMEROCARTELLA].Selected = true;
                        this.ucInfoCartellaInizio20.CodUnitaAtomica = string.Empty;
                        this.ucInfoCartellaInizio20.NumeroCartella = string.Empty;
                        this.uteNumeroCartellaFinale20.Text = string.Empty;
                        break;
                    case MyStatics.TV_MODIFICANUMEROCARTELLAAMBULATORIALE:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_MODIFICANUMEROCARTELLAAMBULATORIALE].Selected = true;
                        this.ucInfoCartellaInizio22.CodScheda = string.Empty;
                        this.ucInfoCartellaInizio22.NumeroCartella = string.Empty;
                        this.uteNumeroCartellaFinale22.Text = string.Empty;
                        break;
                    case MyStatics.TV_CARTELLAAMBULATORIALERIAPRI:
                        this.utcManutenzioneDati.Tabs[MyStatics.TV_CARTELLAAMBULATORIALERIAPRI].Selected = true;
                        this.ucInfoCartellaInizio23.CodScheda = string.Empty;
                        this.ucInfoCartellaInizio23.NumeroCartella = string.Empty;
                        break;
                }

            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "ShowTabPage", this.Text);
            }

        }

        private void SetLabelDescription(string PageName)
        {
            switch (PageName)
            {
                case MyStatics.TV_SPOSTASINGOLOAPPPAZRO:
                    this.lblIDEntitaInizio1.Text = "ID Appuntamento RO";
                    break;
                case MyStatics.TV_SPOSTASINGOLODIARIOCLINICO:
                    this.lblIDEntitaInizio1.Text = "ID Voce Diario";
                    break;
                case MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE:
                    this.lblIDEntitaInizio1.Text = "ID Prescrizione";
                    break;
                case MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI:
                    this.lblIDEntitaInizio1.Text = "ID Parametro Vitale";
                    break;
                case MyStatics.TV_SPOSTASINGOLOTASKINF:
                    this.lblIDEntitaInizio1.Text = "ID Task Infermieristico";
                    break;
                case MyStatics.TV_SPOSTASINGOLOALERTGENERICO:
                    this.lblIDEntitaInizio1.Text = "ID Alert Generico";
                    break;
                case MyStatics.TV_SPOSTAGRUPPOAPPPAZRO: break;
                case MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO: break;
                case MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE: break;
                case MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI: break;
                case MyStatics.TV_SPOSTAGRUPPOTASKINF: break;
            }
        }

        private string GetEntityDescription(string CodTipoEntita, string IDEntita)
        {


            SqlParameter[] sqlParams = new SqlParameter[2];
            SqlParameterCollection spcollout;
            DataSet oDs = null;

            string sRet = string.Empty;
            Guid IDGuid;

            try
            {
                if (CodTipoEntita != string.Empty && IDEntita != string.Empty && Guid.TryParse(IDEntita.Trim(), out IDGuid))
                {
                    sqlParams[0] = new SqlParameter("CodTipoEntita", CodTipoEntita);
                    sqlParams[1] = new SqlParameter("IDEntita", IDEntita.Trim());

                    oDs = UnicodeSrl.ScciManagement.DataBase.ExecStoredProc("MSP_BO_SelInfoEntita", sqlParams, out spcollout);

                    if (oDs != null)
                    {
                        if (oDs.Tables[0].Rows.Count > 0)
                            sRet = oDs.Tables[0].Rows[0]["Info"].ToString();
                        else
                            sRet = "";
                    }
                    else
                        sRet = "";
                }
                else
                    sRet = "";
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "GetEntityDescription", this.Text);
            }

            return sRet;

        }

        private string GetEntityType(string PageName)
        {
            string sRet = string.Empty;

            try
            {
                switch (PageName)
                {
                    case MyStatics.TV_SPOSTASINGOLOAPPPAZAMB:
                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB:
                    case MyStatics.TV_SPOSTASINGOLOAPPPAZRO:
                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZRO:
                        sRet = "APP";
                        break;
                    case MyStatics.TV_SPOSTASINGOLODIARIOCLINICO:
                    case MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO:
                        sRet = "DCL";
                        break;
                    case MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE:
                    case MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE:
                        sRet = "PRF";
                        break;
                    case MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI:
                    case MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI:
                        sRet = "PVT";
                        break;
                    case MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ:
                    case MyStatics.TV_SPOSTASINGOLOSCHEDAEPI:
                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ:
                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI:
                    case MyStatics.TV_RIPRISTINOREVISIONESCHEDA:
                        sRet = "SCH";
                        break;
                    case MyStatics.TV_SPOSTASINGOLOTASKINF:
                    case MyStatics.TV_SPOSTAGRUPPOTASKINF:
                        sRet = "WKI";
                        break;
                    case MyStatics.TV_SPOSTASINGOLOALERTGENERICO:
                    case MyStatics.TV_SPOSTAGRUPPOALERTGENRICO:
                        sRet = "ALG";
                        break;
                    case MyStatics.TV_CARTELLACANCELLA:
                    case MyStatics.TV_CARTELLARIAPRI:
                        sRet = "CAR";
                        break;
                    case MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA:
                        sRet = "EVC";
                        break;

                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "GetEntityDescription", this.Text);
            }

            return sRet;
        }

        private Parametri GetXMLParameters(string sectionKey)
        {
            Scci.DataContracts.ScciAmbiente ambiente = CoreStatics.CoreApplication.Ambiente;
            ambiente.Codlogin = UnicodeSrl.Framework.Utility.Windows.CurrentUser();

            Parametri op = new Parametri(ambiente);

            try
            {
                op.Parametro.Add("CodOpe", sectionKey);
                switch (sectionKey)
                {
                    case MyStatics.TV_SPOSTASINGOLOAPPPAZAMB:
                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio2.Text);
                        op.Parametro.Add("IDPazienteFine", this.utxtIDEntitaFine2.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB:
                        op.TimeStamp.CodEntita = EnumEntita.PAZ.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio6.Text);
                        op.Parametro.Add("IDPazienteFine", this.utxtIDEntitaFine6.Text);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPAMBTOEPI:
                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio18.Text);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine18.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine18.NumeroCartella);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPEPITOAMB:
                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio19.Text);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA:
                        op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio11.Text);
                        op.Parametro.Add("IDEpisodioFine", this.utxtIDEntitaFine11.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOEVIDENZACLINICA:
                        op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                        op.Parametro.Add("IDEpisodioInizio", this.utxtIDEntitaInizio12.Text);
                        op.Parametro.Add("IDEpisodioFine", this.utxtIDEntitaFine12.Text);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPPAZRO:
                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio1.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine1.NumeroCartella);
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio1.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZRO:
                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio5.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine5.NumeroCartella);
                        break;

                    case MyStatics.TV_SPOSTASINGOLODIARIOCLINICO:
                        op.TimeStamp.CodEntita = EnumEntita.DCL.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio1.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine1.NumeroCartella);
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio1.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO:
                        op.TimeStamp.CodEntita = EnumEntita.DCL.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio5.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine5.NumeroCartella);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE:
                        op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio1.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine1.NumeroCartella);
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio1.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE:
                        op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio5.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine5.NumeroCartella);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI:
                        op.TimeStamp.CodEntita = EnumEntita.PVT.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio1.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine1.NumeroCartella);
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio1.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI:
                        op.TimeStamp.CodEntita = EnumEntita.PVT.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio5.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine5.NumeroCartella);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ:
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio3.Text);
                        op.Parametro.Add("IDPazienteFine", this.utxtIDEntitaFine3.Text);
                        op.Parametro.Add("IDSchedaPadreFine", this.utxtIDSchedaPadreFine3.Text);
                        op.Parametro.Add("NumerositaFine", this.umeNumerositaFine3.Text);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOSCHEDAEPI:
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio4.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio4.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine4.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine4.NumeroCartella);
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio4.Text);
                        op.Parametro.Add("IDEntitaFine", this.utxtIDEntitaFine4.Text); op.Parametro.Add("IDSchedaPadreFine", this.utxtIDSchedaPadreFine4.Text);
                        op.Parametro.Add("NumerositaFine", this.umeNumerositaFine4.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ:
                        op.TimeStamp.CodEntita = EnumEntita.PAZ.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio7.Text);
                        op.Parametro.Add("IDPazienteFine", this.utxtIDEntitaFine7.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI:
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio8.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio8.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine8.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine8.NumeroCartella);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOTASKINF:
                        op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio1.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine1.NumeroCartella);
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio1.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOTASKINF:
                        op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio5.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine5.NumeroCartella);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOALERTGENERICO:
                        op.TimeStamp.CodEntita = EnumEntita.ALG.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio1.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine1.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine1.NumeroCartella);
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDEntitaInizio1.Text);
                        break;

                    case MyStatics.TV_SPOSTAGRUPPOALERTGENRICO:
                        op.TimeStamp.CodEntita = EnumEntita.ALG.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio5.NumeroCartella);
                        op.Parametro.Add("CodUAFine", this.ucInfoCartellaFine5.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaFine", this.ucInfoCartellaFine5.NumeroCartella);
                        break;

                    case MyStatics.TV_CARTELLACANCELLA:
                        op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio9.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio9.NumeroCartella);
                        break;

                    case MyStatics.TV_CARTELLARIAPRI:
                        op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio10.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio10.NumeroCartella);
                        break;

                    case MyStatics.TV_ANNULLACANCSCHEDA:
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDSchedaInizio13.Text);

                        List<string> chilrenID = new List<string>();
                        chilrenID.AddRange(this.GetSchedeID(this.UltraTreeSchedeFiglie13.Nodes[0]));
                        if (chilrenID.Count > 0)
                        {
                            op.ParametroRipetibile.Add("IDSchedeFiglie", chilrenID.ToArray());
                        }

                        break;
                    case MyStatics.TV_RIPRISTINOREVISIONESCHEDA:
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDSchedaInizio24.Text);
                        op.Parametro.Add("IDEntitaFine", this.utxtIDSchedaFine24.Text);
                        break;
                    case MyStatics.TV_SBLOCCOSCHEDA:
                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDSchedaInizio13.Text);
                        break;

                    case MyStatics.TV_CANCELLAEVIDENZACLINICA:
                        op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                        op.Parametro.Add("IDEntitaInizio", this.utxtIDSchedaInizio14.Text);
                        break;

                    case MyStatics.TV_CARTELLASCOLLEGA:
                        op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio17.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio17.NumeroCartella);
                        break;

                    case MyStatics.TV_MODIFICANUMEROCARTELLA:
                        op.TimeStamp.CodEntita = EnumEntita.CAR.ToString();
                        op.Parametro.Add("CodUAInizio", this.ucInfoCartellaInizio20.CodUnitaAtomica);
                        op.Parametro.Add("NumeroCartellaInizio", this.ucInfoCartellaInizio20.NumeroCartella);
                        op.Parametro.Add("NumeroCartellaFine", this.uteNumeroCartellaFinale20.Text);
                        op.Parametro.Add("AggiornaContatore", (this.chkAggiornaContatore20.Checked == false ? "0" : "1"));
                        break;

                    case MyStatics.TV_MODIFICANUMEROCARTELLAAMBULATORIALE:
                        op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();
                        op.Parametro.Add("CodSchedaAmbulatorialeInizio", this.ucInfoCartellaInizio22.CodScheda);
                        op.Parametro.Add("NumeroCartellaAmbulatorialeInizio", this.ucInfoCartellaInizio22.NumeroCartella);
                        op.Parametro.Add("NumeroCartellaAmbulatorialeFine", this.uteNumeroCartellaFinale22.Text);
                        op.Parametro.Add("AggiornaContatore", (this.chkAggiornaContatore22.Checked == false ? "0" : "1"));
                        break;

                    case MyStatics.TV_CARTELLAAMBULATORIALERIAPRI:
                        op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();
                        op.Parametro.Add("CodSchedaAmbulatorialeInizio", this.ucInfoCartellaInizio23.CodScheda);
                        op.Parametro.Add("NumeroCartellaAmbulatorialeInizio", this.ucInfoCartellaInizio23.NumeroCartella);
                        break;

                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "GetEntityType", this.Text);
                op = null;
            }

            return op;
        }

        private Image GetPictureBoxImage(bool Esito)
        {

            Image imgRet = null;

            try
            {
                switch (Esito)
                {
                    case false:
                        imgRet = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SI_256);
                        break;
                    case true:
                        imgRet = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SOSPENDI_256);
                        break;
                }
            }
            catch
            {

            }

            return imgRet;

        }

        private bool CheckInput(string sectionKey)
        {
            bool bRet = true;

            try
            {

                switch (sectionKey)
                {
                    case MyStatics.TV_SPOSTASINGOLOAPPPAZAMB:
                        bRet = true;
                        break;
                    case MyStatics.TV_SPOSTASINGOLOAPPPAZRO:
                    case MyStatics.TV_SPOSTASINGOLODIARIOCLINICO:
                    case MyStatics.TV_SPOSTASINGOLOPRESCRIZIONE:
                    case MyStatics.TV_SPOSTASINGOLOPARAMETRIVITALI:
                    case MyStatics.TV_SPOSTASINGOLOTASKINF:
                    case MyStatics.TV_SPOSTASINGOLOALERTGENERICO:
                        bRet = !this.ucInfoCartellaInizio1.ErroreCartella && !this.ucInfoCartellaFine1.ErroreCartella;
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPAMBTOEPI:
                        bRet = (this.utxtIDEntitaInizioDes18.Text != string.Empty && !this.ucInfoCartellaFine18.ErroreCartella &&
this.utxtIDEntitaInizioDes18.Text.Contains("ERRORE") != true);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOAPPEPITOAMB:
                        bRet = (this.utxtIDEntitaInizioDes19.Text != string.Empty && this.utxtIDEntitaInizioDes19.Text.Contains("ERRORE") != true);
                        break;

                    case MyStatics.TV_SPOSTASINGOLOSCHEDAPAZ:
                        bRet = true;
                        break;
                    case MyStatics.TV_SPOSTASINGOLOSCHEDAEPI:
                        bRet = !this.ucInfoCartellaInizio4.ErroreCartella && !this.ucInfoCartellaFine4.ErroreCartella;
                        break;
                    case MyStatics.TV_SPOSTASINGOLOEVIDENZACLINICA:
                        bRet = (this.utxtIDEntitaInizioDes11.Text != string.Empty && this.utxtIDEntitaFineDes11.Text != string.Empty &&
this.utxtIDEntitaInizioDes11.Text.Contains("ERRORE") != true && this.utxtIDEntitaFineDes11.Text.Contains("ERRORE") != true);
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOEVIDENZACLINICA:
                        bRet = (this.utxtIDEntitaInizioDes12.Text != string.Empty && this.utxtIDEntitaFineDes12.Text != string.Empty &&
this.utxtIDEntitaInizioDes12.Text.Contains("ERRORE") != true && this.utxtIDEntitaFineDes12.Text.Contains("ERRORE") != true);
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZAMB:
                        bRet = true;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOAPPPAZRO:
                    case MyStatics.TV_SPOSTAGRUPPODIARIOCLINICO:
                    case MyStatics.TV_SPOSTAGRUPPOPRESCRIZIONE:
                    case MyStatics.TV_SPOSTAGRUPPOPARAMETRIVITALI:
                    case MyStatics.TV_SPOSTAGRUPPOTASKINF:
                    case MyStatics.TV_SPOSTAGRUPPOALERTGENRICO:
                        bRet = !this.ucInfoCartellaInizio5.ErroreCartella && !this.ucInfoCartellaFine5.ErroreCartella;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAPAZ:
                        bRet = true;
                        break;
                    case MyStatics.TV_SPOSTAGRUPPOSCHEDAEPI:
                        bRet = !this.ucInfoCartellaInizio8.ErroreCartella && !this.ucInfoCartellaFine8.ErroreCartella;
                        break;
                    case MyStatics.TV_CARTELLACANCELLA:
                        bRet = !this.ucInfoCartellaInizio9.ErroreCartella;
                        break;
                    case MyStatics.TV_CARTELLARIAPRI:
                        bRet = !this.ucInfoCartellaInizio10.ErroreCartella;
                        break;
                    case MyStatics.TV_SCHEDAGENERARTFSINGOLO:
                        bRet = false;
                        break;
                    case MyStatics.TV_CARTELLASCOLLEGA:
                        bRet = !this.ucInfoCartellaInizio17.ErroreCartella;
                        break;
                    case MyStatics.TV_MODIFICANUMEROCARTELLA:
                        bRet = !this.ucInfoCartellaInizio20.ErroreCartella &&
this.uteNumeroCartellaFinale20.Text != string.Empty &&
this.ucInfoCartellaInizio20.NumeroCartella != this.uteNumeroCartellaFinale20.Text;
                        break;

                    case MyStatics.TV_MODIFICANUMEROCARTELLAAMBULATORIALE:
                        bRet = !this.ucInfoCartellaInizio22.ErroreCartella &&
this.uteNumeroCartellaFinale22.Text != string.Empty &&
this.ucInfoCartellaInizio22.NumeroCartella != this.uteNumeroCartellaFinale22.Text;
                        break;
                    case MyStatics.TV_CARTELLAAMBULATORIALERIAPRI:
                        bRet = !this.ucInfoCartellaInizio23.ErroreCartella;
                        break;

                    case MyStatics.TV_SBLOCCOSCHEDA:
                    case MyStatics.TV_ANNULLACANCSCHEDA:
                        bRet = (this.utxtIDSchedaInizioDes13.Text != "");
                        break;
                    case MyStatics.TV_RIPRISTINOREVISIONESCHEDA:
                        bRet = (this.utxtIDSchedaInizioDes24.Text != "" && this.utxtIDSchedaFineDes24.Text != "");
                        break;
                }

                if (!bRet)
                {
                    MessageBox.Show("Entita iniziale o Entita finale non selezionata o non trovata !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                MyStatics.ExGest(ref ex, "CheckInput", this.Text);
                bRet = false;
            }

            return bRet;
        }

        #endregion

        #region Events

        private void frmManutenzioneDati_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ugEsito != null) this.ugEsito.Dispose();
        }

        private void UltraTreeMenu_AfterSelect(object sender, SelectEventArgs e)
        {
            if (this.UltraTreeMenu.SelectedNodes.Count > 0)
            {
                this.ShowTabPage(this.UltraTreeMenu.SelectedNodes[0].Key.ToString());
            }
            else
            {
                this.ShowTabPage(C_DUMMY);
            }
        }

        private void utxtIDEntitaInizio1_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes1.Text = GetEntityDescription(GetEntityType(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()), this.utxtIDEntitaInizio1.Text);
        }

        private void utxtIDEntitaInizio2_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes2.Text = GetEntityDescription(GetEntityType(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()), this.utxtIDEntitaInizio2.Text);
        }

        private void utxtIDEntitaFine2_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaFineDes2.Text = GetEntityDescription("PAZ", this.utxtIDEntitaFine2.Text);
        }

        private void utxtIDEntitaInizio3_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes3.Text = GetEntityDescription(GetEntityType(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()), this.utxtIDEntitaInizio3.Text);
        }

        private void utxtIDEntitaFine3_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaFineDes3.Text = GetEntityDescription("PAZ", this.utxtIDEntitaFine3.Text);
        }

        private void utxtIDEntitaInizio4_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes4.Text = GetEntityDescription(GetEntityType(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()), this.utxtIDEntitaInizio4.Text);
        }

        private void utxtIDEntitaFine4_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaFineDes4.Text = GetEntityDescription("EPI", this.utxtIDEntitaFine4.Text);
        }

        private void utxtIDEntitaInizio6_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes6.Text = GetEntityDescription("PAZ", this.utxtIDEntitaInizio6.Text);
        }

        private void utxtIDEntitaFine6_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaFineDes6.Text = GetEntityDescription("PAZ", this.utxtIDEntitaFine6.Text);
        }

        private void utxtIDEntitaInizio7_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes7.Text = GetEntityDescription("PAZ", this.utxtIDEntitaInizio7.Text);
        }

        private void utxtIDEntitaFine7_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaFineDes7.Text = GetEntityDescription("PAZ", this.utxtIDEntitaFine7.Text);
        }

        private void utxtIDEntitaInizio11_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes11.Text = GetEntityDescription(GetEntityType(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()), this.utxtIDEntitaInizio11.Text);
        }

        private void utxtIDEntitaFine11_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaFineDes11.Text = GetEntityDescription("EPI", this.utxtIDEntitaFine11.Text);
        }

        private void utxtIDEntitaInizio12_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes12.Text = GetEntityDescription("EPI", this.utxtIDEntitaInizio12.Text);
        }

        private void utxtIDEntitaFine12_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaFineDes12.Text = GetEntityDescription("EPI", this.utxtIDEntitaFine12.Text);
        }

        private void utxtIDSchedaInizio13_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDSchedaInizioDes13.Text = GetEntityDescription("SCH", this.utxtIDSchedaInizio13.Text);

            if (this.utxtIDSchedaInizioDes13.Text != string.Empty)
                this.LoadTreeViewSchedeFiglie(utxtIDSchedaInizio13.Text, utxtIDSchedaInizioDes13.Text);
            else
                this.UltraTreeSchedeFiglie13.Nodes.Clear();
        }

        private void utxtIDSchedaInizio14_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDSchedaInizioDes14.Text = GetEntityDescription("EVC", this.utxtIDSchedaInizio14.Text);
        }

        private void utxtIDEntitaInizio18_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes18.Text = GetEntityDescription("APP", this.utxtIDEntitaInizio18.Text);
        }

        private void utxtIDEntitaInizio19_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDEntitaInizioDes19.Text = GetEntityDescription("APP", this.utxtIDEntitaInizio19.Text);
        }

        private void utxtIDSchedaInizio24_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDSchedaInizioDes24.Text = GetEntityDescription(GetEntityType(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()), this.utxtIDSchedaInizio24.Text);
        }

        private void utxtIDSchedaFine24_ValueChanged(object sender, EventArgs e)
        {
            this.utxtIDSchedaFineDes24.Text = GetEntityDescription(GetEntityType(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()), this.utxtIDSchedaFine24.Text);
        }

        private void ubElabora_Click(object sender, EventArgs e)
        {

            this.utxtEsito.Text = string.Empty;
            this.ugEsito.DataSource = null;
            this.ugEsito.Refresh();
            this.ugEsito.Text = string.Format("{0} ({1:#,##0})", "Errori Elaborazione", this.ugEsito.Rows.Count);
            this.pbEsito.Image = null;

            if (CheckInput(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()) &&
                    MessageBox.Show(this, "Confermi l'elaborazione selezionata ?",
                                "Conferma Elaborazione", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {

                    this.Cursor = Cursors.WaitCursor;
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = UnicodeSrl.Scci.Statics.XmlProcs.XmlSerializeToString(GetXMLParameters(this.UltraTreeMenu.SelectedNodes[0].Key.ToString()));
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    this.ubElabora.Enabled = false;
                    DataSet ds = UnicodeSrl.Scci.Statics.Database.GetDatasetStoredProc("MSP_BO_Elabora", spcoll);

                    if (ds != null)
                    {
                        this.utxtEsito.Text = ds.Tables[0].Rows[0]["DescrizioneErrore"].ToString();
                        this.ugEsito.DataSource = ds.Tables[1];
                        this.pbEsito.Image = GetPictureBoxImage((bool)ds.Tables[0].Rows[0]["Errore"]);
                    }
                    else
                    {
                        this.utxtEsito.Text = string.Empty;
                        this.ugEsito.DataSource = null;
                        this.pbEsito.Image = null;
                    }

                    this.ugEsito.Refresh();
                    this.ugEsito.Text = string.Format("{0} ({1:#,##0})", "Errori Elaborazione", this.ugEsito.Rows.Count);



                }
                catch (Exception ex)
                {
                    MyStatics.ExGest(ref ex, "ubElabora_Click", this.Text);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("Elaborazione terminata.", "Elaborazione", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.ubElabora.Enabled = true;
                }
            }
        }

        private void ugEsito_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            e.Layout.Bands[0].ColHeadersVisible = false;
            e.Layout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand);
        }

        private void ubAggiorna16_Click(object sender, EventArgs e)
        {

            try
            {

                if (this.uteIDScheda16.Text != string.Empty)
                {

                    Scci.DataContracts.ScciAmbiente ambiente = CoreStatics.CoreApplication.Ambiente;
                    ambiente.Codlogin = UnicodeSrl.Framework.Utility.Windows.CurrentUser();
                    MovScheda oMovScheda = new MovScheda(this.uteIDScheda16.Text, ambiente);
                    oMovScheda.GeneraRTF();
                    SqlParameter[] sqlParams = new SqlParameter[5];
                    sqlParams[0] = new SqlParameter("uIDScheda", this.uteIDScheda16.Text);
                    sqlParams[1] = new SqlParameter("sAnteprimaRTF", oMovScheda.AnteprimaRTF);
                    sqlParams[2] = new SqlParameter("sAnteprimaTXT", oMovScheda.AnteprimaTXT);
                    sqlParams[3] = new SqlParameter("sDatiObbligatoriMancantiRTF", oMovScheda.DatiObbligatoriMancantiRTF);
                    sqlParams[4] = new SqlParameter("sDatiRilievoRTF", oMovScheda.DatiRilievoRTF);

                    UnicodeSrl.ScciManagement.DataBase.ExecStoredProcNQ("MSP_BO_AggMovSchedeRTF", ref sqlParams);

                    this.utxtIDSchedaInizioDes16.Text = oMovScheda.AnteprimaRTF;

                }
                else
                {
                    MessageBox.Show("ID Scheda MANCANTE!!!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
            catch (Exception ex)
            {
                this.utxtIDSchedaInizioDes16.Text = string.Empty;
                MessageBox.Show(ex.Message);
            }

        }

        private void ubRicerca21_Click(object sender, EventArgs e)
        {
            this.LoadGriglia21();
        }

        private void ubAggiorna21_Click(object sender, EventArgs e)
        {

            try
            {

                if (MessageBox.Show("Sei sicuro di voler aggiornare " + this.ugGriglia21.Rows.Count + "?", "Aggiorna RTF", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {

                    this.Cursor = Cursors.WaitCursor;

                    this.ultraProgressBar21.Minimum = 0;
                    this.ultraProgressBar21.Maximum = this.ugGriglia21.Rows.Count;
                    this.ultraProgressBar21.Value = 0;

                    foreach (UltraGridRow oRow in this.ugGriglia21.Rows)
                    {

                        Scci.DataContracts.ScciAmbiente ambiente = CoreStatics.CoreApplication.Ambiente;
                        ambiente.Codlogin = UnicodeSrl.Framework.Utility.Windows.CurrentUser();
                        MovScheda oMovScheda = new MovScheda(oRow.Cells["ID"].Text, ambiente);
                        oMovScheda.GeneraRTF();
                        SqlParameter[] sqlParams = new SqlParameter[5];
                        sqlParams[0] = new SqlParameter("uIDScheda", oRow.Cells["ID"].Text);
                        sqlParams[1] = new SqlParameter("sAnteprimaRTF", oMovScheda.AnteprimaRTF);
                        sqlParams[2] = new SqlParameter("sAnteprimaTXT", oMovScheda.AnteprimaTXT);
                        sqlParams[3] = new SqlParameter("sDatiObbligatoriMancantiRTF", oMovScheda.DatiObbligatoriMancantiRTF);
                        sqlParams[4] = new SqlParameter("sDatiRilievoRTF", oMovScheda.DatiRilievoRTF);

                        this.ultraProgressBar21.Value = this.ultraProgressBar21.Value + 1;

                        UnicodeSrl.ScciManagement.DataBase.ExecStoredProcNQ("MSP_BO_AggMovSchedeRTF", ref sqlParams);

                    }

                    MessageBox.Show("Elaborazione terminata.", "Aggiorna RTF", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.ultraProgressBar21.Value = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion
    }
}
