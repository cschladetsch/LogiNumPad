using System;
using System.Timers;
using System.Runtime.InteropServices;

using LedCSharp;

namespace LogiNumLock
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedFlashLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        private bool _numLocked;

        private readonly keyboardNames[] _numKeys =
        {
            keyboardNames.NUM_ZERO,
            keyboardNames.NUM_ONE,
            keyboardNames.NUM_TWO,
            keyboardNames.NUM_THREE,
            keyboardNames.NUM_FOUR,
            keyboardNames.NUM_FIVE,
            keyboardNames.NUM_SIX,
            keyboardNames.NUM_SEVEN,
            keyboardNames.NUM_EIGHT,
            keyboardNames.NUM_NINE,
            keyboardNames.NUM_PERIOD,
        };

        static void Main(string[] args)
            => new Program().Run();

        private void Run()
        {
            if (!LogitechGSDK.LogiLedInit())
            {
                Console.Error.WriteLine("Failed to start LogiTech SDK. Plug in a keyboard or something.");
                return;
            }

            LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_ALL);

            // Read and apply start keypad state
            SetNumKeyState();

            Console.WriteLine($"Monitoring Numlock. Current status={_numLocked}\n\n");

            const int pollTime = 60; // milliseconds
            var timer = new Timer(pollTime) {Enabled = true};
            timer.Elapsed += (sender, eventArgs) => { SetNumKeyState(); };

            Console.WriteLine("Press \"ENTER\" to close.");
            Console.ReadLine();

            LogitechGSDK.LogiLedShutdown();
        }

        private void SetNumKeyState()
        {
            var numLocked = ((ushort) GetKeyState(0x90) & 0xffff) == 0;
            ToggleNumKeys(numLocked);
        }

        private void ToggleNumKeys(bool numLocked)
        {
            if (_numLocked == numLocked)
                return;

            _numLocked = numLocked;

            SetNumKeyColors();
        }

        private void SetNumKeyColors()
        {
            var r = _numLocked ? 255 : 200;
            var g = _numLocked ? 0 : 255;
            var b = _numLocked ? 0 : 10;

            Console.WriteLine($"NumLock = {!_numLocked}");
            foreach (var key in _numKeys)
            {
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key, r, g, b);
            }
        }
    }
}
