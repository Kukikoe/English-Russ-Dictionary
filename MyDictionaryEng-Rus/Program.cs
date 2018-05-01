using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MyDictionaryEng_Rus
{
    class Program
    {
        static void Main(string[] args)
        {
            string english = String.Empty;
            string russ = String.Empty;
            string str = String.Empty;
            string word = String.Empty;
            bool exit = false;
            DictSerializ dictionary = new DictSerializ("words.json");        
            while (!exit)
            {
                Console.WriteLine("Enter the wort", russ);
                russ = Console.ReadLine();
                if (dictionary.ContainsKey(russ))
                {
                    Console.WriteLine("{0} - {1}", russ, dictionary.GetTranslate(russ));
                }
                else
                {
                    Console.WriteLine("У словаря нет перевода этого слова. Хотите добавить перевод? (да - нажмите 1, нет - нажмите 2) \nДля выхода из программы нажмите 3.");
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.D1:
                            Console.WriteLine("Напишите перевод для слова: \n{0}", russ);
                            english = Console.ReadLine();
                            dictionary.Add(russ, english);
                            Console.Clear();
                            break;
                        case ConsoleKey.D2:
                            exit = false;
                            Console.Clear();
                            break;
                        case ConsoleKey.D3:
                            exit = true;
                            break;
                    }
                }
            }
            dictionary.SerializeToFile("words.json");
            Console.ReadKey();
        }
    }
}
