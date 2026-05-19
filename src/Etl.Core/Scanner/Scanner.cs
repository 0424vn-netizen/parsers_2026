using System.Text.RegularExpressions;

namespace Etl.Core.Scanner;

public record ScannedRecord(List<TextLine> Lines, float Progress);

public class Scanner : IDisposable
{
    public static readonly ScannedRecord END = new(new(), 100);
    private readonly (Regex? regex, int offset) _startLayout;
    private readonly (Regex? regex, int offset) _startRecord;
    private readonly (Regex? regex, int offset) _endRecord;
    private readonly StreamReader _streamReader;
    private readonly Func<ScannedRecord, Task>? _onFlushAsync;

    private int _currentLine = 0;
    private bool _disposedValue;


    public Scanner(
        StreamReader readerStream,
        (Regex? regex, int offset) startLayout,
        (Regex? regex, int offset) startRecord,
        (Regex? regex, int offset) endRecord,
        Func<ScannedRecord, Task>? onFlushAsync)
    {
        _streamReader = readerStream;
        _startLayout = startLayout;
        _startRecord = startRecord;
        _endRecord = endRecord;
        _onFlushAsync = onFlushAsync;
    }

    public async Task StartAsync(int? take = null, int? skip = null)
    {
        take = take < 1 ? null : take;
        skip = skip < 1 ? null : skip;
        var reader = GetEnumerator();
        reader.MoveNext();

        MoveTo(_startLayout.regex, _startLayout.offset, reader);

        while (reader.Current != TextLine.End)
        {
            MoveTo(_startRecord.regex, _startRecord.offset, reader);

            if (reader.Current == TextLine.End)
                break;

            var items = GetTo(_endRecord.regex, _endRecord.offset, reader);

            if (skip == null || --skip < 0)
            {
                if (take != null && --take < 0)
                    break;

                var progress = _streamReader.BaseStream.Position * 100 / _streamReader.BaseStream.Length;
                if (_onFlushAsync != null)
                    await _onFlushAsync.Invoke(new(items, progress));
            }
        }

        if (_onFlushAsync != null)
            await _onFlushAsync.Invoke(END);
    }

    private static void MoveTo(Regex? regex, int offset, IEnumerator<TextLine> reader)
    {
        while (reader.Current != TextLine.End && regex != null && !regex.IsMatch(reader.Current.Text))
            reader.MoveNext();

        while (reader.Current != TextLine.End && offset-- > 0)
            reader.MoveNext();
    }

    private static List<TextLine> GetTo(Regex? regex, int offset, IEnumerator<TextLine> reader)
    {
        var items = new List<TextLine>();

        if (regex != null)
        {
            items.Add(reader.Current);
            reader.MoveNext();
            while (reader.Current != TextLine.End && !regex.IsMatch(reader.Current.Text))
            {
                items.Add(reader.Current);
                reader.MoveNext();
            }
        }

        while (reader.Current != TextLine.End && offset-- > 0)
        {
            items.Add(reader.Current);
            reader.MoveNext();
        }

        return items;
    }

    private IEnumerator<TextLine> GetEnumerator()
    {
        IEnumerable<TextLine> GetIEnumerable()
        {
            while (!_streamReader.EndOfStream)
                yield return new TextLine(_streamReader.ReadLine() ?? String.Empty, ++_currentLine);

            yield return TextLine.End;
        }

        return GetIEnumerable().GetEnumerator();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _streamReader?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
