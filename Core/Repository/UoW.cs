using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Core.Repository
{
    public class UoW : IDisposable
    { 
        private readonly DbContext _context;

        public UoW(DbContext context)
        {
            _context = context;
        }

        public DbContext Context
        {
            get { return _context; }
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            var result = await _context.SaveChangesAsync();

            return result;
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
