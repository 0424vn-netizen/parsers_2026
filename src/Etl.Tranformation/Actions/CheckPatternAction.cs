using Etl.Core.Transformation.Actions;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Etl.Tranformation.Actions;

public class CheckPatternAction : TransformAction<CheckPatternActionInst>
{
    public Lazy<Regex>? RegexPattern
    {
        get
        {
            return string.IsNullOrWhiteSpace(Pattern)
                ? null
                : new(() => new Regex(Pattern, RegexOptions.Compiled));
        }
    }

    [XmlAttribute]
    public string Pattern { get; set; } = String.Empty;


    public CheckPatternAction()
    {
    }
}

public class CheckPatternActionInst : ValidateActionInst<CheckPatternAction>
{
    private Lazy<Regex>? _regexPattern;
    protected override void Initialize(CheckPatternAction definition, IServiceProvider sp)
    {
        _regexPattern = definition.RegexPattern;
    }

    protected override void Execute(object? input, ActionArgs args)
    {
        if (_regexPattern?.Value == null)
            return;

        var text = input?.ToString() ?? "";

        if (!_regexPattern.Value.IsMatch(text))
            throw new InvalidOperationException($"Not match pattern '{_regexPattern.Value}'");
    }
}
