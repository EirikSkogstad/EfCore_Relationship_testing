using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test_Ef_Relationship.Helpers;

namespace Test_Ef_Relationship.Repository
{
    public class ComputerRepository : IComputerRepository<ComputerEntity>
    {
        private readonly ComputerContext _computerContext;
        private readonly ComputerMapper _computerMapper;
        private readonly ITimeHelper _timeHelper;

        public ComputerRepository(
            ComputerContext computerContext,
            ComputerMapper computerMapper,
            ITimeHelper timeHelper)
        {
            _computerContext = computerContext;
            _computerMapper = computerMapper;
            _timeHelper = timeHelper;
        }


        public DbComputerEntity CreateOrUpdate(ComputerEntity input)
        {
            var vm = input.Vm;
            var baseUrl = input.BaseUrl;

            return Exists(vm, baseUrl)
                ? Update(input, vm, baseUrl)
                : Create(input);
        }

        public bool Exists(string vm, string baseUrl)
        {
            var computerEntity = _computerContext
                .ComputerEntities
                .AsNoTracking()
                .FirstOrDefault(e => e.Vm == vm && e.BaseUrl == baseUrl);

            return computerEntity != null;
        }

        public DbComputerEntity Create(ComputerEntity input)
        {
            var entity = _computerMapper.ToDbComputerEntity(input);
            SetModificationTimestamp(entity);
            entity.Id = 0;
            var createdEntity = _computerContext
                .ComputerEntities
                .Add(entity)
                .Entity;

            _computerContext.SaveChanges();
            return createdEntity;
        }

        public DbComputerEntity Update(ComputerEntity input, string vm, string baseUrl)
        {
            var entity = _computerMapper.ToDbComputerEntity(input);
            SetModificationTimestamp(entity);
            var id = GetIdFromVmAndBaseUrl(vm, baseUrl);
            entity.Id = id;
            
            var updatedEntity = _computerContext.Update(entity).Entity;

            _computerContext.SaveChanges();
            return updatedEntity;
        }

        private long GetIdFromVmAndBaseUrl(string vm, string baseUrl)
        {
            var entityFromDb = _computerContext
                .ComputerEntities
                .Include(e => e.DiskEntities)
                .Include(e => e.NicEntities)
                .AsNoTracking()
                .First(e => e.Vm == vm && e.BaseUrl == baseUrl);

            return entityFromDb.Id;
        }

        private void SetModificationTimestamp(DbComputerEntity entity)
        {
            var utcNow = _timeHelper.GetUtcNow();
            entity.LastModifiedByProducer = utcNow;
        }
    }

    public interface IComputerRepository<T> where T : class
    {
        bool Exists(string vm, string baseUrl);

        DbComputerEntity CreateOrUpdate(T input);

        DbComputerEntity Create(T input);

        DbComputerEntity Update(T input, string vm, string baseUrl);

//        DbComputerEntity Delete(T input);
    }
}