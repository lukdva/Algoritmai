using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ConsoleApplication2
{
    class paieskaHash
    {
        public void doPaieskaHash()
        {
            // atidaromas failas
            
            Console.WriteLine("Iveskite duomenu faila");
            string duomenys = Console.ReadLine();
            duomenys = Console.ReadLine();

            FileStream fn = new FileStream("testh.txt", FileMode.Create, FileAccess.ReadWrite);
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
            int m = (int)(N / 0.6);  //dalinam is apkrovimo koeficiento
            m = (m % 2 == 0) ? m++ : m;

            setSingleCh(bw, (m + 1) * 21, 'd'); // prapleciam stream'a
            paieskaHash obj = new paieskaHash();
            obj.generator(bw, br, N, m);

            GlVar.reset();
            sw.Start();
            obj.surastiVienodus(bw, br, m,rez);
            sw.Stop();

            Console.WriteLine("Laikas - " + sw.Elapsed);
            Console.WriteLine("Operaciju sk. - " + GlVar.cnt);
            // Uzdarome faila
            rez.Close();
            fn.Close();
        }
        public void test()
        {
            int N = 1000;
            FileStream fr = new FileStream("HashTestREZ.txt", FileMode.Create, FileAccess.Write);
            StreamWriter rez = new StreamWriter(fr);
            rez.WriteLine("Elementu sk.       Trukme         Operaciju sk.");
            for (int i = 0; i < 10; i++)
            {
                

                FileStream fn = new FileStream("HashTestDUOM.txt", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fn);
                BinaryReader br = new BinaryReader(fn);

                Stopwatch sw = new Stopwatch();

                paieskaHash obj = new paieskaHash();

                int m = (int)(N / 0.75);  //dalinam is apkrovimo koeficiento
                m = (m % 2 == 0) ? m++ : m;
                setSingleCh(bw, (m + 1) * 21, 'd'); // prapleciam stream'a
                obj.generator(bw, br, N, m);

                GlVar.reset();
                sw.Start();
                obj.surastiVienodus(bw, br, m, rez);
                sw.Stop();

                rez.WriteLine(N + "    " + sw.Elapsed + "    " + GlVar.cnt);

                // Uzdarome faila
                fn.Close();
                N = N + 3000;
            }
            rez.Close();
        }

        // nuskaitome i-taji elementa is binarinio failo (zinome, kad visi elementai uzima po 4 bitus, todel i-tojo reiksmes pradzia bus i*4)
//============================================================================================================================================================================================
        public static char getSingleCh(BinaryReader br, int k)
        {
            br.BaseStream.Seek(k, SeekOrigin.Begin);
            GlVar.inc(2);
            return br.ReadChar();  
        }
//========================================================================================================================
        public static void setSingleCh(BinaryWriter bw, int k, char c)
        {
            bw.BaseStream.Seek(k, SeekOrigin.Begin);
            bw.Write(c);
            GlVar.inc(2);
        }
//========================================================================================================================
        public static char[] getChar(BinaryReader br, int i)
        {
            int k = (i) * 21;
            char[] studentas = new char[21];
            br.BaseStream.Seek(k, SeekOrigin.Begin);
            GlVar.inc(4);
            return br.ReadChars(21);
        }
//========================================================================================================================
        // priskiriame i-tajam elementui nauja reiksme
        public static void setChar(BinaryWriter bw, int i, char[] T)
        {
            int k = (i) * 21;
            bw.BaseStream.Seek(k, SeekOrigin.Begin);
            bw.Write(T);
            GlVar.inc(3);
        }
//========================================================================================================================
        public static long strToInt(char[] raktas)
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(raktas);
            long paverstas = asciiBytes[0];
            for (int i = 1; i < asciiBytes.Length - 1; i++)
            {
                paverstas = paverstas * asciiBytes[i];
                GlVar.inc(1);
            }
            GlVar.inc(4);
            return paverstas + asciiBytes[asciiBytes.Length - 1];
        }
//========================================================================================================================
        public static int hash(long k, int i, int m)
        {
            GlVar.inc(1);
            return (int)(k % m +3*i + 7*i * i) % m;
        }
//========================================================================================================================
        public void insert(char[] T, long k, BinaryWriter bw, BinaryReader br, int m)
        {
            int i = 0;
            bool idejo = true;
            while (i <= m && idejo)
            {
                int j = hash(k, i, m);
                if ((int)getSingleCh(br, j * 21) == 0) 
                {
                    setChar(bw, j, T);
                    idejo = false;
                    return;
                }
                else
                    i++;
            }
            Console.WriteLine("Hash table overflow");
        }
//========================================================================================================================
        public int search(char[] T, BinaryWriter bw, BinaryReader br, int m)
        {
            long k = strToInt(T); // raktas
            int i = 0;             // suolio nr
            bool netuscia = true;  // ar sekantis suolis tures elementa
            int counter = 0;       // kiek vienodu pavardziu
            while (i <= m && netuscia)  // suoliu skaicius negali virsyti masyvo dydzio ir sekantis suolis turi but netuscias
            {
                int index = hash(k, i, m); // masyvo indeksas suheshavus rakta
                //   Console.WriteLine(index);
                char[] studentas = getChar(br, index);  // imamas studentas
                int j = 0;             // indeksas pagabinis iteruoti pavardes raidems 
                bool sutampa = true;  // islieka true jei sutampa tikrinama pavarde 
                while (sutampa && T.Length < j) // sukasi iki kol sutampa raides arba baigiasi pavarde 
                {
                    if (studentas[j] != T[j++])          // jei kazkuri raide neatitinka
                    {
                        sutampa = false;
                        GlVar.inc(1);
                    }
                    GlVar.inc(1);
                }               // 
                if (sutampa && getSingleCh(br, 20 + index * 21) != 'x')                           // jei pavardes vienodos
                {

                    counter++;
                    // priskaiciuojama 1 pavarde 
                    setSingleCh(bw, 20 + index * 21, 'x');   // pazymi tikrinta studenta kad praleisti veliau
                    GlVar.inc(2);
                }
                if ((int)getSingleCh(br, 21 * hash(k, ++i, m)) == 0) // tikrina ar sekantis suolis yra
                {
                    netuscia = false;
                    GlVar.inc(1);
                }
                GlVar.inc(7);
            }
            GlVar.inc(6);
            return counter;
        }
//========================================================================================================================
        public void surastiVienodus(BinaryWriter bw, BinaryReader br, int m, StreamWriter rez)
        {
            for (int i = 0; i < m; i++)
            {
                char[] studentas = getChar(br, i);
                
                if(getSingleCh(br, i * 21 + 20) != 'x' && (int)studentas[0] != 0)
                {
                    int j = 0;
                    while (studentas[j] != ' ')
                    {
                        j++;
                        GlVar.inc(1);
                    }
                    char[] pavarde = new char[j];
                    for (j = 0; j < pavarde.Length; j++)
                    {
                        pavarde[j] = studentas[j];
                        rez.Write(pavarde[j]);
                        GlVar.inc(2);
                    }
                    rez.Write('-');
                    int x = search(pavarde, bw, br, m);
                    rez.Write(x);
                    rez.WriteLine();
                    GlVar.inc(7);
                }
                GlVar.inc(2);
            }
            GlVar.inc(1);
        }
//===========================================================================================================================================================================================================
        public void generator(BinaryWriter bw, BinaryReader br, int n, int m)
        {
            string[] Names = { "Andrew" ,"Zachary" ,"Cade" ,"Leroy" ,"Amos" ,
                               "Vaughan" ,"Reed" ,"Francis" ,"Caleb" ,"Price" ,"Cole" ,"Zeph" ,"Andrew" ,"Hiram" ,"Tad" ,"Flynn" ,"Baker" ,"Steven" ,"Plato" ,"Rajah" ,"Grant" ,"Hakeem" ,"Clayton" ,"Yoshio" ,
                                "Micah" ,"Amos" ,"Lance" ,"Hall" ,"Laith" ,"Theodore" ,"Camden" ,"Hashim" ,"Alec" ,"Charles" ,"Jordan" ,"Eaton" ,"Oliver" ,"Rajah" ,"Kibo" ,"Dexter" ,"Fletcher" ,"Amir" ,"Ivan" ,
                                "Todd" ,"Warren" ,"Hedley" ,"Chase" ,"Ali" ,"Jason" ,"Zachery" ,"Nathan" ,"Duncan" ,"Nehru" ,"Thor" ,"Orson" ,"Burke" ,"Tiger" ,"Finn" ,"Sylvester" ,"Chancellor" ,"Tad" ,"Yardley" ,
                                "Dale" ,"Elliott" ,"Phelan" ,"Sean" ,"Flynn" ,"Phelan" ,"Dane" ,"Porter" ,"Simon" ,"Garth" ,"Vladimir" ,"Samuel" ,"Thane" ,"Walker" ,"Coby" ,"Stewart" ,"Dante" ,"Quinn" ,"Damon" ,
                                "Jason" ,"Ian" ,"Eaton" ,"Ezekiel" ,"Raphael" ,"Melvin" ,"Peter" ,"Kennedy" ,"Jerry" ,"Walker" ,"Edan" ,"Wing" ,"Samson" ,"Levi" ,"Allen" ,"Dante" ,"Kenyon" ,"Benjamin" ,"Yoshio"};

            string[] Surnames = {"Rush" ,"Burris" ,"Wilder" ,"Fitzgerald" ,"Cline" ,"Moore" ,"Barry" ,"Newton" ,"Gamble" ,"Noble" ,"Fulton" ,"Nunez" ,"Hurst" ,"Hyde" ,"Hyde" ,"Acosta" ,"Alford" ,"Glover" ,"Pacheco" ,
                                    "Baldwin" ,"Berry" ,"Lynn" ,"Justice" ,"Stanley" ,"Rollins" ,"Mayer" ,"Mcfadden" ,"Chaney" ,"Mitchell" ,"Turner" ,"Hobbs" ,"Vaughan" ,"Rollins" ,"Haley" ,"Robinson" ,"Watkins" ,
                                    "Mckinney" ,"Bowers" ,"Potts" ,"Rodriquez" ,"Wilkerson" ,"Sanders" ,"Mccormick" ,"Taylor" ,"Wyatt" ,"Holt" ,"Olson" ,"Woodward" ,"House" ,"Atkins" ,"Owens" ,"Mckay" ,"Bush" ,"Raymond" ,
                                    "Vargas" ,"Obrien" ,"Bartlett" ,"Burke" ,"Blankens" ,"Mercado" ,"Fields" ,"Ward" ,"Dalton" ,"Simon" ,"Santiago" ,"Blanchard" ,"Penning" ,"Williams" ,"Irwin" ,"Mcdonald" ,"Kennedy" ,
                                    "Osborne" ,"Calderon" ,"Parrish" ,"Barnes" ,"Potter" ,"Patton" ,"Ballard" ,"Dejesus" ,"Black" ,"Hatfield" ,"Jimenez" ,"Woodward" ,"Perkins" ,"Decker" ,"Dotson" ,"Bowman" ,"Petty" ,"Curtis" ,
                                    "Langley" ,"Pickett" ,"Porter" ,"Craig" ,"Snyder" ,"Shannon" ,"Stokes" ,"Bush" ,"Spears" ,"Wood" ,"Kramer" };
            Random rand = new Random();
            char[] studentas = new char[21];
            for (int i = 0; i < n; i++)
            {
                string surname = Surnames[rand.Next(0, Surnames.Length)];
                string name = Names[rand.Next(0, Names.Length)];
                int j = 0;
                while (j < surname.Length)
                    studentas[j] = surname[j++];
                studentas[j++] = ' ';
                int z = 0;
                while (z < name.Length)
                    studentas[j++] = name[z++];
                insert(studentas, strToInt(surname.ToCharArray()), bw, br, m);
            }
            
        }
//===========================================================================================================================================================================================================
        public void print(BinaryReader br, int m)
        {
            for (int i = 0; i < m; i++)
            {
                char[] studentas = getChar(br, i);
                if((int)studentas[0] == 0)
                    Console.WriteLine("tuscias");
                else
                {
                    int j = 0;
                    while ((int)studentas[j] != 0)
                        Console.Write(studentas[j++]);
                    Console.WriteLine();
                }
            }
        }
    }
}
