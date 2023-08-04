# docs-gen
C# type to document generator (xml,md,yaml,html)

![image](https://github.com/VikashChauhan51/docs-gen/assets/14816038/6f69350e-c4e7-4120-b467-ca356e9780b0)



## Quick Start Example:

```C#
Type type = typeof(List<>);
var text = new DocGeneratorBuilder().Build(DocType.Html).Generate(type);
File.WriteAllText($"{type.Name.Split('`')[0]}.html", text);
```
