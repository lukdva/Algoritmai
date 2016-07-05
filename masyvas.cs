using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ConsoleApplication2
{
    public static class GlVar
    {
        public static int cnt = 0;
        public static void inc(int k)
        {
            cnt = cnt + k;
        }
        public static void reset()
        {
            cnt = 0;
        }
    }
    class masyvas
    {
        public void doMasyvas()
        {
            GlVar.reset();
            // atidaromas failas
            Console.WriteLine("Iveskite duomenu faila");
            string duomenys = Console.ReadLine();
            duomenys = Console.ReadLine();

            FileStream fn = new FileStream(duomenys, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fn);
            BinaryReader br = new BinaryReader(fn);

            Console.WriteLine("Iveskite rezultatu faila");
            string rezultatai = Console.ReadLine();
            FileStream fr = new FileStream(rezultatai, FileMode.Create, FileAccess.Write);
            StreamWriter rez = new StreamWriter(fr);

            Stopwatch sw = new Stopwatch();
            // Sugeneruojami duomenys
            Console.WriteLine("Iveskite elementu kieki");
            int N = Convert.ToInt32(Console.ReadLine()); ; // total elements
            
            masyvas obj = new masyvas();
            obj.generate(br, bw, N);
            obj.print(br, N, rez);

            GlVar.reset();
            sw.Start();
            obj.quickSort(br, bw, 0, N-1);
            sw.Stop();

            Console.WriteLine("Laikas - " + sw.Elapsed);
            Console.WriteLine("Operaciju sk. - " + GlVar.cnt);
            obj.print(br, N, rez);

            // Uzdarome faila
            fn.Close();
            rez.Close();
            fr.Close();
        }
        public void testas()
        {
            int N = 1000;
            FileStream fr = new FileStream("MasyvoTestREZ.txt", FileMode.Create, FileAccess.Write);
            StreamWriter rez = new StreamWriter(fr);
            rez.WriteLine("Elementu sk.       Trukme         Operaciju sk.");
            for (int i = 0; i < 10; i++)
            {
                GlVar.reset();

                FileStream fn = new FileStream("MasyvoTestDUOM.txt", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fn);
                BinaryReader br = new BinaryReader(fn);

                Stopwatch sw = new Stopwatch();

                masyvas obj = new masyvas();
                obj.generate(br, bw, N);

                sw.Start();
                obj.quickSort(br, bw, 0, N - 1);
                sw.Stop();

                rez.WriteLine(N + "    " + sw.Elapsed + "    " + GlVar.cnt);

                // Uzdarome faila
                fn.Close();
                N = N + 5000;
            }
            rez.Close();
        }

        // nuskaitome i-taji elementa is binarinio failo (zinome, kad visi elementai uzima po 4 bitus, todel i-tojo reiksmes pradzia bus i*4)
        public static float getNumber(BinaryReader br, int i)
        {
            int k = (i) * 4;
            br.BaseStream.Seek(k, SeekOrigin.Begin);
            GlVar.inc(3);
            return br.ReadSingle();
        }

        // priskiriame i-tajam elementui nauja reiksme
        public static void setNumber(BinaryWriter bw, int i, float value)
        {
            int k = (i) * 4;
            bw.BaseStream.Seek(k, SeekOrigin.Begin);
            bw.Write(value);
            GlVar.inc(3);
        }
        public void quickSort(BinaryReader br, BinaryWriter bw, int p, int r)
        {
            if (p < r)
            {
                int q = partition(br, bw, p, r);
                quickSort(br, bw, p, q - 1);
                quickSort(br, bw, q + 1, r);
                GlVar.inc(3);
            }
            GlVar.inc(1);
        }
        public int partition(BinaryReader br, BinaryWriter bw, int p, int r)
        {
            float ch;                               
            float x = getNumber(br, r);            
            int i = p - 1;                         
            
            for (int j = p; j <= r - 1; j++)        
            {
                if (getNumber(br, j) <= x)
                {
                    i++;
                    ch = getNumber(br, i);
                    setNumber(bw, i, getNumber(br, j));
                    setNumber(bw, j, ch);
                    GlVar.inc(5);
                }
                GlVar.inc(1);
            }
            ch = getNumber(br, i + 1);
            setNumber(bw, i + 1, getNumber(br, r));
            setNumber(bw, r, ch);
            GlVar.inc(8);
            return i + 1;
        }
        public void generate(BinaryReader br, BinaryWriter bw, int N)
        {
            Random rand = new Random();
            for (int i = 0; i < N; i++)
            {
                float f = (float)rand.NextDouble();
                bw.Write(f);
            }
        }
        public void print(BinaryReader br, int N, StreamWriter rez)
        {
            for (int elmNumber = 0; elmNumber < N; elmNumber++)
                rez.WriteLine("Element " + elmNumber + ": " + getNumber(br, elmNumber));
        }
    }
}
