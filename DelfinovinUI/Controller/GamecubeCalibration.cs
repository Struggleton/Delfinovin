namespace DelfinovinUI
{
	public class GamecubeCalibration
	{
		private byte[] _leftStickMinMax = new byte[4] { 127, 127, 127, 127 };
		private byte[] _cStickMinMax = new byte[4] { 127, 127, 127, 127 };

		public float[] LeftStickCalibration = new float[4];
		public float[] CStickCalibration = new float[4];
		public int[] StickCenters = new int[4];

		public void SetStickCenters(GamecubeInputState controllerInput)
		{
			StickCenters[0] = 127 - controllerInput.LEFT_STICK_X;
			StickCenters[1] = 127 - controllerInput.LEFT_STICK_Y;
			StickCenters[2] = 127 - controllerInput.C_STICK_X;
			StickCenters[3] = 127 - controllerInput.C_STICK_Y;
		}

		public void ResetCalibration()
		{
			_leftStickMinMax = new byte[4] { 127, 127, 127, 127 };
			_cStickMinMax = new byte[4] { 127, 127, 127, 127 };
			GenerateCalibrations();
		}

		public float[] GetRange()
		{
			float[] ranges = new float[2];
			ranges[0] = ((_leftStickMinMax[1] + _leftStickMinMax[3]) / 2) / 255f;
			ranges[1] = ((_cStickMinMax[1] + _cStickMinMax[3]) / 2) / 255f;

			return ranges;
		}

		public void SetMinMax(GamecubeInputState controllerInput)
		{
			// There should be a way to do this quicker. But this is the most simple way I think.
			if (_leftStickMinMax[0] > controllerInput.LEFT_STICK_X)
				_leftStickMinMax[0] = controllerInput.LEFT_STICK_X;

			if (_leftStickMinMax[1] < controllerInput.LEFT_STICK_X)
				_leftStickMinMax[1] = controllerInput.LEFT_STICK_X;

			if (_leftStickMinMax[2] > controllerInput.LEFT_STICK_Y)
				_leftStickMinMax[2] = controllerInput.LEFT_STICK_Y;

			if (_leftStickMinMax[3] < controllerInput.LEFT_STICK_Y)
				_leftStickMinMax[3] = controllerInput.LEFT_STICK_Y;

			if (_cStickMinMax[0] > controllerInput.C_STICK_X)
				_cStickMinMax[0] = controllerInput.C_STICK_X;

			if (_cStickMinMax[1] < controllerInput.C_STICK_X)
				_cStickMinMax[1] = controllerInput.C_STICK_X;

			if (_cStickMinMax[2] > controllerInput.C_STICK_Y)
				_cStickMinMax[2] = controllerInput.C_STICK_Y;

			if (_cStickMinMax[3] < controllerInput.C_STICK_Y)
				_cStickMinMax[3] = controllerInput.C_STICK_Y;
		}

		public void GenerateCalibrations()
		{
			LeftStickCalibration = GenerateCoefficients(_leftStickMinMax);
			CStickCalibration = GenerateCoefficients(_cStickMinMax);
		}

		private float[] GenerateCoefficients(byte[] minMax)
		{
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
