using System;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// Represent the minimum and maximum values a
    /// Gamecube controller can have. 
    /// </summary>
    public class GamecubeCalibration
    {
        // TO-DO: This class could probably be turned into a struct
        public byte[] LeftStickMinMax = new byte[4] { 127, 127, 127, 127 };
        public byte[] RightStickMinMax = new byte[4] { 127, 127, 127, 127 };

        public void SetMinMax(ControllerStatus controllerInput)
        {
            // Get the minimum/maximum values for each stick
            LeftStickMinMax[0] = (byte)Math.Min(LeftStickMinMax[0], controllerInput.LStick.X);
            LeftStickMinMax[1] = (byte)Math.Max(LeftStickMinMax[1], controllerInput.LStick.X);
            LeftStickMinMax[2] = (byte)Math.Min(LeftStickMinMax[2], controllerInput.LStick.Y);
            LeftStickMinMax[3] = (byte)Math.Max(LeftStickMinMax[3], controllerInput.LStick.Y);

            RightStickMinMax[0] = (byte)Math.Min(RightStickMinMax[0], controllerInput.RStick.X);
            RightStickMinMax[1] = (byte)Math.Max(RightStickMinMax[1], controllerInput.RStick.X);
            RightStickMinMax[2] = (byte)Math.Min(RightStickMinMax[2], controllerInput.RStick.Y);
            RightStickMinMax[3] = (byte)Math.Max(RightStickMinMax[3], controllerInput.RStick.Y);
        }

        public float[] GetRange()
        {
            // Get both left and right stick ranges by adding the left/right
            // min max values and averaging them together.
            float[] ranges = new float[2];
            float leftStickX = (LeftStickMinMax[1] - LeftStickMinMax[0]) / 255.0f;
            float leftStickY = (LeftStickMinMax[3] - LeftStickMinMax[2]) / 255.0f;
            ranges[0] = (float)Math.Round((leftStickX + leftStickY) / 2.0f, 2, MidpointRounding.ToEven);

            float rightStickX = (RightStickMinMax[1] - RightStickMinMax[0]) / 255.0f;
            float rightStickY = (RightStickMinMax[3] - RightStickMinMax[2]) / 255.0f;
            ranges[1] = (float)Math.Round((rightStickX + rightStickY) / 2.0f, 2, MidpointRounding.ToEven);

            return ranges;
        }

        public override string ToString()
        {
            string calibration = $"LeftStickMinMax: Left X/Y - [{LeftStickMinMax[0]}/{LeftStickMinMax[1]}], " +
                                                  $"Right X/Y - [{LeftStickMinMax[2]}/{LeftStickMinMax[3]}]\n" +

                                 $"RightStickMinMax: Left X/Y - [{RightStickMinMax[0]}/{RightStickMinMax[1]}], " +
                                                   $"Right X/Y - [{RightStickMinMax[2]}/{RightStickMinMax[3]}]";
            return calibration;
        }
    }

}
