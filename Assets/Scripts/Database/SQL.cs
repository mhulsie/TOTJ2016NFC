using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

/// <summary>
/// The main class that talks with the database. All queries go trough this class
/// </summary>
public class SQL
{
    private static SQL instance;
    public static SQL Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SQL();
            }
            return instance;
        }
    }

    private SQL() { }
    /// <summary>
    /// The query function.
    /// </summary>
    /// <param name="query"> The query you want to execute</param>
    /// <returns>The webrequest.text. This contains all text of the webpage</returns>
    public string executeQuery(string query)
    {
        //WWW www = new WWW("http://questionmarkgames.nl/API/queryEncrypt.php?q=" + AES_encrypt(WWW.EscapeURL(query)));
        WWW www = new WWW("http://questionmarkgames.nl/API/query.php?s=abcd&q=" + WWW.EscapeURL(query));
        while (!www.isDone) { }
        //return AES_decrypt(www.text);
        return www.text;
    }

    public static String AES_decrypt(String Input)
    {
        RijndaelManaged aes = new RijndaelManaged();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;

        aes.Key = Convert.FromBase64String("PSVJQRk9QTEpNVU1DWUZCRVFGV1VVT0ZOV1RRU1NaWQ=");
        aes.IV = Convert.FromBase64String("ZG9kb2lzY29vbHRlc3QxMg==");
        var decrypt = aes.CreateDecryptor();
        byte[] xBuff = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
            {
                byte[] xXml = Convert.FromBase64String(Input);
                cs.Write(xXml, 0, xXml.Length);
            }
            xBuff = ms.ToArray();
        }
        String Output = Encoding.UTF8.GetString(xBuff);
        return Output;
    }

    public static string Encrypt(string PlainText)
    {
        RijndaelManaged aes = new RijndaelManaged();
        aes.BlockSize = 128;
        aes.KeySize = 256;

        aes.Mode = CipherMode.CBC;
        // aes.Padding = PaddingMode.None;

        aes.Key = Convert.FromBase64String("PSVJQRk9QTEpNVU1DWUZCRVFGV1VVT0ZOV1RRU1NaWQ=");
        aes.IV = Convert.FromBase64String("ZG9kb2lzY29vbHRlc3QxMg==");


        ICryptoTransform encrypto = aes.CreateEncryptor();

        byte[] plainTextByte = ASCIIEncoding.UTF8.GetBytes(PlainText);
        byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
        return Convert.ToBase64String(CipherText);
    }

}