using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Hippo.Core.Interfaces
{
    /// <summary>
    /// <para>
    /// A <see cref="IReadRepository{T}" /> can be used to query instances of <typeparamref name="T" />.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of entity being operated on by this repository.</typeparam>
    public interface IReadRepository<T> where T : class, IAggregateRoot
    {
        /// <summary>
        /// Finds an entity with the given primary key value.
        /// </summary>
        /// <typeparam name="TId">The type of primary key.</typeparam>
        /// <param name="id">The value of the primary key for the entity to be found.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
        /// </returns>
        Task<T> GetAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

        /// <summary>
        /// Finds all entities of <typeparamref name="T" /> from the database.
        /// </summary>
        /// <typeparam name="filter">The LINQ query used to filter results.</typeparam>
        /// <typeparam name="orderBy">A function closure used to order the results.</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="List{T}" /> that contains elements from the input sequence.
        /// </returns>
        Task<List<T>> ListAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the total number of records.
        /// </summary>
        /// <typeparam name="filter">The LINQ query used to filter results.</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// number of elements in the input sequence.
        /// </returns>
        Task<int> CountAsync(
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = default);
    }
}
