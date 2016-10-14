using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KPMG.Models
{
    public class KPMGDatabaseEntities : DbContext
    {
        public KPMGDatabaseEntities(): base()
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}