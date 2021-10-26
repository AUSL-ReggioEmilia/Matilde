using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{

    public class NuovaPrescrizioneEventArgs : EventArgs
    {

        public int CurrentPrestIndex { get; set; }

        public string CurrentPrestID { get; set; }

    }

    public delegate void NuovaPrescrizioneEventHandler(object sender, NuovaPrescrizioneEventArgs e);

    [Serializable()]
    public class ProtocolloPrescrizioni
    {

        #region Declare

        public event NuovaPrescrizioneEventHandler NuovaPrescrizioneCreata;

        #endregion

        #region Property

        public string Codice { get; set; }

        public string Descrizione { get; set; }

        public List<ModelloPrescrizione> ModelliPrescrizioni { get; set; }

        public bool DataOraInizioObbligatoria { get; set; }

        public int VersioneModello { get; set; }

        #endregion

        #region Public methods

        public bool CreaPrescrizioni(string codua, string idpaziente, string idepisodio, string idtrasferimento,
                                     DateTime dataRiferimento, Scci.DataContracts.ScciAmbiente ambiente)
        {

            bool bret = false;

            try
            {
                if (ModelliPrescrizioni != null)
                {
                    for (int i = 0; i <= ModelliPrescrizioni.Count - 1; i++)
                    {

                        MovPrescrizione presc = new MovPrescrizione(codua, idpaziente, idepisodio, idtrasferimento, ambiente);

                        presc.CodViaSomministrazione = ModelliPrescrizioni[i].CodViaSomministrazione;
                        presc.CodTipoPrescrizione = ModelliPrescrizioni[i].CodTipoPrescrizione;

                        MovScheda schedaPrecedente = new MovScheda(ModelliPrescrizioni[i].CodiceScheda, Scci.Enums.EnumEntita.PRF, codua,
                                                        idpaziente, idepisodio, idtrasferimento, ModelliPrescrizioni[i].VersioneScheda,
                                                        ambiente);

                        schedaPrecedente.DatiXML = ModelliPrescrizioni[i].DatiXMLScheda;

                        presc.MovScheda = new MovScheda(ModelliPrescrizioni[i].CodiceScheda, Scci.Enums.EnumEntita.PRF, codua,
                                                        idpaziente, idepisodio, idtrasferimento, ModelliPrescrizioni[i].VersioneScheda,
                                                        ambiente);

                        presc.MovScheda.CopiaDaOrigine(schedaPrecedente, 1, true);

                        presc.Salva();

                        foreach (var modp in ModelliPrescrizioni[i].ModelliPrescrizionitempi)
                        {
                            MovPrescrizioneTempi movp = new MovPrescrizioneTempi(Guid.Parse(presc.IDPrescrizione), ambiente);

                            movp.CodTipoPrescrizioneTempi = modp.CodTipoPrescrizioneTempi;
                            movp.CodProtocollo = modp.CodProtocollo;
                            movp.DataOraInizio = dataRiferimento.AddSeconds(modp.DeltaDataOraInizio);
                            movp.DataOraFine = dataRiferimento.AddSeconds(modp.DeltaDataOraFine);
                            movp.AlBisogno = modp.AlBisogno;
                            movp.Durata = modp.Durata;
                            movp.Continuita = modp.Continuita;
                            movp.PeriodicitaGiorni = modp.PeriodicitaGiorni;
                            movp.PeriodicitaOre = modp.PeriodicitaOre;
                            movp.PeriodicitaMinuti = modp.PeriodicitaMinuti;
                            movp.Manuale = modp.Manuale;

                            if (modp.ModelliTempiManuali != null && modp.ModelliTempiManuali.Count > 0)
                            {
                                DataTable dttm = new DataTable();
                                DataSet dstm = new DataSet();

                                dttm.Columns.Add("DataOraInizio", typeof(DateTime));
                                dttm.Columns.Add("DataOraFine", typeof(DateTime));
                                dttm.Columns.Add("NomeProtocollo", typeof(string));
                                dttm.Columns.Add("EtichettaTempo", typeof(string));

                                dttm.TableName = "Table1";
                                dstm.DataSetName = "NewDataSet";

                                foreach (var modtm in modp.ModelliTempiManuali)
                                {
                                    DataRow r = dttm.NewRow();
                                    r["DataOraInizio"] = dataRiferimento.AddSeconds(modtm.DeltaDataOraInizio);
                                    r["DataOraFine"] = dataRiferimento.AddSeconds(modtm.DeltaDataOraFine);
                                    r["NomeProtocollo"] = modtm.NomeProtocollo;
                                    r["EtichettaTempo"] = modtm.EtichettaTempo;
                                    dttm.Rows.Add(r);
                                }

                                dstm.Tables.Add(dttm);

                                movp.TempiManuali = "<TempiManuali>" + dstm.GetXml() + "</TempiManuali>";
                            }

                            if (modp.CodiceSchedaPosologia != null && modp.CodiceSchedaPosologia != string.Empty && modp.VersioneSchedaPosologia != null)
                            {
                                MovScheda schedaTempiPrecedente = new MovScheda(modp.CodiceSchedaPosologia, Scci.Enums.EnumEntita.PRT, codua,
                                                            idpaziente, idepisodio, idtrasferimento, (int)modp.VersioneSchedaPosologia,
                                                            ambiente);

                                schedaTempiPrecedente.DatiXML = modp.DatiXMLSchedaPosologia;

                                movp.MovScheda = new MovScheda(modp.CodiceSchedaPosologia, Scci.Enums.EnumEntita.PRT, codua,
                                                            idpaziente, idepisodio, idtrasferimento, (int)modp.VersioneSchedaPosologia,
                                                            ambiente);

                                movp.MovScheda.CopiaDaOrigine(schedaTempiPrecedente, 1, true);

                                movp.Posologia = string.Empty;
                            }
                            else
                            {
                                movp.MovScheda = null;
                                movp.Posologia = modp.Posologia;
                            }

                            movp.Salva();
                        }

                        OnNuovaPrescrizioneCreata(i + 1, presc.IDPrescrizione);
                    }
                }

                bret = true;
            }
            catch (Exception ex)
            {
                bret = false;
                throw new Exception(@"ProtocolloPrescrizioni.CreaPrescrizioni()" + Environment.NewLine + ex.Message, ex);
            }

            return bret;
        }

        #endregion

        #region Private Methods

        private void OnNuovaPrescrizioneCreata(int currentPrestIndex, string currentPrestID)
        {
            NuovaPrescrizioneEventArgs e = new NuovaPrescrizioneEventArgs();
            e.CurrentPrestIndex = currentPrestIndex;
            e.CurrentPrestID = currentPrestID;
            NuovaPrescrizioneCreata(this, e);
        }

        #endregion

    }
}
