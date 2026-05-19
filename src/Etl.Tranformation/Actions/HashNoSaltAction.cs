using System.Text;
using System.Security.Cryptography;
using Etl.Core.Transformation.Actions;

namespace Etl.Tranformation.Actions;

public class HashNoSaltAction : TransformAction<HashNoSaltActionInst> { }

public class HashNoSaltActionInst : TransformActionInst<string?>
{
    public HashNoSaltActionInst()
    {

    }

    protected override string? Execute(object? input, ActionArgs args)
    {
        var text = input as string;
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return Hash(text);
    }

    public static string Hash(string text)
    {
        UTF8Encoding encoder = new();

        byte[] bytes = default!;
        using (SHA256Managed hasher = new())
            bytes = hasher.ComputeHash(encoder.GetBytes(text));

        StringBuilder sb = new(bytes.Length);
        for (int i = 0; i < bytes.Length; i++)
            sb.Append(bytes[i].ToString("X2"));

        return sb.ToString();
    }
}
