using System;
using System.IO;
using System.Media;
using System.Text;

namespace TTSHavrylov
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            Console.WriteLine("Enter path to Language folder (enter \\def for default): ");
            var folderName = Console.ReadLine();
            Language lang;
            if (folderName == "\\def")
                lang = new Language("../../../../Havrylov");
            else
                lang = new Language(folderName);
            string str = "";
            do
            {
                try
                {
                    Console.WriteLine("Write line (enter \\ext to exit): ");
                    str = Console.ReadLine();
                    var sound = lang.GenerateSound(str);
                    using (MemoryStream ms = new MemoryStream(sound))
                    {
                        // Construct the sound player
                        SoundPlayer player = new SoundPlayer(ms);
                        player.Play();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            while (str != "\\ext");
        }
    }
}
