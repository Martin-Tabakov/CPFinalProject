using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CPFinalProject
{
    enum Security
    {
        Confidential,
        Secret,
        TopSecret
    }
    enum Activity
    {
        CommingToWork,
        Working,
        LeavingWork
    }
    enum Floor
    {
        Ground,
        Secret,
        TopSecret1,
        TopSecret2
    }

    class Agent
    {
        public int id;
        public Floor currentFloor;
        public Floor desiredFloor;
        Security security;
        Activity activity;

        Elevator elevator;
        Thread thread;
        ManualResetEvent leaveWork = new ManualResetEvent(false);
        public ManualResetEvent canWork = new ManualResetEvent(false);
        Random random = new Random();

        public Agent(int id, Elevator elevator)
        {
            this.id = id;
            this.elevator = elevator;
            this.activity = Activity.CommingToWork;
            this.currentFloor = Floor.Ground;
            this.security = GenerateSecurityLevel();
        }

        public void StartDay()
        {
            thread = new Thread(StartDayPrivate);
            thread.Start();
        }

        private void StartDayPrivate()
        {
            Console.WriteLine($"Agent {id} arrived at work");

            while (activity != Activity.LeavingWork)
            {
                activity = GenerateActivity();
                desiredFloor = GetAccessibleFloor();
                switch (activity)
                {
                    case Activity.Working:
                        Work();
                        break;
                    case Activity.LeavingWork:
                        break;
                    default: throw new Exception("Activity not supported in switch statement!");
                }
            }
            LeaveWork();
        }
        private void Work()
        {
            if (currentFloor != desiredFloor)
            {
                elevator.Call(this);
                canWork.WaitOne();
            }
            DoWork();
        }

        private void DoWork()
        {
            int workDuration = random.Next(500, 1500);
            Console.WriteLine($"Agent {id} is working for {workDuration}");
            Thread.Sleep(workDuration);
            canWork.Reset();
        }
        private void LeaveWork()
        {
            desiredFloor = Floor.Ground;
            if (currentFloor == desiredFloor)
            {
                leaveWork.Set();
                Console.WriteLine($"Agent {id} left.");
            }
            else elevator.Call(this);
        }

        private Activity GenerateActivity()
        {

            int workCoefficent = 5;
            int leaveCoefficent = 2;
            int work = random.Next(10) * workCoefficent;
            int leave = random.Next(10) * leaveCoefficent;
            return (work > leave) ? Activity.Working : Activity.LeavingWork;
        }
        private Floor GetAccessibleFloor()
        {
            while (true)
            {
                Floor newFloor = (Floor)new Random().Next(Enum.GetNames(typeof(Floor)).Length);
                if (HasAccessToFloor(newFloor)) return newFloor;
            }
        }
        public bool HasAccessToFloor(Floor floor)
        {
            switch (security)
            {
                case Security.Confidential: return floor == Floor.Ground;
                case Security.Secret: return floor <= Floor.Secret;
                case Security.TopSecret: return floor <= Floor.TopSecret2;
                default: throw new Exception("Security level non-existent");
            }

        }
        public bool isWorkOver
        {
            get
            {
                return leaveWork.WaitOne(0);
            }
        }
        private Security GenerateSecurityLevel()
        {
            return (Security)new Random().Next(Enum.GetNames(typeof(Security)).Length);
        }
    }
}
