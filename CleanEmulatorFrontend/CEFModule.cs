﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace CleanEmulatorFrontend
{
    class CEFModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MainWindow>().ToSelf();
        }
    }
}
