using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SURLY;
using System.Web.Mvc;
using WebDBMS.Models;

namespace WebDBMS.Controllers
{
    public class Base : Controller
    {
        public void CreateRestOfTables()
        {
            if (WebDataBase.db.Tables.Count < 1)
            {
                //Our "Seed" Method
                //Left out the Game and Movies table, since you need to go through the tutorial
                WebDataBase.db.createTable("GameType",
                                new string[] { "ID", "Name" },
                                new string[] { "int", "string" },
                                new int[] { 9, 25 },
                                new bool[] { false, false });

                WebDataBase.db.createTable("Game_GameType",
                                new string[] { "GameID", "GameTypeID" },
                                new string[] { "int", "int" },
                                new int[] { 9, 9 },
                                new bool[] { false, false });
                WebDataBase.db.createTable("GameDeveloper",
                              new string[] { "GameID", "Devoloper" },
                              new string[] { "int", "string" },
                              new int[] { 9, 100 },
                              new bool[] { false, false });
                WebDataBase.db.createTable("MovieActor",
                                   new string[] { "MovieID", "Actor" },
                                   new string[] { "int", "string" },
                                   new int[] { 9, 100 },
                                   new bool[] { false, false });

                WebDataBase.db.createTable("Console",
                                  new string[] { "ConsoleID", "Console" },
                                  new string[] { "int", "string" },
                                  new int[] { 9, 100 },
                                  new bool[] { false, false });

                WebDataBase.db.createTable("Game_Console",
                                  new string[] { "GameID", "ConsoleID" },
                                  new string[] { "int", "int" },
                                  new int[] { 9, 9 },
                                  new bool[] { false, false });

                //Game Developer Table
                WebDataBase.db.insert("GameDeveloper",
                          new Object[] { 1, "Riot Games" });
                WebDataBase.db.insert("GameDeveloper",
                          new Object[] { 2, "tinyBuild" });
                WebDataBase.db.insert("GameDeveloper",
                           new Object[] { 2, "DoubleDutch Games" });

                //Game_Console Table
                WebDataBase.db.insert("Game_Console",
                          new Object[] { 1, 1});
                WebDataBase.db.insert("Game_Console",
                         new Object[] { 2, 1 });
                WebDataBase.db.insert("Game_Console",
                         new Object[] { 2, 2 });
     
                //Console Table
                WebDataBase.db.insert("Console",
                       new Object[] { 1, "PC" });
                WebDataBase.db.insert("Console",
                       new Object[] { 1, "Xbox One" });
                WebDataBase.db.insert("Console",
                       new Object[] { 1, "PS4" });

                //Movie Actors table
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Daisy Ridley" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Gwendoline Christie" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Oscar Isaac" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Harrison Ford" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Carrie Fisher" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Mark Hamill" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Domhnall Gleeson" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Simon Pegg" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Adam Driver" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Peter Mayhew" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 1, "Andy Serkis" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 2, "Marc Daniels" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 2, "Joseph Pevney" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 2, "Vincent McEveety" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 2, "Ralph Senensky" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 2, "Jud Taylor" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 2, "Herb Wallerstein" });
                WebDataBase.db.insert("MovieActor",
                        new Object[] { 2, "Marvin J. Chomsky" });

                //GameType information
                WebDataBase.db.insert("GameType",
                           new Object[] { 1, "Action" });
                WebDataBase.db.insert("GameType",
                           new Object[] { 2, "Adventure" });
                WebDataBase.db.insert("GameType",
                           new Object[] { 3, "RPG" });
                WebDataBase.db.insert("GameType",
                           new Object[] { 4, "Sandbox" });

                //Game_GameType information
                WebDataBase.db.insert("Game_GameType",
                           new Object[] { 2, 1 });

                WebDataBase.db.insert("Game_GameType",
                           new Object[] { 1, 1 });

                WebDataBase.db.insert("Game_GameType",
                           new Object[] { 1, 2 });



            }
        }

    }
}
