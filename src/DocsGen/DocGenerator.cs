
using System.Reflection;
using DocsGen.Core;

namespace DocsGen;
public abstract class DocGenerator
{
    protected const string Reference_Type = "ReferenceType";
    public abstract DocType DocumentType { get; }
    public abstract Task<string> GenerateAsync(Type type);
    public abstract string Generate(Type type);
    protected bool Valid(Type type)
    {
        if (type is null) return false;

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


}
