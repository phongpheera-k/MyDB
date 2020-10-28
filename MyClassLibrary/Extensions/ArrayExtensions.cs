using System;

namespace MyClassLibrary.Extensions
{
    public static class ArrayExtensions
    {
        public static byte[] Combine(this byte[][] array)
        {
            if (array.Length > 0)
            {
                var arrayTemp = array[0];
                for (var i = 1; i < array.Length; i++)
                {
                    var destinationIndex = arrayTemp.Length;
                    var previousArray = arrayTemp;
                    arrayTemp = new byte[arrayTemp.Length + array[i].Length];
                    Array.Copy(previousArray, 0, arrayTemp, 0, previousArray.Length);
                    Array.Copy(array[i], 0, arrayTemp, destinationIndex, array[i].Length);
                }
                return arrayTemp;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] Seperate(this byte[] array, int offSet, int length)
        {
            var result = new byte[length];
            Array.Copy(array, offSet, result, 0, length);
            return result;
        }

        public static int[] Add(this int[] array, int[] data)
        {
            var result = new int[array.Length + data.Length];
            Array.Copy(array, 0, result, 0, array.Length);
            Array.Copy(data, 0, result, array.Length, data.Length);
            return result;
        }

        public static int[] Add(this int[] array, int data)
        {
            var result = new int[array.Length + 1];
            Array.Copy(array, 0, result, 0, array.Length);
            result[array.Length] = data;
            return result;
        }
    }
}