using Etl.Core.Events;
using Etl.Core.Scanner;

namespace Etl.Core.Extraction;

internal class ExtractorCache
{
    private readonly LayoutCache _layout;
    private readonly List<LayoutCache>? _layoutComments;

    public ExtractorCache(Extractor extractor)
    {
        _layout = new LayoutCache(extractor.Layout);
        _layoutComments = extractor.Comments?.Select(e => new LayoutCache(e)).ToList();
    }

    public ExtractedRecord Execute(List<TextLine> textLines, IEtlEvent? events)
    {
        if (textLines == null)
            throw new ArgumentNullException(nameof(textLines));

        if (_layoutComments != null && _layoutComments.Count > 0)
            ProcessComments(_layoutComments, textLines);

        return _layout.ParseOneRecord(textLines, events);
    }

    private static void ProcessComments(List<LayoutCache> layoutComments, List<TextLine> textLines)
    {
        var i = 0;
        var current = i;
        while (i++ < textLines.Count)
        {
            current = i;
            layoutComments.ForEach(e => i = Math.Max(e.DetectComments(textLines, current), i));

            if (i > current)
                textLines.RemoveRange(current, Math.Min(i, textLines.Count) - current);
            i = current;
        }
    }
}
