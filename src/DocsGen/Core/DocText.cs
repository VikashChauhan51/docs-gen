
using System.Text;

namespace DocsGen.Core;

public sealed class DocText
{
    public string Text { get; init; } = string.Empty;
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Line { get; init; }
    public DocText(string text)
    {
        Text = text;
    }

    public DocText()
    {

    }
    public DocText(string text, bool bold = false, bool italic = false, bool underline = false, bool line = false)
    {
        Text = text;
        Bold = bold;
        Italic = italic;
        Underline = underline;
        Line = line;
    }
    public string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                if (Line)
                    builder.AppendLine(GetXml());
                else
                    builder.Append(GetXml());
                break;
            case DocType.Md:
                if (Line)
                    builder.AppendLine(GetMarkdown());
                else
                    builder.Append(GetMarkdown());
                break;
            case DocType.Yml:
                builder.AppendLine(this.ToString());
                break;
            case DocType.Html:
                if (Line)
                    builder.AppendLine(GetXml());
                else
                    builder.Append(GetXml());
                break;
            default:
                builder.AppendLine(this.ToString());
                break;
        }
        return builder.ToString();
    }
    public override string ToString()
    {
        return Text;
    }

    private string GetMarkdown()
    {
        string openTags = (Underline ? "<u>" : "") + (Bold ? "**" : "") + (Italic ? "*" : "");
        string closeTags = (Italic ? "*" : "") + (Bold ? "**" : "") + (Underline ? "</u>" : "");

        return $"{openTags}{this.ToString()}{closeTags}";
    }


    private string GetXml()
    {
        string openTags = (Underline ? "<u>" : "") + (Bold ? "<b>" : "") + (Italic ? "<i>" : "");
        string closeTags = (Italic ? "</i>" : "") + (Bold ? "</b>" : "") + (Underline ? "</u>" : "");

        return $"{openTags}{this.ToString()}{closeTags}";

    }
}
