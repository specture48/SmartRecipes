﻿using System;
using System.Threading.Tasks;
using FlashOrder.Data;

namespace FlashOrder.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
        // IGenericRepository<Recipe> Recipes { get; }
        IRecipeRepository Recipes { get; }
        IGenericRepository<Item> Items { get; }
        IGenericRepository<Step> Steps { get; }
        IGenericRepository<Follow> Follows { get; }
        IGenericRepository<Rating> Ratings { get; }
        Task save();
    }
}