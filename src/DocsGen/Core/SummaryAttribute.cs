using System.Text;

namespace DocsGen.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class SummaryAttribute : DocsGenAttribute
{
    public SummaryAttribute() { }
    public SummaryAttribute(string message) : base(message) { }

    public override string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                builder.AppendLine("<summary>");
                builder.AppendLine(this.ToString());
                builder.AppendLine("</summary>");
                break;
            case DocType.Md:
                builder.AppendLine("##### summary");
                builder.AppendLine(this.ToString());
                builder.AppendLine(Environment.NewLine);
                break;
            case DocType.Yml:
                builder.AppendLine($"{IndentationSpace}summary: \"{GetMessageAllTrim(this.Message)}\"");
                break;
            case DocType.Html:
                builder.AppendLine("<summary>");
                builder.AppendLine("<h5>summary</h5>");
                builder.AppendLine(this.ToString());
                builder.AppendLine("</summary>");
                break;
            default:
                return ToString();
        }

        return builder.ToString();

    }
}
