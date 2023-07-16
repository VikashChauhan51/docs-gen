using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DocsGen.Core;
using Reflector;

namespace DocsGen;

public sealed class HtmlGenerator : DocGenerator
{
    public override DocType DocumentType => DocType.Html;

    private const string style = @"<style>
strong {
color: #c7254e
}
.container {
    padding-right: 15px;
    padding-left: 15px;
    margin-right: auto;
    margin-left: auto;
}

body {
    font-family: ""Helvetica Neue"",Helvetica,Arial,sans-serif;
    font-size: 14px;
    line-height: 1.42857143;
    color: #333;
    background-color: #fff;
padding: 10px;
}
.card {
  box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2);
padding: 15px;
}

.card-body {
  font-size: 18px;
}

code {
  font-family: monospace;
  
  padding: 2px 4px;
  background-color: #f1f1f1;
  color:#c7254e;
  border-radius: 4px;
}

hr {

  height: 1px;
  background-color: #ddd;
  
  margin: 1rem 0;

  border: none;
}

.card-title {
  font-size: 1.25rem;
  margin-bottom: 0.75rem;
}

ul {
    display: block;
    list-style-type: disc;
    margin-block-start: 1em;
    margin-block-end: 1em;
    margin-inline-start: 0px;
    margin-inline-end: 0px;
    padding-inline-start: 40px;
}

.card-title span {
  font-weight: bold;
  color: #333;
}

label {
    color: #337ab7;
  }

</style>"; 

    public override Task<string> GenerateAsync(Type type) => Task.FromResult(Generate(type));
    public override string Generate(Type type)
    {
        if (!Valid(type)) return type?.ToString() ?? string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine($"<title>{type.Name.Split('`')[0]}</title>");
        sb.AppendLine(style);
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
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
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }

    private void GetConstructorsInfo(Type type, StringBuilder sb)
    {
        var constructors = type.GetAllConstructors().Where(c => c.IsPublic || c.IsFamily).ToList();
        if (constructors != null && constructors.Any())
        {
            sb.AppendLine("<div>");
            sb.AppendLine("<h2>Constructors</h2>");
            sb.AppendLine($@"<hr/>");
            foreach (var constructor in constructors)
            {
                sb.AppendLine(@"<div class=""card"">");
                var parameters = constructor.GetParameters().Select(p => GetGenericParemeterTypeName(p)).ToList();
                sb.AppendLine($@"<h3><p><span>{constructor.DeclaringType.Name.Split('`')[0]}({string.Join(", ", parameters)})</p></h3>");
                sb.AppendLine(@"<div class=""card-body"">");
                foreach (var doc in GetDocsGenAttributes(constructor))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");
        }
    }
    private void GetMethodsInfo(Type type, StringBuilder sb)
    {
        var methods = type.GetPublicAndProtectedInstanceAndStaticMethods();
        if (methods is not null && methods.Any())
        {
            sb.AppendLine("<div>");
            sb.AppendLine("<h2>Methods</h2>");
            sb.AppendLine($@"<hr/>");
            foreach (var method in methods)
            {
                sb.AppendLine(@"<div class=""card"">");
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

                if (method.IsGenericMethod)
                {
                    var pars = method.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList(); ;
                    var parmText = string.Join(",", pars);
                    sb.AppendLine($@"<h3 class=""card-title""><p><span>{method.Name.Split('`')[0]}&lt;{parmText}&gt;({string.Join(", ", parameters)}):<strong>{returnTypeText}</strong></span></p></h3>");
                }
                else
                {
                    sb.AppendLine($@"<h3 class=""card-title""><p><span>{method.Name}({string.Join(", ", parameters)}):<strong>{returnTypeText}</strong></span></p></h3>");
                }
                
                sb.AppendLine(@"<div class=""card-body"">");
                foreach (var doc in GetDocsGenAttributes(method))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div>");
        }
    }
    private void GetPropertiesInfo(Type type, StringBuilder sb)
    {
        var properties = type.GetPublicAndProtectedInstanceAndStaticProperties();
        if (properties is not null && properties.Length > 0)
        {
            sb.AppendLine("<div>");
            sb.AppendLine("<h2>Properties</h2>");
            sb.AppendLine($@"<hr/>");
            foreach (var property in properties)
            {
                sb.AppendLine(@"<div class=""card"">");
                if (property.PropertyType.IsGenericType)
                {
                    var name = property.PropertyType.Name.Split('`')[0];
                    var parameters = property.PropertyType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($@"<h3><p><span>{property.Name}:<strong>{name}&lt;{string.Join(", ", parameters)}&gt;</strong></span></p></h3>");
                }
                else
                {
                    sb.AppendLine($@"<h3><p><span>{property.Name}:<strong>{property.PropertyType.Name}</strong></span></p></h3>");
                }
                sb.AppendLine(@"<div class=""card-body"">");
                foreach (var doc in GetDocsGenAttributes(property))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div>");
        }
    }
    private void GetFieldsInfo(Type type, StringBuilder sb)
    {
        var fields = type.GetPublicAndProtectedInstanceAndStaticFields();
        if (fields is not null && fields.Length > 0)
        {
            sb.AppendLine("<div>");
            sb.AppendLine("<h2>Fields</h2>");
            sb.AppendLine($@"<hr/>");
            foreach (var field in fields)
            {
                sb.AppendLine(@"<div class=""card"">");
                if (field.FieldType.IsGenericType)
                {
                    var name = field.FieldType.Name.Split('`')[0];
                    var parameters = field.FieldType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($@"<h3><p><span>{field.Name.Split('`')[0]}:<strong>{name}&lt;{string.Join(", ", parameters)}&gt;</strong></span></p></h3>");
                }
                else
                {
                    sb.AppendLine($@"<h3><p><span>{field.Name}:<strong>{field.FieldType.Name}</strong></span></p></h3>");
                }
                sb.AppendLine(@"<div class=""card-body"">");
                foreach (var doc in GetDocsGenAttributes(field))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div>");
        }
    }
    private void GetEventsInfo(Type type, StringBuilder sb)
    {
        var events = type.GetPublicAndProtectedInstanceAndStaticEvents();
        if (events is not null && events.Length > 0)
        {
            sb.AppendLine("<div>");
            sb.AppendLine("<h2>Events</h2>");
            sb.AppendLine($@"<hr/>");
            foreach (var @event in events)
            {
                sb.AppendLine(@"<div class=""card"">");
                if (@event.EventHandlerType.IsGenericType)
                {
                    var name = @event.EventHandlerType.Name.Split('`')[0];
                    var parameters = @event.EventHandlerType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
                    sb.AppendLine($@"<h3><p><span>{@event.Name}:<strong>{name}&lt;{string.Join(", ", parameters)}&gt;</strong></span></p></h3>");
                }
                else
                {
                    sb.AppendLine($@"<h3><p><span>{@event.Name}:<strong>{@event.EventHandlerType}/strong></span></p></h3>");
                }

                sb.AppendLine(@"<div class=""card-body"">");
                foreach (var doc in GetDocsGenAttributes(@event))
                {
                    sb.AppendLine(doc.ToString(DocumentType));
                }
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div>");
        }
    }
    private void GetInheritedInfo(Type type, StringBuilder sb)
    {
        var inheritedTypes = type.GetParentTypes();
        if (inheritedTypes != null && inheritedTypes.Any())
        {         
            sb.AppendLine("<h2>Inheritance</h2>");
            sb.AppendLine($@"<hr/>");
            sb.AppendLine(@"<div class=""card"">");
            sb.AppendLine(@"<div class=""card-body"">");
            sb.AppendLine("<ul>");
            foreach (var parent in inheritedTypes)
            {
                sb.AppendLine("<li>");
                GetTypeInfo(parent, sb);
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
        }
    }
    private void GetBasicInfo(Type type, StringBuilder sb)
    {
       
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            sb.AppendLine($@"<h1>{name}<strong>&lt;{parmText}&gt;</strong></h1>");
        }
        else
        {
            sb.AppendLine($@"<h1>{type.Name}</h1>");
        }
        sb.AppendLine($@"<hr/>");
        sb.AppendLine(@"<div class=""card"">");
        sb.AppendLine($@"<p><strong>Namespace: </strong><span>{type.Namespace ?? type.Assembly?.GetName().Name}</span></p>");
        sb.AppendLine(@"<div class=""card-body"">");
        foreach (var doc in GetDocsGenAttributes(type))
        {
            sb.AppendLine(doc.ToString(DocumentType));
        }
        sb.AppendLine("</div>");
        sb.AppendLine("</div>");
    }
    private void GetTypeInfo(Type type, StringBuilder sb)
    {
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            sb.AppendLine($@"<span><b>{name}</b><strong>&lt;{parmText}&gt;</strong></span>");
        }
        else
        {
            sb.AppendLine($@"<span><b>{type.Name}</b></span>");
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
                    return $"<span>{type.Name}:<strong>{Reference_Type}</strong></span>";
                }
                return type.Name;
            }
            else
            {
                return $"<span>{type.Name}:<strong>{constraintsText}</strong></span>";
            }
        }

        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            var parameters = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var parmText = string.Join(",", parameters);
            return $"<span>{name}<strong>&lt;{parmText}&gt;</strong></span>";
        }

        if (type.IsArray)
        {
            var returnParms = type.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"<span>[{type.Name.Split('`')[0]}<strong>&lt;{returnParmText}&gt;</strong>]</span>";
            }
            else
            {
                return $"<span>{type.Name.Split('`')[0]}</span>";
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
            return $"<span>{name}<strong>&lt;{parmText}&gt;</strong> <label>{pName}</label></span>";
        }

        if (type.ParameterType.IsArray)
        {
            var returnParms = type.ParameterType.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList();
            var returnParmText = string.Join(",", returnParms);
            if (!string.IsNullOrEmpty(returnParmText))
            {
                return $"<span>{type.ParameterType.Name.Split('`')[0]}<strong>&lt;{returnParmText}&gt;[]</strong><label> {pName}</label></span>";
            }
        }
        return $"<span><strong>{type.ParameterType.Name.Split('`')[0]}</strong> <label>{pName}</label></span>";
    }
}
