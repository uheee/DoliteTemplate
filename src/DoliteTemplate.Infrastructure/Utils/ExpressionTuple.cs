using System.Linq.Expressions;

namespace DoliteTemplate.Infrastructure.Utils;

/// <summary>
///     表达式元组
/// </summary>
/// <param name="Member">成员表达式</param>
/// <param name="Expression">Lambda表达式</param>
public record ExpressionTuple(
    MemberExpression Member,
    LambdaExpression Expression);