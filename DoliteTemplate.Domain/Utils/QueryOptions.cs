using System.Linq.Expressions;

namespace DoliteTemplate.Domain.Utils;

public static class QueryOptions
{
    public static QueryOptions<TEntity> For<TEntity>()
    {
        return new();
    }

    public static QueryOptions<TEntity> Where<TEntity>(this QueryOptions<TEntity> options,
        params Expression<Func<TEntity, bool>>[] predicates)
    {
        options.Predicates.AddRange(predicates);
        return options;
    }

    public static QueryOptions<TEntity> WhereIf<TEntity>(this QueryOptions<TEntity> options,
        bool condition, params Expression<Func<TEntity, bool>>[] predicates)
    {
        return condition ? options.Where(predicates) : options;
    }

    public static QueryOptions<TEntity> OrderBy<TEntity>(this QueryOptions<TEntity> options,
        Expression<Func<TEntity, object>> orderSelector, OrderType orderType)
    {
        options.OrderSelectors.Add((orderSelector, orderType));
        return options;
    }
}

public class QueryOptions<TEntity>
{
    public List<Expression<Func<TEntity, bool>>> Predicates { get; } = new();
    public List<(Expression<Func<TEntity, object>>, OrderType)> OrderSelectors { get; } = new();
}

public enum OrderType
{
    Asc,
    Desc
}