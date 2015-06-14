using System.Data.Entity;

namespace CleanEmulatorFrontend.SqLiteCache
{
    public partial class GamesCacheEntities
    {
        public GamesCacheEntities(string connectionString) : base(connectionString)
        {
        }
    }
}
