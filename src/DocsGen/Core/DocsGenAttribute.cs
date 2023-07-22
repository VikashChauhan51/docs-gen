namespace DocsGen.Core;

public abstract class DocsGenAttribute : Attribute
{
    public string Message { get; init; } = string.Empty;

    public string IndentationSpace { get; set; } = string.Empty;
    public DocsGenAttribute()
    {

    }
    public DocsGenAttribute(string message)
    {
        this.Message = message ?? string.Empty;
    }

    public abstract string ToString(DocType doc);

    public override string ToString()
    {
        return Message;
    }

    protected string GetMessageAllTrim(string text)
    {

        if (!string.IsNullOrEmpty(text))
        {
            var messages = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (messages == null || messages.Length == 0)
            {
                return string.Empty;
            }
            return string.Join("", messages);
        }
        return text;
    }
}
