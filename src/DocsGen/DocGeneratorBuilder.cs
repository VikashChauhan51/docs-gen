
using DocsGen.Core;

namespace DocsGen;
public class DocGeneratorBuilder
{

    private readonly IDictionary<DocType, Type> docGenerators = new Dictionary<DocType, Type>()
    {
        { DocType.Html,typeof(HtmlGenerator) },
        { DocType.Xml,typeof(XmlGenerator) },
        { DocType.Md,typeof(MarkdownGenerator) },
        { DocType.Yml,typeof(YamlGenerator) }
    };

    public DocGenerator Build(DocType docType)
    {
        if (docGenerators.TryGetValue(docType, out var generator))
        {
            return (DocGenerator)Activator.CreateInstance(generator)!;
        }

        throw new ArgumentException("Invalid DocType");
    }
}
