using System;

namespace BinaryTree
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Welcome to BinaryTree");
            MyBinaryTree headerKey = new MyBinaryTree();// = CreateMoqTreeData();
            var runing = true;
            string choice;
            while (runing)
            {
                choice = RunMenu();
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter number:");
                        var inputInsert = Console.ReadLine();
                        headerKey = AddTree(headerKey, Convert.ToInt32(inputInsert));
                        break;
                    case "2":
                        Console.Write("Result is : ");
                        InOrder(headerKey);
                        Console.WriteLine();
                        break;
                    case "3":
                        Console.Write("What's number do you want to searh : ");
                        var inputSearch = Console.ReadLine();
                        Console.WriteLine(
                            $"Search result is : {SearchNode(headerKey, Convert.ToInt32((inputSearch)))}");
                        break;
                    case "q":
                        runing = false;
                        break;
                    default:
                        Console.WriteLine("I don't got it");
                        break;
                }
            }
        }

        static string RunMenu()
        {
            Console.WriteLine("Menu");
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. List Item");
            Console.WriteLine("3. Search");
            Console.WriteLine("Q. Quit");
            Console.Write("Choose:");
            return RetryChoose();
        }

        static string RetryChoose()
        {
            var result = Console.ReadLine();
            while (result is null)
            {
                Console.Write("Please, Choose:");
                result = Console.ReadLine();
            }
            return result;
        }

        static MyBinaryTree CreateMoqTreeData()
        {
            return new MyBinaryTree
            {
                Value = 3,
                Frequency = 1,
                LeftTree = NewBinaryTree(2),
                RightTree = new MyBinaryTree
                {
                    Value = 10,
                    Frequency = 1,
                    LeftTree = NewBinaryTree(7),
                    RightTree = null
                }
            };
        }

        static MyBinaryTree AddTree(MyBinaryTree? headerKey, int data)
        {
            if (headerKey is null)
                return NewBinaryTree(data);
            else
            {
                var key = FindNode(headerKey, data);
                if (key != null)
                    InsertData(key, data);
                return headerKey;
            }
        }

        static MyBinaryTree? FindNode(MyBinaryTree? headerKey, int data)
        {
            var traversalKey = headerKey;
            var repeat = true;
            while (repeat)
            {
                if (traversalKey is null) {
                    traversalKey = NewBinaryTree(data);
                    repeat = false; //return traversalKey
                }
                else if (traversalKey.Value == data) {
                    return traversalKey;
                }
                else if (data < traversalKey.Value) {
                    //data < value
                    if (traversalKey.LeftTree is null)
                        repeat = false;
                    else
                        traversalKey = traversalKey.LeftTree;
                }
                else {
                    //data > value
                    if (traversalKey.RightTree is null)
                        repeat = false;
                    else
                        traversalKey = traversalKey.RightTree;
                }
            }

            return traversalKey;
        }

        static void InsertData(MyBinaryTree node, int data)
        {
            if (node.Value == data) {
                node.Frequency++;
            }
            else if (data < node.Value) { //data < value
                node.LeftTree = NewBinaryTree(data);
            }
            else { //data > value
                node.RightTree = NewBinaryTree(data);
            }
        }

        static MyBinaryTree NewBinaryTree(int value) //, MyBinaryTree? leftTree, MyBinaryTree? rightTree)
            => new MyBinaryTree
            {
                Value = value,
                Frequency = 1,
                LeftTree = null, //leftTree,
                RightTree = null //rightTree
            };

        static void InOrder(MyBinaryTree? node)
        {
            var isMidRead = false;
            var isLeftRead = false;
            var isRightRead = false;
            if (node != null)
            {
                while ((!isLeftRead || node.LeftTree != null)
                       && !isMidRead
                       && (!isRightRead || node.RightTree != null))
                {
                    if (node.LeftTree != null && !isLeftRead) {
                        InOrder(node.LeftTree);
                        isLeftRead = true;
                    }

                    if (!isMidRead) {
                        DisplayItem(node);
                        isMidRead = true;
                    }

                    if (node.RightTree != null && !isRightRead) {
                        InOrder(node.RightTree);
                        isRightRead = true;
                    }
                }
            }
        }

        static void DisplayItem(MyBinaryTree node)
        {
            for (int i = 1; i <= node.Frequency; i++) {
                Console.Write($"{node.Value} ");
            }
        }

        static bool SearchNode(MyBinaryTree? node, int? data)
        {
            if (node is null)
                return false;
            else if (node.Value == data)
                return true;
            else if (data < node.Value) {
                return SearchNode(node.LeftTree, data);
            }
            else {
                return SearchNode(node.RightTree, data);
            }
        }
    }

    class MyBinaryTree
    {
        public int Value { get; set; }
        public int Frequency { get; set; }
        public MyBinaryTree? LeftTree { get; set; }
        public MyBinaryTree? RightTree { get; set; }
    }
}