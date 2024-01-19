using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     加密助手
/// </summary>
public class EncryptHelper
{
    /// <summary>
    ///     密钥存储路径
    /// </summary>
    private readonly string _keyPath;

    /// <summary>
    ///     加密助手
    /// </summary>
    /// <param name="keyPath">密钥存储路径</param>
    /// <exception cref="Exception">密钥路径未指定</exception>
    public EncryptHelper(string? keyPath)
    {
        _keyPath = keyPath ?? throw new Exception("Missing key path");
        if (!Directory.Exists(_keyPath))
        {
            Directory.CreateDirectory(_keyPath);
        }
    }

    /// <summary>
    ///     私钥字典
    /// </summary>
    public ConcurrentDictionary<string, ECDsaSecurityKey> PrivateKeys { get; } = new();

    /// <summary>
    ///     公钥字典
    /// </summary>
    public ConcurrentDictionary<string, ECDsaSecurityKey> PublicKeys { get; } = new();

    /// <summary>
    ///     获取私钥
    ///     <remarks>当指定私钥不存在时，将重新创建密钥对</remarks>
    /// </summary>
    /// <param name="name">私钥名称</param>
    /// <returns>私钥</returns>
    public ECDsaSecurityKey GetPrivateKey(string name)
    {
        if (PrivateKeys.TryGetValue(name, out var securityKey))
        {
            return securityKey;
        }

        if (!UpdateKey(name, true))
        {
            GenerateKeys(name);
            UpdateKey(name, true);
            UpdateKey(name, false);
        }

        PrivateKeys.TryGetValue(name, out securityKey);
        return securityKey!;
    }

    /// <summary>
    ///     获取公钥
    ///     <remarks>当指定公钥不存在时，将重新基于私钥创建公钥</remarks>
    /// </summary>
    /// <param name="name">公钥名称</param>
    /// <returns>公钥</returns>
    public ECDsaSecurityKey GetPublicKey(string name)
    {
        if (PublicKeys.TryGetValue(name, out var securityKey))
        {
            return securityKey;
        }

        if (!UpdateKey(name, false))
        {
            GenerateKey(GetPrivateKey(name).ECDsa, name, false);
            UpdateKey(name, false);
        }

        PublicKeys.TryGetValue(name, out securityKey);
        return securityKey!;
    }

    /// <summary>
    ///     生成JWT Token
    /// </summary>
    /// <param name="keyName">密钥名称</param>
    /// <param name="payload">JWT负载</param>
    /// <returns></returns>
    public string GenerateToken(string keyName, JwtPayload payload)
    {
        var privateKey = GetPrivateKey(keyName);
        var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.EcdsaSha256);
        var descriptor = new JwtSecurityToken(new JwtHeader(credentials), payload);

        return new JwtSecurityTokenHandler().WriteToken(descriptor);
    }

    /// <summary>
    ///     生成JWT Token
    /// </summary>
    /// <param name="keyName">密钥名称</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="claims">Claim</param>
    /// <returns></returns>
    public string GenerateToken(string keyName, TimeSpan expiration, params Claim[] claims)
    {
        var privateKey = GetPrivateKey(keyName);
        var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.EcdsaSha256);
        var currentTime = DateTime.Now;
        var expiresTime = currentTime + expiration;
        var descriptor = new JwtSecurityToken(signingCredentials: credentials, claims: claims,
            expires: expiresTime, notBefore: currentTime);

        return new JwtSecurityTokenHandler().WriteToken(descriptor);
    }

    /// <summary>
    ///     更新密钥至文件
    /// </summary>
    /// <param name="name">密钥名称</param>
    /// <param name="isPrivate">是否为私钥</param>
    /// <returns></returns>
    private bool UpdateKey(string name, bool isPrivate)
    {
        var file = Path.Combine(_keyPath, $"{name}{(isPrivate ? "_private" : "_public")}.pem");
        if (!File.Exists(file))
        {
            return false;
        }

        var content = File.ReadAllText(file);
        var key = ECDsa.Create();
        key.ImportFromPem(content);
        var securityKey = new ECDsaSecurityKey(key);
        (isPrivate ? PrivateKeys : PublicKeys).AddOrUpdate(name, securityKey, (_, _) => securityKey);
        return true;
    }

    /// <summary>
    ///     生成密钥对
    /// </summary>
    /// <param name="name">密钥名称</param>
    private void GenerateKeys(string name)
    {
        var ecdsa = ECDsa.Create();
        ecdsa.GenerateKey(ECCurve.NamedCurves.nistP256);
        GenerateKey(ecdsa, name, true);
        GenerateKey(ecdsa, name, false);
    }

    /// <summary>
    ///     生成密钥
    /// </summary>
    /// <param name="ecdsa">算法</param>
    /// <param name="name">密钥名称</param>
    /// <param name="isPrivate">是否为私钥</param>
    private void GenerateKey(ECAlgorithm ecdsa, string name, bool isPrivate)
    {
        var key = isPrivate
            ? ecdsa.ExportECPrivateKey()
            : ecdsa.ExportSubjectPublicKeyInfo();
        var content = new string(PemEncoding.Write(isPrivate ? "EC PRIVATE KEY" : "PUBLIC KEY", key));
        var path = Path.Combine(_keyPath, $"{name}{(isPrivate ? "_private" : "_public")}.pem");
        File.WriteAllText(path, content);
    }
}