
using System.Text;

namespace DocsGen.Core;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
public sealed class ParameterAttribute : DocsGenAttribute
{
    public string ParameterType { get; init; } = string.Empty;
    public string ParameterName { get; init; } = string.Empty;
    public ParameterAttribute() { }
    public ParameterAttribute(string name, string typeName, string message) : base(message)
    {
        ParameterName = name;
        ParameterType = typeName;
    }

    public override string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                builder.AppendLine($"<param {(!string.IsNullOrEmpty(this.ParameterName) ? $"name = \"{this.ParameterName}\"" : "")} {(!string.IsNullOrEmpty(this.ParameterType) ? $"cref = \"{this.ParameterType}\"" : "")} >{Message}</param>");
                break;
            case DocType.Md:
                builder.AppendLine($"> **param:** `{ParameterType}` {ParameterName}: {Message}");
                break;
            case DocType.Yml:
                builder.AppendLine($"{IndentationSpace}param:");
                builder.AppendLine($"{IndentationSpace}  - name: \"{ParameterName}\"");
                builder.AppendLine($"{IndentationSpace}  - type: \"{ParameterType}\"");
                builder.AppendLine($"{IndentationSpace}  - message: \"{GetMessageAllTrim(this.Message)}\"");
                break;
            case DocType.Html:
                builder.AppendLine($"<p><strong>param: </strong><span>{this.ParameterName} <strong>({this.ParameterType})</strong> : </span><abbr>{Message}</abbr></p>");
                break;
            default:
                return this.ToString();
        }

        return builder.ToString();

    }

    public override string ToString()
    {
        return $"{ParameterType} {ParameterName}: {Message}";
    }
}

