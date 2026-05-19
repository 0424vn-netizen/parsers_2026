using Etl.Core.Settings;
using Etl.Core.Utils;
using System.Text;
using System.Xml;

namespace Etl.Tranformation;

public interface ICryptorInfo
{
    (byte[]? key, byte[]? iv) Config { get; }

    string? SaltHashString { get; }
}

public class CryptorInfo : ICryptorInfo
{
    public (byte[]? key, byte[]? iv) Config { get;  }

    public string? SaltHashString { get;  }

    public CryptorInfo(EtlSetting etlSetting)
    {
        var cryptor = etlSetting.Transformation?.Cryptor;
        var hashFilePath = cryptor?.HashFile;
        var cryptorFilePath = cryptor?.CryptorFile;

        if (!string.IsNullOrEmpty(hashFilePath))
        {
            if (!File.Exists(hashFilePath))
                throw new FileNotFoundException($"Hash File not exist {hashFilePath}");

            var doc = new XmlDocument();
            doc.Load(hashFilePath);
            SaltHashString = Cryptor.Decrypt(doc.SelectSingleNode("//salts/salt")?.Attributes?["value"]?.Value);
        }

        byte[]? encryptorKey = null;
        byte[]? encryptorIv = null;
        if (!string.IsNullOrEmpty(cryptorFilePath))
        {
            if (!File.Exists(cryptorFilePath))
                throw new FileNotFoundException($"File not exist {cryptorFilePath}");
            var doc = new XmlDocument();
            doc.Load(cryptorFilePath);
            var attributes = doc.SelectNodes("//keys/key")?[0]?.Attributes;

            encryptorKey = Encoding.ASCII.GetBytes(Cryptor.Decrypt(attributes?["value"]?.Value));
            encryptorIv = Encoding.ASCII.GetBytes(Cryptor.Decrypt(attributes?["iv"]?.Value));
        }

        Config = (encryptorKey, encryptorIv);
    }
}
