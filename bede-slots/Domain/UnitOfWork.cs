using bede_slots.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace bede_slots.Domain
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
        void Save();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        IRepository<Player> PlayerRepo { get; }
        IRepository<SlotItem> SlotItemRepo { get; }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        IDbContextTransaction dbContextTransaction;

        private IRepository<Player> _playerRepo;
        private IRepository<SlotItem> _slotItemRepo;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IRepository<Player> PlayerRepo
        {
            get 
            {
                if (_playerRepo == null)
                    _playerRepo = new Repository<Player>(_appDbContext);
                return _playerRepo;
            }
        }
        public IRepository<SlotItem> SlotItemRepo
        {
            get
            {
                if (_slotItemRepo == null)
                    _slotItemRepo = new Repository<SlotItem>(_appDbContext);
                return _slotItemRepo;
            }
        }

        public void BeginTransaction()
        {
            dbContextTransaction = _appDbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (dbContextTransaction != null)
                dbContextTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            if(dbContextTransaction != null)
                dbContextTransaction.Rollback();
        }

        public void Save()
        {
           _appDbContext.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                    _appDbContext.Dispose();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
