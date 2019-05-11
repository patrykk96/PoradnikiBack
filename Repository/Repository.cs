using Data.DbModels;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly DataContext _context;
        private DbSet<T> _dbSet;

        public Repository(DataContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> func)
        {
            bool doesItExist =  await _dbSet.AnyAsync(func);

            return doesItExist;
        }

        public async Task<T> GetSingleEntity(Expression<Func<T, bool>> func)
        {
            var entity = await _dbSet.SingleOrDefaultAsync(func);

            return entity;
        }

        public async void Add(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async void Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> func)
        {
            var list = await _dbSet.Where(func).ToListAsync();

            return list;
        }
    }
}
