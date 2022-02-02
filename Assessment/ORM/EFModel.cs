using Microsoft.EntityFrameworkCore;

namespace Assessment.ORM
{
    public class EFModel : DbContext
    {
        public EFModel(DbContextOptions options) : base (options)
        {

        }
    }
}
