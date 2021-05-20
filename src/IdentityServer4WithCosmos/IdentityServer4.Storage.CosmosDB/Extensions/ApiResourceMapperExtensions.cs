﻿using AutoMapper;
using IdentityServer4.Storage.CosmosDB.Mappers;
using IdentityServer4.Models;
using EF = IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.Storage.CosmosDB.Extensions
{
    /// <summary>
    ///     Extension methods to map IdentityServer4.Models.ApiResource to
    ///     IdentityServer4.Storage.CosmosDB.Entities.ApiResource.
    /// </summary>
    public static class ApiResourceMapperExtensions
    {
        static ApiResourceMapperExtensions()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        ///     Will map all data in IdentityServer4.Storage.CosmosDB.Entities.ApiResource to IdentityServer4.Models.ApiResource.
        /// </summary>
        /// <param name="entity">The IdentityServer4.Storage.CosmosDB.Entities.ApiResource to gather data from.</param>
        /// <returns>An instance of IdentityServer4.Models.ApiResource.</returns>
        public static ApiResource ToModel(this Entities.ApiResource entity)
        {
            return entity == null ? null : Mapper.Map<ApiResource>(entity);
        }

        /// <summary>
        ///     Will map all data in IdentityServer4.Models.ApiResource to IdentityServer4.Storage.CosmosDB.Entities.ApiResource.
        /// </summary>
        /// <param name="model">The IdentityServer4.Models.ApiResource to gather data from.</param>
        /// <returns>An instance of IdentityServer4.Storage.CosmosDB.Entities.ApiResource.</returns>
        public static Entities.ApiResource ToEntity(this ApiResource model)
        {
            return model == null ? null : Mapper.Map<Entities.ApiResource>(model);
        }

        /// <summary>
        ///     Will map all data in IdentityServer4.EntityFramework.Entities.ApiResource to IdentityServer4.Storage.CosmosDB.Entities.ApiResource.
        /// </summary>
        /// <param name="model">The IdentityServer4.EntityFramework.Entities.ApiResource to gather data from.</param>
        /// <returns>An instance of IdentityServer4.Storage.CosmosDB.Entities.ApiResource.</returns>
        public static Entities.ApiResource ToEntity(this EF.ApiResource model)
        {
            return model == null ? null : Mapper.Map<Entities.ApiResource>(model);
        }
    }
}