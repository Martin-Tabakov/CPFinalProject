using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CPFinalProject
{

    class Elevator
    {

        Agent agentCalledElevator = null;
        Queue<Agent> queuedCalls = new Queue<Agent>();
        internal Floor currentElevatorFloor;
        internal Floor targetElevatorFloor;
        public ManualResetEvent workDayEnd = new ManualResetEvent(false);

        public Elevator()
        {
            currentElevatorFloor = Floor.Ground;
        }
        public void Start()
        {
            Thread thread = new Thread(StartElevator);
            thread.Start();
        }
        private void StartElevator()
        {
            while (!workDayEnd.WaitOne(0))
            {
                if (agentCalledElevator == null)
                {
                    if (queuedCalls.TryPeek(out agentCalledElevator))
                    {
                        targetElevatorFloor = agentCalledElevator.currentFloor;
                        if (targetElevatorFloor != currentElevatorFloor)
                        {
                            MoveEmptyElevator();
                        }
                        else SendElevator();
                    }
                }
                else
                {
                    SendElevator();
                }
            }
        }
        private void MoveEmptyElevator()
        {
            Console.WriteLine($"Elevator moving from {currentElevatorFloor} to {targetElevatorFloor}");
            MoveDelay();
            currentElevatorFloor = targetElevatorFloor;
            queuedCalls.Dequeue();
            
        }
        public void Call(Agent agent)
        {
            Console.WriteLine($"Agent {agent.id} Called the elevator to {agent.currentFloor}, wants to go to {agent.desiredFloor}");
            queuedCalls.Enqueue(agent);
        }
        private void SendElevator()
        {
            Console.WriteLine($"Elevator moving from {currentElevatorFloor} to {agentCalledElevator.desiredFloor}, carying Agent {agentCalledElevator.id}");
            MoveDelay();
            targetElevatorFloor = agentCalledElevator.desiredFloor;
            currentElevatorFloor = targetElevatorFloor;
            Leave();
            
        }
        private void MoveDelay()
        {
            int time = Math.Abs(targetElevatorFloor - currentElevatorFloor) * 1000;
            Thread.Sleep(time);
        }
        public void Leave()
        {
            Console.WriteLine($"Agent {agentCalledElevator.id} Leaves the elevator at floor {currentElevatorFloor}");
            agentCalledElevator.canWork.Set();
            //agentCalledElevator.currentFloor = agentCalledElevator.desiredFloor;
            agentCalledElevator = null;
        }
    }
}
