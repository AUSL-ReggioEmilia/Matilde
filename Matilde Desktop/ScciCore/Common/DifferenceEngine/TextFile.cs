using System;
using System.IO;
using System.Collections;

namespace UnicodeSrl.ScciCore.DifferenceEngine
{
    public class TextLine : IComparable
    {
        public string Line;
        public int _hash;

        public TextLine(string str)
        {
            Line = str.Replace("\t", "    ");
            _hash = str.GetHashCode();
        }
        #region IComparable Members

        public int CompareTo(object obj)
        {
            return _hash.CompareTo(((TextLine)obj)._hash);
        }

        #endregion
    }


    public class DiffList_TextFile : IDiffList
    {
        private const int MaxLineLength = 1024;
        private ArrayList _lines;

        public DiffList_TextFile(string fileName)
        {
            _lines = new ArrayList();
            using (StreamReader sr = new StreamReader(fileName))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    _lines.Add(new TextLine(line));
                }
            }
        }
        #region IDiffList Members

        public string Caption { get; set; }

        public int Count()
        {
            return _lines.Count;
        }

        public IComparable GetByIndex(int index)
        {
            return (TextLine)_lines[index];
        }

        #endregion

    }

    public class DiffList_Text : IDiffList
    {
        private const int MaxLineLength = 1024;
        private ArrayList _lines;

        public DiffList_Text(string text)
        {
            _lines = new ArrayList();
            using (StringReader sr = new StringReader(text))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    _lines.Add(new TextLine(line));
                }
            }
        }

        #region IDiffList Members

        public string Caption { get; set; }

        public int Count()
        {
            return _lines.Count;
        }

        public IComparable GetByIndex(int index)
        {
            return (TextLine)_lines[index];
        }

        #endregion

    }

}