using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DummyReflection
{
    /// <summary>
    /// Represents a reflected Field or Property
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Is <see cref="MInfo"/> a <seealso cref="PropertyInfo"/>?
        /// </summary>
        public bool IsProperty { get; init; }

        /// <summary>
        /// Is <see cref="MInfo"/> a <seealso cref="FieldInfo"/>?
        /// </summary>
        public bool IsField => !IsProperty;

        /// <summary>
        /// Determines if the <see cref="MemberInfo"/> can be read
        /// </summary>
        /// <remarks>Always true if <see cref="IsField"/> is true</remarks>
        public bool CanRead
        {
            get
            {
                //not null, we throw exceptions in the ctor...
                if (IsProperty)
                    return ((PropertyInfo)MInfo).CanRead;

                //we can read any field :)
                return true;
            }
        }

        /// <summary>
        /// Determines if the <see cref="MemberInfo"/> can be written to
        /// </summary>
        /// <remarks>False if <see cref="MInfo"/> is const</remarks>
        public bool CanWrite
        {
            get
            {
                if (IsProperty)
                    return ((PropertyInfo)MInfo).CanWrite;

                //return opposite of is const.
                return !((FieldInfo)MInfo).IsLiteral;
            }
        }


        /// <summary>
        /// FieldInfo or PropertyInfo
        /// </summary>
        public MemberInfo MInfo { get; init; }

        /// <summary>
        /// An instance of the type, not required for static items.
        /// </summary>
        public object? Instance { get; init; } = null;

        /// <summary>
        /// Create a Variable based on a <see cref="FieldInfo"/> or <seealso cref="PropertyInfo"/>
        /// </summary>
        /// <param name="member">Info on the Field or Property</param>
        /// <param name="instance">Instance of type, can be null</param>
        /// <param name="requireInstance">Require instance</param>
        /// <exception cref="ArgumentNullException">Throw if <paramref name="member"/> null</exception>
        /// <exception cref="Exception">Thrown if invalid <see cref="MemberInfo"/></exception>
        public Variable(MemberInfo member, object? instance = null, bool requireInstance = false)
        {
            MInfo = member ?? throw new ArgumentNullException(nameof(member));
            Instance = instance;

            if (Instance == null && requireInstance)
                throw new Exception("This Field or Property requires an instance.");

            if (MInfo is not PropertyInfo && MInfo is not FieldInfo)
                throw invalidInfo;

            IsProperty = MInfo is PropertyInfo;

        }

        /// <summary>
        /// Read the value of your Field or Property
        /// </summary>
        /// <returns></returns>
        public object? Get()
        {
            if (!CanRead)
            {
                Debug.Send("Cannot Read MemberInfo");
                return null;
            }

            if (IsProperty)
                return ((PropertyInfo)MInfo).GetValue(Instance);

            return ((FieldInfo)MInfo).GetValue(Instance);
        }

        /// <summary>
        /// Convert to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="Get"/> as a <typeparamref name="T"/></returns>
        public T? Get<T>()
        {
            return (T?)Get();
        }

        /// <summary>
        /// Set the value of your Field or Property
        /// </summary>
        /// <param name="value"></param>
        public void Set(object value)
        {
            if(value is null)
            {
                Debug.Send("Value cannot be null");
                return;
            }
            else if (!CanWrite)
            {
                Debug.Send("Cannot write to MemberInfo");
                return;
            }
            else if (value.GetType() != MInfo.DeclaringType)
            {
                Debug.Send("Set value mismatch!");
                return;
            }

            if (IsProperty)
            {
                ((PropertyInfo)MInfo).SetValue(Instance, value);
                return;
            }

            ((FieldInfo)MInfo).SetValue(Instance, value);
        }


        /// <summary>
        /// Get the methods of a property.
        /// </summary>
        /// <param name="info">Read property</param>
        /// <returns>The getter and setter of a <see cref="PropertyInfo"/></returns>
        public static (MethodInfo? get, MethodInfo? set) PropertyMethods(PropertyInfo info)
        {
            return (info.GetGetMethod(true),  info.GetSetMethod(true));
        }


        private readonly Exception invalidInfo =
            new("Invalid MemberInfo provided, please provide only FieldInfo or PropertyInfo");

    }
}
