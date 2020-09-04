using System;
using System.Runtime.InteropServices;
using System.Timers;
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
        };

        static void Main(string[] args)
            => new Program().Run();

        void Run()
        {
            if (!LogitechGSDK.LogiLedInit())//WithName("Keypad fix. See https://github.com/cschladetsch"))
            {
                Console.WriteLine("Failed to start LogiTech SDK. Plug in a keyboard or something.");
                return;
            }

            LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_ALL);
            SetNumKeyState();

            // poll the key every 100ms
            var timer = new Timer(100) {Enabled = true}; // milliseconds
            timer.Elapsed += (sender, eventArgs) => { SetNumKeyState(); };

            Console.WriteLine("Press \"ENTER\" to close.");
            Console.ReadLine();

            LogitechGSDK.LogiLedShutdown();
        }

        private void SetNumKeyState()
        {
            var numLocked = (((ushort) GetKeyState(0x90)) & 0xffff) == 0;
            ToggleNumKeys(numLocked);
        }

        private void ToggleNumKeys(bool numLocked)
        {
            if (_numLocked == numLocked)
                return;

            _numLocked = numLocked;

            SetNumKeyColors();
        }

        void SetNumKeyColors()
        {
            int onRed = 255, onGreen = 20, onBlue = 0;
            int offRed = 203, offGreen = 255, offBlue = 10;
            var r = _numLocked ? onRed : offRed;
            var g = _numLocked ? onGreen : offGreen;
            var b = _numLocked ? onBlue : offBlue;

            foreach (var key in _numKeys)
            {
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key, r, g, b);
            }
        }
    }
}
