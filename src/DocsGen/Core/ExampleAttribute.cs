using System.Text;

namespace DocsGen.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class ExampleAttribute : DocsGenAttribute
{
    public string Code { get; init; } = string.Empty;
    public ExampleAttribute(string message, string code) : base(message)
    {
        this.Code = code;
    }


    public override string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                builder.AppendLine("<example>");
                builder.AppendLine(base.ToString());
                builder.AppendLine("<code>");
                builder.AppendLine(this.Code);
                builder.AppendLine("</code>");
                builder.AppendLine("</example>");
                break;
            case DocType.Md:
                builder.AppendLine("##### Example");
                builder.AppendLine(base.ToString());
                builder.AppendLine(Environment.NewLine);
                builder.AppendLine($"```C#{Environment.NewLine}{this.Code}{Environment.NewLine}```");
                break;
            case DocType.Yml:
                builder.AppendLine($"example: {base.ToString()}");
                builder.AppendLine($"- code: {this.Code}");
                break;
            case DocType.Html:
                builder.AppendLine("<summary>");
                builder.AppendLine("<h5>Example</h5>");
                builder.AppendLine(base.ToString());
                builder.AppendLine("<code><pre>");
                builder.AppendLine(this.Code);
                builder.AppendLine("</pre></code>");
                builder.AppendLine("</summary>");
                break;
            default:
                return this.ToString();
        }

        return builder.ToString();

    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine(Message);
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine(Code);
        return builder.ToString();
    }
}
