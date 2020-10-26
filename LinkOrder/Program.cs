using System;

namespace LinkOrder
{
    class Program
    {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            var list = new MyList();
            var repeat = true;
            var choice = string.Empty;
            while (repeat) {
                choice = DisplayMenu();
                switch (choice) {
                    case "1":
                        Console.WriteLine(string.Join(",", list.GetList()));
                        break;
                    case "2":
                        Console.Write("Enter number:");
                        list.Add(Console.ReadLine());
                        break;
                    case "3":
                        Console.Write("Enter number:");
                        list.DeleteOne(Console.ReadLine());
                        break;
                    case "e":
                        repeat = false;
                        break;
                }
            }
        }

        static string DisplayMenu() {
            Console.WriteLine("1.Display List");
            Console.WriteLine("2.Add number");
            Console.WriteLine("3.Delete one number");
            Console.WriteLine("e.Exit");
            Console.Write("Choose:");
            return Console.ReadLine();
        }
    }

    class MyList
    {
        string[] child = new string[] {};

        public string[] GetList() => child;

        string[] CopyArray(string[] source, int start, int end) {
            var result = new string[source.Length];
            for (int i = start; i <= end; i++) {
                result[i] = source[i];
            }
            return result;
        }

        int BinarySearch(string data) {
            if (child.Length < 0) {
                //Out of range
                throw new ArgumentOutOfRangeException();
            }
            else {
                var start = 0;
                var end = child.Length - 1;
                var n = default(int);
                while (start <= end) {
                    n = (start + end) / 2;
                    if (data == child[n]) {
                        return n;
                    }
                    else if (Convert.ToInt32(data) < Convert.ToInt32(child[n])) { // data < child[n]
                        end = n - 1;
                    }
                    else {   // data > child[n]
                        start = n + 1;
                    }

                }

                return start == child.Length ? child.Length : n;
            }
        }

        public void Add(string data) {
            var temp = new string[child.Length + 1];
            
            var n = BinarySearch(data);
            MoveItemInBox(child, ref temp, 0 , n-1, 0, 0);
            temp[n] = data;
            MoveItemInBox(child, ref temp, n, child.Length -1, 0, 1);
            child = temp;
        }

        public void DeleteOne(string data) {
            var temp = new string[child.Length - 1];

            var n = BinarySearch(data);
            MoveItemInBox(child, ref temp, 0, n-1, 0, 0);
            MoveItemInBox(child, ref temp, n+1, child.Length -1, 0, -1);
            child = temp;
        }
        
        void MoveItemInBox(string[] source, ref string[] result, int start, int end, int nSource, int nResult) {
            for (int i = start; i <= end; i++) {
                result[i + nResult] = source[i + nSource];
            }
        }
    }

}