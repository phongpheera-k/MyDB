using System;
using System.Text;

namespace MyClassLibrary.Extensions
{
    public static class FileBinaryExtensions
    {
        public static byte[] ToBytes(this int input)
        {
            return BitConverter.GetBytes(input);
        }

        // public static byte[] ToBytes(this int? input)
        // {
        //     if (expr)
        //     {
        //         
        //     }
        // }

        public static byte[] ToBytes(this string input, Encoding encoding)
        {
            return encoding.GetBytes(input);
        }

        public static int ToInt32(this byte[] input)
        {
            return BitConverter.ToInt32(input);
        }

        public static string ToStringWithEncoding(this byte[] input, Encoding encoding)
        {
            return encoding.GetString(input);
        }
    }
}