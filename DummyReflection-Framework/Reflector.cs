using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DummyReflection
{
    /// <summary>
    /// Helps with finding methods, variables, and constructors.
    /// </summary>
    public static class Reflector
    {
        /// <summary>
        /// Get a Field or Property
        /// </summary>
        /// <param name="instance">Instance of <see cref="Type"/> that contains a field or property</param>
        /// <param name="name">Name of the Field or Property. Case sensitive</param>
        /// <param name="bindingFlags">Flags of the field or property, set to all for easy purposes</param>
        /// <returns>A variable that may be read or written to. May be null</returns>
        public static Variable GetVariable(this object instance, string name, BindingFlags bindingFlags = (BindingFlags)(-1))
        {
            MemberInfo member = instance.GetType().GetField(name, bindingFlags);

            //if field is null check for a property.
            if (member == null)
                member = instance.GetType().GetProperty(name, bindingFlags);

            //if both were null just give em a null 
            if (member == null)
                return null;

            return new Variable(member, instance, true);

        }

        /// <summary>
        /// Get a static Field or Property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name">Name of the Field or Property. Case sensitive</param>
        /// <param name="bindingFlags">Flags of the field or property, set to all for easy purposes</param>
        /// <returns>A variable that may be read or written to. May be null</returns>
        public static Variable GetVariable(this Type type, string name, BindingFlags bindingFlags = (BindingFlags)(-1))
        {
            MemberInfo member = type.GetField(name, bindingFlags);

            //if field is null check for a property.
            if (member == null)
                member = type.GetProperty(name, bindingFlags);

            //if both were null just give em a null 
            if (member == null)
                return null;

            return new Variable(member, null, false);

        }


        /// <summary>
        /// Get a method from an instance
        /// </summary>
        /// <param name="instance">Instance of type</param>
        /// <param name="name">Name of method</param>
        /// <param name="genericTypeCount">Generic Type count</param>
        /// <param name="flags"></param>
        /// <returns>An invocable method</returns>
        public static Method GetMethod(this object instance, string name, int genericTypeCount = 0, BindingFlags flags = (BindingFlags)(-1))
        {
            if(genericTypeCount < 0)
            {
                Debug.Send("Generic type count reset to 0");
                genericTypeCount = 0;
            }

            MethodInfo method = Find(instance.GetType(), name, genericTypeCount, flags);


            if (method == null)
                return null;

            return new Method(method, instance, genericTypeCount, true);
        }

        /// <summary>
        /// Get a static method from an instance
        /// </summary>
        /// <param name="type">Type that contains method.</param>
        /// <param name="name">Name of method</param>
        /// <param name="genericTypeCount">Generic Type count</param>
        /// <param name="flags"></param>
        /// <returns>A static invocable method</returns>
        public static Method GetStaticMethod(this Type type, string name, int genericTypeCount = 0, BindingFlags flags = (BindingFlags)(-1))
        {
            if (genericTypeCount < 0)
            {
                Debug.Send("Generic type count reset to 0");
                genericTypeCount = 0;
            }

            MethodInfo method = Find(type, name, genericTypeCount, flags);


            if (method == null)
                return null;

            return new Method(method, null, genericTypeCount, false);
        }

        /// <summary>
        /// Get a constructor from a type, can be used with <see cref="CreateInstance"/>
        /// </summary>
        /// <param name="type">Type that contains ctor</param>
        /// <param name="types">Types that match the ctor</param>
        /// <returns></returns>
        public static ConstructorInfo GetCTOR(this Type type, params Type[] types)
        {
            return type.GetConstructor((BindingFlags)(-1), null, types, null);
        }

        /// <summary>
        /// Create an instance of a CTOR refer to <see cref="GetCTOR(Type, Type[])"/>
        /// </summary>
        /// <param name="info">CTOR</param>
        /// <param name="args">Args to pass in</param>
        /// <returns></returns>
        public static object CreateInstance(ConstructorInfo info, params object[] args)
        {
            return info.Invoke((BindingFlags)(-1), null, args, null);
        }

        /// <summary>
        /// Find generic or non-generic methods!
        /// </summary>
        /// <param name="type">Type that contains method</param>
        /// <param name="name">Name of method</param>
        /// <param name="genericCount">Generic type count, can be 0</param>
        /// <param name="flags">Method binding flags</param>
        /// <returns></returns>
        public static MethodInfo Find(Type type, string name, int genericCount, BindingFlags flags)
        {
            //check for generics and simple naming conventions
            foreach (MethodInfo m in type.GetMethods(flags))
            {
                if (m.IsGenericMethod)
                    if (m.GetGenericArguments().Length == genericCount && m.Name == name)
                        return m;

                if (m.Name == name)
                    return  m;
            }

            return null;
        }

        /// <summary>
        /// A recursive method to determine if <paramref name="a"/> inherits or equals <paramref name="b"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True if a : b or a = b</returns>
        public static bool Derives(Type a, Type b)
        {
            if (a == b)
                return true;

            Type nextA = a.BaseType;
            Type nextB = b.BaseType;

            if (nextA == null || nextB == null)
                return false;

            if (a == nextB || b == nextA)
                return true;

            //do a test on the base type of B
            if (Derives(a, nextB))
                return true;

            //do a test on the base type of a and check b
            return Derives(b, nextA);
        }

    }
}
