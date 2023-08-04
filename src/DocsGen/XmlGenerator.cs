using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DocsGen.Core;
using Reflector;

namespace DocsGen;
internal sealed class XmlGenerator : DocGenerator
{
    public override DocType DocumentType => DocType.Xml;


    public override Task<string> GenerateAsync(Type type) => Task.FromResult(Generate(type));
    public override string Generate(Type type)
    {
        if (!Valid(type)) return type?.ToString() ?? string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("<doc>");
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
        sb.AppendLine("</doc>");
        return sb.ToString();
    }

    private void GetConstructorsInfo(Type type, StringBuilder sb)
    {
        var constructors = type.GetAllConstructors().Where(c => c.IsStatic || c.IsPublic || c.IsFamily).ToList();
        if (constructors != null && constructors.Any())
        {
            sb.AppendLine("<constructors>");
            foreach (var constructor in constructors)
            {

                var parameters = constructor.GetParameters().Select(p => GetGenericParemeterTypeName(p)).ToList();
                sb.AppendLine($@"<member name=""C:{constructor.DeclaringType.Name.Split('`')[0]}({string.Join(", ", parameters)})"">");
                sb.AppendLine($@"<declaration>{GetConstructorAsString(constructor)}</declaration>");
                foreach (var doc in GetDocsGenAttributes(constructor))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</member>");
            }
            sb.AppendLine("</constructors>");
        }
    }
    private void GetMethodsInfo(Type type, StringBuilder sb)
    {
        var methods = type.GetPublicAndProtectedInstanceAndStaticMethods();
        if (methods is not null && methods.Any())
        {
            sb.AppendLine("<methods>");
            foreach (var method in methods)
            {
                var parameters = method.GetParameters().Select(p => GetGenericParemeterTypeName(p)).ToList();
                string returnTypeText;
                if (method.ReturnType.IsGenericType)
                {
                    var returnParms = method.ReturnType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    var returnParmText = string.Join(",", returnParms);
                    returnTypeText = $"{method.ReturnType.Name.Split('`')[0]}&lt;{returnParmText}&gt;";
                }
                else if (method.ReturnType.IsArray || method.ReturnType.IsSZArray)
                {
                    var returnParms = method.ReturnType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    var returnParmText = string.Join(",", returnParms);
                    if (!string.IsNullOrEmpty(returnParmText))
                    {
                        returnTypeText = $"{method.ReturnType.Name.Split('`')[0]}&lt;{returnParmText}&gt;[]";
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

                var name = method.Name.Split('`')[0];
                if (name == "<Clone>$")
                {
                        name = "&lt;Clone&gt;$";
                }

                if (method.IsGenericMethod)
                {
                    var pars = method.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList(); ;
                    var parmText = string.Join(",", pars);
                    sb.AppendLine($@"<member name=""M:{name}&lt;{parmText}&gt;({string.Join(", ", parameters)}):{returnTypeText}"">");
                }
                else
                {
                    sb.AppendLine($@"<member name=""M:{name}({string.Join(", ", parameters)}):{returnTypeText}"">");
                }

                sb.AppendLine($@"<declaration>{GetMethodAsString(method)}</declaration>");
                foreach (var doc in GetDocsGenAttributes(method))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</member>");
            }
            sb.AppendLine("</methods>");
        }
    }
    private void GetPropertiesInfo(Type type, StringBuilder sb)
    {
        var properties = type.GetPublicAndProtectedInstanceAndStaticProperties();
        if (properties is not null && properties.Length > 0)
        {
            sb.AppendLine("<properties>");
            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType)
                {
                    var name = property.PropertyType.Name.Split('`')[0];
                    var parameters = property.PropertyType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($@"<member name=""P:{property.Name}:{name}&lt;{string.Join(", ", parameters)}&gt;"">");
                }
                else
                {
                    sb.AppendLine($@"<member name=""P:{property.Name}:{property.PropertyType.Name}"">");
                }
                sb.AppendLine($@"<declaration>{GetPropertyAsString(property)}</declaration>");
                foreach (var doc in GetDocsGenAttributes(property))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</member>");
            }
            sb.AppendLine("</properties>");
        }
    }
    private void GetFieldsInfo(Type type, StringBuilder sb)
    {
        var fields = type.GetPublicAndProtectedInstanceAndStaticFields();
        if (fields is not null && fields.Length > 0)
        {
            sb.AppendLine("<fields>");
            foreach (var field in fields)
            {
                if (field.FieldType.IsGenericType)
                {
                    var name = field.FieldType.Name.Split('`')[0];
                    var parameters = field.FieldType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($@"<member name=""F:{field.Name.Split('`')[0]}:{name}&lt;{string.Join(", ", parameters)}&gt;"">");
                }
                else
                {
                    sb.AppendLine($@"<member name=""F:{field.Name}:{field.FieldType.Name}"">");
                }
                sb.AppendLine($@"<declaration>{GetFieldAsString(field)}</declaration>");
                foreach (var doc in GetDocsGenAttributes(field))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</member>");
            }
            sb.AppendLine("</fields>");
        }
    }
    private void GetEventsInfo(Type type, StringBuilder sb)
    {
        var events = type.GetPublicAndProtectedInstanceAndStaticEvents();
        if (events is not null && events.Length > 0)
        {
            sb.AppendLine("<events>");
            foreach (var @event in events)
            {

                if (@event.EventHandlerType.IsGenericType)
                {
                    var name = @event.EventHandlerType.Name.Split('`')[0];
                    var parameters = @event.EventHandlerType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($@"<member name=""M:{@event.Name}:{name}&lt;{string.Join(", ", parameters)}&gt;"">");
                }
                else
                {
                    sb.AppendLine($@"<member name=""M:{@event.Name}:{@event.EventHandlerType}"">");
                }
                sb.AppendLine($@"<declaration>{GetEventAsString(@event)}</declaration>");
                foreach (var doc in GetDocsGenAttributes(@event))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</member>");
            }
            sb.AppendLine("</events>");
        }
    }
    private void GetInheritedInfo(Type type, StringBuilder sb)
    {
        var inheritedTypes = type.GetParentTypes();
        if (inheritedTypes != null && inheritedTypes.Any())
        {
            sb.AppendLine("<inheritance>");
            sb.AppendLine("<members>");
            foreach (var parent in inheritedTypes)
            {
                GetTypeInfo(parent, sb);
                sb.AppendLine("</member>");
            }
            sb.AppendLine("</members>");
            sb.AppendLine("</inheritance>");
        }
    }
    private void GetBasicInfo(Type type, StringBuilder sb)
    {
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            sb.AppendLine($@"<type-info name=""T:{name}&lt;{parmText}&gt;"">");
        }
        else
        {
            sb.AppendLine($@"<type-info name=""T:{type.Name}"">");
        }
        sb.AppendLine($@"<namespace name=""{type.Namespace ?? type.Assembly?.GetName().Name}""></namespace>");
        sb.AppendLine($@"<assembly name=""{type.Assembly?.GetName().Name}""></assembly>");
        sb.AppendLine($@"<syntax>{GetTypeAsString(type)}</syntax>");
        foreach (var doc in GetDocsGenAttributes(type))
        {
            sb.AppendLine(doc.ToString(DocumentType));
        }
        sb.AppendLine("</type-info>");
    }
    private void GetTypeInfo(Type type, StringBuilder sb)
    {
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            sb.AppendLine($@"<member name=""T:{name}&lt;{parmText}&gt;"">");
        }
        else
        {
            sb.AppendLine($@"<member name=""T:{type.Name}"">");
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
            return $"{name}&lt;{parmText}&gt;";
        }

        if (type.IsArray)
        {
            var returnParms = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"[{type.Name.Split('`')[0]}&lt;{returnParmText}&gt;]";
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
            return $"{name}&lt;{parmText}&gt; {pName}";
        }

        if (type.ParameterType.IsArray)
        {
            var returnParms = type.ParameterType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"{type.ParameterType.Name.Split('`')[0]}&lt;{returnParmText}&gt;[] {pName}";
            }
        }
        return $"{type.ParameterType.Name.Split('`')[0]} {pName}";
    }
}
