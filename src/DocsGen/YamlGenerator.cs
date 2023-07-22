using DocsGen.Core;
using Reflector;
using System.Reflection;
using System.Text;

namespace DocsGen;
public class YamlGenerator : DocGenerator
{
    public override DocType DocumentType => DocType.Yml;


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
        var constructors = type.GetAllConstructors().Where(c => c.IsStatic || c.IsPublic || c.IsFamily).ToList();
        if (constructors != null && constructors.Any())
        {
            sb.AppendLine("constructors:");
            foreach (var constructor in constructors)
            {
                sb.AppendLine("  - constructor:");
                var parameters = constructor.GetParameters().Select(p => GetGenericParemeterTypeName(p)).ToList();
                sb.AppendLine($"    name: \"{constructor.DeclaringType.Name.Split('`')[0]}({string.Join(", ", parameters)})\"");
                sb.AppendLine($"    declaration: \"{GetConstructorAsString(constructor)}\"");
                foreach (var doc in GetDocsGenAttributes(constructor))
                {
                    doc.IndentationSpace = "    ";
                    sb.AppendLine(doc.ToString(DocumentType));
                }
            }
        }
    }
    private void GetMethodsInfo(Type type, StringBuilder sb)
    {
        var methods = type.GetPublicAndProtectedInstanceAndStaticMethods();
        if (methods is not null && methods.Any())
        {
            sb.AppendLine("methods:");
            foreach (var method in methods)
            {
                sb.AppendLine("  - method:");
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
                    sb.AppendLine($"    name: \"{method.Name.Split('`')[0]}<{parmText}>({string.Join(", ", parameters)}): {returnTypeText}\"");
                }
                else
                {
                    sb.AppendLine($"    name: \"{method.Name}({string.Join(", ", parameters)}): {returnTypeText}\"");
                }
                sb.AppendLine($"    declaration: \"{GetMethodAsString(method)}\"");
                foreach (var doc in GetDocsGenAttributes(method))
                {
                    doc.IndentationSpace = "    ";
                    sb.AppendLine(doc.ToString(DocumentType));
                }
            }
        }
    }
    private void GetPropertiesInfo(Type type, StringBuilder sb)
    {
        var properties = type.GetPublicAndProtectedInstanceAndStaticProperties();
        if (properties is not null && properties.Length > 0)
        {
            sb.AppendLine("properties:");
            foreach (var property in properties)
            {
                sb.AppendLine("  - property:");
                if (property.PropertyType.IsGenericType)
                {
                    var name = property.PropertyType.Name.Split('`')[0];
                    var parameters = property.PropertyType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($"    name: \"{property.Name}: {name}<{string.Join(", ", parameters)}>\"");
                }
                else
                {
                    sb.AppendLine($"    name: \"{property.Name} : {property.PropertyType.Name}\"");
                }
                sb.AppendLine($"    declaration: \"{GetPropertyAsString(property)}\"");

                foreach (var doc in GetDocsGenAttributes(property))
                {
                    doc.IndentationSpace = "    ";
                    sb.AppendLine(doc.ToString(DocumentType));
                }
            }
        }
    }
    private void GetFieldsInfo(Type type, StringBuilder sb)
    {
        var fields = type.GetPublicAndProtectedInstanceAndStaticFields();
        if (fields is not null && fields.Length > 0)
        {
            sb.AppendLine("fields:");
            foreach (var field in fields)
            {
                sb.AppendLine("  - field:");
                if (field.FieldType.IsGenericType)
                {
                    var name = field.FieldType.Name.Split('`')[0];
                    var parameters = field.FieldType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($"    name: \"{field.Name.Split('`')[0]}: {name}<{string.Join(", ", parameters)}>\"");
                }
                else
                {
                    sb.AppendLine($"    name: \"{field.Name}: {field.FieldType.Name}\"");
                }
                sb.AppendLine($@"    declaration: ""{GetFieldAsString(field)}""");
                foreach (var doc in GetDocsGenAttributes(field))
                {
                    doc.IndentationSpace = "    ";
                    sb.AppendLine(doc.ToString(DocumentType));
                }

            }
        }
    }
    private void GetEventsInfo(Type type, StringBuilder sb)
    {
        var events = type.GetPublicAndProtectedInstanceAndStaticEvents();
        if (events is not null && events.Length > 0)
        {
            sb.AppendLine("events:");
            foreach (var @event in events)
            {
                sb.AppendLine("  - event:");
                if (@event.EventHandlerType.IsGenericType)
                {
                    var name = @event.EventHandlerType.Name.Split('`')[0];
                    var parameters = @event.EventHandlerType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($"    name: \"{@event.Name}: {name}<{string.Join(", ", parameters)}>\"");
                }
                else
                {
                    sb.AppendLine($"    name: \"{@event.Name}: {@event.EventHandlerType}\"");
                }

                sb.AppendLine($@"    declaration: ""{GetEventAsString(@event)}""");
                foreach (var doc in GetDocsGenAttributes(@event))
                {
                    doc.IndentationSpace = "    ";
                    sb.AppendLine(doc.ToString(DocumentType));
                }
            }
        }
    }
    private void GetInheritedInfo(Type type, StringBuilder sb)
    {
        var inheritedTypes = type.GetParentTypes();
        if (inheritedTypes != null && inheritedTypes.Any())
        {
            sb.AppendLine("inherited:");
            foreach (var parent in inheritedTypes)
            {
                GetTypeInfo(parent, sb, "  - ");
            }

        }
    }
    private void GetBasicInfo(Type type, StringBuilder sb)
    {
        sb.AppendLine("---");
        sb.AppendLine("type-info:");
        GetTypeInfo(type, sb, "  name:");
        sb.AppendLine($"  namespace: \"{type.Namespace ?? type.Assembly?.GetName().Name}\"");
        sb.AppendLine($"  assembly: \"{type.Assembly?.GetName().Name}\"");
        sb.AppendLine($"  syntax: \"{GetTypeAsString(type)}\"");
        foreach (var doc in GetDocsGenAttributes(type))
        {
            doc.IndentationSpace = "  ";
            sb.AppendLine(doc.ToString(DocumentType));
        }
    }
    private void GetTypeInfo(Type type, StringBuilder sb, string seprator)
    {
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            sb.AppendLine($"{seprator} \"{name}<{parmText}>\"");
        }
        else
        {
            sb.AppendLine($"{seprator} \"{type.Name}\"");
        }
    }
    private string GetGenericTypeName(Type type)
    {
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints().Where(x => x != typeof(ValueType)).Select(x => x.Name).ToArray();
            var constraintsText = string.Join(",", constraints);

            if (string.IsNullOrEmpty(constraintsText))
            {
                var attributes = type.GenericParameterAttributes;

                if ((attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint)
                {
                    if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                    {
                        return $"{type.Name}:{Reference_Type},{Default_Constructor}";
                    }
                    return $"{type.Name}:{Reference_Type}";

                }

                if ((attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
                {
                    return $"{type.Name}:{ValueType_Type}";
                }
                if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                {
                    return $"{type.Name}:{Default_Constructor}";
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
            return $"{name}<{parmText}> {pName}";
        }

        if (type.ParameterType.IsArray)
        {
            var returnParms = type.ParameterType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"{type.ParameterType.Name.Split('`')[0]}<{returnParmText}>[] {pName}";
            }
        }
        return $"{type.ParameterType.Name.Split('`')[0]} {pName}";
    }
}

