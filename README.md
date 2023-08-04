# docs-gen
C# type to document generator (xml,md,yaml,html)

![image](https://github.com/VikashChauhan51/docs-gen/assets/14816038/1e335309-188b-4f10-9fdf-1633bf7bf6b2)
![image](https://github.com/VikashChauhan51/docs-gen/assets/14816038/fabb8b5e-9242-49b2-b02b-3be3a09c6ddd)


## Quick Start Example:

```C#
Type type = typeof(List<>);
var text = new DocGeneratorBuilder().Build(DocType.Html).Generate(type);
File.WriteAllText($"{type.Name.Split('`')[0]}.html", text);
```
