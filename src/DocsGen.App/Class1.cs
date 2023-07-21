using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocsGen.Core;
using static System.Net.Mime.MediaTypeNames;

namespace DocsGen.App;
class MyClassDemo
{

}
public class Class1
{

    public T[] GetArray<T>() where T : class, new()
    {
        return new T[0];
    }

    public T[] GetArray<T>(string[] data, List<string> info, BasicGeneric<Base, int> state, T[] kgb)
    {
        return kgb;
    }
}

[Summary("This is document class for demo.")]
[Remarks(Message = "Demo remarks", DocTexts = new string[] { "this is first paragraph", "this is second paragraph." })]
public class BasicGeneric<T, V>
{

}

public class BGeneric<T, V, X> where T : unmanaged where V : unmanaged where X : class, new()
{

}

[Summary("This is document class for demo.")]
[Remarks(Message = "Demo remarks", DocTexts = new string[] { "this is first paragraph", "this is second paragraph." })]
public class AdvanceGeneric<T, U> : Test<T, U> where T : class, new()
where U : struct
{

}
public class Base
{
    public Base()
    {

    }
}
public class Test<T, U>
    where U : struct
    where T : class, new()
{
    public event EventHandler<U> status;

    public MyClass<T, U> item;

    public U MyProperty { get; set; }
    public T GetData<V, G>(V flag, G status) where V : class
        where G : struct
    {
        return default(T);
    }
}
public class MyClass<T, X> where T : class
{
    public T Myfield;
    public T MyProperty { get; set; }
    public T MyPropertyGetOnly { get; }
    public T MyPropertySetOnly { set { Myfield = value; } }
    public T MyPropertyInitOnly { get; init; }

    public T MyPropertyPrivateSet { get; private set; }

    public T MyPropertyProtectedSet { get; protected set; }
    public T GetData<V>(V flag) where V : struct
    {
        return default(T);
    }
}


public class MyPropClassBase
{
    public int MyProperty { get; set; }
}

public class MyPropClass : MyPropClassBase
{
    public int MyProperty2 { get; set; }
}

public class MyFieldClassBase
{
    public int MyProperty;
}

public class MyFieldClass : MyFieldClassBase
{
    public int MyProperty2;
}

public class MyConstructorClassBase
{
    public MyConstructorClassBase()
    {

    }

    public MyConstructorClassBase(string name)
    {

    }
    public MyConstructorClassBase(string name, int age)
    {

    }
}

public class MyConstructorClass<T> : MyConstructorClassBase
{
    public List<T> MyProperty2 { get; set; }
    public MyConstructorClass()
    {

    }

    static MyConstructorClass()
    {

    }

    public MyConstructorClass(string name)
    {

    }

    protected internal MyConstructorClass(string name,int age)
    {

    }
}

public class MyTuple
{

    public (string name, int age) GetData()
    {

        return ("", 1);
    }
}


[Summary("This is document class for demo.")]
[Remarks(Message = "Demo remarks", DocTexts = new string[] { "this is first paragraph", "this is second paragraph." })]
public class DocDataClass
{
    [Summary("Get the list of user of same name.")]
    public DocDataClass()
    {

    }

    [Summary("Get the list of user of same name.")]
    [Example("example to get user:", @"
var obj new DocDataClass();

var data = obj.GetUsersByName('ram');")]
    [Exception("user null exception", nameof(ArgumentNullException))]
    [Parameter("name", nameof(String), "the user name")]
    [Returns("return the list of users match with provided name.")]
    public string[] GetUsersByName(string name)
    {
        return new string[0];
    }
}

/// <summary>
/// This is My example class.
/// </summary>
/// <remarks>
/// Demo remarks
/// <para>
/// this is first paragraph
/// </para>
/// <para>
/// this is second paragraph.
/// </para>
/// </remarks>
public class MyXmlClass
{
    public string Name { get; set; }

    /// <summary>
    /// Get the list of user of same name.
    /// </summary>
    public MyXmlClass() { }

    /// <summary>
    /// Get the list of user of same name.
    /// </summary>
    /// <param name="name">name of the user</param>
    public MyXmlClass(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Get the list of user of same name.
    /// </summary>
    /// <param name="name" <see cref="string"/> >name of the user</param>
    /// <example>
    /// example to get user:
    /// 
    /// var obj new DocDataClass();
    ///  var data = obj.GetUsersByName('ram');
    /// </example>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    /// <returns>return the list of users match with provided name.</returns>
    public string[] GetUsersByName(string name)
    {
        return new string[0];
    }
}
