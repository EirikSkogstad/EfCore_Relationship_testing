using AutoMapper;

namespace Test_Ef_Relationship.Repository
{
    public class ComputerMapper
    {
        public ComputerEntity ToComputerEntity(DbComputerEntity dbComputerEntity)
        {
            return GetMapper().Map<ComputerEntity>(dbComputerEntity);
        }

        public DbComputerEntity ToDbComputerEntity(ComputerEntity computerEntity)
        {
            return GetMapper().Map<DbComputerEntity>(computerEntity);
        }

        private IMapper GetMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DbComputerEntity, ComputerEntity>();
                cfg.CreateMap<DbNicEntity, NicEntity>();
                cfg.CreateMap<DbDiskEntity, DiskEntity>();

                cfg.CreateMap<ComputerEntity, DbComputerEntity>()
                    .ForMember(src => src.Id, dest => dest.Ignore());
                
//                    .ForMember(src => src.DiskEntities, dest => dest.MapFrom(e => e.DiskEntities))
//                    .ForMember(src => src.NicEntities, dest => dest.MapFrom(e => e.NicEntities));
                    
                cfg.CreateMap<NicEntity, DbNicEntity>()
                    .ForMember(src => src.Id, dest => dest.Ignore());
                cfg.CreateMap<DiskEntity, DbDiskEntity>()
                    .ForMember(src => src.Id, dest => dest.Ignore());
            });

            return mapperConfiguration.CreateMapper();
        }
    }
}