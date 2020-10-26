using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static Linkedint? myLink;
        static DoubleLinkedInt? myDbLinkHead;
        static DoubleLinkedInt? myDbLinkTail;
        
        static void Main(string[] args) {
            var running = true;
            char key;
            while (running) {
                key = RunMenu();
                Console.WriteLine();
                switch (key) {
                    case '1':
                        InputNumberToDoubleLink();
                        break;
                    case '2':
                        Display();
                        break;
                    case 'Q':
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Not in menu");
                        break;
                }
            }
        }

        static char RunMenu() {
            Console.WriteLine("Menu");
            Console.WriteLine("1. Input Number");
            Console.WriteLine("2. Display");
            Console.WriteLine("Q. Quit");
            return Console.ReadKey().KeyChar;
        }

        static void InputNumber() {
            Console.WriteLine("Input number");
            var input = Convert.ToInt32(Console.ReadLine());

            myLink = new Linkedint
            {
                Value = input,
                NextLink = myLink
            };
        }

        static void InputNumberToDoubleLink() {
            Console.WriteLine("Input Number");
            var input = Convert.ToInt32(Console.ReadLine());

            var node = new DoubleLinkedInt
            {
                Vaule = input,
                PreviousLink = myDbLinkTail
            };
            
            if (myDbLinkHead is null) {
                myDbLinkHead = myDbLinkTail = node;
            }
            else {
                myDbLinkTail!.NextLink = node;
                myDbLinkTail = node;
            }
            
        }

        static void Display() {
            if (myDbLinkHead is null) {
                Console.WriteLine("Don't have data");
                return;
            }
            
            Console.WriteLine("Result from head is");
            var currentNode = myDbLinkHead;
            while (currentNode != null) {
                Console.Write($"{currentNode.Vaule} ");
                currentNode = currentNode.NextLink;
            }
            Console.WriteLine();
            Console.WriteLine("Result from tail is");
            currentNode = myDbLinkTail;
            while (currentNode != null) {
                Console.Write($"{currentNode.Vaule} ");
                currentNode = currentNode.PreviousLink;
            }
            Console.WriteLine();
        }
    }

    class Linkedint
    {
        public int Value { get; set; }
        public Linkedint? NextLink { get; set; }
    }

    class DoubleLinkedInt
    {
        public int Vaule { get; set; }
        public DoubleLinkedInt? NextLink { get; set; }
        public DoubleLinkedInt? PreviousLink { get; set; }
    }
}