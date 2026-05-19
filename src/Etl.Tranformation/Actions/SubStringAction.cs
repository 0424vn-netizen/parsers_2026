using Etl.Core.Transformation.Actions;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Etl.Tranformation.Actions;

public class SubStringAction : TransformAction<SubStringActionInst>
{
    [XmlAttribute]
    public string? Start { get; set; }

    [XmlAttribute]
    public int StartOffset { get; set; }

    [XmlAttribute]
    public string? End { get; set; }

    [XmlAttribute]
    public int EndOffset { get; set; }

    internal Lazy<Regex>? _regexStart;
    internal Lazy<Regex>? _regexEnd;

    public SubStringAction()
    {
        _regexStart = string.IsNullOrWhiteSpace(Start) ? null : new Lazy<Regex>(() => new Regex(Start, RegexOptions.Compiled));
        _regexEnd = string.IsNullOrWhiteSpace(End) ? null : new Lazy<Regex>(() => new Regex(End, RegexOptions.Compiled));
    }
   public void RefreshRegex()
    {
        _regexStart = string.IsNullOrWhiteSpace(Start) ? null : new Lazy<Regex>(() => new Regex(Start, RegexOptions.Compiled));
        _regexEnd = string.IsNullOrWhiteSpace(End) ? null : new Lazy<Regex>(() => new Regex(End, RegexOptions.Compiled));
    }
}
public class SubStringActionInst : TransformActionInst<SubStringAction, string?>
{
    private SubStringAction? _definition;

    protected override void Initialize(SubStringAction definition, IServiceProvider sp)
    {
        definition.RefreshRegex();
        _definition = definition;
    }


    protected override string? Execute(object? input, ActionArgs args)
    {
        if (input is not string text)
            return null;

        var def = _definition!;
        var regexStart = def._regexStart;
        var regexEnd = def._regexEnd;

        var start = 0;
        if (regexStart != null)
        {
            var match = regexStart.Value.Match(text);
            if (match.Success)
                start = regexStart.Value.Match(text).Index;
            else
                return null;
        }

        start += def.StartOffset;
        if (start >= text.Length)
            return null;

        var length = 0;
        if (regexEnd != null)
        {
            var match = start >= text.Length ? null : regexEnd.Value.Match(text, start);
            if (match != null && match.Success)
                length = match.Index - start;
            else
                throw new InvalidOperationException($"{nameof(SubStringAction)} can't find {nameof(_definition.End)}: {def.End}.");
        }

        length += def.EndOffset;

        return regexEnd == null && length == 0 || start + length >= text.Length
            ? text[start..]
            : text.Substring(start, length);
    }
}
