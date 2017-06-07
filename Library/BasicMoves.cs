using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    /// <summary>
    /// Hexapod Movement Source (18 servos)
    /// http://orangenarwhals.blogspot.bg/2011/06/how-to-set-up-arduino-pololu-mini.html
    /// </summary>
    public abstract class BasicMoves
    {
        public const double UP = 0.6;
        public const double DOWN = 0.1;
        public const double FORWARD = 0.2;
        public const double BACKWARD = 0.8;

        protected Dictionary<byte, byte> legMap = new Dictionary<byte, byte>
        {
            {0,7 },
            {1,8 },
            {2,9 },
            {3,10 },
            {4,11 },
            {5,12 },
            {6,14 },
            {7,15 },
            {8,1 },
            {9,2 },
            {10,0 },
            {11,13 },
            {12,byte.MaxValue },
            {13,byte.MaxValue },
            {14,byte.MaxValue },
            {15,byte.MaxValue }
        };

        protected Dictionary<byte, MinMaxServo> ServoValues = new Dictionary<byte, MinMaxServo>
        {
            {6,new MinMaxServo{ MinValue=0.2,MaxValue=0.8} },
            {7,new MinMaxServo{ MinValue=0.2,MaxValue=0.8} },
            {8,new MinMaxServo{ MinValue=0.2,MaxValue=0.8} },
            {9,new MinMaxServo{ MinValue=0.2,MaxValue=0.8} },
            {10,new MinMaxServo{ MinValue=0.2,MaxValue=0.8} },
            {11,new MinMaxServo{ MinValue=0.2,MaxValue=0.8} },
        };

        protected Dictionary<byte, double> f0 = new Dictionary<byte, double>
        {
            {0, UP },
            {4, UP },
            {2, UP },
        };

        protected Dictionary<byte, double> f1 = new Dictionary<byte, double>
        {
            {6, FORWARD },
            {8, FORWARD },
            {10, FORWARD },
            {7, BACKWARD },
            {9, BACKWARD },
            {11, BACKWARD }
        };

        protected double ReverseMove(byte servo, double value)
        {
            if (servo > 6 && servo < 10)
            {
                if (value == FORWARD)
                {
                    return BACKWARD;
                }
                else
                {
                    return FORWARD;
                }
            }
            else
            {
                return value;
            }
        }

        protected Dictionary<byte, double> f2 = new Dictionary<byte, double>
        {
            {0, DOWN },
            {4, DOWN },
            {2, DOWN },
            {1, UP },
            {3, UP },
            {5, UP }
        };

        protected Dictionary<byte, double> f3 = new Dictionary<byte, double>
        {
            {6, BACKWARD },
            {8, BACKWARD },
            {10, BACKWARD },
            {7, FORWARD },
            {9, FORWARD },
            {11, FORWARD }
        };

        protected Dictionary<byte, double> f4 = new Dictionary<byte, double>
        {
            {0, UP },
            {4, UP },
            {2, UP },
            {1, DOWN },
            {3, DOWN },
            {5, DOWN }
        };

        protected class MinMaxServo
        {
            public double MinValue { get; set; }
            public double MaxValue { get; set; }
        }

        protected enum MovesEnum
        {
            FORWARD,
            BACKWARD
        }

    }
}