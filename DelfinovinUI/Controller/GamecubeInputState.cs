namespace DelfinovinUI
{
	public class GamecubeInputState
	{
		public bool IsPowered;
		public bool NormalType;
		public bool WavebirdType;

		public bool BUTTON_A;
		public bool BUTTON_B;
		public bool BUTTON_X;
		public bool BUTTON_Y;
		public bool DPAD_LEFT;
		public bool DPAD_RIGHT;
		public bool DPAD_DOWN;
		public bool DPAD_UP;

		public bool BUTTON_START;
		public bool BUTTON_Z;
		public bool BUTTON_R;
		public bool BUTTON_L;

		public byte LEFT_STICK_X = 127;
		public byte LEFT_STICK_Y = 127;
		public byte C_STICK_X = 127;
		public byte C_STICK_Y = 127;

		public byte ANALOG_LEFT;
		public byte ANALOG_RIGHT;

		public bool IsEqual(GamecubeInputState compareState)
		{
			bool faceButtons = BUTTON_A == compareState.BUTTON_A && 
								BUTTON_B == compareState.BUTTON_B && 
								BUTTON_X == compareState.BUTTON_X && 
								BUTTON_Y == compareState.BUTTON_Y && 
								BUTTON_START == compareState.BUTTON_START;

			bool dpadButtons = DPAD_LEFT == compareState.DPAD_LEFT && 
								DPAD_RIGHT == compareState.DPAD_RIGHT && 
								DPAD_DOWN == compareState.DPAD_DOWN && 
								DPAD_UP == compareState.DPAD_UP;

			bool leftStick = LEFT_STICK_X == compareState.LEFT_STICK_X &&
								LEFT_STICK_Y == compareState.LEFT_STICK_Y;

			bool rightStick = C_STICK_X == compareState.C_STICK_X && 
								C_STICK_Y == compareState.C_STICK_Y;

			bool triggerAnalog = ANALOG_LEFT == compareState.ANALOG_LEFT && 
									ANALOG_RIGHT == compareState.ANALOG_RIGHT;

			return faceButtons && dpadButtons && leftStick && rightStick && triggerAnalog;
		}

		public bool IsPlugged()
		{
			return NormalType || WavebirdType;
		}
	}
}
