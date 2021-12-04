# Dummy Reflection

A reflection library that contains abilities to find any value that is private or internal

``using DummyReflection;``

## Variables - Fields and Properties

```csharp

A a = new A();

//we can find fields and properties easily by using the getvariable method
Variable v = a.GetVariable("fieldOrProp"); //we can also include binding flags

//let's pretend that the field or property named "fieldOrProp" is of an int type. It can be got and set.
int aVal = v.Get<int>();
//setting
v.Set(10 /*object*/);

```

### Static

```csharp

//we will use the actual type to get this field or property instead of an instance
Variable v = typeof(A).GetVariable("fieldOrProperty"); //we can also add binding flags.

//same as before
int aVal = v.Get<int>();
//setting
v.Set(10 /*object*/);

```

## Method | Instance / Static

```csharp

A a = new A();

//this method takes in two int types
Method m = a.GetMethod("Add", genericTypeCount: 0); //we can also add in binding flags

//to get a static method we could say something like
//Method m = typeof(A).GetStaticMethod("StaticAdd", genericTypeCount: 0); with binding flags if needed.

object? addedObj = m.Invoke(10, 20);

//we can also do T? result = m.Invoke<int>(10, 20);

int result = 0;

if(addedObj != null)
   result = (int)addedObj;


Method gM = a.GetMethod("genMethodName", genericTypeCount: 1); //we can also add in binding flags

//let's invoke a generic method
//ex. PrintTypeName<T>(int times); method will print the type name x times

gM.Invoke(new Type[] { typeof(int) }, 5); //no return type.

```

## Constructors

```csharp

//getting ctors is as easy as pie.

ConstructorInfo? _ctor = typeof(Person).GetCTOR(typeof(string), typeof(int));

if(_ctor != null)
{
    object? instance = _ctor.CreateInstance("BIGDummyHead", 17); //we create an instance with the two instances of the types of the ctor
    //optionally we can say. T? instance = _ctor.CreateInstance<Person>("BIGDummyHead", 17); if we have the type ofc
    
    //we can than do whatever we want with the new instance.
    Method m = instance.GetMethod("GetName");
}


```

## Extras

```csharp

//this is the main method for finding all kinds of methods and such, no matter if they are of an instance, static, or generic.
int genCount = 0;
Method? m = Reflector.Find(typeof(Person), "MethodName" genCount, BindingFlags.NonPublic);

//this recursive method can figure out if type A inherits type B, it does so by searching all base types until it is sure it does not derive from type A and vice versa
bool derived = Reflector.Derives(typeof(A), typeof(C));
```

