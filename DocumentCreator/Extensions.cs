using DocumentCreator.Core.Model;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace DocumentCreator
{
    public static class Extensions
    {
        public static MemoryStream ToMemoryStream(this Stream source)
        {
            var ms = new MemoryStream();
            source.Position = 0;
            source.CopyTo(ms);
            return ms;
        }

        /// <summary>
        /// Creates a paged set of results.
        /// </summary>
        /// <typeparam name="T">The type of the source IQueryable.</typeparam>
        /// <param name="queryable">The source IQueryable.</param>
        /// <param name="page">The page number you want to retrieve.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="orderBy">The field or property to order by.</param>
        /// <param name="ascending">Indicates whether or not the order should be ascending (true) or descending (false.)</param>
        /// <returns>Returns a paged set of results.</returns>
        /// <see cref="https://schneids.net/paging-in-asp-net-web-api"/>
        public static PagedResults<T> CreatePagedResults<T>(
            this IQueryable<T> queryable,
            int page,
            int pageSize,
            string orderBy,
            bool ascending)
        {
            if (page <= 0)
                throw new ArgumentException("Page must be greater than 0.");
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 0.");

            var skipCount = pageSize * (page - 1);
            var pageResults = queryable
                .AsQueryable()
                .OrderByPropertyOrField(orderBy, ascending)
                .Skip(skipCount)
                .Take(pageSize);

            var totalNumberOfRecords = queryable.Count();
            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            var results = pageResults.ToList();
            return new PagedResults<T>()
            {
                Page = page,
                PageSize = pageSize,
                Total = totalNumberOfRecords,
                TotalPages = totalPageCount,
                Results = results
            };
        }

        /// <summary>
        /// Order the IQueryable by the given property or field.
        /// </summary>

        /// <typeparam name="T">The type of the IQueryable being ordered.</typeparam>
        /// <param name="queryable">The IQueryable being ordered.</param>
        /// <param name="propertyOrFieldName">The name of the property or field to order by.</param>
        /// <param name="ascending">Indicates whether or not the order should be ascending (true) or descending (false.)</param>
        /// <returns>Returns an IQueryable ordered by the specified field.</returns>
        /// <see cref="https://schneids.net/paging-in-asp-net-web-api"/>
        public static IQueryable<T> OrderByPropertyOrField<T>(this IQueryable<T> queryable, string propertyOrFieldName, bool ascending = true)
        {
            var elementType = typeof(T);
            var orderByMethodName = ascending ? "OrderBy" : "OrderByDescending";

            var parameterExpression = Expression.Parameter(elementType);
            var propertyOrFieldExpression = Expression.PropertyOrField(parameterExpression, propertyOrFieldName);
            var selector = Expression.Lambda(propertyOrFieldExpression, parameterExpression);

            var orderByExpression = Expression.Call(typeof(Queryable), orderByMethodName,
                new[] { elementType, propertyOrFieldExpression.Type }, queryable.Expression, selector);

            return queryable.Provider.CreateQuery<T>(orderByExpression);
        }
    }
}
