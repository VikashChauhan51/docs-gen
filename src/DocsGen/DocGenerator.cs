
using System.Reflection;
using DocsGen.Core;
using Reflector;

namespace DocsGen;
public abstract class DocGenerator
{
    protected const string Reference_Type = "class";
    protected const string ValueType_Type = "struct";
    protected const string Default_Constructor = "new()";
    public abstract DocType DocumentType { get; }
    public abstract Task<string> GenerateAsync(Type type);
    public abstract string Generate(Type type);
    protected bool Valid(Type type)
    {
        if (type is null || type.IsPrimitive()) return false;

        return type.IsClass || type.IsEnum || type.IsInterface || type.IsValueType;
    }

    protected IEnumerable<DocsGenAttribute> GetDocsGenAttributes(Type type)
    {
        return type?.GetCustomAttributes<DocsGenAttribute>() ?? new List<DocsGenAttribute>();
    }

    protected IEnumerable<DocsGenAttribute> GetDocsGenAttributes(FieldInfo field)
    {
        return field?.GetCustomAttributes<DocsGenAttribute>() ?? new List<DocsGenAttribute>();
    }

    protected IEnumerable<DocsGenAttribute> GetDocsGenAttributes(PropertyInfo property)
    {
        return property?.GetCustomAttributes<DocsGenAttribute>() ?? new List<DocsGenAttribute>();
    }

    protected IEnumerable<DocsGenAttribute> GetDocsGenAttributes(MethodInfo method)
    {
        return method?.GetCustomAttributes<DocsGenAttribute>() ?? new List<DocsGenAttribute>();
    }

    protected IEnumerable<DocsGenAttribute> GetDocsGenAttributes(EventInfo @event)
    {
        return @event?.GetCustomAttributes<DocsGenAttribute>() ?? new List<DocsGenAttribute>();
    }

    protected IEnumerable<DocsGenAttribute> GetDocsGenAttributes(ConstructorInfo constructor)
    {
        return constructor?.GetCustomAttributes<DocsGenAttribute>() ?? new List<DocsGenAttribute>();
    }


    protected string GetTypeAsString(Type type)
    {
        if (!Valid(type))
        {
            return string.Empty;
        }
        var accessSpecifier = type.GetAccessModifier();
        var typeModifier = type.GetTypeModifiers();
        var typename = GetTypeName(type);
        var inhereted = new List<string>();
        if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType) && type.BaseType != typeof(Enum))
        {
            inhereted.Add(GetTypeName(type.BaseType!));
        }

        foreach (var inter in type.GetDirectImplementedInterfaces())
        {
            inhereted.Add(GetTypeName(inter));
        }
        var inheritedText = string.Join(",", inhereted);
        var constraints = GetTypeConstraints(type);
        var constraintsText = string.Join(" ", constraints);
        if (DocumentType == DocType.Html)
        {
            return $@"&nbsp;<br><span class=""hljs-keyword"">{accessSpecifier}</span> <span class=""hljs-keyword"">{typeModifier}</span><span class=""hljs-type"">{typename}</span>{(inheritedText.Length > 0 ? $":{inheritedText}" : "")} {(constraints.Any() ? constraintsText : "")}<br>&nbsp;";

        }
        return $@"{accessSpecifier} {typeModifier}{typename}{(inheritedText.Length > 0 ? $":{inheritedText}" : "")} {(constraints.Any() ? constraintsText : "")}";

    }
    protected string GetTypeName(Type type)
    {
        var (startTag, endTag) = GetWrapperTag(type.IsPrimitive(), type.IsInterface);
        var (start, end) = GetSeperators();
        var (defaultStartTag, defaultEndTag) = GetDefaultWrapperTag();
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            return $"{startTag}{name}{endTag}{defaultStartTag}{start}{parmText}{end}{defaultEndTag}";
        }
        return $"{startTag}{type.Name}{endTag}";
    }

    private string GetGenericTypeName(Type type)
    {
        var (startTag, endTag) = GetWrapperTag(type.IsPrimitive(), type.IsInterface);
        var (start, end) = GetSeperators();

        if (type.IsGenericType)
        {

            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            return $"{startTag}{name}{endTag}{start}{parmText}{end}";
        }

        if (type.IsArray)
        {
            var returnParms = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"[{startTag}{type.Name.Split('`')[0]}{endTag}{start}{returnParmText}{end}]";
            }
            else
            {
                return $"{startTag}{type.Name.Split('`')[0]}{endTag}";
            }
        }
        return $"{startTag}{type.Name}{endTag}";
    }


    protected HashSet<string> GetTypeConstraints(Type type)
    {
        var (defaultStartTag, defaultEndTag) = GetWrapperTag(true, false);
        var parmData = new HashSet<string>();
        if (type.IsGenericType)
        {
            var parmeters = type.GetGenericArguments();
            foreach (var prm in parmeters)
            {
                var (parmStartTag, parmEndTag) = GetWrapperTag(prm.IsPrimitive(), prm.IsInterface);
                if (prm.IsGenericParameter)
                {
                    var constrains = prm.GetGenericParameterConstraints().Where(x => x != typeof(ValueType)).Select(x =>
                    {
                        if (x.IsInterface || x.IsEnum)
                        {
                            return $"{parmStartTag}{x.Name}{parmEndTag}";
                        }
                        var (parmTypeStartTag, parmTypeEndTag) = GetWrapperTag(type.IsPrimitive(), false);
                        return $"{parmTypeStartTag}{x.Name}{parmTypeEndTag}";
                    }
                   ).ToArray();

                    var constraintsText = string.Join(",", constrains);
                    if (constraintsText.Length > 0)
                    {
                        //custom type as constraints
                        parmData.Add($"{defaultStartTag}where{defaultEndTag} {parmStartTag}{prm.Name}{parmEndTag}:{constraintsText}");
                    }
                    else
                    {
                        var attributes = prm.GenericParameterAttributes;
                        // default classs constraints
                        if ((attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint)
                        {
                            if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                            {
                                parmData.Add($"{defaultStartTag}where{defaultEndTag} {parmStartTag}{prm.Name}{parmEndTag}:{defaultStartTag}class{defaultEndTag},{defaultStartTag}new{defaultEndTag}()");
                            }
                            else
                            {
                                parmData.Add($"{defaultStartTag}where{defaultEndTag} {parmStartTag}{prm.Name}{parmEndTag}:{defaultStartTag}class{defaultEndTag}");
                            }
                        }
                        else if ((attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
                        {
                            parmData.Add($"{defaultStartTag}where{defaultEndTag} {parmStartTag}{prm.Name}{parmEndTag}:{defaultStartTag}struct{defaultEndTag}");
                        }
                        else if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                        {
                            parmData.Add($"{defaultStartTag}where{defaultEndTag} {parmStartTag}{prm.Name}{parmEndTag}:{defaultStartTag}new{defaultEndTag}()");
                        }

                    }
                }
            }
        }

        return parmData;
    }
    protected (string start, string end) GetSeperators()
    {
        switch (DocumentType)
        {
            case DocType.Xml:
                return new("<", ">");
            case DocType.Md:
                return new("<", ">");
            case DocType.Yml:
                return new("<", ">");
            case DocType.Html:
                return new("&lt;", "&gt;");
            default:
                return new("<", ">");
        }
    }
    protected (string start, string end) GetWrapperTag(bool isPrimitive, bool isInterface)
    {
        switch (DocumentType)
        {
            case DocType.Xml:
                return new("", "");
            case DocType.Md:
                return new("`", "`");
            case DocType.Yml:
                return new("", "");
            case DocType.Html:
                return new($@"<span class=""{(isPrimitive ? "hljs-keyword" : isInterface ? "hljs-interface" : "hljs-type")}"">", "</span>");
            default:
                return new("", "");
        }
    }

    protected (string start, string end) GetDefaultWrapperTag()
    {
        switch (DocumentType)
        {
            case DocType.Xml:
                return new("", "");
            case DocType.Md:
                return new("`", "`");
            case DocType.Yml:
                return new("", "");
            case DocType.Html:
                return new($@"<span class=""hljs-default"">", "</span>");
            default:
                return new("", "");
        }
    }
}
