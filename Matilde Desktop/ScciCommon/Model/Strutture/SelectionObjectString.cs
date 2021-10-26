using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci.Model.Strutture
{
    public class SelectionObjectString
    {
        public SelectionObjectString(int indx,  string valore, string descrizione)
        {
            this.Indice = indx;
            this.Valore = valore;
            this.Descrizione = descrizione;
            this.Selezionato = false;
        }

        public SelectionObjectString(int indx, string valore, string descrizione, bool selezionato)
        {
            this.Indice = indx;
            this.Valore = valore;
            this.Descrizione = descrizione;
            this.Selezionato = selezionato;
        }

        public int Indice { get; set; }

        public bool     Selezionato { get; set; }

        public string Valore { get; set; }

        public string   Descrizione { get; set; }
    }
}
