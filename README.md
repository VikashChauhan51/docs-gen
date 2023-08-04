# docs-gen
C# type to document generator (xml,md,yaml,html)

## Quick Start Example:

```C#
Type type = typeof(List<>);
var text = new DocGeneratorBuilder().Build(DocType.Html).Generate(type);
File.WriteAllText($"{type.Name.Split('`')[0]}.html", text);
```
