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
using System;
using System.Reflection;

namespace Newtonsoft.Json.Meta
{
    internal class TraceMetaclass : Metaclass
    {
        public override void ApplyToType(Type type)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                                        | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            // Instrument constructors
            ConstructorInfo[] constructors = type.GetConstructors(bindingFlags);
            foreach (ConstructorInfo constructor in constructors)
            {
                if (!MetaPrimitives.IsImplicitlyDeclared(constructor))
                {
                    MetaPrimitives.ApplyDecorator(constructor, new TraceDecorator());
                }
            }

            // Instrument methods
            MethodInfo[] methods = type.GetMethods(bindingFlags);
            foreach (MethodInfo method in methods)
            {
                if (!MetaPrimitives.IsPropertyAccessor(method) && !MetaPrimitives.IsIterator(method))
                {
                    MetaPrimitives.ApplyDecorator(method, new TraceDecorator());
                }
            }

            // Instrument properties and indexers
            PropertyInfo[] properties = type.GetProperties(bindingFlags);
            foreach (PropertyInfo property in properties)
            {
                MetaPrimitives.ApplyDecorator(property, new TraceDecorator());
            }
        }
    }
}