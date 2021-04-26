using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelfinovinActX2
{
    public class GamecubeInputState
    {
        // Byte 1
        public bool IsPowered; // bit 2, If the adapter is plugged in
        public bool NormalType; // bit 4, whether or not the controller is wireless or not.
        public bool WavebirdType; // bit 5

        // Byte 2
        public bool BUTTON_A; // bit 0
        public bool BUTTON_B;
        public bool BUTTON_X;
        public bool BUTTON_Y;
        public bool DPAD_LEFT;
        public bool DPAD_RIGHT;
        public bool DPAD_DOWN;
        public bool DPAD_UP; // bit 7

        // Byte 3
        public bool BUTTON_START; // bit 0
        public bool BUTTON_Z;
        public bool BUTTON_R;
        public bool BUTTON_L; // bit 3

        // Bytes 4 through 9
        public byte LEFT_STICK_X = 127; //  4
        public byte LEFT_STICK_Y = 127; // 5
        public byte C_STICK_X = 127; // 6
        public byte C_STICK_Y = 127; // 7

        public byte ANALOG_LEFT; // 8
        public byte ANALOG_RIGHT; // 9

        public bool IsEqual(GamecubeInputState compareState)
        {
            bool faceButtons = BUTTON_A == compareState.BUTTON_A
                && BUTTON_B == compareState.BUTTON_B
                && BUTTON_X == compareState.BUTTON_X
                && BUTTON_Y == compareState.BUTTON_Y
                && BUTTON_START == compareState.BUTTON_START;

            bool dpadButtons = DPAD_LEFT == compareState.DPAD_LEFT
                && DPAD_RIGHT == compareState.DPAD_RIGHT
                && DPAD_DOWN == compareState.DPAD_DOWN
                && DPAD_UP == compareState.DPAD_UP;

            bool leftStick = LEFT_STICK_X == compareState.LEFT_STICK_X
                && LEFT_STICK_Y == compareState.LEFT_STICK_Y;

            bool rightStick = C_STICK_X == compareState.C_STICK_X
                && C_STICK_Y == compareState.C_STICK_Y;

            bool triggerAnalog = ANALOG_LEFT == compareState.ANALOG_LEFT
                && ANALOG_RIGHT == compareState.ANALOG_RIGHT;

            return faceButtons && dpadButtons && leftStick && rightStick && triggerAnalog;
        }

        public bool IsPlugged()
        {
            return NormalType || WavebirdType;
        }
        
        public override string ToString()
        {
            string output = "";
            output += string.Format(Strings.INFO_STICK, LEFT_STICK_X, LEFT_STICK_Y);
            output += string.Format(Strings.INFO_CSTICK, C_STICK_X, C_STICK_Y);
            output += string.Format(Strings.INFO_FACEBUTTONS,
                        ConvertYesNo(BUTTON_A), ConvertYesNo(BUTTON_B),
                        ConvertYesNo(BUTTON_X), ConvertYesNo(BUTTON_Y),
                        ConvertYesNo(BUTTON_START));

            output += string.Format(Strings.INFO_TRIGGERS, ANALOG_LEFT, ANALOG_RIGHT, ConvertYesNo(BUTTON_Z));
            output += string.Format(Strings.INFO_DPAD,
                        ConvertYesNo(DPAD_LEFT), ConvertYesNo(DPAD_RIGHT),
                        ConvertYesNo(DPAD_UP), ConvertYesNo(DPAD_DOWN));
            output += "\n";
            return output;
        }
        
        private string ConvertYesNo(bool input)
        {
            return input ? "Yes" : "No";
        }
    }
}
