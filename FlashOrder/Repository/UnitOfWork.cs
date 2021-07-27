﻿using System;
using System.Threading.Tasks;
using FlashOrder.Data;
using FlashOrder.IRepository;

namespace FlashOrder.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private IGenericRepository<Item> _items;
        private IGenericRepository<Recipe> _recipes;
        
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public IGenericRepository<Recipe> Recipes => _recipes ??= new GenericRepository<Recipe>(_context);
        public IGenericRepository<Item> Items=>_items ??= new GenericRepository<Item>(_context);
        
        public async Task save()
        {
            await _context.SaveChangesAsync();
        }
        
        public void Dispose()
        {
            //like garbage collector,like when i finish ,free the memory
            //free all the db context connections that the context was using  
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}