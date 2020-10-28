using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MyClassLibrary.Enums;
using MyClassLibrary.Extensions;

namespace DbOnFile
{
    public class IndexOnFile<TK> : IComparer<TK>
    {
        public MyNode<TK> Header { get; set; } = new MyNode<TK>();
        public string FileName { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;
        public int BlockSize { get; set; }
        public int KeySize { get; set; }
        public Encoding Encode { get; set; } = default!;
        
        readonly Func<TK,TK, int> compareWorker;
        readonly Func<TK, Encoding, byte[]> convertWorker;
        readonly Func<byte[], Encoding, TK> transformWorker;
        FileStream indexStream = null!;

        public IndexOnFile(Func<TK, TK, int> compareWorker,Func<TK, Encoding, byte[]> convertWorker,Func<byte[], Encoding, TK> transformWorker)
        {
            this.compareWorker = compareWorker;
            this.convertWorker = convertWorker;
            this.transformWorker = transformWorker;
        }
        public IndexOnFile(string folder, string fileName,int keySize, Encoding encode, Func<TK,TK, int> compareWorker, 
            Func<byte[], Encoding, TK> transformWorker, Func<TK, Encoding, byte[]> convertWorker) : this(compareWorker, convertWorker, transformWorker)
        {
            SetIndex(folder, fileName, keySize, encode);
        }

        public void SetIndex(string folder, string fileName, int keySize, Encoding encode)
        {
            Folder = folder;
            FileName = fileName;
            KeySize = keySize;
            Encode = encode;
            Directory.CreateDirectory(Folder);
            indexStream = File.Open(Path.Combine(Folder, FileName), FileMode.OpenOrCreate);
            BlockSize = GetBlockSize;
            
            WarmUpIndexHeader();
        }

        int GetBlockSize => KeySize
                            + sizeof(int) //offset
                            + sizeof(int) //ValueOrder
                            + sizeof(int) //LeftOffset
                            + sizeof(int) //RightOffset
                            + sizeof(int); //CenterOffset

        int GetLastOrder => (int)(indexStream.Position / BlockSize);
        int GetIndexOffset(int num)  => num * BlockSize;

        public int Compare(TK x, TK y)
        {
            if (x is null || y is null)
                throw new ArgumentNullException();
            else
                return compareWorker(x, y);
        }

        void WarmUpIndexHeader()
        {
            if (indexStream.Length > 0)
            {
                Header = GetNode(0);
            }
        }
        

        public bool Indexing(TK key, int valueOrder)
        {
            var (side, node) = FindNode(key, Header);
            var insert = InsertValue(side, node, key, valueOrder);
            return insert;
        }

        (NodeSide, MyNode<TK>?) FindNode(TK key, MyNode<TK>? tree)
        {
            var result = (NodeSide.Null, tree);
            if (tree is null)
            {
                return result;
            }
            else if (Compare(tree.Key, key) < 0)
            {
                if (tree.LeftRef == -1) // left is null
                    return (NodeSide.Left, tree);
                else
                    result = FindNode(key, GetNode(tree.LeftRef));
            }
            else if (Compare(tree.Key, key) > 0)
            {
                if (tree.RightRef == -1) // right is null
                    return (NodeSide.Right, tree);
                else
                    result = FindNode(key, GetNode(tree.RightRef));
            }
            else
            {
                if (tree.CenterRef == -1) // center is null
                    return (NodeSide.Center, tree);
                else
                    result = FindNode(key, GetNode(tree.CenterRef));
            }

            return result;
        }
        
        bool InsertValue(NodeSide side, MyNode<TK>? node, TK key, int valueOrder)
        {
            if (side == NodeSide.Null)
            {
                node = new MyNode<TK>(key, 0, valueOrder);
                PutNode(node);
                return true;
            }
            else if (side == NodeSide.Center && node != null)
            {
                
            }
            else if (side == NodeSide.Left && node != null)
            {
                node.LeftRef = CreateNewNode(NodeSide.Left, key, valueOrder);
                UpdateNodeSide(node.LeftRef, NodeSide.Left, node.Offset);
                return true;
            }
            else if (side == NodeSide.Right && node != null)
            {
                node.RightRef = CreateNewNode(NodeSide.Right, key, valueOrder);
                UpdateNodeSide(node.RightRef, NodeSide.Right, node.Offset);
                return true;
            }

            return false;
        }

        int CreateNewNode(NodeSide side, TK key, int valueOrder)
        {
            var order = GetLastOrder;
            var sideNode = new MyNode<TK>(key, order, valueOrder);
            return PutNode(sideNode);
        }
        
        public int[] SearchNode(TK key)
        {
            var node = SearchInTree(Header, key);
            if (node is null)
                return new int[] { };
            else
                return GetValueOrderInSameKey(node);
        }

        MyNode<TK>? SearchInTree(MyNode<TK>? node, TK key)
        {
            if (node is null)
                return null;
            else if (Compare(node.Key, key) == 0)
                return node;
            else if (Compare(node.Key, key) < 0) {
                return SearchInTree(GetNode(node.LeftRef), key);
            }
            else {
                return SearchInTree(GetNode(node.RightRef), key);
            }
        }

        int[] GetValueOrderInSameKey(MyNode<TK> node)
        {
            var result = new int[] { };
            return result;
        }

        #region Get Put Convert
        int PutNode(MyNode<TK> node)
        {
            var data = ToByte(node);
            indexStream.Seek(0, SeekOrigin.End);
            indexStream.Write(data, 0, BlockSize);
            return (int) (indexStream.Position / BlockSize);
        }

        int PutNode(MyNode<TK> node, int order)
        {
            var data = ToByte(node);
            var indexOffset = GetIndexOffset(order);
            indexStream.Seek(indexOffset, SeekOrigin.Begin);
            indexStream.Write(data, 0, BlockSize);
            return (int) (indexStream.Position / BlockSize);
        }

        void UpdateNodeSide(int sideData, NodeSide side, int order)
        {
            var nodeOffset = side == NodeSide.Center ? BlockSize - sizeof(int)
                : side == NodeSide.Right ? BlockSize - (sizeof(int) * 2)
                : BlockSize - (sizeof(int) * 3);

            var data = sideData.ToBytes();
            var indexOffset = GetIndexOffset(order) + nodeOffset;
            indexStream.Seek(indexOffset, SeekOrigin.Begin);
            indexStream.Write(data, 0, sizeof(int));
        }

        MyNode<TK> GetNode(int num)
        {
            var data = new byte[BlockSize];
            var indexOffset = GetIndexOffset(num);
            indexStream.Seek(indexOffset, SeekOrigin.Begin);
            indexStream.Read(data, 0, BlockSize);
            return ToNode(data);
        }

        TK GetNodeKey(int indexOffset)
        {
            var data = new byte[KeySize];
            indexStream.Seek(indexOffset, SeekOrigin.Begin);
            indexStream.Read(data, 0, KeySize);
            return transformWorker(data, Encode);
        }

        byte[] ToByte(MyNode<TK> node)
        {
            return (new[]
            {
                convertWorker(node.Key, Encode),
                node.Offset.ToBytes(),
                node.ValueOrder.ToBytes(),
                node.LeftRef.ToBytes(),
                node.RightRef.ToBytes(),
                node.CenterRef.ToBytes()
            }).Combine();
        }

        MyNode<TK> ToNode(byte[] data)
        {
            var key = data.Seperate(0, KeySize);
            var offset = data.Seperate(KeySize, sizeof(int));
            var valueOrder = data.Seperate(KeySize + sizeof(int), sizeof(int));
            var leftOffset = data.Seperate(KeySize + sizeof(int) + sizeof(int), sizeof(int));
            var rightOffset = data.Seperate(BlockSize - (sizeof(int) + sizeof(int)), sizeof(int));
            var centerOffset = data.Seperate(BlockSize - sizeof(int), sizeof(int));
            
            var result = new MyNode<TK>();
            result.Key = transformWorker(key, Encode);
            result.ValueOrder = valueOrder.ToInt32();
            result.LeftRef = leftOffset.ToInt32();
            result.RightRef = rightOffset.ToInt32();
            result.CenterRef = centerOffset.ToInt32();

            return result;
        }
        #endregion
    }

    public class MyNode<TK>
    {
        public TK Key { get; set; } = default!;

        public int Offset { get; set; }
        public int ValueOrder { get; set; }
        public int LeftRef { get; set; } = -1;
        public int RightRef { get; set; } = -1;
        public int CenterRef { get; set; } = -1;

        public MyNode()
        {
        }

        public MyNode(TK key, int offset)
        {
            Key = key;
            Offset = offset;
        }

        public MyNode(TK key, int offset, int valueOrder)
        {
            Key = key;
            Offset = offset;
            ValueOrder = valueOrder;
        }

    }

}