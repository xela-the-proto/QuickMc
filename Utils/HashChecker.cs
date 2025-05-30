using System;
using System.IO;
using System.Security.Cryptography;

namespace QuickMC.Utils;

public class HashChecker
{
    public static string GenerateHashLocal(string filename)
    {
        byte[] hash;
        using (FileStream stream = File.Open(filename, FileMode.Open))
        {
            var hashFun = SHA256.Create();
            hash =  hashFun.ComputeHash(stream);
        }
        return BitConverter.ToString(hash).Replace("-",string.Empty);
        
    }

    public bool isEqualHash(string hash1, string hash2)
    {
        if (hash1 == hash2)
        {
            return true;
        }
        
        return false;
        
    }
}