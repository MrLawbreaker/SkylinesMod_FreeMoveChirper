using ColossalFramework.IO;
using System.IO;
namespace FreeMoveChirper
{
    public class FMConfiguration
    {
        public bool ctrlToMove
        {
            get;
            private set;
        }

        private const string CTRL_TO_MOVE_KEY = "b_ctrlToMove";

        private string configPath
        {
            get
            {
                string path = string.Format("{0}/{1}/", DataLocation.localApplicationData, "ModConfig");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                path += "free-move-chirpy.cfg";

                return path;
            }
        }

        private string[] configlines
        {
            get
            {
                if (File.Exists(configPath))
                {
                    return File.ReadAllLines(configPath);
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(configPath))
                    {
                        sw.WriteLine("# Free Move Chirper Config");
                        sw.WriteLine();
                        sw.WriteLine("# Should ctrl be pushed to move chirper.");
                        sw.WriteLine(string.Format("{0}=false", CTRL_TO_MOVE_KEY));
                        sw.WriteLine();
                    }
                    return File.ReadAllLines(configPath);
                }
            }
        }

        public FMConfiguration()
        {
            ctrlToMove = false;

            foreach (string configline in configlines)
            {
                var line = configline.Trim();

                if (line.StartsWith("#"))
                {
                    continue;
                }
                else if (line.StartsWith(CTRL_TO_MOVE_KEY + "="))
                {
                    var splitline = line.Split('=');
                    if (splitline.Length > 1) ctrlToMove = splitline[1].ToLower().Equals("true");
                }
            }
        }
    }
}