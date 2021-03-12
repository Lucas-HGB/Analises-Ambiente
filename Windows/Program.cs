using System;
using ClassLibrary;

namespace Main
{
    class Program
    {

        static int Print_Menu()
        {
            int main_menu = 0;
            Console.WriteLine("1 - Discos");
            Console.WriteLine("2 - Rede");
            Console.WriteLine("3 - Host");
            Console.WriteLine("4 - Uso de Memória");
            Console.WriteLine("5 - Uso de CPU");
            Console.WriteLine("6 - Logs");
            Console.WriteLine("7 - MySQL");
            Console.WriteLine("99 - Sair");
            try
            {
                main_menu = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException) { Console.Clear(); Console.WriteLine("Please insert numeric value!"); }
            return main_menu;
        }


        static void Main(string[] args)
        {
            Hardware infra = new Hardware();
            Sistema sys = new Sistema();
            bool exit = false;
            do
            {
                int main_menu = Print_Menu();
                switch (main_menu)
                {
                    case 1:
                        Console.Clear();
                        infra.list_disks();
                        // Disks
                        break;
                    case 2:
                        Console.Clear();
                        sys.rede();
                        break;
                    case 3:
                        Console.Clear();
                        infra.host();
                        break;
                    case 4:
                        Console.Clear();
                        // Memory Usage
                        break;
                    case 5:
                        Console.Clear();
                        // CPU Usage
                        break;
                    case 6:
                        // Logs
                        Console.Clear();
                        Console.WriteLine("1 - Aplicação");
                        Console.WriteLine("2 - Sistema");
                        Console.WriteLine("3 - Segurança");
                        Console.WriteLine("4 - Hardware");
                        int logtype = Convert.ToInt32(Console.ReadLine());
                        Console.Clear();
                        switch (logtype)
                        {
                            case 1:
                                Console.WriteLine("--- Application Logs ---");
                                sys.readlogs("application");
                                break;
                            case 2:
                                Console.WriteLine("--- System Logs ---");
                                sys.readlogs("system");
                                break;
                            case 3:
                                Console.WriteLine("--- Security Logs ---");
                                sys.readlogs("security");
                                break;
                            case 4:
                                Console.WriteLine("--- Hardware Logs ---");
                                sys.readlogs("hardware");
                                break;
                        }
                        break;
                    case 7:
                        MySQL banco = new MySQL();
                        banco.Main();
                        break;
                    case 99:
                        exit = true;
                        break;
                    default:
                        break;
                }
            } while (!exit);
        }
    }
}