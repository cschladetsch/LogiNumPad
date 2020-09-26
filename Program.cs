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

        // CAPITAL is usually 0x14, but it's been remapped in registry for InCode
        //const int VK_CAPITAL = 0xA3;//0x14;
        const int VK_CAPITAL = 0x14;
        const int VK_NUMLOCK = 0x90;

        private bool _numLocked;
        private bool _capsLocked;

        private readonly keyboardNames[] _incodeKeys =
        {
            keyboardNames.Q,
            keyboardNames.E,
            keyboardNames.S,
            keyboardNames.D,
            keyboardNames.F,
            keyboardNames.R,
            keyboardNames.V,
            keyboardNames.SPACE,
        };

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
            SetKeyState();

            Console.WriteLine($"Monitoring NumLock and CapsLock. Current status: num={_numLocked}, caps={_capsLocked}\n");

            const int pollTime = 60; // milliseconds
            var timer = new Timer(pollTime) {Enabled = true};
            timer.Elapsed += (sender, eventArgs) => { SetKeyState(); };

            Console.WriteLine("Press \"ENTER\" to close.");
            Console.ReadLine();

            LogitechGSDK.LogiLedShutdown();
        }

        private void SetKeyState()
        {
            var numLocked = ((ushort) GetKeyState(VK_NUMLOCK) & 0xffff) == 0;
            ToggleNumKeys(numLocked);

            // TODO: why doesn't this work?
            var caps = ((ushort)GetKeyState(VK_CAPITAL) & 0xffff) == 0;
            ToggleCapsLock(caps);
        }

        private void ToggleCapsLock(bool capsLock)
        {
            if (_capsLocked == capsLock)
                return;

            _capsLocked = capsLock;
            Console.WriteLine($"CapsLock = {_capsLocked}");
            var r = _capsLocked ? 0 : 255;
            var g = _capsLocked ? 55 : 0;
            var b = _capsLocked ? 255 : 100;
            LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(keyboardNames.CAPS_LOCK, r, g, b);
            //HighlightIncodeKeys();
        }

        private void HighlightIncodeKeys()
        {
            var r = _capsLocked ? 255 : 200;
            var g = _capsLocked ? 0 : 255;
            var b = _capsLocked ? 0 : 10;
            foreach (var key in _incodeKeys)
            {
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key, r, g, b);
            }
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
