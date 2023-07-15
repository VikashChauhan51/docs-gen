
using DocsGen.Core;
using Reflector;
using System.Reflection;
using System.Text;

namespace DocsGen;
public class MarkdownGenerator : DocGenerator
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
        // Public events
        GetEventsInfo(type, sb);
        // Public fields
        GetFieldsInfo(type, sb);
        // Public properties
        GetPropertiesInfo(type, sb);
        // Public methods
        GetMethodsInfo(type, sb);

        return sb.ToString();
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
                if (method.IsGenericMethod)
                {
                    var pars = method.GetGenericArguments().Select(p =>
                    {
                        var pName = p.Name.Split('`')[0];
                        if (p.IsGenericParameter)
                        {
                            var constraints = p.GetGenericParameterConstraints().Select(x => x.Name).ToArray();
                            var constraintsText = string.Join(",", constraints);
                            return $"{pName}:{(string.IsNullOrEmpty(constraintsText) ? Reference_Type : constraintsText)}";

                        }
                        return pName;
                    }
                    ).ToList();
                    var parmText = string.Join(",", pars);
                    var parameters = method.GetParameters().Select(p =>
                    {
                        if (p.ParameterType.IsGenericType)
                        {
                            var pars = p.ParameterType.GetGenericArguments().Select(p => p.Name.Split('`')[0]).ToList();
                            var parmText = string.Join(",", pars);
                            return $"`{p.ParameterType.Name.Split('`')[0]}<{parmText}>` {p.Name}";
                        }
                        return $"`{p.ParameterType.Name}` {p.Name}";
                    });

                    if (method.ReturnType.IsGenericType)
                    {
                        var returnParms = method.ReturnType.GetGenericArguments().Select(p => p.Name.Split('`')[0]).ToList();
                        var returnParmText = string.Join(",", returnParms);
                        sb.AppendLine($"### {method.Name.Split('`')[0]}`<{parmText}>`({string.Join(", ", parameters)}): `{method.ReturnType.Name.Split('`')[0]}<{returnParmText}>`");
                    }
                    else
                    {
                        sb.AppendLine($"### {method.Name.Split('`')[0]}`<{parmText}>`({string.Join(", ", parameters)}): `{method.ReturnType.Name}`");
                    }

                }
                else
                {
                    var parameters = method.GetParameters().Select(p =>
                    {
                        if (p.ParameterType.IsGenericType)
                        {
                            var pars = p.ParameterType.GetGenericArguments().Select(p => p.Name.Split('`')[0]).ToList();
                            var parmText = string.Join(",", pars);
                            return $"`{p.ParameterType.Name.Split('`')[0]}<{parmText}>` {p.Name}";
                        }
                        return $"`{p.ParameterType.Name}` {p.Name}";
                    });
                    sb.AppendLine($"### {method.Name}({string.Join(", ", parameters)}): `{method.ReturnType.Name.Split('`')[0]}`");
                }


                foreach (var doc in GetDocsGenAttributes(method))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
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
                if (property.PropertyType.IsGenericType)
                {
                    var name = property.PropertyType.Name.Split('`')[0];
                    var parameters = property.PropertyType.GetGenericArguments().Select(p => p.Name.Split('`')[0]);
                    sb.AppendLine($"### {property.Name}: `{name}<{string.Join(", ", parameters)}>`");
                }
                else
                {
                    sb.AppendLine($"### {property.Name} : `{property.PropertyType.Name}`");
                }

                foreach (var doc in GetDocsGenAttributes(property))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
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
                if (field.FieldType.IsGenericType)
                {
                    var name = field.FieldType.Name.Split('`')[0];
                    if (field.FieldType.IsGenericType)
                    {
                        var parameters = field.FieldType.IsGenericParameter ? field.FieldType.GetGenericParameterConstraints().Select(p =>
                        {
                            var pName = p.Name.Split('`')[0];
                            if (p.IsGenericParameter)
                            {
                                var constraints = p.GetGenericParameterConstraints().Select(x => x.Name).ToArray();
                                var constraintsText = string.Join(",", constraints);
                                return $"{pName}:{(string.IsNullOrEmpty(constraintsText) ? Reference_Type : constraintsText)}";
                            }
                            return pName;
                        }).ToList() : field.FieldType.GetGenericArguments().Select(x =>
                        {
                            if (x.IsGenericParameter)
                            {
                                var constraints = x.GetGenericParameterConstraints().Select(x => x.Name).ToArray();
                                var constraintsText = string.Join(",", constraints);
                                return $"{x.Name}:{(string.IsNullOrEmpty(constraintsText) ? Reference_Type : constraintsText)}";
                            }
                            return x.Name;
                        }).ToList();
                        sb.AppendLine($"### {field.Name.Split('`')[0]}: `{name}<{string.Join(", ", parameters)}>`");
                    }



                }
                else
                {
                    sb.AppendLine($"### {field.Name}: `{field.FieldType.Name}`");
                }

                foreach (var doc in GetDocsGenAttributes(field))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
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

                if (@event.EventHandlerType.IsGenericType)
                {
                    var name = @event.EventHandlerType.Name.Split('`')[0];
                    var parameters = @event.EventHandlerType.GetGenericArguments().Select(p => p.Name.Split('`')[0]);
                    sb.AppendLine($"### {@event.Name}: `{name}<{string.Join(", ", parameters)}>`");
                }
                else
                {
                    sb.AppendLine($"### {@event.Name}: `{@event.EventHandlerType}`");
                }



                foreach (var doc in GetDocsGenAttributes(@event))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                    sb.AppendLine();
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
        GetTypeInfo(type, sb,"#");
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

        return type.Name;
    }

}
