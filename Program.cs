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

            // poll the key every 100ms
            var timer = new Timer(100) {Enabled = true}; // milliseconds
            timer.Elapsed += (sender, eventArgs) =>
            {
                var numLocked = (((ushort) GetKeyState(0x90)) & 0xffff) != 0;
                ToggleNumKeys(numLocked);
            };

            LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(keyboardNames.L, 0, 100, 100);

            Console.WriteLine("Press \"ENTER\" to close.");
            Console.ReadLine();

            LogitechGSDK.LogiLedShutdown();
        }

        private void ToggleNumKeys(bool numLocked)
        {
            if (_numLocked == numLocked)
                return;

            _numLocked = numLocked;

            int onRed = 200, onGreen = 0, onBlue = 0;
            int offRed =20, offGreen = 200, offBlue = 10;

            var r = numLocked ? onRed : offRed;
            var g = numLocked ? onGreen : offGreen;
            var b = numLocked ? onBlue : offBlue;

            foreach (var key in _numKeys)
            {
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key, r, g, b);
            }
        }
    }
}
