using System;

namespace MyClassLibrary.Services
{
    public class MenuService
    {
        public string[] MainMenu { get; set; }

        public MenuService(string[] mainMenu)
        {
            MainMenu = mainMenu;
        }

        public string DisplayMainMenu()
        {
            DisplayEndOfLine();
            Console.WriteLine("-------Menu-------");
            foreach (var menu in MainMenu)
            {
                Console.WriteLine(menu);
            }
            Console.WriteLine("Q. Quit");
            DisplayEndOfLine();
            Console.Write("Choose:");
            return RetryChoose();
        }

        public void DisplayEndOfLine()
        {
            Console.WriteLine("------------------");
        }

        string RetryChoose()
        {
            var result = Console.ReadLine();
            while (result is null)
            {
                Console.Write("Please, Choose:");
                result = Console.ReadLine();
            }
            return result;
        }
        
        public string InputString(string message)
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
        
        public int InputNumber(string message)
        {
            var result = InputString(message);
            while (!IsNumeric(result))
            {
                Console.WriteLine("You must input only numeric");
                result = InputString(message);
            }

            return Convert.ToInt32(result);
        }
        
        bool IsNumeric(string input)
        {
            var result = true;
            foreach (var ch in input)
            {
                if (ch < '0' && ch > '9')
                    result = false;
            }

            return result;
        }
    }
}