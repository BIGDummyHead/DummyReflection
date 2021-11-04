using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DummyReflection
{
    /// <summary>
    /// A reflected <see cref="MethodInfo"/>
    /// </summary>
    public class Method
    {
        /// <summary>
        /// Has any return type?
        /// </summary>
        public bool HasReturnType => MethodI.ReturnType == typeof(void);

        /// <summary>
        /// The return type of the method.
        /// </summary>
        public Type ReturnType => MethodI.ReturnType;

        /// <summary>
        /// Instance of type, can be null
        /// </summary>
        public object? Instance { get; init; }

        /// <summary>
        /// Method to invoke
        /// </summary>
        public MethodInfo MethodI { get; init; }

        /// <summary>
        /// Create a method that you can invole easily
        /// </summary>
        /// <param name="info">Info on the method</param>
        /// <param name="instance">Instance if any, one may be required</param>
        /// <param name="generic">The amount of generic parameters if any, must be greater than 0, will not be fixed in CTOR</param>
        /// <param name="requireInstance">Require an instance to be provided, set to false by default</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public Method(MethodInfo info, object? instance = null, int generic = 0, bool requireInstance = false)
        {
            gen = generic;
            MethodI = info ?? throw new ArgumentNullException(nameof(info));

            if (MethodI.IsGenericMethod)
                if (MethodI.GetGenericArguments().Length != generic)
                    throw new Exception("Generic argument mismatch");

            Instance = instance;

            if (Instance == null && requireInstance)
                throw new Exception("Method is not static, an instance is required.");
        }

        /// <summary>
        /// Invoke a non-generic method.
        /// </summary>
        /// <param name="prs">Arguments</param>
        /// <returns>The return type of the method if any.</returns>
        public object? Invoke(params object[] prs)
        {
            if(MethodI.IsGenericMethod)
            {
                Debug.Send("Non-generic call on generic method.");
                return null;
            }

            return MethodI.Invoke(Instance, prs);
        }

        /// <summary>
        /// Invoke a non-generic method.
        /// </summary>
        /// <param name="prs">Arguments</param>
        /// <returns>The return type of the method if any.</returns>
        public T? Invoke<T>(params object[] prs)
        {
            return (T?)Invoke(prs);
        }

        /// <summary>
        /// Invoke generic method.
        /// </summary>
        /// <param name="types">Generic types</param>
        /// <param name="prs">Arguments</param>
        /// <returns>The return type of the method if any.</returns>
        public object? Invoke(Type[] types, params object[] prs)
        {
            if (gen == 0)
            {
                Debug.Send("Generic call on non-generic method.");
                return Invoke(prs);
            }
            else if(types.Length != gen)
            {
                Debug.Send("Generic call failed, Type count mismatch");
                return null;
            }

            return MethodI.MakeGenericMethod(types).Invoke(Instance, prs);
        }

        /// <summary>
        /// Invoke generic method.
        /// </summary>
        /// <param name="types">Generic types</param>
        /// <param name="prs">Arguments</param>
        /// <returns>The return type of the method if any. (CASTED)</returns>
        public T? Invoke<T>(Type[] types, params object[] prs)
        {
            return (T?)Invoke(types, prs);
        }


        private int gen;
    }
}
