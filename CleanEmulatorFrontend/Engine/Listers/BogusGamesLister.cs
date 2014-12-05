using System.Collections.Generic;
using GamesData.DatData;

namespace CleanEmulatorFrontend.Engine.Listers
{
    public class BogusGamesLister : IGamesLister
    {
        #region IGamesLister Members

        public IList<Game> ListAll()
        {
            return new List<Game>
                       {
                           new Game
                               {
                                   Description = "Aladdin (EUR)",
                                   LaunchPath = "Aladdin (EUR)",
                                   Roms = new List<Rom>
                                              {
                                                  new Rom
                                                      {
                                                          Name = "Aladdin (EUR).smc"
                                                      }
                                              }
                               },
                                                          new Game
                               {
                                   Description = "Donkey Kong Country (EUR)",
                                   LaunchPath = "Donkey Kong Country (EUR).smc",
                                   Roms = new List<Rom>
                                              {
                                                  new Rom
                                                      {
                                                          Name = "Aladdin (EUR).smc"
                                                      }
                                              }
                               },
                       };
        }

        #endregion
    }
}