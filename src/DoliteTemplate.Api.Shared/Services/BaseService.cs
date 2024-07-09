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
public abstract class BaseService : ControllerBase
{
    /// <summary>
    ///     映射器
    /// </summary>
    public IMapper Mapper { get; init; }
}

/// <inheritdoc cref="BaseService" />
/// <typeparam name="TService">派生服务类型</typeparam>
public class BaseService<TService> :
    BaseService,
    ICulturalResource<TService>
    where TService : BaseService<TService>
{
    /// <summary>
    ///     本地化组件
    /// </summary>
    public IStringLocalizer<TService> Localizer { get; init; }

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
        var errTemplate = Localizer[errCode];
        var errMsg = string.Format(errTemplate, args);
        if (string.IsNullOrEmpty(errMsg))
        {
            errMsg = "unknown";
        }

        return new BusinessException(errCode, errMsg);
    }
}

/// <inheritdoc cref="BaseService{TService}" />
/// <typeparam name="TDbContext">数据库上下文类型</typeparam>
public class BaseService<TService, TDbContext> :
    BaseService<TService>
    where TService : BaseService<TService, TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     数据库上下文惰性实例
    /// </summary>
    public Lazy<TDbContext> DbContextProvider { get; init; }

    /// <summary>
    ///     数据库上下文
    /// </summary>
    public TDbContext DbContext => DbContextProvider.Value;

    /// <summary>
    ///     数据库连接
    /// </summary>
    public DbConnection DbConnection => DbContext.Database.GetDbConnection();
}