// See https://aka.ms/new-console-template for more information
using DocsGen;
using DocsGen.App;
using DocsGen.Core;

Type type = typeof(List<>);
var text = new DocGeneratorBuilder().Build(DocType.Html).Generate(type);
File.WriteAllText($"{type.Name.Split('`')[0]}.html", text);


