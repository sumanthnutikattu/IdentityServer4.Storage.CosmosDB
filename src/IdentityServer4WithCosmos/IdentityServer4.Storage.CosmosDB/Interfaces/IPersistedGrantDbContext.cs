﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.Storage.CosmosDB.Entities;

namespace IdentityServer4.Storage.CosmosDB.Interfaces
{
    public interface IPersistedGrantDbContext : IDisposable
    {
        IQueryable<PersistedGrant> PersistedGrants(string partitionKey = "");

        Task Add(PersistedGrant entity);

        Task Update(PersistedGrant entity);
        Task Update(Expression<Func<PersistedGrant, bool>> filter, PersistedGrant entity);

        Task Remove(PersistedGrant entity);
        Task Remove(Expression<Func<PersistedGrant, bool>> filter);

        Task RemoveExpired();
    }
}