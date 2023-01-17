using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Delfinovin
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GamecubeControllerButtons : long
    {
        None = 0,
        A = 1 << 0,
        B = 1 << 1,
        X = 1 << 2,
        Y = 1 << 3,
        Z = 1 << 4,
        L = 1 << 5,
        R = 1 << 6,
        Start = 1 << 7,

        DpadLeft = 1 << 8,
        DpadUp = 1 << 9,
        DpadRight = 1 << 10,
        DpadDown = 1 << 11,

        LAnalog = 1 << 12,
        RAnalog = 1 << 13,
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Sticks : long
    {
        None = 0,
        LStickLeft = 1 << 1,
        LStickUp = 1 << 2,
        LStickRight = 1 << 3,
        LStickDown = 1 << 4,

        RStickLeft = 1 << 5,
        RStickUp = 1 << 6,
        RStickRight = 1 << 7,
        RStickDown = 1 << 8,

        LStick = 1 << 9,
        RStick = 1 << 10,

        // Generic Catch-all
        Up = LStickUp | RStickUp,
        Down = LStickDown | RStickDown,
        Left = LStickLeft | RStickLeft,
        Right = LStickRight | RStickRight,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum XboxControllerButtons
    {
        None,
        A, B, X, Y,
        LeftThumbButton, RightThumbButton,
        LeftShoulder, RightShoulder, LeftShoulderAnalog, RightShoulderAnalog,
        DPadLeft, DPadRight, DPadDown, DPadUp,
        Start, Guide, Back
    }
}
