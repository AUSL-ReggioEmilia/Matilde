using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace UnicodeSrl.Scci.PluginClient
{

    [Serializable()]
    public class Plugins : IList<Plugin>
    {

        private List<Plugin> items;

        public Plugins()
        {
            items = new List<Plugin>();
        }

        public int IndexOf(Plugin item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Plugin item)
        {
            items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public Plugin this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        public void Add(Plugin item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(Plugin item)
        {
            return items.Contains(item);
        }

        public void CopyTo(Plugin[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(Plugin item)
        {
            return items.Remove(item);
        }

        public IEnumerator<Plugin> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

    }

    [Serializable()]
    public class Plugin
    {

        public Plugin(string codice, string descrizione, string nomeplugin, string comando, string modalita, string codua, int ordine, byte[] icona)
        {
            Codice = codice;
            Descrizione = descrizione;
            NomePlugin = nomeplugin;
            Comando = comando;
            Modalita = modalita;
            CodUA = codua;
            Ordine = ordine;
            Icona = icona;
        }

        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public string NomePlugin { get; set; }
        public string Comando { get; set; }
        public string Modalita { get; set; }
        public string CodUA { get; set; }
        public int Ordine { get; set; }
        public byte[] Icona { get; set; }

    }

}
