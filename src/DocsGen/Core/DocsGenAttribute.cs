namespace DocsGen.Core;

public abstract class DocsGenAttribute : Attribute
{
    public string Message { get; init; } = string.Empty;

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
}
