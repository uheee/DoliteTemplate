#pragma warning disable CS1573
#pragma warning disable CS1712

using System.Data.Common;
using AutoMapper;
using DoliteTemplate.Api.Shared.Errors;
using DoliteTemplate.Api.Shared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Shared.Services;

/// <summary>
///     基础服务
/// </summary>
/// <param name="mapper">映射器</param>
public abstract class BaseService(IMapper mapper) : ControllerBase
{
    /// <summary>
    ///     映射器
    /// </summary>
    public IMapper Mapper { get; } = mapper;
}

/// <inheritdoc cref="BaseService" />
/// <param name="localizer">本地化组件</param>
/// <typeparam name="TService">派生服务类型</typeparam>
public class BaseService<TService>(
    IMapper mapper,
    IStringLocalizer<TService> localizer) :
    BaseService(mapper),
    ICulturalResource<TService>
    where TService : BaseService<TService>
{
    /// <summary>
    ///     业务逻辑错误
    /// </summary>
    /// <param name="errCode">
    ///     错误代码
    ///     <para>涉及参数以{0} {1}...方式配置</para>
    /// </param>
    /// <param name="args">参数</param>
    /// <returns></returns>
    [NonAction]
    public BusinessException Error(string errCode, params object[] args)
    {
        var errTemplate = localizer[errCode];
        var errMsg = string.Format(errTemplate, args);
        if (string.IsNullOrEmpty(errMsg))
        {
            errMsg = "unknown";
        }

        return new BusinessException(errCode, errMsg);
    }
}

/// <inheritdoc cref="BaseService{TService}" />
/// <param name="dbContextProvider">数据库上下文惰性实例</param>
/// <typeparam name="TDbContext">数据库上下文类型</typeparam>
public class BaseService<TService, TDbContext>(
    IMapper mapper,
    IStringLocalizer<TService> localizer,
    Lazy<TDbContext> dbContextProvider) :
    BaseService<TService>(
        mapper,
        localizer)
    where TService : BaseService<TService, TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     数据库上下文
    /// </summary>
    public TDbContext DbContext => dbContextProvider.Value;

    /// <summary>
    ///     数据库连接
    /// </summary>
    public DbConnection DbConnection => DbContext.Database.GetDbConnection();
}