using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DocsGen.Core;
using Reflector;

namespace DocsGen;

internal sealed class HtmlGenerator : DocGenerator
{
    public override DocType DocumentType => DocType.Html;

    private const string style = @"<style>
strong {
color: #c7254e
}
.container {
    width: 100%;
    margin: 0 auto;
    max-width: 75.5em;
}

body {
    background: #fff;
    color: rgba(0,0,0,.8);
    padding: 0;
    margin: 0;
    font-family:serif;
    line-height: 1;
    position: relative;
    cursor: auto;
    -moz-tab-size: 4;
    -o-tab-size: 4;
    tab-size: 4;
    word-wrap: anywhere;
    -moz-osx-font-smoothing: grayscale;
    -webkit-font-smoothing: antialiased;
}
.card {
border: 1px solid #cccccc;
  border-radius: 5px;
margin-bottom: 10px;
padding: 15px;
}

.card-body {
  font-size: 18px;
}

.card h3 {
  font-size: 22px;
  margin-bottom: 10px;
 border-bottom: 1px solid #ccc;
}

.card p {
  font-size: 16px;
  margin-bottom: 20px;
}

.card a {
  text-decoration: none;
  color: #000000;
}

.card a:hover {
  color: #0000ff;
}


code
{
font-family:monospace;
font-weight:200;
color:rgba(0,0,0,.9);
border-radius: 4px;
background-color: #f1f1f1;
}
pre
{
white-space:pre-wrap;
color:rgba(0,0,0,.9);
font-family:monospace;
line-height:1.0;
text-rendering:optimizeSpeed;
 background-color: #f1f1f1;
padding: 10px;

}
pre code,pre pre{color:inherit;font-size:inherit;line-height:inherit}
pre>code{display:block}
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
    font-size: 1.0rem;
}

.card-title span {
  font-weight: bold;
  color: #333;
}

label {
    color: #337ab7;
  }


.pill {
  display: inline-block;
  background-color: #ffffff;
  border: 1px solid #cccccc;
  border-radius: 5px;
  padding: 10px 15px;
  text-align: center;
  cursor: pointer;
}

.pill:hover {
  background-color: #eeeeee;
}

.pill.active {
  background-color: #000000;
  color: #ffffff;
}

.capsule {
  display: inline-block;
  background-color: #ffffff;
  border: 1px solid #cccccc;
  border-radius: 30px;
  padding: 10px 15px;
  text-align: center;
  cursor: pointer;
}

.capsule:hover {
  background-color: #eeeeee;
}

.capsule.active {
  background-color: #000000;
  color: #ffffff;
}
.hljs {
  background-color: inherit;
  padding: 0;
}
.hljs-keyword {
  color: #0c84ef;
}

.hljs-string {
  color: #0c84ef;
}
.hljs-type {
  color: rgb(86,156,214);
}

.hljs-interface {
  color: #9f9f38;
}
.hljs-default {
    color: rgba(0,0,0,.9);
    font-size: inherit;
    line-height: inherit;
}
.declaration {
    color: #a2a2a2;
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
        sb.AppendLine(@"<link rel=""stylesheet"" href=""https://fonts.googleapis.com/css?family=Open+Sans:300,300italic,400,400italic,600,600italic%7CNoto+Serif:400,400italic,700,700italic%7CDroid+Sans+Mono:400,700""/>");
        sb.AppendLine(style);
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($@"<div class=""container"">");
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
        sb.AppendLine("<div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }

    private void GetConstructorsInfo(Type type, StringBuilder sb)
    {
        var constructors = type.GetAllConstructors().Where(c =>c.IsStatic || c.IsPublic || c.IsFamily).ToList();
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
                sb.AppendLine($@"<h5 class=""declaration"">Declaration</h5>");
                sb.AppendLine($@"<pre><code class=""hljs"">{GetConstructorAsString(constructor)}</code></pre>");
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

                var name = method.Name.Split('`')[0];
                if (name == "<Clone>$")
                {
                    name = "&lt;Clone&gt;$";
                }
                if (method.IsGenericMethod)
                {
                    var pars = method.GetGenericArguments().Select(p => GetGenericTypeName(p)).ToList(); ;
                    var parmText = string.Join(",", pars);
                    sb.AppendLine($@"<h3 class=""card-title""><p><span>{name}&lt;{parmText}&gt;({string.Join(", ", parameters)}):<strong>{returnTypeText}</strong></span></p></h3>");
                }
                else
                {
                    sb.AppendLine($@"<h3 class=""card-title""><p><span>{name}({string.Join(", ", parameters)}):<strong>{returnTypeText}</strong></span></p></h3>");
                }
                sb.AppendLine($@"<h5 class=""declaration"">Declaration</h5>");
                sb.AppendLine($@"<pre><code class=""hljs"">{GetMethodAsString(method)}</code></pre>");
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
                sb.AppendLine($@"<h5 class=""declaration"">Declaration</h5>");
                sb.AppendLine($@"<pre><code class=""hljs"">{GetPropertyAsString(property)}</code></pre>");
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

                sb.AppendLine($@"<h5 class=""declaration"">Declaration</h5>");
                sb.AppendLine($@"<pre><code class=""hljs"">{GetFieldAsString(field)}</code></pre>");
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
                sb.AppendLine($@"<h5 class=""declaration"">Declaration</h5>");
                sb.AppendLine($@"<pre><code class=""hljs"">{GetEventAsString(@event)}</code></pre>");
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
        sb.AppendLine($@"<p><strong>Assembly: </strong><span>{type.Assembly?.GetName().Name}.dll</span></p>");
        sb.AppendLine($@"<h4>Syntax</h4>");
        sb.AppendLine($@"<pre><code class=""hljs"">{GetTypeAsString(type)}</code></pre>");
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
            var constraints = type.GetGenericParameterConstraints().Where(x => x != typeof(ValueType)).Select(x => x.Name).ToArray();
            var constraintsText = string.Join(",", constraints);

            if (string.IsNullOrEmpty(constraintsText))
            {
                var attributes = type.GenericParameterAttributes;
                if ((attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint)
                {
                    if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                    {
                        return $"<span>{type.Name}:<strong>{Reference_Type}</strong>,<strong>{Default_Constructor}</strong></span>";
                    }
                    return $"<span>{type.Name}:<strong>{Reference_Type}</strong></span>";

                }

                if ((attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
                {
                    return $"<span>{type.Name}:<strong>{ValueType_Type}</strong></span>";
                }
                if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
                {
                    return $"<span>{type.Name}:<strong>{Default_Constructor}</strong></span>";
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
