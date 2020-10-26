using System;
using System.Collections.Generic;

namespace DbOnMemory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to DB on memory");
            var personTree = new MyNode<int,Person>(CompareInt, true);
            var nameIndex = new MyNode<string,Person>(CompareString, false);
            var ageIndex = new MyNode<int,Person>(CompareInt, false);
            var runing = true;
            string choice;
            while (runing)
            {
                choice = RunMenu();
                switch (choice)
                {
                    case "1":
                        var idInsert = InputNumber("Enter ID:");
                        var nameInsert = InputString("Enter name:");
                        var ageInsert = InputNumber("Enter age:");

                        var insertResult = personTree.InsertNode(idInsert, new Person(idInsert, nameInsert, ageInsert));
                        if (!insertResult) Console.WriteLine("Can not input duplicate key(ID)");
                        nameIndex.InsertNode(nameInsert, new Person(idInsert, nameInsert, ageInsert));
                        ageIndex.InsertNode(ageInsert, new Person(idInsert, nameInsert, ageInsert));
                        break;
                    case "2":
                        var persons = personTree.GetOrderList();
                        foreach (var person in persons)
                        {
                            DisplayPerson(person.Value);
                        }
                        break;
                    case "3":
                        var idSearch = InputNumber("Enter ID:");
                        var (idFound, personResult) = personTree.SearchNode(idSearch);
                        if (idFound && personResult != null)
                            DisplayPerson(personResult.Value);
                        break;
                    case "4":
                        var nameSearch = InputString("Enter name:");
                        var (nameFound, nameResult) = nameIndex.SearchNode(nameSearch);
                        if (nameFound && nameResult != null)
                            foreach (var nameValue in nameResult.ValueArray)
                            {
                                DisplayPerson(nameValue);
                            }
                        break;
                    case "5":
                        var ageSearch = InputNumber("Enter age:");
                        var (ageFound, ageResult) = ageIndex.SearchNode(ageSearch);
                        if (ageFound && ageResult != null)
                            foreach (var ageValue in ageResult.ValueArray)
                            {
                                DisplayPerson(ageValue);
                            }
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
        
        static void DisplayPerson(Person person)
        {
            Console.WriteLine($"ID:{person.Id}");
            Console.WriteLine($"Name:{person.Name}");
            Console.WriteLine($"Age:{person.Age}");
        }

        #region CompareLogic
        static readonly Func<int, int, int> CompareInt = (x, y) =>
        {
            if (x > y)
                return 1;
            else if (x < y)
                return -1;
            else
                return 0;
        };

        static readonly Func<string, string, int> CompareString = string.Compare;
        #endregion

        #region Menu
        static string RunMenu()
        {
            Console.WriteLine("-----Menu-----");
            Console.WriteLine("1. Add Person");
            Console.WriteLine("2. List Person");
            Console.WriteLine("3. Get Person by ID");
            Console.WriteLine("4. Search Person by name");
            Console.WriteLine("5. Search Person by age");
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
        #endregion

        #region Util
        static string InputString(string message)
        {
            Console.Write(message);
            var result = Console.ReadLine();
            if (result is null)
            {
                Console.WriteLine("Can not input null");
                result = InputString(message);
            }

            return result;
        }

        static int InputNumber(string message)
        {
            var result = InputString(message);
            while (!IsNumeric(result))
            {
                Console.WriteLine("You must input only numeric");
                result = InputString(message);
            }

            return Convert.ToInt32(result);
        }

        static bool IsNumeric(string input)
        {
            var result = true;
            foreach (var ch in input)
            {
                if (ch < '0' && ch > '9')
                    result = false;
            }

            return result;
        }
        #endregion
    }

    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
    }

    public enum NodeSide
    {
        Left,
        Right,
        Center,
        Null
    }

    public class MyNode<TK,TV> : IComparer<TK>
    {
        public TK Key { get; set; } = default!;
        public TV Value { get; set; } = default!;
        public TV[] ValueArray { get; set; } = new TV[] { };
        public MyNode<TK,TV>? Left { get; set; }
        public MyNode<TK,TV>? Right { get; set; }
        

        readonly Func<TK,TK, int> work;
        readonly bool isKey;

        public MyNode(Func<TK,TK, int> work, bool isKey)
        {
            this.work = work;
            this.isKey = isKey;
        }

        public MyNode(TK key, TV value, TV[] valueArray, Func<TK, TK, int> work, bool isKey) : this(work, isKey)
        {
            Key = key;
            Value = value;
            ValueArray = valueArray;
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

        public bool InsertNode(TK key, TV value)
        {
            var (side, node) = FindNode(key, this);
            return InsertValue(side, node, key, value);
        }

        bool InsertValue(NodeSide side, MyNode<TK, TV>? node, TK key, TV value)
        {
            if (side == NodeSide.Null)
            {
                // Key = key;
                // Value = value;
                // ValueArray = isKey ? new TV[] { } : new [] {value};
                node = isKey
                    ? new MyNode<TK, TV>(key, value, new TV[] { }, work, isKey)
                    : new MyNode<TK, TV>(key, value, new [] { value}, work, isKey);
                return true;
            }
            else if (side == NodeSide.Center && node != null)
            {
                if (node.isKey) return false;
                else
                {
                    var temp = new TV[ValueArray.Length + 1];
                    temp[ValueArray.Length] = value;
                    ValueArray = temp;
                    return true;
                }
            }
            else if (side == NodeSide.Left && node != null)
            {
                node.Left = node.isKey
                    ? new MyNode<TK, TV>(key, value, new TV[] { }, work, isKey)
                    : new MyNode<TK, TV>(key, value, new [] { value}, work, isKey);
                return true;
            }
            else if (side == NodeSide.Right && node != null)
            {
                node.Right = node.isKey
                    ? new MyNode<TK, TV>(key, value, new TV[] { }, work, isKey)
                    : new MyNode<TK, TV>(key, value, new [] { value}, work, isKey);
                return true;
            }

            return false;
        }

        (NodeSide, MyNode<TK, TV>?) FindNode(TK key, MyNode<TK,TV>? tree)
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

        public MyNode<TK, TV>[] GetOrderList()
        {
            return InOrder(this);
        }
        
        MyNode<TK, TV>[] InOrder(MyNode<TK, TV>? node)
        {
            var result = new MyNode<TK, TV>[] { };
            
            var isMidRead = false;
            var isLeftRead = false;
            var isRightRead = false;
            if (node != null)
            {
                while ((!isLeftRead || node.Left != null)
                       && !isMidRead
                       && (!isRightRead || node.Right != null))
                {
                    if (node.Left != null && !isLeftRead) {
                        InOrder(node.Left);
                        isLeftRead = true;
                    }

                    if (!isMidRead) {
                        result = AddInOrderResult(result, node);
                        isMidRead = true;
                    }

                    if (node.Right != null && !isRightRead) {
                        InOrder(node.Right);
                        isRightRead = true;
                    }
                }
            }

            return result;
        }

        MyNode<TK, TV>[] AddInOrderResult(MyNode<TK, TV>[] result, MyNode<TK, TV> value)
        {
            var temp = new MyNode<TK, TV>[result.Length + 1];
            temp[result.Length] = value;
            return temp;
        }

        public (bool, MyNode<TK, TV>?) SearchNode(TK key)
        {
            return SearchInTree(this, key);
        }

        (bool, MyNode<TK, TV>?) SearchInTree(MyNode<TK, TV>? node, TK key)
        {
            if (node is null)
                return (false, node);
            else if (Compare(node.Key,key) == 0)
                return (true, node);
            else if (Compare(node.Key, key) < 0) {
                return SearchInTree(node.Left, key);
            }
            else {
                return SearchInTree(node.Right, key);
            }
        }
    }
}