﻿using Data.DbModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        Task<bool> Exists(Expression<Func<T, bool>> func);
        Task<T> GetSingleEntity(Expression<Func<T, bool>> func);
        Task<List<T>> GetAllBy(Expression<Func<T, bool>> func);
        Task<List<T>> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}

