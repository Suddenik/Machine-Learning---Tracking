using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OgarniatorDanych_v3
{
    class Program
    {
        static string path = "C:\\Users\\Patryk\\Desktop\\ML_DATABASE\\tracking_data.rpt";
        private static string outputPath = "C:\\Users\\Patryk\\Desktop\\ML_DATABASE\\tracking_dataOut_TEST.csv";
        //private static FileStream fs;

        private static long counter = 0;
        private static long fileLength = -2;
        private static int suggestedLineSize = 0;

        static List<string> naglowki = new List<string>();
        static List<int> dlugosciNaglowkow = new List<int>();

        private static void GatherFileData()
        {

            /*using (StreamReader sr = File.OpenText(path))
            {
                int longest = 0;
                string linia = "";
                linia = sr.ReadLine();
                while ((linia = sr.ReadLine()) != null)
                {
                    fileLength++;
                    if (fileLength % 1000000 == 0)
                    {
                        int x = linia.Length;
                        if (longest < x)
                        {
                            longest = x;
                        }
                        Console.WriteLine("{0} linijek", fileLength);
                        //break;
                    }
                }

                suggestedLineSize = longest;
                Console.WriteLine("{0} dlugosc lini", suggestedLineSize);
            }

            Console.WriteLine("Plik posiada {0} linijek danych. ",fileLength);*/

            //zbieramy nazwy tabel
            using (StreamReader sr = File.OpenText(path))
            {
                string line = sr.ReadLine();
                int iloscNaglowkow = 0;
                foreach (var subline in line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
                {
                    naglowki.Add(subline);
                    iloscNaglowkow++;
                }

                System.Console.Write("Jest {0} tabeli o dlugosciach: ", iloscNaglowkow);

                //zbieramy dlugosci tabel
                line = sr.ReadLine();
                int iloscdanych = 0;
                foreach (var subline in line.Split(' '))
                {
                    dlugosciNaglowkow.Add(subline.Length + 1);
                    iloscdanych++;
                    suggestedLineSize += subline.Length;
                    System.Console.Write("{0},  ", subline.Length);
                }

                System.Console.WriteLine();
            }

            //TODO: przerobic naglowki na sztywno, bo i tak beda troche inne
            using (StreamWriter sw = File.AppendText(outputPath))
            {
                //pomijamy ID paczki
                for (int i = 1; i < naglowki.Count - 3; i++)
                {
                    sw.Write(naglowki[i] + ";");
                }
                sw.WriteLine(naglowki[naglowki.Count - 3]);
            }

        }

        private static void CrateOutputFile()
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(outputPath))
            {
                sw.Write("");
            }
        }

        private static int ConvertTry(long startindex)
        {
            //long avg = 0;
            Console.WriteLine("Rozpoczynam od " + startindex);
            using (StreamReader sr = File.OpenText(path))
            {
                using (StreamWriter sw = File.AppendText(outputPath))
                {
                    string line = "";
                    try
                    {
                        for (int a = 0; a < startindex; a++)
                        {
                            sr.ReadLine();
                        }

                        //int percent = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            counter++;
                            if (counter % 1000000 == 0)
                            {
                                Console.WriteLine("Przerobiono " + counter);
                                break;
                            }

                            /*if (line.Length != suggestedLineSize)
                            {
                                int x = suggestedLineSize - line.Length;
                                //avg += line.Length;
                                Console.WriteLine("line length is bad "+x);
                                continue;
                            }*/

                            if (line.Contains("NULL"))
                            {
                                //Console.WriteLine("Has null");
                                //skip this lame line
                                continue;
                            }

                            string fixedLine = FixLine(line);
                            if (fixedLine != null)
                            {
                                //zapisanie ogarnietych danych
                                sw.WriteLine(fixedLine);
                            }
                        }


                        System.Console.WriteLine("There were {0} lines.", counter);
                        System.Console.WriteLine("Ukonczono pomyslnie");
                        System.Console.WriteLine("Nacisjnij cokolwiek by zamknac program");
                        Console.ReadKey();

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Jakis error " + e.Message);
                        System.Console.WriteLine(counter + " Linia: " + line + " :Koniec.");
                        System.Console.WriteLine("Nacisjnij cokolwiek by probowac kontynuowac");
                        Console.ReadKey();
                    }
                }

                //double average = avg / counter;
                //Console.WriteLine("Avg: "+average);
                // success
                return 0;
            }
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        private static string ConvertCountryToNumber(string country)
        {
            string ASCII="";
            foreach (char c in country)
            {
                ASCII+=System.Convert.ToInt32(c).ToString();
            }
            return ASCII;
        }

        private static string ConvertNumberToCountry(string number)
        {
            string ASCII = "", first_part="",second_part="";
            int counter = 0, unicode;
            bool tryParse;
            char character;

            foreach (char c in number)
            {
                if (counter < 2)
                {
                    first_part += c;
                }
                else if (counter >= 2)
                {
                    second_part += c;
                }
                counter++;
            }
            tryParse = Int32.TryParse(first_part,out unicode);
            if (tryParse)
            {
                character = (char)unicode;
                ASCII += character.ToString();
            }
            else
                Console.WriteLine("Błąd konwersji");

            tryParse = Int32.TryParse(second_part, out unicode);
            if (tryParse)
            {
                character = (char)unicode;
                ASCII += character.ToString();
            }
            else
                Console.WriteLine("Błąd konwersji");

            return ASCII;
        }


        private static string FixLine(string line)
        {
            string fixedLine = "";
            int debugMax = 0;
            string dateFormat = "yyyy-MM-dd HH:mm:ss";
            long centuryBegin = new DateTime(2001, 1, 1).Ticks;

            try
            {
                //1 SHIPMENT_IDENTCODE
                string tempLine = line.Substring(0, dlugosciNaglowkow[0]);
                tempLine = tempLine.Replace(" ", "");
                tempLine += ";";
                //System.Console.Write(tempLine);
                //fixedLine += tempLine;
                line = line.Substring(dlugosciNaglowkow[0], line.Length - dlugosciNaglowkow[0]);

                //2 SHIPMENT_CREATEDATE
                tempLine = line.Substring(0, dlugosciNaglowkow[1] - 5);
                DateTime date = DateTime.ParseExact(tempLine, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                date = date.AddMinutes(-date.Minute).AddSeconds(-date.Second);//tylko godziny z danej daty - nie musimy znać czasu dostawy co do sekundy
                tempLine = ConvertToUnixTimestamp(date).ToString();//konwertowanie nie do ticków (duża liczba) a do czasu unixowego
                //tempLine = date.ToString();
                //tempLine = date.Ticks.ToString();
                //tempLine = tempLine.GetHashCode().ToString();
                tempLine += ";";
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                fixedLine += tempLine;
                line = line.Substring(dlugosciNaglowkow[1], line.Length - dlugosciNaglowkow[1]);

                //3 FIRST_EVENT
                tempLine = line.Substring(0, dlugosciNaglowkow[2] - 5);
                date = DateTime.ParseExact(tempLine, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                date = date.AddMinutes(-date.Minute).AddSeconds(-date.Second);//tylko godziny z danej daty - nie musimy znać czasu dostawy co do sekundy
                tempLine = ConvertToUnixTimestamp(date).ToString();//konwertowanie nie do ticków (duża liczba) a do czasu unixowego
                //tempLine = date.Ticks.ToString();
                tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[2], line.Length - dlugosciNaglowkow[2]);

                //4 LAST_EVENT
                tempLine = line.Substring(0, dlugosciNaglowkow[3] - 5);
                date = DateTime.ParseExact(tempLine, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                date = date.AddMinutes(-date.Minute).AddSeconds(-date.Second);//tylko godziny z danej daty - nie musimy znać czasu dostawy co do sekundy
                tempLine = ConvertToUnixTimestamp(date).ToString();//konwertowanie nie do ticków (duża liczba) a do czasu unixowego
                //tempLine = date.Ticks.ToString();
                tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[3], line.Length - dlugosciNaglowkow[3]);

                //5 RECIEVER_ZIP
                tempLine = line.Substring(0, dlugosciNaglowkow[4]);
                tempLine = tempLine.Replace(" ", "");
                tempLine = tempLine.Replace("-", "");
                //tylko kody pocztowe które są liczbowe - odrzuca kody NL i GB między innymi  //TODO: zrobić hasha (ale żeby potem można odhashować) tych kodów pocztowych które nie są liczbami
                int result;
                bool isNumber = Int32.TryParse(tempLine, out result);
                if (!isNumber)
                    return null;
                //tempLine = tempLine.GetHashCode().ToString();
                tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[4], line.Length - dlugosciNaglowkow[4]);

                //6 RECEIVER_COUNTRY_ISO2
                tempLine = line.Substring(0, dlugosciNaglowkow[5]);
                tempLine = tempLine.Replace(" ", "");
                tempLine = ConvertCountryToNumber(tempLine);
                //tempLine += ConvertNumberToCountry(tempLine);
                //tempLine = tempLine.GetHashCode().ToString();
                tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[5], line.Length - dlugosciNaglowkow[5]);

                //7 SENDER_ZIP
                tempLine = line.Substring(0, dlugosciNaglowkow[6]);
                tempLine = tempLine.Replace(" ", "");
                tempLine = tempLine.Replace("-", "");
                //tylko kody pocztowe które są liczbowe - odrzuca kody NL i GB między innymi
                isNumber = Int32.TryParse(tempLine, out result);
                if (!isNumber)
                    return null;
                //tempLine = tempLine.GetHashCode().ToString();
                tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[6], line.Length - dlugosciNaglowkow[6]);

                //8 SENDER_COUNTRY_ISO2
                tempLine = line.Substring(0, dlugosciNaglowkow[7]);
                tempLine = tempLine.Replace(" ", "");
                tempLine = ConvertCountryToNumber(tempLine);
                //tempLine += ConvertNumberToCountry(tempLine);
                //tempLine = tempLine.GetHashCode().ToString();
                tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[7], line.Length - dlugosciNaglowkow[7]);

                //9 SHIPMENT_WEIGHT
                tempLine = line.Substring(0, dlugosciNaglowkow[8]);
                tempLine = tempLine.Replace(" ", "");
                //tempLine = tempLine.GetHashCode().ToString();
                //tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[8], line.Length - dlugosciNaglowkow[8]);

                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                    Console.ReadKey();
                    System.Console.WriteLine(fixedLine);
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                if (counter < debugMax)
                {
                    System.Console.WriteLine("Exception: " + ex.Message);
                    System.Console.WriteLine(line);
                }
                return null;
            }
            return fixedLine;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting..!");
            CrateOutputFile();
            GatherFileData();
            ConvertTry(5);
            Console.WriteLine("Zapisuje plik w: " + outputPath);
        }
    }
}
