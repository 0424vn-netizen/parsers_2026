using Etl.Core.Extraction;
using Etl.Core.Load;
using Etl.Core.Scanner;
using Etl.Core.Transformation;

namespace Etl.Core.Events;

public static class LoggerHelper
{
    public static void ApplyLog(IEtlEvent events, ILogWriter writer, LogOptions options)
    {
        if (options.OnScanned)
            events.OnScanned = e => OnScanned(writer.Debug, e);

        if (options.OnExtracting)
            events.OnExtracting = (layout, hierarchy, block, start, from, end, dataField, value) =>
                OnExtracting(writer.Debug, layout, hierarchy, block, start, from, end, dataField, value);

        if (options.OnExtracted)
            events.OnExtracted = e => OnExtracted(writer.Debug, e);

        if (options.OnTransformed)
            events.OnTransformed = e => OnTransformed(writer.Debug, e);

        if (options.OnTransformedBatch)
            events.OnTransformedBatch = e => OnTransformedBatch(writer.Debug, e);

        if (options.OnError)
            events.OnError = (message, e) => OnError(writer.Error, message, e);

        if (options.OnStatusIntervalSeconds > 0)
            events.OnStatusChanged = (options.OnStatusIntervalSeconds, status => OnStatus(writer.Debug, status));
    }

    public static void OnStart(Action<string> writer, int count)
        => writer($"==========   SCANNED RECORD {count}   ============");

    public static void OnScanned(Action<string> writer, IReadOnlyCollection<TextLine> textLines)
    {
        if (textLines != null)
            foreach (var e in textLines)
                writer(e.ToString());
    }

    public static void OnExtracting(
        Action<string> writer,
        Layout layout,
        int hierarchy,
        TextBlock block,
        (int row, int column)? start, (int row, int column)? from, (int row, int column)? to,
        string? dataField = null, IExtractedInfo? result = null)
    {
        if (block.Count == 0)
            return;

        var sb = new StringBuilder(" ".PadLeft(hierarchy * 4));

        if (start != null)
        {
            sb.Append($"STR {layout.Direction}");

            if (!string.IsNullOrEmpty(layout.Start))
                sb.Append($", Regex={layout.Start}");

            if (layout.StartOffset != 0)
                sb.Append($", Offset={layout.StartOffset}");

            sb.Append($", Current(R.{block[Math.Min(block.Count - 1, start.Value.row)].Row},{WriteColumn(start)})");

            if (to != null)
                sb.Append($", To(R.{block[Math.Min(block.Count - 1, to.Value.row)].Row},{WriteColumn(to)})");

            writer(sb.ToString());
        }
        else if (from != null)
        {
            sb.Append($"    From(R.{block[Math.Min(block.Count - 1, from.Value.row)].Row},{WriteColumn(from)})");

            if (to != null)
                sb.Append($", To(R.{block[Math.Min(block.Count - 1, to.Value.row)].Row},{WriteColumn(to)})");

            if (!string.IsNullOrEmpty(dataField))
                sb.Append($" ===> Field({dataField},{(result == null ? "NULL" : block.GetValue(result.From, result.To))})");

            writer(sb.ToString());
        }
        else if (to != null)
        {
            sb.Append($"END {layout.Direction}");

            if (!string.IsNullOrEmpty(layout.End))
                sb.Append($", Regex={layout.End}");

            if (layout.EndOffset != 0)
                sb.Append($", Offset={layout.EndOffset}");

            sb.Append($", EndPos(R.{block[Math.Min(block.Count - 1, to.Value.row)].Row},{WriteColumn(to)})");
            writer(sb.ToString());
        }

        static string WriteColumn((int row, int column)? pos)
            => pos == null || pos.Value.column == int.MaxValue ? "~" : pos.Value.column.ToString();
    }

    public static void OnExtracted(Action<string> writer, ExtractedRecord record)
        => writer(LogExtractedRecord("\nEXTRACT RESULT:\n____________\n", record));
    public static string LogExtractedRecord(string message, ExtractedRecord record)
    {
        var sb = new StringBuilder(message);
        foreach (var e in record)
            LogExtractedValue("", e, record.Block, sb);

        return sb.ToString();
    }

    public static void LogExtractedValue(string pad, KeyValuePair<string, IExtractedInfo> field, TextBlock block, StringBuilder sb)
    {
        var space = "    ";
        if (field.Value is ExtractedRecord group)
        {
            sb.AppendLine($"{pad}{field.Key} :");
            foreach (var f in group)
                LogExtractedValue(pad + space, f, block, sb);
        }
        else if (field.Value is ExtractedArray array)
        {
            sb.AppendLine($"{pad}{field.Key} :");
            foreach (var record in array)
                if (record.Count > 0)
                {
                    sb.AppendLine(pad + space + "---------------");
                    foreach (var f in record)
                        LogExtractedValue(pad + space, f, block, sb);
                }
        }
        else
            sb.AppendLine($"{pad}{field.Key} : {block.GetValue(field.Value)}");
    }

    public static void OnTransformed(Action<string> writer, TransformResult result)
    {
        writer("\nTRANSFORM RESULT:");
        foreach (var e in result.Batch)
            writer(BuildMessage("---------------\n", e));

        if (result.Errors != null)
            foreach (var e in result.Errors)
                writer($"---------------\n{e}");
    }

    public static void OnTransformedBatch(Action<string> writer, BatchResult result)
        => writer($"BATCH RESULT: {result}");

    public static void OnError(Action<string, Exception> writer, string message, Exception exception)
        => writer(message, exception);

    private static void OnStatus(Action<string> writter, IEtlStatus status)
        => writter($"STATUS: {status}");

    public static string BuildMessage(string message, IRecord record)
    {
        var sb = new StringBuilder(message);
        foreach (var e in record)
            BuildMessage("", e, sb);

        return sb.ToString();
    }

    public static void BuildMessage(string pad, KeyValuePair<string, object?> field, StringBuilder sb)
    {
        var space = "    ";
        if (field.Value is IRecord group)
        {
            sb.AppendLine($"{pad}{field.Key} :");
            foreach (var f in group)
                BuildMessage(pad + space, f, sb);
        }
        else if (field.Value is Records array)
        {
            sb.AppendLine($"{pad}{field.Key} :");
            foreach (var record in array)
                if (record.Count > 0)
                {
                    sb.AppendLine(pad + space + "---------------");
                    foreach (var f in record)
                        BuildMessage(pad + space, f, sb);
                }
        }
        else
            sb.AppendLine($"{pad}{field.Key} : {field.Value}");
    }
}
