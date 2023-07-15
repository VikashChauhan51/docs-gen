// See https://aka.ms/new-console-template for more information
using DocsGen;

Console.WriteLine("Hello, World!");
Type type = typeof(ValueTuple);
var markdown = new MarkdownGenerator().Generate(type);
File.WriteAllText($"{type}.md", markdown);
