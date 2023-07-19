// See https://aka.ms/new-console-template for more information
using DocsGen;
using DocsGen.App;
using DocsGen.Core;

Console.WriteLine("Hello, World!");
Type type = typeof(DocType);
var markdown = new HtmlGenerator().Generate(type);
File.WriteAllText($"{type.Name.Split('`')[0]}.html", markdown);

//
