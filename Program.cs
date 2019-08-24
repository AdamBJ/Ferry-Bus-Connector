using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    // Valid until Jun 20th, 2019
    static string[] busDepartures = new string[] {"6:00 AM", "5:25 PM", "6:23 PM", "7:40 PM", "8:30 PM", "9:55 PM", "11:02 PM"};

    static string[] expressBusDepartures = new string[] {"5:30 AM", "5:25 PM", "6:11 PM", "6:50 PM", "7:40 PM", "8:30 PM", "9:55 PM"};
    // Valid until Jun 19th, 2019
    static string[] ferryDepartures = new string[] {"5:30 PM", "6:55 PM", "9:10 PM", "11:20 PM"};
    static int ferryDurationInMin = 40;

    static string separator = "    ";

    // tt gives AM/PM
    static string timeFormatString = "h:mm tt";

    enum Mode
    {
        INVALID,
        GET_SHORTEST_WAIT,
        CHOOSE_FERRY,
        DISPLAY_TABLE
        
    }

    static void Main(string[] args)
    {
        Mode choice = Mode.INVALID;
        do 
        {
            Console.WriteLine(
                "Choose mode: 1) Get shortest overall wait time. 2) Get wait info for a specific sailing. 3) Display wait graph for all available sailings.");
            try
            {
                choice = (Mode)Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
            }
        }
        while (choice != Mode.GET_SHORTEST_WAIT && choice != Mode.CHOOSE_FERRY && choice != Mode.DISPLAY_TABLE);

        switch (choice)
        {
            case Mode.GET_SHORTEST_WAIT:
            {
                string sailing = GetShortestWaitFerry(Program.ferryDepartures);
                Console.WriteLine(
                    @"The shortest wait will be on the {0} sailing from Horseshoe Bay. You'll arrive in Langdale at around {1} and 
                    can take an express bus at {2} or a normal bus at {3}.",
                    sailing,
                    DateTimeToString(GetFerryArrival(sailing, Program.ferryDurationInMin)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: true)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: false)));

                break;
            }
            case Mode.CHOOSE_FERRY:
            {
                int sailingNo = 0;
                do
                {
                    Console.Write("Choose which sailing you'll be on: ");
                    for (int i = 0; i < ferryDepartures.Length; i++)
                    {
                        Console.Write("{0}. {1} ", i+1, ferryDepartures[i]);
                    }
                    Console.WriteLine();
                    
                    try
                    {
                        sailingNo = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                    }
                } 
                while (sailingNo < 1 || sailingNo > ferryDepartures.Length);
                
                string sailing = ferryDepartures[sailingNo - 1];
                Console.WriteLine(
                    @"If you're on the {0} sailing from Horseshoe Bay you'll arrive in Langdale at around {1}. 
                    The next express bus you can take leaves at {2}, the next normal bus you can take leaves at {3}.",
                    sailing,
                    DateTimeToString(GetFerryArrival(sailing, Program.ferryDurationInMin)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: true)),
                    DateTimeToString(DetermineBus(sailing, isExpressBus: false)));
                    break;
            }
            case Mode.DISPLAY_TABLE:
            {
                Console.WriteLine("The entries in the following table have been sorted by shortest wait time.");
                Console.WriteLine(String.Join(separator, new string[]{
                    "Ferry Departure",
                    "Ferry Arrival",
                    "Earliest Express Bus Departure",
                    "Earliest Normal Bus Departure",
                    "Shortest Wait For Any Bus"}));

                List<string> ferryDeps = new List<string>(Program.ferryDepartures);

                for(int i=0; i<Program.ferryDepartures.Length; i++)
                {
                    string sailing = GetShortestWaitFerry(ferryDeps.ToArray());
                    ferryDeps.Remove(sailing);
                    Console.WriteLine(
                        String.Join(
                            separator,
                            new string[] {
                                sailing,
                                DateTimeToString(GetFerryArrival(sailing, Program.ferryDurationInMin)),
                                DateTimeToString(DetermineBus(sailing, isExpressBus: true)),
                                DateTimeToString(DetermineBus(sailing, isExpressBus: false))})
                    );
                }  
                break;
            }
            default:
                throw new ArgumentException(String.Format("Invalid choice {0} passed to switch block.", choice));
        }
    }

    static string GetShortestWaitFerry(string[] ferryDepartures)
    {           
        string shortestWaitFerry = null;
        TimeSpan shortestWait = new TimeSpan(24,0,0);
        foreach(string sailing in ferryDepartures)
        {
            DateTime ferryArrival = GetFerryArrival(sailing, Program.ferryDurationInMin);

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
        arrivalDT = arrivalDT.AddMinutes(Program.ferryDurationInMin);
        return arrivalDT;
    }

    static string DateTimeToString(DateTime dt)
    {
        return dt.ToString(timeFormatString, CultureInfo.InvariantCulture);
    }

    static List<DateTime> GetFilteredBuses(DateTime ferryArrival, string[] busArray)
    {
        // Give 15 minutes padding in case boat is delayed.
        return busArray.
                Select(time => DateTime.Parse(time)).
                Where(busDep => busDep > ferryArrival.AddMinutes(15)).
                ToList();
    }

    static DateTime DetermineBus(string ferryDeparture, bool isExpressBus)
    {
        DateTime ferryArrival= GetFerryArrival(ferryDeparture, Program.ferryDurationInMin);

        List<DateTime> filteredBusDepartures;
        if (isExpressBus)
        {
            filteredBusDepartures = GetFilteredBuses(ferryArrival, Program.expressBusDepartures);
        }
        else
        {
            filteredBusDepartures = GetFilteredBuses(ferryArrival, Program.busDepartures);
        }
        filteredBusDepartures.Sort();

        if (filteredBusDepartures.Count == 0)
        {
            // First bus the next morning
            return DateTime.Parse(
                isExpressBus? Program.expressBusDepartures[0] : Program.busDepartures[0])
                .AddDays(1);
        }

        return filteredBusDepartures.First();
    }
}
