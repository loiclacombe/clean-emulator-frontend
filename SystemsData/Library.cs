using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesData
{
    public partial class Library
    {

        public virtual string Folder
        {
            get { return ConfigurationManager.AppSettings[LibraryFolderKey]; }
        }
    }
}
