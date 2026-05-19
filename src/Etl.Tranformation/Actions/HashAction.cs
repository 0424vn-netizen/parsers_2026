using Etl.Core.Transformation.Actions;

namespace Etl.Tranformation.Actions;

public class HashAction : TransformAction<HashActionInst> { }

public class HashActionInst : TransformActionInst<string?>
{
    private readonly ICryptorInfo _crytorInfo;

    private static readonly Lazy<ParserHashData.HashData> _lazyHashInstance = new Lazy<ParserHashData.HashData>(() => new ParserHashData.HashData(), LazyThreadSafetyMode.ExecutionAndPublication);

    public HashActionInst(ICryptorInfo cryptorInfo)
    {
        _crytorInfo = cryptorInfo;
    }

    protected override string? Execute(object? input, ActionArgs args)
    {
        var text = input as string;
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return string.IsNullOrWhiteSpace(_crytorInfo?.SaltHashString) ? Hash(text) : Hash(text + _crytorInfo.SaltHashString);
    }

    public static string Hash(string text)
    {
        try
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var result = _lazyHashInstance.Value.HashCardNumber(text);
            return result ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Hash method: {ex.Message}");
            return string.Empty;
            throw;
        }

    }
}
