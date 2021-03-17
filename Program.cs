using System;
using System.Linq;
using System.Timers;
using System.Drawing;
using System.Configuration;
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

        private const int VK_NUMLOCK = 0x90;
        private bool _first = true;
        private bool _numLocked;

        private Color _keyPadOn;
        private Color _keyPadOff;
        private Color _numLockOn;
        private Color _numLockOff;
        
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

            GetAppSettings();
            SetKeyState();

            const int pollTime = 60; // milliseconds
            var timer = new Timer(pollTime) {Enabled = true};
            timer.Elapsed += (sender, eventArgs) => { SetKeyState(); };

            Console.WriteLine("Press \"ENTER\" to close.");
            Console.ReadLine();

            LogitechGSDK.LogiLedShutdown();
        }

        private void GetAppSettings()
        {
            _keyPadOn = ColorFromAppSettings("keyPadOn");
            _keyPadOff = ColorFromAppSettings("keyPadOff");
            _numLockOn = ColorFromAppSettings("numLockOn");
            _numLockOff = ColorFromAppSettings("numLockOff");
        }

        public static Color ColorFromAppSettings(string key)
        {
            var text = ConfigurationManager.AppSettings.Get(key);
            var channels = text.Split(new[]{','}).Select(c => int.Parse(c)).ToArray();
            return Color.FromArgb(1, channels[0], channels[1], channels[2]);
        }

        private void SetKeyState()
        {
            var numLocked = ((ushort) GetKeyState(VK_NUMLOCK) & 0xffff) == 0;
            ToggleNumKeys(numLocked);
        }

        private void ToggleNumKeys(bool numLocked)
        {
            if (_numLocked == numLocked || _first)
            {
                _first = false;
                return;
            }

            _numLocked = numLocked;
            SetNumKeyColors();
        }

        private void SetNumKeyColors()
        {
            Color color = _numLocked ? _keyPadOff : _keyPadOn;
            Console.WriteLine($"NumLock = {!_numLocked}");
            foreach (var key in _numKeys)
            {
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key, color.R, color.G, color.B);
            }

            var numLockColor = _numLocked ? _numLockOff : _numLockOn;
            LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(keyboardNames.NUM_LOCK, numLockColor.R, numLockColor.G, numLockColor.B);
        }
    }
}
