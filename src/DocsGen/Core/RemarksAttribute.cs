using System.Text;

namespace DocsGen.Core;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class RemarksAttribute : DocsGenAttribute
{
    public string[] DocTexts { get; init; } = new string[0];
    public RemarksAttribute() { }
    public RemarksAttribute(string message, params string[] parms) : base(message)
    {
        this.DocTexts = parms ?? new string[0];
    }

    public override string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                builder.AppendLine("<remarks>");
                builder.AppendLine(base.ToString());
                foreach (var para in DocTexts)
                {
                    if (!string.IsNullOrEmpty(para))
                    {
                        builder.AppendLine("<para>");
                        builder.AppendLine(para);
                        builder.AppendLine("</para>");
                    }
                }
                builder.AppendLine("</remarks>");
                break;
            case DocType.Md:
                builder.AppendLine("##### Remarks");
                builder.AppendLine(base.ToString());
                builder.AppendLine(Environment.NewLine);
                foreach (var para in DocTexts)
                {
                    if (!string.IsNullOrEmpty(para))
                    {
                        builder.AppendLine(para);
                        builder.AppendLine(Environment.NewLine);
                    }
                }
                break;
            case DocType.Yml:
                builder.AppendLine($"remarks: {base.ToString()}");
                foreach (var para in DocTexts)
                {
                    if (!string.IsNullOrEmpty(para))
                    {
                        builder.AppendLine($"- {para}");
                    }
                }
                break;
            case DocType.Html:
                builder.AppendLine("<summary>");
                builder.AppendLine("<h5>Remarks</h5>");
                builder.AppendLine(base.ToString());
                foreach (var para in DocTexts)
                {
                    if (!string.IsNullOrEmpty(para))
                    {
                        builder.AppendLine("<p>");
                        builder.AppendLine(para);
                        builder.AppendLine("</p>");
                    }
                }
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
        foreach (var para in DocTexts)
        {
            if (!string.IsNullOrEmpty(para))
            {
                builder.AppendLine(para);
            }
        }
        return builder.ToString();
    }
}
