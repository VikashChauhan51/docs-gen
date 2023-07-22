// See https://aka.ms/new-console-template for more information
using System.Reflection;
using DocsGen;
using DocsGen.App;
using DocsGen.Core;
using Reflector;

Console.WriteLine("Hello, World!");
Type type = typeof(MyrecordStruct);

var getMethods = type.GetPublicAndProtectedInstanceAndStaticMethods();
var markdown = new HtmlGenerator().Generate(type);
File.WriteAllText($"{type.Name.Split('`')[0]}.html", markdown);

 
