using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OgarniatorDanych_v4
{
    class Program
    {
        static string path = "C:\\Users\\Patryk\\Desktop\\ML_DATABASE\\tracking_data.rpt";
        static string pathZIP = @"C:\Users\Patryk\Desktop\ML_DATABASE\KODY POCZTOWE\PL.csv";
        private static string outputPath = "C:\\Users\\Patryk\\Desktop\\ML_DATABASE\\tracking_dataOut_TEST_POLSKA.csv";
        //private static FileStream fs;

        private static long counter = 0;
        private static long fileLength = -2;
        private static int suggestedLineSize = 0;

        static List<string> naglowki = new List<string>();
        static List<int> dlugosciNaglowkow = new List<int>();

        private static void GatherFileData()
        {
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
                for (int i = 1; i < naglowki.Count - 4; i++)
                {
                    sw.Write(naglowki[i] + ";");
                }
                sw.WriteLine(naglowki[naglowki.Count - 4]);
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
                            if (counter % 10000 == 0)
                            {
                                Console.WriteLine("Przerobiono " + counter);
                                break;
                            }


                            if (line.Contains("NULL"))
                            {
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

        //metody pomocnicze - zamieniające dane w liczby lub odwrotnie

        private static string GetDistance(string firstZIP, string secondZIP)
        {
            string distance = "0", firstZIP_GEO = "", secondZIP_GEO = "", tempLine = "";
            string[] words_1, words_2;
            bool tryParse;
            float firstLatitude = 0, firstLongitude = 0, secondLatitude = 0, secondLogtitude = 0;
            int flag_1 = 0, flag_2 = 0;

            foreach (var line in File.ReadAllLines(pathZIP))
            {//odczytuje odpowiednie linie z pliku ze współrzędnymi dla kodów pocztowych
                if (line.Contains(firstZIP) && flag_1 == 0)
                {
                    firstZIP_GEO = line;

                    words_1 = firstZIP_GEO.Split(null);

                    firstLatitude = float.Parse(words_1[1]);
                    firstLongitude = float.Parse(words_1[2]);
                    /*tryParse = Single.TryParse(words_1[1], out firstLongitude);
                    if (!tryParse)
                        break;
                    tryParse = Single.TryParse(words_1[2], out firstLatitude);
                    if (!tryParse)
                        break;*/

                    firstZIP_GEO = words_1[1] + ";" + words_1[2];

                    flag_1 = 1;
                }
                if (line.Contains(secondZIP) && flag_2 == 0)
                {
                    secondZIP_GEO = line;

                    words_2 = secondZIP_GEO.Split(null);

                    secondLatitude = float.Parse(words_2[1]);
                    secondLogtitude = float.Parse(words_2[2]);
                    /*tryParse = Single.TryParse(words_2[1], out secondLogtitude);
                    if (!tryParse)
                        break;
                    tryParse = Single.TryParse(words_2[2], out secondLatitude);
                    if (!tryParse)
                        break;*/

                    secondZIP_GEO = words_2[1] + ";" + words_2[2];

                    flag_2 = 1;
                }
                if (flag_1 == 1 && flag_2 == 1)
                {
                    var distance_ = new Coordinates(firstLatitude, firstLongitude)
                        .DistanceTo(
                        new Coordinates(secondLatitude, secondLogtitude),
                        UnitOfLength.Kilometers
                        );
                    return distance_.ToString();
                }
            }
            //distance = firstZIP_GEO +"----------"+secondZIP_GEO;

            //distance = (firstLatitude + firstLongitude).ToString();



            //distance = distance_.ToString();

            return distance;
        }


        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        private static string ConvertFromUnixTimestamp(string unix)
        {//do poprawy - narazie daje według czasu GMT a powinno według polskiego +1h
            long unixTime;
            bool tryParse;
            unix += "00";
            string date;

            tryParse = Int64.TryParse(unix, out unixTime);
            if (!tryParse)
                Console.WriteLine("Błąd konwersji z unix na date.");
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            date = epoch.AddSeconds(unixTime).ToString();

            return date;
        }

        private static string ConvertPostalToNumber(string postal_code)
        {
            string ASCII = "";
            foreach (char c in postal_code)
            {
                ASCII += System.Convert.ToInt32(c).ToString();
            }
            return ASCII;
        }

        private static string ConvertCountryToNumber(string country)
        {
            string ASCII = "";
            foreach (char c in country)
            {
                ASCII += System.Convert.ToInt32(c).ToString();
            }
            return ASCII;
        }

        private static string AddComma(string _string)
        {
            string floatNumber;
            var builder = new StringBuilder();
            int counter = 0;
            foreach (var c in _string)
            {
                builder.Append(c);
                if (++counter == 6)//po 6 znakach dodaje kropke do liczby zeby byl float
                {
                    builder.Append('.');
                }
            }
            floatNumber = builder.ToString();
            return floatNumber;
        }

        private static string RemoveComma(string _string)
        {
            _string = _string.Replace(".", "");
            return _string;
        }

        private static string ConvertNumberToPostal(string postal_code)
        {
            string ASCII = "", temp = "0";
            int counter = 0, unicode;
            bool tryParse;
            char character;

            foreach (char c in postal_code)
            {
                temp += c;
                counter++;
                if (counter == 2)
                {
                    tryParse = Int32.TryParse(temp, out unicode);
                    if (tryParse)
                    {
                        character = (char)unicode;
                        ASCII += character.ToString();
                        temp = "0";
                        counter = 0;
                    }
                    else
                        Console.WriteLine("Błąd konwersji");
                }
            }
            return ASCII;
        }

        private static string ConvertNumberToCountry(string number)
        {
            string ASCII = "", temp = "0";
            int counter = 0, unicode;
            bool tryParse;
            char character;

            foreach (char c in number)
            {
                temp += c;
                counter++;
                if (counter == 2)
                {
                    tryParse = Int32.TryParse(temp, out unicode);
                    if (tryParse)
                    {
                        character = (char)unicode;
                        ASCII += character.ToString();
                        temp = "0";
                        counter = 0;
                    }
                    else
                        Console.WriteLine("Błąd konwersji");
                }
            }
            return ASCII;
        }

        private static string FixLine(string line)
        {
            string fixedLine = "";
            int debugMax = 0, result;
            float result_float;
            string dateFormat = "yyyy-MM-dd HH:mm:ss";
            long centuryBegin = new DateTime(2001, 1, 1).Ticks;
            bool isNumber;

            try
            {
                //    string tempLine;
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
                //tempLine = date.Ticks.ToString();//interwal 100 nanosekund od 0001-01-01
                //tempLine = tempLine.GetHashCode().ToString();
                isNumber = Int32.TryParse(tempLine, out result);
                if (!isNumber)
                    return null;
                tempLine = tempLine.Substring(0, tempLine.Length - 2);//usunięcie dwóch ostatnich zer z unixtimestamp - żeby była mniejsza liczba przy uczeniu
                //tempLine = ConvertFromUnixTimestamp(tempLine);
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
                //tempLine = date.ToString();
                isNumber = Int32.TryParse(tempLine, out result);
                if (!isNumber)
                    return null;
                tempLine = tempLine.Substring(0, tempLine.Length - 2);//usunięcie dwóch ostatnich zer z unixtimestamp - żeby była mniejsza liczba przy uczeniu
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
                //tempLine = date.ToString();
                isNumber = Int32.TryParse(tempLine, out result);
                if (!isNumber)
                    return null;
                tempLine = tempLine.Substring(0, tempLine.Length - 2);//usunięcie dwóch ostatnich zer z unixtimestamp - żeby była mniejsza liczba przy uczeniu
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
                if (!tempLine.Equals("PL"))
                    return null;
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
                if (!tempLine.Equals("PL"))
                    return null;
                tempLine += ";";
                fixedLine += tempLine;
                if (counter < debugMax)
                {
                    System.Console.WriteLine(tempLine);
                }
                line = line.Substring(dlugosciNaglowkow[7], line.Length - dlugosciNaglowkow[7]);

                //9 SHIPMENT_WEIGHT
                tempLine = line.Substring(0, dlugosciNaglowkow[8]);
                //tempLine = GetDistance();
                tempLine += "0";
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
    public class UnitOfLength
    {
        public static UnitOfLength Kilometers = new UnitOfLength(1.609344);
        public static UnitOfLength NauticalMiles = new UnitOfLength(0.8684);
        public static UnitOfLength Miles = new UnitOfLength(1);

        private readonly double _fromMilesFactor;

        private UnitOfLength(double fromMilesFactor)
        {
            _fromMilesFactor = fromMilesFactor;
        }

        public double ConvertFromMiles(double input)
        {
            return input * _fromMilesFactor;
        }
    }
    public static class CoordinatesDistanceExtensions
    {
        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates)
        {
            return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometers);
        }

        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates, UnitOfLength unitOfLength)
        {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }
    }
    public class Coordinates
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}


