﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlashOrder.Data;
using FlashOrder.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FlashOrder.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<IList> GetAll(Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query=_db;
            
            if (expression!=null)
            {
                query=query.Where(expression);
            }
            
            if (includes!=null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy!=null)
            {
                query = orderBy(query);
            }

            //here we ask him for not tracking the object status
            return await query.AsNoTracking().ToListAsync();
        }
        

        //The expression can be a lambda expression 
        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes=null)
        {
            IQueryable<T> query=_db;
            if (includes!=null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public async Task Delete(T entity)
        {
            _db.Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        // public async Task<IList> GetAllWithFilters(
        //     Expression<Func<T, bool>> expression = null,
        //     Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        //     List<string> includes = null,
        //     IList<BaseSpecification<T>> specifications = null
        // )
        // {
        //     IQueryable<T> query=_db;
        //
        //     if (specifications != null)
        //     {
        //         foreach (var specificationProperty in specifications)
        //         {
        //             query = query.Specify(new RecipeContainsIngredients(new List<string>(){"Caviar"}));
        //         }
        //     }
        //     
        //     if (expression!=null)
        //     {
        //         query=query.Where(expression);
        //     }
        //     
        //     if (includes!=null)
        //     {
        //         foreach (var includeProperty in includes)
        //         {
        //             query = query.Include(includeProperty);
        //         }
        //     }
        //
        //     if (orderBy!=null)
        //     {
        //         query = orderBy(query);
        //     }
        //
        //     //here we ask him for not tracking the object status
        //     return await query.AsNoTracking().ToListAsync();
        // }
    }
}