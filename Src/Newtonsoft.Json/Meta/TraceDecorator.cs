#region License
// Copyright (c) 2016 Aleksandar Dalemski
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using CSharp.Meta;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft.Json.Meta
{
    internal class TraceDecorator : Decorator
    {
        public override void DecorateConstructor(ConstructorInfo constructor, object thisObject, object[] arguments)
        {
            string constructorDescription;

            ParameterInfo[] parameters = constructor.GetParameters();
            int parameterCount = parameters.Length;
            if (parameterCount == 0)
            {
                constructorDescription = string.Format(CultureInfo.InvariantCulture, "{0}.{1}()", constructor.DeclaringType, constructor.Name);
            }
            else
            {
                var argumentStrings = new string[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                {
                    argumentStrings[i] = arguments[i].ToString();
                }
                constructorDescription = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}({2})",
                    constructor.DeclaringType,
                    constructor.Name,
                    string.Join(", ", argumentStrings));
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Entering {0}", constructorDescription));

            // Splice location
            constructor.Invoke(thisObject, arguments);

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Exiting {0}", constructorDescription));
        }

        public override object DecorateMethod(MethodInfo method, object thisObject, object[] arguments)
        {
            string methodDescription;

            ParameterInfo[] parameters = method.GetParameters();
            int parameterCount = parameters.Length;
            if (parameterCount == 0)
            {
                methodDescription = string.Format(CultureInfo.InvariantCulture, "{0}.{1}()", method.DeclaringType, method.Name);
            }
            else
            {
                var argumentStrings = new string[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                {
                    argumentStrings[i] = arguments[i].ToString();
                }
                methodDescription = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}({2})",
                    method.DeclaringType,
                    method.Name,
                    string.Join(", ", argumentStrings));
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Entering {0}", methodDescription));

            if (method.ReturnType == typeof(void))
            {
                // Splice location
                method.Invoke(thisObject, arguments);

                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Exiting {0}", methodDescription));

                return MetaPrimitives.DefaultValue(method.ReturnType);
            }
            else
            {
                // Splice location
                object result = method.Invoke(thisObject, arguments);

                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Exiting {0} with result {1}", methodDescription, result));

                return result;
            }
        }

        public override object DecorateIndexerGet(PropertyInfo indexer, object thisObject, object[] arguments)
        {
            string indexerDescription;

            ParameterInfo[] parameters = indexer.GetIndexParameters();
            int parameterCount = parameters.Length;
            var argumentStrings = new string[parameterCount];
            for (int i = 0; i < parameterCount; i++)
            {
                argumentStrings[i] = arguments[i].ToString();
            }
            indexerDescription = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.this.get({1})",
                indexer.DeclaringType,
                string.Join(", ", argumentStrings));

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Entering {0}", indexerDescription));

            // Splice location
            object result = indexer.GetValue(thisObject, arguments);

            if (indexer.PropertyType == typeof(void))
            {
                throw new MetaException("An indexer should not have type void.");
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Exiting {0} with result {1}", indexerDescription, result));
            }

            return result;
        }

        public override void DecorateIndexerSet(PropertyInfo indexer, object thisObject, object[] arguments, object value)
        {
            string indexerDescription;

            ParameterInfo[] parameters = indexer.GetIndexParameters();
            int parameterCount = parameters.Length;
            var argumentStrings = new string[parameterCount + 1];
            for (int i = 0; i < parameterCount; i++)
            {
                argumentStrings[i] = arguments[i].ToString();
            }
            argumentStrings[parameterCount] = value.ToString();
            indexerDescription = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.this.set({1})",
                indexer.DeclaringType,
                string.Join(", ", argumentStrings));

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Entering {0}", indexerDescription));

            // Splice location
            indexer.SetValue(thisObject, value, arguments);

            if (indexer.PropertyType == typeof(void))
            {
                throw new MetaException("An indexer should not have type void.");
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Exiting {0}", indexerDescription));
            }
        }

        public override object DecoratePropertyGet(PropertyInfo property, object thisObject)
        {
            string propertyDescription = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.get()", property.DeclaringType, property.Name);

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Entering {0}", propertyDescription));

            // Splice location
            object result = property.GetValue(thisObject, null);

            if (property.PropertyType == typeof(void))
            {
                throw new MetaException("A property should not have type void.");
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Exiting {0} with result {1}", propertyDescription, result));
            }

            return result;
        }

        public override void DecoratePropertySet(PropertyInfo property, object thisObject, object value)
        {
            string propertyDescription = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.set({2})", property.DeclaringType, property.Name, value);

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Entering {0}", propertyDescription));

            // Splice location
            property.SetValue(thisObject, value, null);

            if (property.PropertyType == typeof(void))
            {
                throw new MetaException("A property should not have type void.");
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Exiting {0}", propertyDescription));
            }
        }
    }
}