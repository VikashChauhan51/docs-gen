using System.Text;

namespace DocsGen.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
public class ExceptionAttribute : DocsGenAttribute
{
    public string MemberType { get; init; } = string.Empty;
    public ExceptionAttribute(string message, string type) : base(message)
    {
        this.MemberType = type;
    }


    public override string ToString(DocType doc)
    {
        var builder = new StringBuilder();
        switch (doc)
        {
            case DocType.Xml:
                builder.AppendLine($"<exception {(!string.IsNullOrEmpty(this.MemberType) ? $"cref = \"{this.MemberType}\"" : "")}>{base.ToString()}</exception>");
                break;
            case DocType.Md:
                builder.AppendLine($"> {(!string.IsNullOrEmpty(this.MemberType) ? $"`{this.MemberType}`" : "")}: {base.ToString()}");
                break;
            case DocType.Yml:
                builder.AppendLine($"exception: {base.ToString()}");
                if (string.IsNullOrEmpty(this.MemberType))
                    builder.AppendLine($"- type: {this.MemberType}");
                break;
            default:
                return this.ToString();
        }

        return builder.ToString();

    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        if (string.IsNullOrEmpty(this.MemberType)) builder.AppendLine($"{MemberType}:");
        builder.AppendLine(Message);
        return builder.ToString();
    }
}
