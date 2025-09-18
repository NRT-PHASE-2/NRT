using System;
using System.Media;

namespace NRTVending
{
    public class Music
    {
        public static void Play(string filePath)
        {
            try
            {
                var player = new SoundPlayer(filePath);
                player.Load();
                player.PlayLooping();
                Console.WriteLine($"Now playing: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing file: {ex.Message}");
            }
        }
    }
}
