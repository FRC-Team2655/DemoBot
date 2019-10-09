using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace BonusLevel {
    public class Program {
        public const int TIMING = 20;

        public static void Main() {
            Robot robot = new Robot();

            while (true) {
                robot.feed();
                System.Threading.Thread.Sleep(TIMING);
            }
        }
    }
}
