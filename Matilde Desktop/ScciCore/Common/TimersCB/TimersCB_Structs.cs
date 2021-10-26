using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore.Common.TimersCB
{
    public class TimersCB_Controllo_Data
    {
        internal int DiarioClinico;
        internal int Allergie;
        internal int Alert;
        internal int EvidenzaClinica;
        internal bool Connettivita;
        internal int Segnalibri;
        internal int CartelleInVisione;
        internal int PazientiInVisione;
        internal int PazientiSeguiti;
        internal int PazienteSeguito;
        internal int PazientiSeguitiDaAltri;
        internal int NewsHard;
        internal int NewsLite;
        internal int MatHome;

        public TimersCB_Controllo_Data()
        {
            this.DiarioClinico = 0;
            this.Allergie = 0;
            this.Alert = 0;
            this.EvidenzaClinica = 0;
            this.Connettivita = false;
            this.Segnalibri = 0;
            this.CartelleInVisione = 0;
            this.PazientiInVisione = 0;
            this.PazientiSeguiti = 0;
            this.PazienteSeguito = 0;
            this.PazientiSeguitiDaAltri = 0;
            this.NewsHard = 0;
            this.NewsLite = 0;
            this.MatHome = 0;
        }

    }

}
