using DoliteTemplate.Api.Shared.Services;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     事务注解
///     <remarks>用于标记<see cref="BaseService" />派生类中的HTTP方法，该方法会以事务的方式执行。</remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TransactionAttribute : Attribute;