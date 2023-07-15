using System.Text;

namespace DocsGen.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Interface |  AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class SummeryAttribute : DocsGenAttribute
{
    public SummeryAttribute() { }
    public SummeryAttribute(string message) : base(message) { }

    public override string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                builder.AppendLine("<summery>");
                builder.AppendLine(this.ToString());
                builder.AppendLine("</summery>");
                break;
            case DocType.Md:
                builder.AppendLine("##### Summery");
                builder.AppendLine(this.ToString());
                builder.AppendLine(Environment.NewLine);
                break;
            case DocType.Yml:
                builder.AppendLine($"summery: {this.ToString()}");
                break;
            default:
                return ToString();
        }

        return builder.ToString();

    }
}
