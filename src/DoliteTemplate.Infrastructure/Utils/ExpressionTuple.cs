using System.Linq.Expressions;

namespace DoliteTemplate.Infrastructure.Utils;

public record ExpressionTuple(
    MemberExpression Member,
    LambdaExpression Expression);