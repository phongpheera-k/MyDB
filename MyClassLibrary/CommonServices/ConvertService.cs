using System;
using System.Dynamic;
using System.Text;
using MyClassLibrary.Extensions;

namespace MyClassLibrary.CommonServices
{
    public static class ConvertService
    {
        public static readonly Func<int, Encoding, byte[]> ConvertInt = (i, e) => i.ToBytes();
        public static readonly Func<string, Encoding, byte[]> ConvertString = (s, e) => s.ToBytes(e);

        public static readonly Func<byte[], Encoding, int> TransformInt = (b, e) => b.ToInt32();
        public static readonly Func<byte[], Encoding, string> TransfromString = (b, e) => b.ToStringWithEncoding(e);
    }
}