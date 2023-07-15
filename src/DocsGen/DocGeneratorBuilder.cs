
using DocsGen.Core;

namespace DocsGen;
public class DocGeneratorBuilder
{

    private readonly IDictionary<DocType, Func<DocGenerator>> docGenerators = new Dictionary<DocType, Func<DocGenerator>>();

    public DocGeneratorBuilder Register<T>(DocType docType) where T : DocGenerator, new()
    {
        docGenerators[docType] = () => new T();
        return this;
    }
    public DocGenerator Build(DocType docType)
    {
        if (docGenerators.TryGetValue(docType, out var generatorFactory))
        {
            return generatorFactory();
        }

        throw new ArgumentException("Invalid DocType");
    }
}
