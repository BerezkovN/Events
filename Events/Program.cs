using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Events
{
    public static class TickClass
    {
        public static event TickHandler Tick;
        public delegate void TickHandler(int time);

        public static Stopwatch stopwatch = new Stopwatch();

        public static void DoTicks()
        {
            stopwatch.Start();

            while (true)
            {
                if (Tick != null)
                {
                    Tick((int)stopwatch.ElapsedMilliseconds);
                }
            }
        }

        public static void DoTicks(int EndTime)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                if (Tick != null)
                {
                    Tick((int)stopwatch.ElapsedMilliseconds);
                }

                if (EndTime <= (int)stopwatch.ElapsedMilliseconds)
                    return;
            }
        }
    }

    public class Metronome
    {
        public string Name;
        public int IntervalTime;
        public int PassedCount = 0;
        public int Offset = 0;

        public event IntervalHandler Interval;
        public delegate void IntervalHandler(Metronome metronome);

        public Metronome(string Name, int IntervalTime)
        {
            this.Name = Name;
            this.IntervalTime = IntervalTime;

            TickClass.Tick += new TickClass.TickHandler(IntervalLoop);
        }

        public Metronome(string Name, int IntervalTime, int Offset)
        {
            this.Name = Name;
            this.IntervalTime = IntervalTime;
            this.Offset = Offset;

            TickClass.Tick += new TickClass.TickHandler(IntervalLoop);
        }

        public void IntervalLoop(int time)
        {
            if (time - IntervalTime* PassedCount - Offset > IntervalTime)
            {
                Interval(this);
                PassedCount++;
            } 
        }
    }

    public class Listener
    {
        public string Name;

        public Listener(string Name)
        {
            this.Name = Name;
        }

        public void Subscribe(Metronome metronome)
        {
            metronome.Interval += new Metronome.IntervalHandler(HeardIt);
        }

        private void HeardIt(Metronome metronome)
        {
            Console.WriteLine(this.Name + " heard the " + metronome.Name +"'s event");
        }
    }

    class Program
    {
        static void Task1()
        {
            Metronome m1 = new Metronome("M1", 3000);
            Metronome m2 = new Metronome("M2", 1000);

            Listener listener1 = new Listener("Listener1");
            Listener listener2 = new Listener("Listener2");
            listener1.Subscribe(m1);
            listener2.Subscribe(m2);
            TickClass.DoTicks();
        }

        static void Task2()
        {
            List<Metronome> list = new List<Metronome>();

            //Creating metronomes
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new Metronome("Metronome" + i, rnd.Next(500, 2000)));
            }

            //Setting listener
            Listener listener = new Listener("Main listener");

            for (int i = 0; i < list.Count; i++)
            {
                listener.Subscribe(list[i]);
            }

            TickClass.DoTicks(10000);
        }
        static void Task3()
        {
            int switchTime = 1000;
            Metronome red = new Metronome("Red", 4 * switchTime, -4 * switchTime);
            Metronome orange = new Metronome("Orange", 2 * switchTime, -1 * switchTime);
            Metronome green = new Metronome("Green", 4 * switchTime, -2 * switchTime);

            Listener padestrian = new Listener("Padestrian");
            padestrian.Subscribe(red);
            padestrian.Subscribe(orange);
            padestrian.Subscribe(green);

            TickClass.DoTicks();
        }
        

        static void Main()
        {
            Task3();
        }
    }
}