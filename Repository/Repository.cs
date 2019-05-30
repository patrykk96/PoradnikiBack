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

        //metoda do sprawdzenia czy podany element jest w bazie
        public async Task<bool> Exists(Expression<Func<T, bool>> func)
        {
            bool doesItExist =  await _dbSet.AnyAsync(func);

            return doesItExist;
        }

        //metoda do pobrania wybranego elementu z bazy
        public async Task<T> GetSingleEntity(Expression<Func<T, bool>> func)
        {
            var entity = await _dbSet.SingleOrDefaultAsync(func);

            return entity;
        }

        //metoda do dodania elementu do bazy
        public async void Add(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        //metoda do edycji elementu w bazie
        public async void Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        //metoda do usuniecia elementu z bazy
        public async void Delete(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        //metoda do pobrania listy elementow jednego typu spelniajacych podane warunki
        public async Task<List<T>> GetAllBy(Expression<Func<T, bool>> func)
        {
            var list = await _dbSet.Where(func).ToListAsync();

            return list;
        }

        //metoda zwracajaca wszystkie elementy jednego typu
        public async Task<List<T>> GetAll()
        {
            var list = await _dbSet.ToListAsync();

            return list;
        }
    }
}
