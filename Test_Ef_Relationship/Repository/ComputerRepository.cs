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
            // 1. GetId since we're updating based on Vm and BaseUrl
            var id = GetIdFromVmAndBaseUrl(vm, baseUrl);

            // 2. Clear existing "OneToMany" relationship.
            var dbEntity = _computerContext
                .ComputerEntities
                .Include(e => e.DiskEntities)
                .Include(e => e.NicEntities)
                .First(e => e.Id == id);
            
            // 3. "Cheat" EF Core by creating a new Entity that is not tracked, and update this instead.
            var inputEntity = _computerMapper.ToDbComputerEntity(input);
            inputEntity.Id = id;

//            var updatedEntity = _computerContext.Update(inputEntity).Entity;
            _computerContext.Entry(dbEntity).CurrentValues.SetValues(input);
            
            _computerContext.RemoveRange(dbEntity.DiskEntities);
            _computerContext.RemoveRange(dbEntity.NicEntities);
            
            dbEntity.DiskEntities.AddRange(inputEntity.DiskEntities);
            dbEntity.NicEntities.AddRange(inputEntity.NicEntities);
            
            _computerContext.SaveChanges();
            return dbEntity;
        }
        
        /*
         // 1. GetId since we're updating based on Vm and BaseUrl
            var id = GetIdFromVmAndBaseUrl(vm, baseUrl);

            // 2. Clear existing "OneToMany" relationship.
            var dbEntity = _computerContext
                .ComputerEntities
                .Include(e => e.DiskEntities)
                .Include(e => e.NicEntities)
                .First(e => e.Id == id);
//            dbEntity.DiskEntities.Clear();
//            dbEntity.NicEntities.Clear();

            dbEntity.DiskEntities.ForEach(e => _computerContext.DiskEntities.Remove(e));
            dbEntity.NicEntities.ForEach(e => _computerContext.NicEntities.Remove(e));

            // 3. "Cheat" EF Core by creating a new Entity that is not tracked, and update this instead.
            var inputEntity = _computerMapper.ToDbComputerEntity(input);
            dbEntity.DiskEntities = inputEntity.DiskEntities;
            dbEntity.NicEntities = inputEntity.NicEntities;

            inputEntity.Id = id;
            var updatedEntity = _computerContext.Update(inputEntity).Entity;
            SetModificationTimestamp(inputEntity);

            _computerContext.SaveChanges();
            return updatedEntity;
         */

        /*
         *
         * _computerContext.DiskEntities.Where(e => e.)
            
            
            // Fetch and set all the ID's from the database
            entity.DiskEntities.ForEach(e =>
            {
                e.Id = _computerContext.DiskEntities
                    .Where(d => d.Vm == vm && d.BaseUrl == baseUrl)
                    .Select(d => d.Id)
                    .First();
            });
            
            entity.NicEntities.ForEach(e =>
            {
                e.Id = _computerContext.NicEntities
                    .Where(d => d.Vm == vm && d.BaseUrl == baseUrl)
                    .Select(d => d.Id)
                    .First();
            });
            
         */

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