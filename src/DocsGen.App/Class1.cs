﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


public class BasicGeneric<T, V>
{

}
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

public class MyConstructorClass<T>: MyConstructorClassBase
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
}

public class MyTuple
{

    public (string name, int age) GetData()
    {

        return ("", 1);
    }
}