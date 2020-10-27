using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using MyClassLibrary.Enums;
using MyClassLibrary.Extensions;

namespace DbOnFile
{
    public class IndexOnFile<TK, TV> : IComparer<TK>
    {
        public TK Key { get; set; }
        public TV Value { get; set; }
        
        public MyNode<TK, TV>? Head { get; set; }
        public string FileName { get; set; }
        public string Folder { get; set; }
        public int BlockSize { get; set; }
        public int KeySize { get; set; }
        public Encoding Encode { get; set; }
        
        readonly Func<TK,TK, int> compareWorker;
        readonly Func<TK, Encoding, byte[]> convertWorker;
        readonly Func<byte[], Encoding, TK> transformWorker;

        public IndexOnFile(Func<TK, TK, int> compareWorker,Func<TK, Encoding, byte[]> convertWorker,Func<byte[], Encoding, TK> transformWorker)
        {
            this.compareWorker = compareWorker;
            this.convertWorker = convertWorker;
            this.transformWorker = transformWorker;
        }
        public IndexOnFile(string folder, string fileName,int keySize, Encoding encode, Func<TK,TK, int> compareWorker, 
            Func<byte[], Encoding, TK> transformWorker, Func<TK, Encoding, byte[]> convertWorker) : this(compareWorker, convertWorker, transformWorker)
        {
            Folder = folder;
            FileName = fileName;
            KeySize = keySize;
            Encode = encode;
            Directory.CreateDirectory(Folder);
            BlockSize = GetBlockSize;
        }

        public void SetIndex(string folder, string fileName, int keySize, Encoding encode)
        {
            Folder = folder;
            FileName = fileName;
            KeySize = keySize;
            Encode = encode;
            Directory.CreateDirectory(Folder);
            BlockSize = GetBlockSize;
        }

        int GetBlockSize => KeySize
                            + sizeof(int) //ValueOrder
                            + sizeof(int) //LeftOffset
                            + sizeof(int) //RightOffset
                            + sizeof(int); //CenterOffset
        int GetIndexOffset(int num)  => num * BlockSize;

        public int Compare(TK x, TK y)
        {
            if (x is null || y is null)
                throw new ArgumentNullException();
            else
                return compareWorker(x, y);
        }
        
        

        public bool Indexing(TK key, int valueOrder)
        {
            var (side, node) = FindNode(key, Head);
            return InsertValue(side, node, key);
        }

        (NodeSide, MyNode<TK, TV>?) FindNode(TK key, MyNode<TK, TV>? tree)
        {
            var result = (NodeSide.Null, tree);
            if (tree is null)
            {
                return result;
            }
            else if (Compare(tree.Key, key) < 0)
            {
                if (tree.Left is null)
                    return (NodeSide.Left, tree);
                else
                    result = FindNode(key, tree.Left);
            }
            else if (Compare(tree.Key, key) > 0)
            {
                if (tree.Right is null)
                    return (NodeSide.Right, tree);
                else
                    result = FindNode(key, tree.Right);
            }
            else
                return (NodeSide.Center, tree);

            return result;
        }
        
        bool InsertValue(NodeSide side, MyNode<TK, TV>? node, TK key)
        {
            if (side == NodeSide.Null)
            {
                node = new MyNode<TK, TV>(compareWorker);
                return true;
            }
            else if (side == NodeSide.Center && node != null)
            {
                if (node.Center is null) return false;
                else
                {
                    node.Center = new MyNode<TK, TV>(compareWorker);
                    return true;
                }
            }
            else if (side == NodeSide.Left && node != null)
            {
                node.Left = new MyNode<TK, TV>(compareWorker);
                return true;
            }
            else if (side == NodeSide.Right && node != null)
            {
                node.Right = new MyNode<TK, TV>(compareWorker);
                return true;
            }

            return false;
        }

        int PutNode(MyNode<TK, TV> node)
        {
            var data = ToByte(node);
            var result = default(int);
            using (FileStream fs = File.Open(Path.Combine(Folder, FileName), FileMode.OpenOrCreate))
            {
                fs.Seek(0, SeekOrigin.End);
                result = (int) fs.Position;
                fs.Write(data, 0, BlockSize);
            }
            return result;
        }

        MyNode<TK, TV> GetNode(int num)
        {
            var data = new byte[BlockSize];
            var indexOffset = GetIndexOffset(num);
            using (FileStream fs = File.Open(Path.Combine(Folder, FileName), FileMode.OpenOrCreate))
            {
                fs.Seek(indexOffset, SeekOrigin.Begin);
                fs.Read(data, 0, BlockSize);
            }
            return ToNode(data);
        }

        byte[] ToByte(MyNode<TK, TV> node)
        {
            return (new[]
            {
                convertWorker(node.Key, Encode),
                node.ValueOrder.ToBytes(),
                node.LeftOffset.ToBytes(),
                node.RightOffset.ToBytes(),
                node.CenterOffset.ToBytes()
            }).Combine();
        }

        MyNode<TK, TV> ToNode(byte[] data)
        {
            var key = data.Seperate(0, KeySize);
            var valueOrder = data.Seperate(KeySize, sizeof(int));
            var leftOffset = data.Seperate(KeySize + sizeof(int), sizeof(int));
            var rightOffset = data.Seperate(KeySize + sizeof(int) + sizeof(int), sizeof(int));
            var centerOffset = data.Seperate(BlockSize - sizeof(int), sizeof(int));
            
            var result = new MyNode<TK, TV>(compareWorker);
            result.Key = transformWorker(key, Encode);
            result.ValueOrder = valueOrder.ToInt32();
            result.LeftOffset = leftOffset.ToInt32();
            result.RightOffset = rightOffset.ToInt32();
            result.CenterOffset = centerOffset.ToInt32();

            return result;
        }
    }

    public class MyNode<TK, TV> : IComparer<TK>
    {
        public TK Key { get; set; } = default!;
        public TV Value { get; set; } = default!;
        public MyNode<TK, TV>? Left { get; set; }
        public MyNode<TK, TV>? Right { get; set; }
        public MyNode<TK, TV>? Center { get; set; }

        public int ValueOrder { get; set; }
        public int LeftOffset { get; set; }
        public int RightOffset { get; set; }
        public int CenterOffset { get; set; }

        readonly Func<TK, TK, int> work;

        public MyNode(Func<TK, TK, int> work)
        {
            this.work = work;
        }
        
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