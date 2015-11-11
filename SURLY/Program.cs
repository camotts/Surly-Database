using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace SURLY
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database();
            bool errorThrow = false;
            // Initialize a new instance of the SpeechSynthesizer.
            SpeechSynthesizer synth = new SpeechSynthesizer();

            // Configure the audio output. 
            synth.SetOutputToDefaultAudioDevice();
            var voice = synth.GetInstalledVoices();

            //Check to see if it is on a windows 8 or higher machine
            Version win8version = new Version(6, 2, 9200, 0);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8version)
            {
                //This breaks if it is a window 7 machine. 
                synth.SelectVoice(voice[2].VoiceInfo.Name);
            }





            //synth.Speak("Creating table: Game");
            db.createTable("Game",
                            new string[] { "ID", "Name", "Rating", "Price", "InventoryCount" },
                            new string[] { "int", "string", "int", "int", "int" },
                            new int[] { 9, 50, 2, 2, 3 },
                            new bool[] { false, false, true, false, false });

            //synth.Speak("Creating table: Game Type");
            db.createTable("GameType",
                            new string[] { "ID", "Name" },
                            new string[] { "int", "string" },
                            new int[] { 9, 25 },
                            new bool[] { false, false });

            //synth.Speak("Creating many to many relationship: Game to Game Type");
            db.createTable("Game_GameType",
                            new string[] { "GameID", "GameTypeID" },
                            new string[] { "int", "int" },
                            new int[] { 9, 9 },
                            new bool[] { false, false });

            //synth.Speak("Creating table: Movies");
            db.createTable("Movies",
                            new string[] { "ID", "Name", "Rating", "Price", "InventoryCount" },
                            new string[] { "int", "string", "int", "int", "int" },
                            new int[] { 9, 50, 2, 2, 3 },
                            new bool[] { false, false, true, false, false });
            //Game information
            db.insert("Game",
                      new Object[] { 1, "League", null, 99, 999 });
            db.insert("Game",
                       new Object[] { 2, "Speed Runners", 99, 99, 23 });

            //GameType information
            db.insert("GameType",
                       new Object[] { 1, "Action" });
            db.insert("GameType",
                       new Object[] { 2, "Adventure" });
            db.insert("GameType",
                       new Object[] { 3, "RPG" });
            db.insert("GameType",
                       new Object[] { 4, "Sandbox" });

            //Game_GameType information
            db.insert("Game_GameType",
                       new Object[] { 2, 1 });

            db.insert("Game_GameType",
                       new Object[] { 1, 1 });

            db.insert("Game_GameType",
                       new Object[] { 1, 2 });

            //Movies information
            db.insert("Movies",
                       new Object[] { 1, "Star Trek", 15, 20, 300 });
            db.insert("Movies",
                      new Object[] { 2, "Star Wars Episode III", 75, 99, 520 });
            db.insert("Movies",
                      new Object[] { 3, "Iron Bacon", 99, 99, 1 });

            if (errorThrow)
            {
                db.insert("Movies",
                       new Object[] { 1, "Star Trek", 15, 20, 300, 20 });
                db.insert("Movies",
                       new Object[] { null, "Star Trek", 15, 20, 300 });
                db.insert("Movies",
                       new Object[] { "test", "Star Trek", 15, 20, 300 });
                db.insert("Movies",
                       new Object[] { 1, "Star Trek", 15, 20000, 300 });

                synth.Speak("I'm sorry.");
            }

            db.print("Game");
            db.print("GameType");
            db.print("Game_GameType");
            db.print("Movies");

            db.parse("Delete from Game where ID = 1");
            db.print("Game");
            db.update("Game", new string[] { "Name" }, new string[] { "WHY" });
            db.print("Game");

            db.print(db.union("Game", "Game_GameType", "ID", "GameID"));
            db.print(db.select("Game", "Price", "99"));

            //synth.Speak("Deleting Movies' Content because it destroyed our formatting -_-");
            //Delete and print the movie table to show it's been deleted
            db.delete("Movies");
            db.print("Movies");

            //synth.Speak("Destroy Movies!");
            //Show that Destroy works
            db.destroy("Movies");
            db.print("Movies");

            //Read a key so it doesn't shutdown immediately after finishing
            Console.ReadKey();
        }
    }
}
