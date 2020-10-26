using System;
using System.Linq;
using System.Text;
using MyClassLibrary.Model;
using MyClassLibrary.Services;

namespace DbOnFile
{
    static class Program
    {
        static void Main(string[] args)
        {
            var db = new PersonOnFile(Encoding.Unicode);
            
            var menu = new MenuService(new string[]
            {
                "1. Add Person",
                "2. List Person",
                "3. Get Person by ID",
                "4. Search Person by name",
                "5. Search Person by age"
            });
            var runing = true;
            string choice;
            while (runing)
            {
                choice = menu.DisplayMainMenu();
                switch (choice)
                {
                    case "1":
                        var idInsert = menu.InputNumber("Input ID:");
                        var nameInsert = menu.InputString("Input name:");
                        var ageInsert = menu.InputNumber("Input age:");
                        db.InsertData(new Person(idInsert, nameInsert, ageInsert));
                        Console.WriteLine("Insert complete");
                        Console.WriteLine("----------------");
                        break;
                    case "2":
                        var persons = db.SelectAllData();
                        persons.Display();
                        break;
                    case "3":
                        var idFromSearch = menu.InputNumber("Input ID:");
                        var personSearchId = db.SelectById(idFromSearch);
                        if (personSearchId is null)
                            Console.WriteLine("ID not found");
                        else
                            personSearchId.Display();
                        Console.WriteLine("----------------");
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "q":
                        runing = false;
                        Console.WriteLine("Sayonara Bye bye");
                        Console.WriteLine("----------------");
                        db.Disconncect();
                        break;
                    default:
                        Console.WriteLine("I don't got it");
                        break;
                }

                Console.Read();
            }
        }

        static void Display(this Person[] persons)
        {
            foreach (var person in persons)
            {
                person.Display();
                Console.WriteLine("----------------");
            }
        }
    }
}