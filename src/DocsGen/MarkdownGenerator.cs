
using DocsGen.Core;
using Reflector;
using System.Reflection;
using System.Text;

namespace DocsGen;
public sealed class MarkdownGenerator : DocGenerator
{
    public override DocType DocumentType => DocType.Md;


    public override Task<string> GenerateAsync(Type type) => Task.FromResult(Generate(type));
    public override string Generate(Type type)
    {
        if (!Valid(type)) return type?.ToString() ?? string.Empty;

        var sb = new StringBuilder();
        // Class name
        GetBasicInfo(type, sb);
        //Inherited types
        GetInheritedInfo(type, sb);
        // Constructors
        GetConstructorsInfo(type, sb);
        // Public fields
        GetFieldsInfo(type, sb);
        // Public events
        GetEventsInfo(type, sb);
        // Public properties
        GetPropertiesInfo(type, sb);
        // Public methods
        GetMethodsInfo(type, sb);

        return sb.ToString();
    }

    private void GetConstructorsInfo(Type type, StringBuilder sb)
    {
        var constructors = type.GetAllConstructors().Where(c => c.IsPublic || c.IsFamily).ToList();
        if (constructors!=null && constructors.Any())
        {
            sb.AppendLine("## Constructors");
            sb.AppendLine();
            foreach (var constructor in constructors)
            {
                foreach (var doc in GetDocsGenAttributes(constructor))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
                }

                var parameters = constructor.GetParameters().Select(p => GetGenericParemeterTypeName(p)).ToList();
                sb.AppendLine($"### {constructor.DeclaringType.Name.Split('`')[0]}({string.Join(", ", parameters)})");
                
            }

            sb.AppendLine();
        }
    }
    private void GetMethodsInfo(Type type, StringBuilder sb)
    {
        var methods = type.GetPublicAndProtectedInstanceAndStaticMethods();
        if (methods is not null && methods.Any())
        {
            sb.AppendLine("## Methods");
            sb.AppendLine();
            foreach (var method in methods)
            {
                foreach (var doc in GetDocsGenAttributes(method))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
                }
                var parameters = method.GetParameters().Select(p => GetGenericParemeterTypeName(p)).ToList();
                string returnTypeText;
                if (method.ReturnType.IsGenericType)
                {
                    var returnParms = method.ReturnType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    var returnParmText = string.Join(",", returnParms);
                    returnTypeText = $"{method.ReturnType.Name.Split('`')[0]}<{returnParmText}>";
                }
                else if (method.ReturnType.IsArray || method.ReturnType.IsSZArray)
                {
                    var returnParms = method.ReturnType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    var returnParmText = string.Join(",", returnParms);
                    if (!string.IsNullOrEmpty(returnParmText))
                    {
                        returnTypeText = $"{method.ReturnType.Name.Split('`')[0]}<{returnParmText}>[]";
                    }
                    else
                    {
                        returnTypeText = $"{method.ReturnType.Name.Split('`')[0]}";
                    }

                }
                else
                {
                    returnTypeText = method.ReturnType.Name;
                }

                if (method.IsGenericMethod)
                {
                    var pars = method.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList(); ;
                    var parmText = string.Join(",", pars);
                    sb.AppendLine($"### {method.Name.Split('`')[0]}`<{parmText}>`({string.Join(", ", parameters)}): `{returnTypeText}`");
                }
                else
                {
                    sb.AppendLine($"### {method.Name}({string.Join(", ", parameters)}): `{returnTypeText}`");
                }

                
            }
            sb.AppendLine();
        }
    }
    private void GetPropertiesInfo(Type type, StringBuilder sb)
    {
        var properties = type.GetPublicAndProtectedInstanceAndStaticProperties();
        if (properties is not null && properties.Length > 0)
        {
            sb.AppendLine("## Properties");
            sb.AppendLine();
            foreach (var property in properties)
            {

                foreach (var doc in GetDocsGenAttributes(property))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
                }

                if (property.PropertyType.IsGenericType)
                {
                    var name = property.PropertyType.Name.Split('`')[0];
                    var parameters = property.PropertyType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($"### {property.Name}: `{name}<{string.Join(", ", parameters)}>`");
                }
                else
                {
                    sb.AppendLine($"### {property.Name} : `{property.PropertyType.Name}`");
                }

            }
            sb.AppendLine();
        }
    }
    private void GetFieldsInfo(Type type, StringBuilder sb)
    {
        var fields = type.GetPublicAndProtectedInstanceAndStaticFields();
        if (fields is not null && fields.Length > 0)
        {
            sb.AppendLine("## Fields");
            sb.AppendLine();
            foreach (var field in fields)
            {
                foreach (var doc in GetDocsGenAttributes(field))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
                }

                if (field.FieldType.IsGenericType)
                {
                    var name = field.FieldType.Name.Split('`')[0];
                    var parameters = field.FieldType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($"### {field.Name.Split('`')[0]}: `{name}<{string.Join(", ", parameters)}>`");
                }
                else
                {
                    sb.AppendLine($"### {field.Name}: `{field.FieldType.Name}`");
                }

            }
            sb.AppendLine();
        }
    }
    private void GetEventsInfo(Type type, StringBuilder sb)
    {
        var events = type.GetPublicAndProtectedInstanceAndStaticEvents();
        if (events is not null && events.Length > 0)
        {
            sb.AppendLine("## Events");
            sb.AppendLine();
            foreach (var @event in events)
            {
                foreach (var doc in GetDocsGenAttributes(@event))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
                }

                if (@event.EventHandlerType.IsGenericType)
                {
                    var name = @event.EventHandlerType.Name.Split('`')[0];
                    var parameters = @event.EventHandlerType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($"### {@event.Name}: `{name}<{string.Join(", ", parameters)}>`");
                }
                else
                {
                    sb.AppendLine($"### {@event.Name}: `{@event.EventHandlerType}`");
                }
                
            }
            sb.AppendLine();
        }
    }
    private void GetInheritedInfo(Type type, StringBuilder sb)
    {
        var inheritedTypes = type.GetParentTypes();
        if (inheritedTypes != null && inheritedTypes.Any())
        {
            sb.AppendLine("## Inherited");
            sb.AppendLine();
            foreach (var parent in inheritedTypes)
            {
                GetTypeInfo(parent, sb, "-");
            }

        }
    }
    private void GetBasicInfo(Type type, StringBuilder sb)
    {
        GetTypeInfo(type, sb, "#");
        sb.AppendLine();
        sb.AppendLine($"**Namespace:** `{type.Namespace ?? type.Assembly?.GetName().Name}`");

        foreach (var doc in GetDocsGenAttributes(type))
        {
            sb.AppendLine(doc.ToString(DocumentType));
            sb.AppendLine();
        }
    }
    private void GetTypeInfo(Type type, StringBuilder sb, string seprator)
    {
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            sb.AppendLine($"{seprator} `{name}<{parmText}>`");
        }
        else
        {
            sb.AppendLine($"{seprator} `{type.Name}`");
        }
    }
    private string GetGenericTypeName(Type type)
    {
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints().Select(x => x.Name).ToArray();
            var constraintsText = string.Join(",", constraints);

            if (string.IsNullOrEmpty(constraintsText))
            {
                var attributes = type.GenericParameterAttributes;
                if ((attributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                {
                    return $"{type.Name}:{Reference_Type}";
                }
                return type.Name;
            }
            else
            {
                return $"{type.Name}:{constraintsText}";
            }
        }

        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            return $"{name}<{parmText}>";
        }

        if (type.IsArray)
        {
            var returnParms = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"[{type.Name.Split('`')[0]}<{returnParmText}>]";
            }
            else
            {
                return $"{type.Name.Split('`')[0]}";
            }
        }
        return type.Name;
    }
    private string GetGenericParemeterTypeName(ParameterInfo type)
    {
        var pName = type.Name.Split('`')[0];

        if (type.ParameterType.IsGenericType)
        {
            var name = type.ParameterType.Name.Split('`')[0];
            var parameters = type.ParameterType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            return $"`{name}<{parmText}>` {pName}";
        }

        if (type.ParameterType.IsArray)
        {
            var returnParms = type.ParameterType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"`{type.ParameterType.Name.Split('`')[0]}<{returnParmText}>[]` {pName}";
            }
        }
        return $"`{type.ParameterType.Name.Split('`')[0]}` {pName}"; ;
    }
}
