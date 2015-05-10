using System;
using System.Collections.Generic;
using CleanEmulatorFrontend.GamesData;

namespace CleanEmulatorFrontend.Cache.Thrift
{
    [Serializable]
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

        public bool IsCacheValid()
        {
            return PersistedVersion == CurrentVersion;
        }
    }
}