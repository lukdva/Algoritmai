using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ConsoleApplication2
{
    class sarasas
    {
        public void doSarasas()
        {
            // atidaromas failas
            
            Console.ReadLine();
            Console.WriteLine("Iveskite duomenu faila");
            string duomenys = Console.ReadLine();
            FileStream fn = new FileStream(duomenys, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fn);
            BinaryReader br = new BinaryReader(fn);

            Console.WriteLine("Iveskite rezultatu faila");
            string rezultatai = Console.ReadLine();
            FileStream fr = new FileStream(rezultatai, FileMode.Create, FileAccess.Write);
            StreamWriter rez = new StreamWriter(fr);

            // Sugeneruojami duomenys
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Iveskite elementu kieki");
            int N = Convert.ToInt32(Console.ReadLine());  // total elements

            sarasas obj = new sarasas();
            obj.generate(br, bw, N);
            obj.print(br, N, 4, rez);

            GlVar.reset();
            sw.Start();
            obj.quickSort(br, bw, 4, (N-1)*12 + 4);
            sw.Stop();

            Console.WriteLine("Laikas - " + sw.Elapsed);
            Console.WriteLine("Operaciju sk. - " + GlVar.cnt);
            obj.print(br, N, 4, rez);

            // Uzdarome faila
            rez.Close();
            fn.Close();
        }
        public void testas()
        {
            int N = 1000;
            FileStream fr = new FileStream("SarasoTestREZ.txt", FileMode.Create, FileAccess.Write);
            StreamWriter rez = new StreamWriter(fr);
            rez.WriteLine("Elementu sk.       Trukme         Operaciju sk.");
            for (int i = 0; i < 10; i++)
            {
                GlVar.reset();

                FileStream fn = new FileStream("SarasoTestDUOM.txt", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fn);
                BinaryReader br = new BinaryReader(fn);

                Stopwatch sw = new Stopwatch();

                sarasas obj = new sarasas();
                obj.generate(br, bw, N);

                sw.Start();
                obj.quickSort(br, bw, 4, (N - 1) * 12 + 4);
                sw.Stop();

                rez.WriteLine(N + "    " + sw.Elapsed + "    " + GlVar.cnt);

                // Uzdarome faila
                fn.Close();
                N = N + 5000;
            }
            rez.Close();
        }
        //--------------------------------------------------------------------------------------------------
        public static Int32 next(BinaryReader br, int i) // gauna kito elemento adresa
        {
            int k = i + 4;
            br.BaseStream.Seek(k, SeekOrigin.Begin);
            GlVar.inc(3);
            return br.ReadInt32();
        }
        public static Int32 current(BinaryReader br, int i) // gauna elemento adresa
        {
            int k = 4 + 12 * i;
            br.BaseStream.Seek(k, SeekOrigin.Begin);
            return br.ReadInt32();
        }
        public static Int32 previous(BinaryReader br, int i) // gauna praeito elemento adresa
        {
            int k = i - 4;
            br.BaseStream.Seek(k, SeekOrigin.Begin);
            GlVar.inc(3);
            return br.ReadInt32();
        }
        public static float getNumber(BinaryReader br, int k)
        {
            br.BaseStream.Seek(k, SeekOrigin.Begin);
            GlVar.inc(2);
            return br.ReadSingle();
        }
        //--------------------------------------------------------------------------------------------------
        // priskiriame i-tajam elementui nauja reiksme
        public static void setNumber(BinaryWriter bw, int k, float value)
        {
            bw.BaseStream.Seek(k, SeekOrigin.Begin);
            bw.Write(value);
            GlVar.inc(2);
        }
        //--------------------------------------------------------------------------------------------------
        public void quickSort(BinaryReader br, BinaryWriter bw, int p, int r)
        {
            if (r != -1 && p != r && p != next(br, r) )
            {
                int q = partition(br, bw, p, r);
                quickSort(br, bw, p, previous(br , q)); // q-1 == previous node
                quickSort(br, bw, next(br, q) , r); // q+1 == next node
                GlVar.inc(3);
            }
            GlVar.inc(1);
        }
        //--------------------------------------------------------------------------------------------------
        public int partition(BinaryReader br, BinaryWriter bw, int p, int r)
        {
            float ch;
            float x = getNumber(br, r); // pakeisti
            int i = previous(br, p);
         
            for (int j = p; j <= previous(br, r); j = next(br, j))
            {
                if (getNumber(br, j) <= x)
                {
                    i = (i == -1) ? p : next(br, i);
                    ch = getNumber(br, i);
                    setNumber(bw, i, getNumber(br, j));
                    setNumber(bw, j, ch);
                    GlVar.inc(5);
                }
                GlVar.inc(1);
            }
            i = (i == -1) ? p : next(br, i);
            ch = getNumber(br, i);
            setNumber(bw, i, getNumber(br, r));
            setNumber(bw, r, ch);
            GlVar.inc(9);
            return i;

        }
        //--------------------------------------------------------------------------------------------------
        public void generate(BinaryReader br, BinaryWriter bw, int N)
        {
            Random rand = new Random();
            Int32 nuoroda = -1;
            bw.Write(nuoroda);
            float pr = (float)rand.NextDouble();
            bw.Write(pr);
            nuoroda = 16;
            bw.Write(nuoroda);
            for (int i = 1; i < N - 1; i++)
            {
                float f = (float)rand.NextDouble();
                nuoroda = 4 + 12 * (i - 1);
                bw.Write(nuoroda);
                bw.Write(f);
                nuoroda = 4 + 12 * (i + 1);
                bw.Write(nuoroda);
            }
            nuoroda = 4 + 12 * (N - 2);
            bw.Write(nuoroda);
            float pb = (float)rand.NextDouble();
            bw.Write(pb);
            // nuoroda = 4 + 12 * (N - 1); rodo i save paskutinis elementas
            nuoroda = -1;
            bw.Write(nuoroda);
        }
        //--------------------------------------------------------------------------------------------------
        public void print(BinaryReader br, int N, Int32 p, StreamWriter rez)
        {
            Int32 adresas = p;
            while (adresas != -1)
            {
                rez.WriteLine(previous(br, adresas) +" <- " +getNumber(br, adresas)+" -> " + next(br,adresas));
                adresas = next(br, adresas);
            }
        }

    }
    
}
