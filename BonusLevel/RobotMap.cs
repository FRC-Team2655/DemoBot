using System;
using Microsoft.SPOT;

namespace BonusLevel {
    public class RobotMap {
        // Button IDs
        public const int BACK = 9;
        public const int START = 10;
        public const int INTAKE_IN = 6;
        public const int INTAKE_OUT = 5;
        public const int DRIVE_AXIS = 1;
        public const int ROTATE_AXIS = 2;
        public const int RAISE_INTAKE = 2;
        public const int LOWER_INTAKE = 3;
        public const int OPEN_INTAKE = 8;

        // Solenoid IDs
        public const int RAISE_LOWER_A = 4;
        public const int RAISE_LOWER_B = 3;
        public const int OPEN_CLOSE_A = 2;
        public const int OPEN_CLOSE_B = 1;

        // Motor Controller IDs
        public const int LEFT_MASTER = 1;
        public const int LEFT_SLAVE1 = 2;
        public const int LEFT_SLAVE2 = 3;
        public const int RIGHT_MASTER = 4;
        public const int RIGHT_SLAVE1 = 5;
        public const int RIGHT_SLAVE2 = 6;
        public const int INTAKE1 = 7;
        public const int INTAKE2 = 8;
    }
}