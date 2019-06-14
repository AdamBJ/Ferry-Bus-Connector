using System;
using System.Linq;
using System.Collections.Generic;

namespace FerryBusConnector
{
    class Program
    {
        // Valid until Jun 20th, 2019
        static string[] buses = new string[] {"5:25PM", "6:23PM", "7:40PM", "8:30PM", "9:55PM", "11:02"};

        static string[] expressBuses = new string[] {"5:25PM", "6:11PM", "6:50PM", "7:40PM", "8:30PM", "9:55PM"};
        // Valid until Jun 19th, 2019
        static string[] ferries = new string[] {"5:30PM", "6:55PM", "9:10PM", "11:20PM"};
        static int ferryDurationInMin = 40;
        static void Main(string[] args)
        {
            Console.Write("Choose which sailing you'll be on: ");
            for (int i = 1; i < ferries.Length; i++)
            {
                if (i < ferries.Length)
                {
                    Console.Write("{0}. {1} ", i, ferries[i-1]);
                }
                else
                {
                    Console.WriteLine("{0}. {1}", i, ferries[i-1]);
                }
            }
            int sailingNo = Convert.ToInt32(Console.ReadLine());
            string sailing = ferries[sailingNo - 1];
            Console.WriteLine(
                @"If you're on the {0} sailing from Horseshoe Bay you'll arrive in Langdale at around {1}. 
The next express bus you can take leaves at {2}, the next normal bus you can take leaves at {3}.",
                sailing,
                GetFerryArrival(sailing, Program.ferryDurationInMin),
                DetermineBus(sailing, isExpressBus: true),
                DetermineBus(sailing, isExpressBus: false));
        }

        static string GetShortestWait()
        {
            
            foreach(string ferryDeparture in Program.ferries)
            {
                string expressBusDep = DetermineBus(ferryDeparture, isExpressBus: true);
                string normalBusDep = DetermineBus(ferryDeparture, isExpressBus: false);
                string ferryArrival = GetFerryArrival(ferryDeparture, Program.ferryDurationInMin);


                string result = String.Format(@"If you're on the {0} sailing from Horseshoe Bay you'll arrive in Langdale at around {1}. 
The next express bus you can take leaves at {2}, the next normal bus you can take leaves at {3}.",
                ferryDeparture,
                GetFerryArrival(ferryDeparture, Program.ferryDurationInMin),
                DetermineBus(ferryDeparture, isExpressBus: true),
                DetermineBus(ferryDeparture, isExpressBus: false)
            }
        }

        static string GetFerryArrival(string departure, int voyageDuration)
        {
            DateTime arrivalDT= DateTime.Parse(departure);
            // Give 15 minutes padding in case boat is delayed.
            arrivalDT = arrivalDT.AddMinutes(Program.ferryDurationInMin);
            return DateTimeToString(arrivalDT);
        }

        static string DateTimeToString(DateTime dt)
        {
            return String.Format("{0}:{1}PM", dt.Hour % 12, dt.Minute);
        }

        static List<DateTime> GetSortedBuses(DateTime ferryArrival, string[] busArray)
        {
            return busArray.
                    Select(time => DateTime.Parse(time)).
                    Where(busDep => busDep > ferryArrival.AddMinutes(15)).
                    ToList();
        }

        static string DetermineBus(string ferryDeparture, bool isExpressBus)
        {
            string arrival = GetFerryArrival(ferryDeparture, Program.ferryDurationInMin);
            DateTime ferryArrival= DateTime.Parse(arrival);

            List<DateTime> sortedBusDepartures;
            if (isExpressBus)
            {
                sortedBusDepartures = GetSortedBuses(ferryArrival, Program.expressBuses);
            }
            else
            {
                sortedBusDepartures = GetSortedBuses(ferryArrival, Program.buses);
            }
            sortedBusDepartures.Sort();


            if (sortedBusDepartures.Count == 0)
            {
                return "dawn";
            }

            return DateTimeToString(sortedBusDepartures.First());
        }
    }
}
