using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// Provide a class of button image, names and button values
    /// so images are not required to be created multiple times.
    /// </summary>
    public class ButtonImagePairs
    {
        public static Dictionary<XboxControllerButtons, Tuple<string, BitmapImage>> XboxButtonPairs = new Dictionary<XboxControllerButtons, Tuple<string, BitmapImage>>
        {
            { XboxControllerButtons.A, new Tuple<string, BitmapImage>("A Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/A.png", UriKind.Relative)))},
            { XboxControllerButtons.B, new Tuple<string, BitmapImage>("B Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/B.png", UriKind.Relative))) },
            { XboxControllerButtons.X, new Tuple<string, BitmapImage>("X Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/X.png", UriKind.Relative))) },
            { XboxControllerButtons.Y, new Tuple<string, BitmapImage>("Y Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Y.png", UriKind.Relative))) },
            { XboxControllerButtons.LeftThumbButton, new Tuple<string, BitmapImage>("Left Stick (Click)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Left Stick Click.png", UriKind.Relative))) },
            { XboxControllerButtons.RightThumbButton, new Tuple<string, BitmapImage>("Right Stick (Click)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Right Stick Click.png", UriKind.Relative))) },
            { XboxControllerButtons.LeftShoulder, new Tuple<string, BitmapImage>("Left Bumper", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Left Bumper.png", UriKind.Relative))) },
            { XboxControllerButtons.RightShoulder, new Tuple<string, BitmapImage>("Right Bumper", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Right Bumper.png", UriKind.Relative))) },
            { XboxControllerButtons.LeftShoulderAnalog, new Tuple<string, BitmapImage>("Left Trigger (Analog)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Left Trigger.png", UriKind.Relative))) },
            { XboxControllerButtons.RightShoulderAnalog, new Tuple<string, BitmapImage>("Right Trigger (Analog)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Right Trigger.png", UriKind.Relative))) },
            { XboxControllerButtons.DPadLeft, new Tuple<string, BitmapImage>("D-Pad Left", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/D-Pad Left.png", UriKind.Relative))) },
            { XboxControllerButtons.DPadRight, new Tuple<string, BitmapImage>("D-Pad Right", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/D-Pad Right.png", UriKind.Relative))) },
            { XboxControllerButtons.DPadUp, new Tuple<string, BitmapImage>("D-Pad Up", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/D-Pad Up.png", UriKind.Relative))) },
            { XboxControllerButtons.DPadDown, new Tuple<string, BitmapImage>("D-Pad Down", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/D-Pad Down.png", UriKind.Relative))) },
            { XboxControllerButtons.Guide, new Tuple<string, BitmapImage>("Guide", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Guide.png", UriKind.Relative))) },
            { XboxControllerButtons.Start, new Tuple<string, BitmapImage>("Start", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Start.png", UriKind.Relative))) },
            { XboxControllerButtons.Back, new Tuple<string, BitmapImage>("Back", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Xbox/Back.png", UriKind.Relative))) },
        };

        public static Dictionary<GamecubeControllerButtons, Tuple<string, BitmapImage>> GamecubeButtonPairs = new Dictionary<GamecubeControllerButtons, Tuple<string, BitmapImage>>()
        {
            { GamecubeControllerButtons.A, new Tuple<string, BitmapImage>("A Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/A.png", UriKind.Relative))) },
            { GamecubeControllerButtons.B, new Tuple<string, BitmapImage>("B Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/B.png", UriKind.Relative))) },
            { GamecubeControllerButtons.X, new Tuple<string, BitmapImage>("X Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/X.png", UriKind.Relative))) },
            { GamecubeControllerButtons.Y, new Tuple<string, BitmapImage>("Y Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/Y.png", UriKind.Relative))) },
            { GamecubeControllerButtons.Z, new Tuple<string, BitmapImage>("Z Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/Z.png", UriKind.Relative))) },
            { GamecubeControllerButtons.Start, new Tuple<string, BitmapImage>("Start Button", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/Start.png", UriKind.Relative))) },
            { GamecubeControllerButtons.L, new Tuple<string, BitmapImage>("Left Trigger (Digital)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/L Digital.png", UriKind.Relative))) },
            { GamecubeControllerButtons.R, new Tuple<string, BitmapImage>("Right Trigger (Digital)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/R Digital.png", UriKind.Relative))) },
            { GamecubeControllerButtons.DpadLeft, new Tuple<string, BitmapImage>("D-Pad Left", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/D-Pad Left.png", UriKind.Relative))) },
            { GamecubeControllerButtons.DpadRight, new Tuple<string, BitmapImage>("D-Pad Right", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/D-Pad Right.png", UriKind.Relative))) },
            { GamecubeControllerButtons.DpadUp, new Tuple<string, BitmapImage>("D-Pad Up", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/D-Pad Up.png", UriKind.Relative))) },
            { GamecubeControllerButtons.DpadDown, new Tuple<string, BitmapImage>("D-Pad Down", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/D-Pad Down.png", UriKind.Relative))) },
            { GamecubeControllerButtons.LAnalog, new Tuple<string, BitmapImage>("Left Trigger (Analog)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/L Analog.png", UriKind.Relative))) },
            { GamecubeControllerButtons.RAnalog, new Tuple<string, BitmapImage>("Right Trigger (Analog)", new BitmapImage(new Uri("/Delfinovin;component/Resources/Icons/Buttons/Gamecube/R Analog.png", UriKind.Relative))) },
        };
    }
}
