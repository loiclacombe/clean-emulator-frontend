using System;
using System.Collections.Generic;
using GamesData;

namespace Cache
{
    public class PersistenceContainer
    {
        public const int CurrentVersion = 1;
        private int _persistedVersion = CurrentVersion;
        public List<SystemNode> SystemGroups { get; set; }

        public int PersistedVersion
        {
            get { return _persistedVersion; }
            set { _persistedVersion = value; }
        }

        public Boolean IsCacheValid()
        {
            return PersistedVersion == CurrentVersion;
        }
    }
}