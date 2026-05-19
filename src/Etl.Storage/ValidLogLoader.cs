using Etl.Core.Load;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Etl.Storage;

public class ValidLogLoader : FileLoader<ValidLogLoader, ValidLogLoaderInst>
{
    [XmlAttribute]
    public override string OutPath { get; set; } = "$path/$name.log";
    public List<CustomField> CustomFields { get; set; }
    public List<string> Fields { get; set; }
    [XmlAttribute]
    public string FileType { get; set; }
    [XmlAttribute]
    public string ReportDate { get; set; }
    public ValidLogLoader()
    {
        this.Fields = new()
        {
            ConstantValidLog.TotalRecordCount,
           ConstantValidLog.TotalTransformSuccess,
           ConstantValidLog.TotalTransformErrors,
        };
    }

}

public class ValidLogLoaderInst : FileLoaderInst<ValidLogLoader, ValidLogLoaderInst>
{
    public List<CustomField> _customFields { get; set; } = new List<CustomField> { };

    private List<string> _defaultFields { get; set; }

    private IDictionary<string, object> _dictCustomFields { get; set; } = new Dictionary<string, object>();

    private string _fileName { get; set; }

    private string _reportDate { get; set; }

    protected override void Initalize(ValidLogLoader defintion, LoaderArgs args)
    {
        base.Initalize(defintion, args);

        HashSet<string> selectedFields = defintion.Fields.Count == 0
            ? new(args.Fields.Select(e => e.Alias ?? e.DataField))
            : new(defintion.Fields);

        this._reportDate = defintion.ReportDate;
        this._defaultFields = defintion.Fields;
        this._fileName = (new FileInfo(args.InputFile)).Name;
        this._customFields = defintion.CustomFields;
    }

    public override async Task ProcessBatchAsync(BatchResult batch)
    {
        try
        {
            await this.CheckAddCustomFiledToDict(batch);
            if (batch.IsLast)
            {
                await this.ProcessReportDate(batch);
                foreach (string _defaultField in this._defaultFields)
                {
                    await this.WriteLineAsync(_defaultField, this.GetValueByDefaultField(_defaultField, batch));
                }
                foreach (CustomField _customField in this._customFields)
                {
                    await this.WriteLineAsync(_customField.Text, this.GetValueInDictionary(_customField.Text, this._dictCustomFields));
                }
                this.OnCompleted();
            }
        }
        catch (Exception)
        {

        }
    }

    #region Private method
    private async Task CheckAddCustomFiledToDict(BatchResult result)
    {
        try
        {
            bool? nullable;
            if (result != null)
            {
                var batch = result.Batch;
                if (batch != null)
                {
                    nullable = new bool?(Enumerable.Any<IDictionary<string, object>>(batch));
                }
                else
                {
                    nullable = null;
                }
                bool? nullable1 = nullable;
                if (!nullable1.GetValueOrDefault() & nullable1.HasValue)
                {
                    return;
                }
            }
            if (_customFields == null || _customFields.Count == 0) { return; }
            foreach (CustomField _customField in this._customFields)
            {
                var cusFieldsData = result?.Batch?.ElementAt(0) ?? new Dictionary<string, object?>();
                if (this._dictCustomFields.ContainsKey(_customField.Text))
                {
                    if (_customField.Force)
                    {
                        continue;
                    }
                    var valueInDictionary = GetValueInDictionary(_customField.Text, this._dictCustomFields);
                    if (!cusFieldsData.ContainsKey(_customField.Value) || valueInDictionary.Equals(this.GetValueInDictionary(_customField.Value, cusFieldsData)))
                    {
                        continue;
                    }
                    this._dictCustomFields[_customField.Text] = "It must be unique value!";
                }
                else if (_customField.Force)
                {
                    this._dictCustomFields.Add(_customField.Text, _customField.Value);
                }
                else if (!cusFieldsData.ContainsKey(_customField.Value))
                {
                    this._dictCustomFields.Add(_customField.Text, _customField.Text);
                }
                else
                {
                    this._dictCustomFields.Add(_customField.Text, this.GetValueInDictionary(_customField.Value, cusFieldsData));
                }
            }

        }
        catch (Exception ex)
        {
            await this.WriteLineAsync("ErrorException", ex.Message);
        }
    }

    private async Task ProcessReportDate(BatchResult result)
    {
        try
        {
            DateTime dateTime = new DateTime();
            if (string.IsNullOrWhiteSpace(this._reportDate))
            {
                return;
            }
            string stringDateTimeValue = this.GetStringDateTimeValue(this._reportDate);
            if (!string.IsNullOrWhiteSpace(stringDateTimeValue))
            {
                await this.WriteLineAsync("ReportDate", stringDateTimeValue);
                return;
            }
            if (!result.Batch.ElementAt(0).ContainsKey(this._reportDate))
            {
                MatchCollection matchCollection = (new Regex("\\d{8}")).Matches(this._fileName);
                if (matchCollection.Count() > 0)
                {
                    stringDateTimeValue = matchCollection.ElementAt(matchCollection.Count() - 1).ToString();
                    stringDateTimeValue = string.Concat(new string[] { stringDateTimeValue.Substring(0, 4), "/", stringDateTimeValue.Substring(4, 2), "/", stringDateTimeValue.Substring(6, 2) });
                    if (DateTime.TryParse(stringDateTimeValue, out dateTime))
                    {
                        await this.WriteLineAsync("ReportDate", dateTime.ToString("yyyy/MM/dd"));
                    }
                }
            }
            else
            {
                stringDateTimeValue = this.GetValueInDictionary(this._reportDate, result.Batch.ElementAt(0));
                stringDateTimeValue = this.GetStringDateTimeValue(stringDateTimeValue);
                if (!string.IsNullOrWhiteSpace(stringDateTimeValue))
                {
                    await this.WriteLineAsync("ReportDate", stringDateTimeValue);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            await this.WriteLineAsync("ErrorException", ex.Message);
        }
    }

    private string GetValueByDefaultField(string propName, BatchResult result)
    {
        string str = "";
        switch (propName)
        {
            case ConstantValidLog.TotalRecordCount:
                str = (result.TotalTransformSuccess + result.TotalTransformErrors).ToString();
                break;
            case ConstantValidLog.TotalTransformSuccess:
                str = result.TotalTransformSuccess.ToString();
                break;
            case ConstantValidLog.TotalTransformErrors:
                str = result.TotalTransformErrors.ToString();
                break;
            default:
                break;
        }
        return str;
    }

    private async Task WriteLineAsync(string fieldName, string? value)
    {
        if (Writer != null)
        {
            await Writer.WriteLineAsync(string.Concat(fieldName, ":", value));
        }
    }
    #endregion
}

public class ConstantValidLog
{
    public const string TotalRecordCount = "TotalRecordCount";
    public const string TotalTransformSuccess = "GoodRecordCount";
    public const string TotalTransformErrors = "BadRecordCount";
}
