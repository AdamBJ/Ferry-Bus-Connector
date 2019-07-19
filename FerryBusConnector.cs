﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace FerryBusConnector
{
    class FerryBusConnector
    {
        // Valid until Jun 20th, 2019
        static string[] busDepartures = new string[] {"6:00AM", "5:25PM", "6:23PM", "7:40PM", "8:30PM", "9:55PM", "11:02PM"};

        static string[] expressBusDepartures = new string[] {"5:30AM", "5:25PM", "6:11PM", "6:50PM", "7:40PM", "8:30PM", "9:55PM"};
        // Valid until Jun 19th, 2019
        static string[] ferryDepartures = new string[] {"5:30PM", "6:55PM", "9:10PM", "11:20PM"};
        static int ferryDurationInMin = 40;

        static void Main(string[] args)
        {
            int choice = 0;
            do 
            {
                Console.WriteLine("Choose mode: 1) Get shortest overall wait time 2) Get wait info for a specific sailing.");
                choice = Convert.ToInt32(Console.ReadLine());
            }
            while (choice != 1 && choice != 2);

            if (choice == 1)
            {
                string sailing = GetShortestWaitFerry();
                Console.WriteLine(
                    @"The shortest wait will be on the {0} sailing from Horseshoe Bay. You'll arrive in Langdale at around {1} and 
    can take an express bus at {2} or a normal bus at {3}.",
                    sailing,
                    DateTimeToString(GetFerryArrival(sailing, FerryBusConnector.ferryDurationInMin)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: true)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: false)));
            }
            else if (choice == 2)
            {
                Console.Write("Choose which sailing you'll be on: ");
                for (int i = 0; i < ferryDepartures.Length; i++)
                {
                    Console.Write("{0}. {1} ", i+1, ferryDepartures[i]);
                }
                Console.WriteLine();
                
                int sailingNo = Convert.ToInt32(Console.ReadLine());
                string sailing = ferryDepartures[sailingNo - 1];
                Console.WriteLine(
                    @"If you're on the {0} sailing from Horseshoe Bay you'll arrive in Langdale at around {1}. 
    The next express bus you can take leaves at {2}, the next normal bus you can take leaves at {3}.",
                    sailing,
                    DateTimeToString(GetFerryArrival(sailing, FerryBusConnector.ferryDurationInMin)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: true)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: false)));
            }
        }

        static string GetShortestWaitFerry()
        {           
            string shortestWaitFerry = null;
            TimeSpan shortestWait = new TimeSpan(24,0,0);
            foreach(string sailing in FerryBusConnector.ferryDepartures)
            {
                DateTime ferryArrival = GetFerryArrival(sailing, FerryBusConnector.ferryDurationInMin);

                DateTime expBusDep = DetermineBus(sailing, isExpressBus: true);
                TimeSpan expBusWait = expBusDep.Subtract(ferryArrival);
                if (expBusWait < shortestWait)
                {
                    shortestWait = expBusWait;
                    shortestWaitFerry = sailing;
                }

                DateTime normalBusDep = DetermineBus(sailing, isExpressBus: false);
                TimeSpan normalBusWait = normalBusDep.Subtract(ferryArrival);
                if (normalBusWait < shortestWait)
                {
                    shortestWait = normalBusWait;
                    shortestWaitFerry = sailing;
                }
            }

            return shortestWaitFerry;
        }

        static DateTime GetFerryArrival(string departure, int voyageDuration)
        {
            DateTime arrivalDT= DateTime.Parse(departure);
            arrivalDT = arrivalDT.AddMinutes(FerryBusConnector.ferryDurationInMin);
            return arrivalDT;
        }

        static string DateTimeToString(DateTime dt)
        {
            return String.Format("{0}:{1}PM", dt.Hour % 12, dt.Minute);
        }

        static List<DateTime> GetSortedBuses(DateTime ferryArrival, string[] busArray)
        {
            // Give 15 minutes padding in case boat is delayed.
            return busArray.
                    Select(time => DateTime.Parse(time)).
                    Where(busDep => busDep > ferryArrival.AddMinutes(15)).
                    ToList();
        }

        static DateTime DetermineBus(string ferryDeparture, bool isExpressBus)
        {
            DateTime ferryArrival= GetFerryArrival(ferryDeparture, FerryBusConnector.ferryDurationInMin);

            List<DateTime> sortedBusDepartures;
            if (isExpressBus)
            {
                sortedBusDepartures = GetSortedBuses(ferryArrival, FerryBusConnector.expressBusDepartures);
            }
            else
            {
                sortedBusDepartures = GetSortedBuses(ferryArrival, FerryBusConnector.busDepartures);
            }
            sortedBusDepartures.Sort();

            if (sortedBusDepartures.Count == 0)
            {
                // First bus the next morning
                return DateTime.Parse(
                    isExpressBus? FerryBusConnector.expressBusDepartures[0] : FerryBusConnector.busDepartures[0]);
            }

            return sortedBusDepartures.First();
        }
    }
}