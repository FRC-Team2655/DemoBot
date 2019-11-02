using System;
using Microsoft.SPOT;

namespace DemoBot
{
    public class RobotMap
    {
        // Button IDs
        public const int BACK = 9;
        public const int START = 10;
        public const int DRIVE_AXIS = 1;
        public const int ROTATE_AXIS = 2;
        public const int B_KEY = 3;
        public const int LEFT_TRIGGER = 7;
        public const int RIGHT_TRIGGER = 8;

        // Motor Controller IDs
        public const int LEFT_MASTER = 1;
        public const int LEFT_SLAVE = 2;
        public const int RIGHT_MASTER = 4;
        public const int RIGHT_SLAVE = 5;
        public const int TOP_INTAKE = 6;
        public const int BOTTOM_INTAKE = 7;
        public const int SHOOTER_WHEEL_SLAVE = 5;
        public const int SHOOTER_WHEEL_MASTER = 6;

        public const double MAX_WHEEL_SPEED = 0.65;
        public const double WHEEL_SPEED_INCR = 0.002;
        public const double INTAKE_SPEED = 0.25;
    }
}