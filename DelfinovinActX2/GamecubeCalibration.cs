using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelfinovinActX2
{
    public class GamecubeCalibration
    {
        private byte[] _LeftStickMinMax = new byte[4] { 127, 127, 127, 127 }; // x_min, x_max, y_min, y_max
        private byte[] _CStickMinMax = new byte[4] { 127, 127, 127, 127 };

        public float[] LeftStickCalibration = new float[4]; // x-mult, y-mult, x-off, y-off
        public float[] CStickCalibration = new float[4];

        public int[] StickCenters = new int[4];

        public CalibrationStatus CurrentStatus;

        public GamecubeCalibration()
        {
            CurrentStatus = CalibrationStatus.Uncalibrated;
        }

        public void SetStickCenters(GamecubeInputState controllerInput)
        {
            StickCenters[0] = 127 - controllerInput.LEFT_STICK_X;
            StickCenters[1] = 127 - controllerInput.LEFT_STICK_Y;
            StickCenters[2] = 127 - controllerInput.C_STICK_X;
            StickCenters[3] = 127 - controllerInput.C_STICK_Y;

            CurrentStatus = CalibrationStatus.Centered;
        }

        public void GetMinMax(GamecubeInputState controllerInput)
        {
            // There is probably a better way of getting min-max values for the sticks, but this is the easiest way I think.
            if (_LeftStickMinMax[0] > controllerInput.LEFT_STICK_X)
                _LeftStickMinMax[0] = controllerInput.LEFT_STICK_X;

            if (_LeftStickMinMax[1] < controllerInput.LEFT_STICK_X)
                _LeftStickMinMax[1] = controllerInput.LEFT_STICK_X;

            if (_LeftStickMinMax[2] > controllerInput.LEFT_STICK_Y)
                _LeftStickMinMax[2] = controllerInput.LEFT_STICK_Y;

            if (_LeftStickMinMax[3] < controllerInput.LEFT_STICK_Y)
                _LeftStickMinMax[3] = controllerInput.LEFT_STICK_Y;

            if (_CStickMinMax[0] > controllerInput.C_STICK_X)
                _CStickMinMax[0] = controllerInput.C_STICK_X;

            if (_CStickMinMax[1] < controllerInput.C_STICK_X)
                _CStickMinMax[1] = controllerInput.C_STICK_X;

            if (_CStickMinMax[2] > controllerInput.C_STICK_Y)
                _CStickMinMax[2] = controllerInput.C_STICK_Y;

            if (_CStickMinMax[3] < controllerInput.C_STICK_Y)
                _CStickMinMax[3] = controllerInput.C_STICK_Y;

            CurrentStatus = CalibrationStatus.Calibrating;
        }

        public void GenerateCalibrations()
        {
            LeftStickCalibration = GenerateCoefficients(_LeftStickMinMax);
            CStickCalibration = GenerateCoefficients(_CStickMinMax);

            CurrentStatus = CalibrationStatus.Calibrated;
        }

        private float[] GenerateCoefficients(byte[] minMax)
        {
            // Thanks to @Barking_Frogssb for these mathematics
            float[] stickCalibration = new float[4];

            stickCalibration[0] = 256f / (minMax[1] - minMax[0]);
            stickCalibration[1] = 256f / (minMax[3] - minMax[2]);
            stickCalibration[2] = 127f * stickCalibration[0] - 127f;
            stickCalibration[3] = 127f * stickCalibration[1] - 127f;

            return stickCalibration;
        }
    }

    public enum CalibrationStatus
    {
        Uncalibrated,
        Centered,
        Calibrating,
        Calibrated
    }
}
