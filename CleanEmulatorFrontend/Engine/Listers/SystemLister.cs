using System.Collections.Generic;
using SystemData = CleanEmulatorFrontend.Engine.Data.System;


namespace CleanEmulatorFrontend.Engine.Listers
{
    public class SystemLister
    {
        private GenericGameLister _genericGameLister;

        public SystemLister(GenericGameLister genericGameLister)
        {
            _genericGameLister = genericGameLister;
            Systems = new SortedSet<SystemData>
                                                    {
                                                        new SystemData(genericGameLister)
                                                            {
                                                                ShortName = "SNES",
                                                                Name = "Super Nintendo Entertainment System"
                                                            },
                                                        new SystemData(genericGameLister)
                                                            {
                                                                ShortName = "Genesis",
                                                                Name = "Genesis"
                                                            },
                                                    };
        }

        public readonly SortedSet<SystemData> Systems;


        public SystemLister()
        {
          this._systems  
        }

    }
}
