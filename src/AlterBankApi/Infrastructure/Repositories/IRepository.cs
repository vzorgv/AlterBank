namespace AlterBankApi.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository declaration
    /// </summary>
    /// <typeparam name="TEntity">The entity of specific type></typeparam>
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Reads all entities
        /// </summary>
        /// <returns>Collection of entities</returns>
        Task<IEnumerable<TEntity>> Read();

        /// <summary>
        /// Reads entity by its identity
        /// </summary>
        /// <param name="Id">The identity</param>
        /// <returns>Entity associated with identity</returns>
        Task<TEntity> ReadById(string Id);

        /// <summary>
        /// Creates entity
        /// </summary>
        /// <param name="entity">The entity to be created</param>
        /// <returns></returns>
        Task<TEntity> Create(TEntity entity);

        /// <summary>
        /// Updates entity
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>Updated entity</returns>
        Task<TEntity> Update(TEntity entity);
    }
}
