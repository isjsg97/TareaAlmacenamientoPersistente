using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;

public static class FileSystem
{
    private const string FILE_SECRET = "secret";

    private static Secret _mySecret;

    public class Secret
    {
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }
    }

    private static Secret MySecret
    {
        get
        {
            if (_mySecret == null) _mySecret = GetSecret();
            return _mySecret;
        }
    }

    private static Secret GetSecret()
    {
        var fileSecret = Resources.Load(FILE_SECRET) as TextAsset;
        return JsonConvert.DeserializeObject<Secret>(fileSecret.text);
    }

    public static bool Load<T>(string fileName, out T data)
    {
        data = default;
        var loaded = false;

        if (LoadBytes(fileName, out var bytes))
        {
            using (var ms = new MemoryStream(bytes))
            using (var sr = new StreamReader(ms))
            {
                data = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                loaded = true;
            }
        }

        return loaded;
    }

    public static bool LoadEncrypt<T>(string fileName, out T data)
    {
        data = default;
        var secret = MySecret;
        bool loaded = false;  

        if (LoadBytes(fileName, out var bytes))
        {
            using (var aes = Aes.Create())
            {
                var decryptor = aes.CreateDecryptor(secret.Key, secret.IV);

                using (var msDecrypt = new MemoryStream(bytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    data = JsonConvert.DeserializeObject<T>(srDecrypt.ReadToEnd());
                    loaded = true;
                }
            }
        }

        return loaded;
    }

    public static bool Save<T>(T data, string fileName)
    {
        var saved = false;

        using (var ms = new MemoryStream())
        {
            using (var sw = new StreamWriter(ms))
            {
                sw.Write(JsonConvert.SerializeObject(data));            
            }
            saved = SaveBytes(ms.ToArray(), fileName);
        }

        return saved;
    }

    public static bool SaveEncrypt<T>(T data, string fileName)
    {
        var secret = MySecret;
        bool saved = false;
        var json = JsonConvert.SerializeObject(data);

        using (var aes = Aes.Create())
        {
            var encryptor = aes.CreateEncryptor(secret.Key, secret.IV);

            using (var msEncrypt = new MemoryStream())
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(json);
                }
                saved = SaveBytes(msEncrypt.ToArray(), fileName);
            }
        }

        return saved;
    }

    private static string GetFilePath(string fileName)
    {
        StringBuilder pathBuilder = new StringBuilder();

        #if UNITY_EDITOR
                pathBuilder.Append(Path.Combine(Application.dataPath, @"..\.."));
        #else
                        pathBuilder.Append(Directory.GetParent(Application.dataPath));
        #endif

        pathBuilder.Append("/Data");

        var parentPath = pathBuilder.ToString();
        if (!Directory.Exists(parentPath)) Directory.CreateDirectory(parentPath);

        pathBuilder.Append($"/{fileName}");

        return pathBuilder.ToString();
    }

    private static bool LoadBytes(string fileName, out byte[] data)
    {
        data = null;
        var loaded = false;
        string path = GetFilePath(fileName); 

        try
        {
            if (File.Exists(path))
            {
                data = File.ReadAllBytes(path);
                loaded = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"File System Error: {ex.Message} -> {ex.StackTrace}");
        }

        return loaded;
    }

    private static bool SaveBytes(byte[] data, string fileName)
    {
        var saved = false;
        string path = GetFilePath(fileName);

        try
        {
            File.WriteAllBytes(path, data);
            saved = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"File System Error: {ex.Message} -> {ex.StackTrace}");
        }

        return saved;
    }
}
