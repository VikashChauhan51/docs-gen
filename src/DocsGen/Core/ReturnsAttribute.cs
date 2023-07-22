
using System.Text;

namespace DocsGen.Core;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Delegate | AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
public sealed class ReturnsAttribute : DocsGenAttribute
{
    public ReturnsAttribute() { }
    public ReturnsAttribute(string message) : base(message) { }

    public override string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                builder.AppendLine("<returns>");
                builder.AppendLine(this.ToString());
                builder.AppendLine("</returns>");
                break;
            case DocType.Md:
                builder.AppendLine("##### Returns");
                builder.AppendLine(this.ToString());
                builder.AppendLine(Environment.NewLine);
                break;
            case DocType.Yml:
                builder.AppendLine($"{IndentationSpace}returns: \"{GetMessageAllTrim(this.Message)}\"");
                break;
            case DocType.Html:
                builder.AppendLine("<summary>");
                builder.AppendLine("<h5>Returns</h5>");
                builder.AppendLine(this.ToString());
                builder.AppendLine("</summary>");
                break;
            default:
                return ToString();
        }

        return builder.ToString();

    }
}
