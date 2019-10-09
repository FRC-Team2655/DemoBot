using System;
using Microsoft.SPOT;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix;
using Math = System.Math;
using CTRE.Phoenix.MotorControl.CAN;
using CTRE.Phoenix.MotorControl;

namespace BonusLevel {
    public class Robot {
        bool haveEnabledButtonsBeenReleased = true;

        public enum State { Enabled, Disabled, None };

        GameController js0 = null;

        CTRE.Phoenix.PneumaticControlModule pcm = new PneumaticControlModule(0);

        TalonSRX leftMaster = new TalonSRX(RobotMap.LEFT_MASTER);
        TalonSRX leftSlave1 = new TalonSRX(RobotMap.LEFT_SLAVE1);
        TalonSRX leftSlave2 = new TalonSRX(RobotMap.LEFT_SLAVE2);
        TalonSRX rightMaster = new TalonSRX(RobotMap.RIGHT_MASTER);
        TalonSRX rightSlave1 = new TalonSRX(RobotMap.RIGHT_SLAVE1);
        TalonSRX rightSlave2 = new TalonSRX(RobotMap.RIGHT_SLAVE2);

        private State currentState = State.None, newState = State.Disabled;

        private void robotInit() {
            js0 = new GameController(UsbHostDevice.GetInstance());
            leftSlave1.Follow(leftMaster);
            leftSlave2.Follow(leftMaster);
            rightSlave1.Follow(rightMaster);
            rightSlave2.Follow(rightMaster);

            rightMaster.SetInverted(true);
            rightSlave1.SetInverted(true);
            rightSlave2.SetInverted(true);
        }

        private void robotPeriodic() {
            // If both buttons have been pressed, enable the robot
            if (js0.GetButton(RobotMap.BACK) && js0.GetButton(RobotMap.START) && currentState == State.Disabled) {
                switchState(State.Enabled);
                haveEnabledButtonsBeenReleased = false;
            }

            // If both buttons have been released, allow the ability to disable
            if (!js0.GetButton(RobotMap.BACK) && !js0.GetButton(RobotMap.START) && !haveEnabledButtonsBeenReleased) {
                haveEnabledButtonsBeenReleased = true;
            }

            // If either button has been pressed, disable
            if (haveEnabledButtonsBeenReleased && (js0.GetButton(RobotMap.BACK) || js0.GetButton(RobotMap.START)) && currentState == State.Enabled) {
                switchState(State.Disabled);
            }
        }

        private void enabledInit() {
            Debug.Print("Enabled");
            pcm.StartCompressor();

            leftMaster.SetNeutralMode(NeutralMode.Coast);
            leftSlave1.SetNeutralMode(NeutralMode.Coast);
            leftSlave2.SetNeutralMode(NeutralMode.Coast);
            rightMaster.SetNeutralMode(NeutralMode.Coast);
            rightSlave1.SetNeutralMode(NeutralMode.Coast);
            rightSlave2.SetNeutralMode(NeutralMode.Coast);
        }

        private void enabledPeriodic()
        {
            // 2 - left and right, right stick
            // 5 - up and down, right stick
            float power = -0.3f * js0.GetAxis(RobotMap.DRIVE_AXIS);

            // cubic function generated with ti regression model
            // 4x^3 - 6x^2 + 3x
            power = (float)(4 * Math.Pow(power, 3) - 6 * Math.Pow(power, 2) + 3 * power);

            float rotate = -0.4f * js0.GetAxis(RobotMap.ROTATE_AXIS);
            drivePercentage(power, rotate);
        }

        private void disabledInit() {
            Debug.Print("Disabled");
            leftMaster.SetNeutralMode(NeutralMode.Brake);
            leftSlave1.SetNeutralMode(NeutralMode.Brake);
            leftSlave2.SetNeutralMode(NeutralMode.Brake);
            rightMaster.SetNeutralMode(NeutralMode.Brake);
            rightSlave1.SetNeutralMode(NeutralMode.Brake);
            rightSlave2.SetNeutralMode(NeutralMode.Brake);
            pcm.StopCompressor();
        }

        private void disabledPeriodic() {
        }

        double[] arcadeDrive(float xSpeed, float zRotation) {
            float leftMotorOutput;
            float rightMotorOutput;

            // Prevent -0 from breaking the arcade drive...
            xSpeed += 0.0f;
            zRotation += 0.0f;

            float maxInput = (float) Math.Max(Math.Abs(xSpeed), Math.Abs(zRotation));
            // If xSpeed is negative make maxInput negative
            if (xSpeed < 0) maxInput *= -1;

            if (xSpeed >= 0.0) {
                // First quadrant, else second quadrant
                if (zRotation >= 0.0) {
                    leftMotorOutput = maxInput;
                    rightMotorOutput = xSpeed - zRotation;
                } else {
                    leftMotorOutput = xSpeed + zRotation;
                    rightMotorOutput = maxInput;
                }
            } else {
                // Third quadrant, else fourth quadrant
                if (zRotation >= 0.0) {
                    leftMotorOutput = xSpeed + zRotation;
                    rightMotorOutput = maxInput;
                } else {
                    leftMotorOutput = maxInput;
                    rightMotorOutput = xSpeed - zRotation;
                }
            }

            return new double[] { leftMotorOutput, rightMotorOutput };
        }

        public void drivePercentage(float speed, float rotation) {
            double[] speeds = arcadeDrive(speed, rotation);
            leftMaster.Set(ControlMode.PercentOutput, speeds[1]);
            rightMaster.Set(ControlMode.PercentOutput, speeds[0]);
        }



        /// ////////////////////////////////
        /// DO NOT TOUCH!!!
        //////////////////////////////////////
        public void feed() {
            // Run robotInit if there is no current state
            if (currentState == State.None) {
                robotInit();
            }

            // Change the current state (and run init funciton) if there is a new state
            if (newState != currentState) {
                currentState = newState;

                switch(currentState) {
                    case State.Enabled:
                        enabledInit();
                        break;
                    case State.Disabled:
                        disabledInit();
                        break;
                }
            }

            switch (currentState) {
                case State.Enabled:
                    enabledPeriodic();
                    break;
                case State.Disabled:
                    disabledPeriodic();
                    break;
            }
            periodicSafetyCheck();
            robotPeriodic();
        }

        public void switchState(State state) {
            // Do not allow enable without the game controller being enabled
            if (state == State.Enabled && js0.GetConnectionStatus() == UsbDeviceConnection.NotConnected) {
                return;
            } 
            newState = state;
        }

        UsbDeviceConnection lastConnectionStatus = UsbDeviceConnection.NotConnected;
        public void periodicSafetyCheck() {
            if (currentState == State.Enabled) {
                // Keeps talonsrx enabled
                CTRE.Phoenix.Watchdog.Feed();
            }

            // was the Game Controller disconnected safety check
            UsbDeviceConnection currentConnectionStatus = js0.GetConnectionStatus();

            if (currentConnectionStatus == UsbDeviceConnection.NotConnected && lastConnectionStatus == UsbDeviceConnection.Connected) {
                switchState(State.Disabled);
            }

            lastConnectionStatus = currentConnectionStatus;
        }




        //////////////////////////////////////
        /// Debug helper functions
        //////////////////////////////////////

        void printGamepadAxes() {
            GameControllerValues values = new GameControllerValues();
            js0.GetAllValues(ref values);
            string output = "";

            for (int i = 0; i < values.axes.Length; ++i) {
                output += values.axes[i] + ",";
            }

            Debug.Print(output);
        }

        void printGamepadButtons() {
            GameControllerValues values = new GameControllerValues();
            js0.GetAllValues(ref values);
            string output = "";

            for (int i = 0; i < 15; ++i) { 
                uint current = values.btns & 0x1;
                values.btns = values.btns >> 1;

                output += current + ",";
            }
            Debug.Print(output);
        }
    }
}
