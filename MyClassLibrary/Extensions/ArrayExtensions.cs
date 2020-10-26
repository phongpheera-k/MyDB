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
    }
}