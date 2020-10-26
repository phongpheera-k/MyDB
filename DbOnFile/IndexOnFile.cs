using System;
using System.Collections.Generic;
using System.IO;

namespace DbOnFile
{
    public class IndexOnFile<TK, TV>
    {
        public TK Key { get; set; }
        public TV Value { get; set; }
        
        public int Left { get; set; }
        public int Right { get; set; }
        public int Center { get; set; }
        public string FileName { get; set; }
        public string Folder { get; set; }
        
        readonly Func<TK,TK, int> work;

        public IndexOnFile(string folder, string fileName, Func<TK,TK, int> work)
        {
            Folder = folder;
            FileName = fileName;
            this.work = work;
            Directory.CreateDirectory(Folder);
        }
        
        
    }

    public class MyNode<TK, TV> : IComparer<TK>
    {
        
        readonly Func<TK,TK, int> work;
        
        public int Compare(TK x, TK y)
        {
            if (x is null || y is null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                return work(x, y);
            }
        }
    }
}