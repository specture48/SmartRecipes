﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlashOrder.IRepository
{
    public interface IGenericRepository<T> where T :class
    {
        Task<IList> GetAll(
            Expression<Func<T,bool>> expression=null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy=null,
            List<string> includes=null
        );
        

        Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes=null);

        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(int id);
        Task Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
        
        
    }
}