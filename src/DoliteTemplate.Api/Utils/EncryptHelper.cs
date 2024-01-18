using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DoliteTemplate.Api.Utils;

public class EncryptHelper
{
    private readonly string _keyPath;

    public EncryptHelper(string? keyPath)
    {
        _keyPath = keyPath ?? throw new Exception("Missing key path");
        if (!Directory.Exists(_keyPath))
        {
            Directory.CreateDirectory(_keyPath);
        }
    }

    public ConcurrentDictionary<string, ECDsaSecurityKey> PrivateKeys { get; } = [];
    public ConcurrentDictionary<string, ECDsaSecurityKey> PublicKeys { get; } = [];

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

    public string GenerateToken(string keyName, JwtPayload payload)
    {
        var privateKey = GetPrivateKey(keyName);
        var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.EcdsaSha256);
        var descriptor = new JwtSecurityToken(new JwtHeader(credentials), payload);

        return new JwtSecurityTokenHandler().WriteToken(descriptor);
    }

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

    private void GenerateKeys(string name)
    {
        var ecdsa = ECDsa.Create();
        ecdsa.GenerateKey(ECCurve.NamedCurves.nistP256);
        GenerateKey(ecdsa, name, true);
        GenerateKey(ecdsa, name, false);
    }

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