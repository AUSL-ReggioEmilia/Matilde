using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace UnicodeSrl.Scci.PluginClient
{

    public class Azioni : IDictionary<string, Azione>
    {

        private Dictionary<string, Azione> items;

        public Azioni()
        {
            items = new Dictionary<string, Azione>();
        }

        public void Add(string key, Azione value)
        {
            items.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return items.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return items.Keys; }
        }

        public bool Remove(string key)
        {
            return items.Remove(key);
        }

        public bool TryGetValue(string key, out Azione value)
        {
            throw new NotImplementedException();
        }

        public ICollection<Azione> Values
        {
            get { return items.Values; }
        }

        public Azione this[string key]
        {
            get
            {
                return items[key];
            }
            set
            {
                items[key] = value;
            }
        }

        public void Add(KeyValuePair<string, Azione> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(KeyValuePair<string, Azione> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, Azione>[] array, int arrayIndex)
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

        public bool Remove(KeyValuePair<string, Azione> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, Azione>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

    }

    public class Azione
    {

        public Azione()
        {
            Codice = string.Empty;
            Descrizione = string.Empty;
            Plugins = new Plugins();
        }
        public Azione(string codice, string descrizione)
        {
            Codice = codice;
            Descrizione = descrizione;
            Plugins = new Plugins();
        }

        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public Plugins Plugins { get; set; }

    }

}
