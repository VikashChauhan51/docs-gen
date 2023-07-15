using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DocsGen.App;
internal class Class1
{
}


public class BasicGeneric<T,V>
{

}
public class AdvanceGeneric<T, U>: Test<T,U> where T : class ,new()
    where U: struct
{

}
public class Base { }
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
