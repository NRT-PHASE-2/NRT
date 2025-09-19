using System;
using System.Media;
using System.Threading;

namespace NRTVending
{
    public static class Music
    {
        private static SoundPlayer _player;
        private static Thread _musicThread;

        public static void PlayLoop(string filePath)
        {
            Stop(); // stop any previous music

            _musicThread = new Thread(() =>
            {
                try
                {
                    _player = new SoundPlayer(filePath);
                    _player.Load();
                    _player.PlayLooping(); // play continuously
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error playing music: {ex.Message}");
                }
            });

            _musicThread.IsBackground = true;
            _musicThread.Start();
        }

        public static void Stop()
        {
            if (_player != null)
            {
                _player.Stop();
            }
            if (_musicThread != null && _musicThread.IsAlive)
            {
                _musicThread.Abort(); // safe because it's background
            }
        }
    }
}
