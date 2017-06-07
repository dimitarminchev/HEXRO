using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    public sealed class Commander : BasicMoves
    {
        // Private Part
        private object padlock = new object();
        private Timer timer;
        private volatile bool walking;
        public const byte MOTOR_COUNT = 16;
        private ConcurrentQueue<string> commands;

        public void EnqueueCommand(string command)
        {
            this.commands.Enqueue(command);
        }

        private static Commander _instance;
        public static Commander Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Commander();
                }
                return _instance;
            }
        }

        // Constructor
        private Commander()
        {
            commands = new ConcurrentQueue<string>();
            walking = false;
            timer = new Timer(new TimerCallback(TimerTick), null, 500, 500);
        }

        // Timer
        private void TimerTick(object state)
        {
            while (commands.Any())
            {
                string command = null;
                if (commands.TryDequeue(out command)) ParseCommand(command);
            }
        }

        /// <summary>
        /// Parse the Commands
        /// </summary>
        /// <param name="rawCommand">Raw Command for Execution</param>
        private void ParseCommand(string rawCommand)
        {
            walking = false;
            string command = null;
            if (rawCommand.IndexOf('|') != -1) command = rawCommand.Substring(0, rawCommand.IndexOf("|"));
            else command = rawCommand;

            switch (command)
            {
                case "stop": this.Stop(); break;
                case "rotate": this.Rotate(); break;
                case "walk":
                    {
                        walking = true;
                        this.Stop();
                        this.Prepare();
                        while (walking) this.Walk();                   
                    }
                    break;
                // Unrecognized Command
                default: break;
            }
        }

        // Stop
        private void Stop()
        {
            if (PwmDriverSingleton.DriverInstance != null && PwmDriverSingleton.DriverInstance.IsDevicedInited)
                for (byte i = 0; i < MOTOR_COUNT; i++)
                    PwmDriverSingleton.DriverInstance.DrivePercentage(i, 0.1);
            Task.Delay(200);
        }

        // Prepare 
        private void Prepare()
        {
            foreach (var move in f0)
            {
                Move(move.Key, move.Value);
            }
            Task.Delay(200).Wait();
        }

        /// <summary>
        /// Moving One Servo
        /// </summary>
        /// <param name="servo">Servo Number [0..11]</param>
        /// <param name="percent">Percent [0..1]</param>
        private void Move(byte servo, double percent)
        {
            if (PwmDriverSingleton.DriverInstance != null && PwmDriverSingleton.DriverInstance.IsDevicedInited)
            {
                try
                {
                    PwmDriverSingleton.DriverInstance.DrivePercentage(legMap[servo], percent);
                }
                catch { ;; } // On Error Do Nothing
            }
        }

        // Rotate
        private void Rotate()
        {
            foreach (var move in f1)  Move(move.Key, move.Value);            
            Task.Delay(250).Wait();

            foreach (var move in f2) Move(move.Key, move.Value);            
            Task.Delay(250).Wait();

            foreach (var move in f3) Move(move.Key, move.Value);            
            Task.Delay(250).Wait();

            foreach (var move in f4) Move(move.Key, move.Value);            
            Task.Delay(250).Wait();
        }

        // Walk
        private void Walk()
        {
            foreach (var move in f1) Move(move.Key, ReverseMove(move.Key, move.Value));            
            Task.Delay(250).Wait();

            foreach (var move in f2) Move(move.Key, move.Value);            
            Task.Delay(250).Wait();

            foreach (var move in f3) Move(move.Key, ReverseMove(move.Key, move.Value));            
            Task.Delay(250).Wait();

            foreach (var move in f4) Move(move.Key, move.Value);            
            Task.Delay(250).Wait();
        }
    }
}
