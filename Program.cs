using System;
using System.Collections.Generic;
using System.Linq;

namespace CPFinalProject
{
    class Program
    {
        static void Main(string[] args)
        {
            const int totalAgents = 5;
            List<Agent> agents = new List<Agent>(totalAgents);
            Elevator elevator = new Elevator();

            elevator.Start();

            for (int i = 0; i < totalAgents; i++)
            {
                agents.Add(new Agent(i, elevator));
                agents[i].StartDay();
            }

            while(agents.Any(a => !a.isWorkOver))
            {

            }
            elevator.workDayEnd.Set();



            Console.WriteLine("End Of Day");
        }
    }
}
