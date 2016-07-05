using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ConsoleApplication2.masyvas;

namespace ConsoleApplication2
{
    class main
    {
        static void Main(string[] args)
        {
            bool neiseiti = true;

            while (neiseiti)
            {
                Console.WriteLine("1 - Masyvas, 2 - Sarasas, 3 - Hash table, 4 - testas, 5 - Exit");
                switch (Console.Read())
                {
                    case '1':
                        masyvas mas = new masyvas();
                        mas.doMasyvas();
                        Console.WriteLine();
                        break;
                    case '2':

                        sarasas sar = new sarasas();
                        sar.doSarasas();
                        Console.WriteLine();
                        break;
                    case '3':
                        paieskaHash paieska = new paieskaHash();
                        paieska.doPaieskaHash();
                        Console.WriteLine();
                        break;
                    case '4':
                        masyvas masT = new masyvas();
                        sarasas sarT= new sarasas();
                        paieskaHash paieskaT = new paieskaHash();
                     //   masT.testas();
                       // sarT.testas();
                      paieskaT.test();
                        break;
                    case '5':
                        neiseiti = false;
                        break;
                }
            }
        }
    }
}
