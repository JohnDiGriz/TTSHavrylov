using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TTSHavrylov.Concatenation_Waves;

namespace TTSHavrylov
{
    public class Language
    {
        private Dictionary<string, byte[]> _sounds;
        private HashSet<string> _stopSounds;
        private string _spaceRegex;
        public Language(string folderName)
        {
            _sounds = new Dictionary<string, byte[]>();
            var files = Directory.GetFiles(folderName, "*.wav", SearchOption.AllDirectories);
            foreach (var fileName in files)
            {
                _sounds.Add(fileName.Split('/', '\\').Last().Replace(".wav", ""), File.ReadAllBytes(fileName));
            }
            var stopSoundPath = Directory.GetFiles(folderName, "config.txt").FirstOrDefault();
            var configLines = File.ReadAllLines(stopSoundPath);
            _stopSounds = configLines[0].Split(" ").Select(x => x.Trim()).ToHashSet();
            _spaceRegex = configLines[1].Trim();
        }

        public byte[] GenerateSound(string str)
        {
            List<byte[]> sound = new List<byte[]>();
            str = str.ToLower();
            var lastPhonem = "";
            for (int i = 0; i < str.Length; i++)
            {
                
                if (!_stopSounds.Contains(lastPhonem))
                    try
                    {
                        var newSound = _sounds[Convert(lastPhonem + str[i])];
                    }
                    catch (Exception ex)
                    {
                        sound.Add(_sounds[Convert(lastPhonem)]);
                        lastPhonem = "";
                    }
                else
                {
                    lastPhonem = "";
                }
                lastPhonem += str[i];
            }
            try
            {
                sound.Add(_sounds[Convert(lastPhonem)]);
            }
            catch(Exception ex) { }
            return WaveIO.Merge(sound);
        }

        private string Convert(string value)
        {
            if (Regex.IsMatch(value, _spaceRegex))
                return "+space";
            else if (value == "ї")
                return "йі";
            else
                return value;
        }
    }
}
