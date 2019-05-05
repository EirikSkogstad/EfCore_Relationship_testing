using System;
using System.Collections.Generic;

namespace Test_Ef_Relationship.Repository
{
    public class DbComputerEntity
    {
        public long Id { get; set; }

        public string Vm { get; set; }
        public string BaseUrl { get; set; }
        public string Name { get; set; }

        public List<DbDiskEntity> DiskEntities { get; set; }
        public List<DbNicEntity> NicEntities { get; set; }
        public DateTimeOffset? LastModifiedByProducer { get; set; }
    }
}