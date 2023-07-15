﻿// See https://aka.ms/new-console-template for more information
using DocsGen;
using DocsGen.App;

Console.WriteLine("Hello, World!");
Type type = typeof(DocDataClass);
var markdown = new MarkdownGenerator().Generate(type);
File.WriteAllText($"{type.Name.Split('`')[0]}.md", markdown);

//
