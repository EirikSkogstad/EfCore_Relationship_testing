using System;
using System.Collections.Generic;

namespace Test_Ef_Relationship.Repository
{
    public class ComputerEntity
    {
        public string Vm { get; set; }
        public string BaseUrl { get; set; }
        public string Name { get; set; }

        public List<DiskEntity> DiskEntities { get; set; }
        public List<NicEntity> NicEntities { get; set; }
    }
}