using System;
using System.IO;
using TagLib;

class Program
{
    static int Main(string[] args)
    {
        var dir = args.Length > 0 ? args[0] : Environment.CurrentDirectory;
        if (!Directory.Exists(dir))
        {
            Console.Error.WriteLine("Directory not found: " + dir);
            return 1;
        }

        var mp3Files = Directory.GetFiles(dir, "*.mp3");
        foreach (var path in mp3Files)
        {
            var baseName = Path.GetFileNameWithoutExtension(path);
            var artist = baseName;
            var dashIndex = baseName.IndexOf('-');
            if (dashIndex >= 0)
                artist = baseName.Substring(0, dashIndex).Trim();

            try
            {
                using (var file = TagLib.File.Create(path))
                {
                    file.Tag.AlbumArtists = new[] { artist };
                    // Some TagLib versions expose AlbumArtist (singular); try to set if available
                    var tagType = file.Tag.GetType();
                    var prop = tagType.GetProperty("AlbumArtist");
                    if (prop != null && prop.CanWrite)
                        prop.SetValue(file.Tag, artist);

                    file.Save();
                }

                Console.WriteLine($"Updated: {Path.GetFileName(path)} -> '{artist}'");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to update '{path}': {ex.Message}");
            }
        }

        return 0;
    }
}
