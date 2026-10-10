using System;
using System.Text;
using KindergartenManager.Service;
using KindergartenManager.Repository;
using KindergartenManager.View;

namespace KindergartenManager
{
    class Program
    {
        static void Main(string[] args)
        {
  
            Console.OutputEncoding = Encoding.Unicode ;
            Console.InputEncoding = Encoding.Unicode;

            Services services = new Services();

            ConsoleView view = new ConsoleView(services);

            view.ShowMainMenu();
        }
    }
}