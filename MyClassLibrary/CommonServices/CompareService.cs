using System;

namespace MyClassLibrary.CommonServices
{
    public static class CompareService
    {
        public static readonly Func<int, int, int> CompareInt = (x, y) =>
        {
            if (x > y)
                return 1;
            else if (x < y)
                return -1;
            else
                return 0;
        };
        
        public static readonly Func<string, string, int> CompareString = string.Compare;
    }
}