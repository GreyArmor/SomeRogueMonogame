using NamelessRogue.Engine.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NamelessRogue.Engine.Serialization
{
    public static class SerializationCodeGenerator
    {
        static bool IsInTargetedAssemblies(Type t)
        {
            return t.Assembly.ManifestModule.Name == "NamelessRogue.dll" || t.Assembly.ManifestModule.Name == "MonoGame.Framework.dll" || t.Assembly.ManifestModule.Name == "Microsoft.Xna.Framework.dll";
        }
        static string ToGenericTypeString(Type t, bool fullNames)
        {
            if (!t.IsGenericType)
                return fullNames ? t.FullName : t.Name;
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeString(ta, fullNames)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        static string ToGenericTypeStringStorage(Type t)
        {
            if (!t.IsGenericType)
            { 
                if (t.Name == "Entity" || t.Name == "IEntity")
                {
                    return "EntityStorage";
                }
                else if (IsInTargetedAssemblies(t))
                {
                    return t.Name + "Storage";
                }
                else
                {
                    return t.Name;
                }
            }
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeStringStorage(ta)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        static string GetGenericTypeStringStorage(Type t)
        {
            
            if (!t.IsGenericType)
            {
                if (t.Name == "Entity" || t.Name == "IEntity")
                {
                    return "EntityStorage";
                }
                else if (IsInTargetedAssemblies(t))
                {
                    return t.Name + "Storage";
                }
                else
                {
                    return t.Name;
                }
            }
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeStringStorage(ta)).ToArray());
            return genericArgs;
        }


        static string GetGenericArgsString(Type t, bool fullNames)
        {
            if (!t.IsGenericType)
                return t.Name;
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeString(ta, fullNames)).ToArray());
            return genericArgs;
        }


        static void FindTypesInMyAssembly(Type o, List<Type> nestedTypes, bool firstTime = true)
        {

            if (o != null)
            {
                Type t = o;
                if (!firstTime && o.Assembly.ManifestModule.Name == "NamelessRogue.dll" && !nestedTypes.Any(x => x.FullName == t.FullName))
                {
                    firstTime = false;
                    nestedTypes.Add(t);
                }
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    var pt = pi.PropertyType;
                    if (
                          !pt.IsPrimitive &&
                          !pt.IsEnum &&
                          !nestedTypes.Any(x => x.FullName == pt.FullName) &&
                          !pt.IsGenericType &&
                          !pt.IsArray &&
                          (pt.Assembly.ManifestModule.Name == "NamelessRogue.dll")
                       )
                    {
                        FindTypesInMyAssembly(pt, nestedTypes, false);
                    }
                    else if (pt.IsGenericType && !nestedTypes.Any(x => x.FullName == pt.FullName))
                    {
                        foreach (var genericArgType in pt.GetGenericArguments())
                        {
                            if (!nestedTypes.Any(x => x.FullName == genericArgType.FullName))
                            {
                                FindTypesInMyAssembly(genericArgType, nestedTypes, false);
                            }
                        }
                    }
                    else if (pt.IsArray && !nestedTypes.Any(x => x.FullName == pt.FullName))
                    {
                        var elemType = pt.GetElementType();
                        if (!nestedTypes.Any(x => x.FullName == elemType.FullName))
                        {
                            FindTypesInMyAssembly(elemType, nestedTypes, false);
                        }
                    }
                }
            }
        }

        static string GetPropertyTypeName(PropertyInfo property, List<Type> types)
        {
            var t = property.PropertyType;
            var propertyTypeName = ToGenericTypeString(property.PropertyType, false);
            if (property.PropertyType.Name == "Entity" || property.PropertyType.Name == "IEntity")
            {
                propertyTypeName = "EntityStorage";
            }
            else if (property.PropertyType.IsGenericType)
            {
                propertyTypeName = ToGenericTypeStringStorage(property.PropertyType);
            }
            else if (property.PropertyType.IsArray)
            {
                var elemType = property.PropertyType.GetElementType();
                while (elemType.IsArray)
                {
                    elemType = elemType.GetElementType();
                }
                if (propertyTypeName.Contains(elemType.Name) && IsInTargetedAssemblies(elemType))
                {
                    propertyTypeName = propertyTypeName.Replace(elemType.Name, elemType.Name + "Storage");
                }
            }
            else if(!t.IsPrimitive && t!=typeof(Guid) && t!=typeof(string)) {
                propertyTypeName += "Storage";
            }
            return propertyTypeName;
        }

        public static void Generate()
        {
            var icomponent = typeof(IComponent);

            var ienumerable = typeof(IEnumerable<object>);
            //get all component types
            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => icomponent.IsAssignableFrom(p) && p != icomponent && !p.IsAbstract).ToList();

            //then get all types referenced in component types
            var nestedTypes = types.ToList();

            foreach (var type in types)
            {
                FindTypesInMyAssembly(type, nestedTypes);
            }

            types = nestedTypes.Distinct().ToList();

            //and now we need types from game assembly

            types = types.Where(t => t.Assembly.ManifestModule.Name == "NamelessRogue.dll" && !t.IsAbstract).ToList();

            var baseComponentProperties = typeof(Component).GetProperties();
            var listOfEnums = new List<string>();

            List<Tuple<string, string>> filenameAndCodePairs = new List<Tuple<string, string>>();

            string commonHeader = @"
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{";


            foreach (var type in types)
            {
                var storageName = type.Name + "Storage";
                var typename = type.Name;

                var typeFullName = type.FullName;
                StringBuilder code = new StringBuilder();
                code.Append(commonHeader);
                if (type.GetCustomAttributes(true).Any(a => a.GetType() == typeof(NamelessRogue.Engine.Serialization.SkipClassGeneration)))
                {
                    continue;
                }


                int flatBufferAttributeCounter = 0;
                code.Append($@"
    [FlatBufferTable]
    public class {storageName} : IStorage<{typeFullName}>
    {{");
                foreach (var property in baseComponentProperties)
                {
                    var propertyTypeName = GetPropertyTypeName(property, types);


                    code.Append($@"
            [FlatBufferItem({flatBufferAttributeCounter})]  public {propertyTypeName} {property.Name} {{ get; set; }}");
                    flatBufferAttributeCounter++;
                }

                foreach (var property in type.GetProperties().Where(p => baseComponentProperties.Any(bp => bp.Name == p.Name) == false && p.CanWrite && p.CanRead))
                {

                    var propertyTypeName = GetPropertyTypeName(property, types);


                    if (property.PropertyType.IsEnum)
                    {
                        listOfEnums.Add(property.PropertyType.Name);
                    }
                    code.Append(@$"
        [FlatBufferItem({flatBufferAttributeCounter})]  public {propertyTypeName} {property.Name} {{ get; set; }}
");
                    flatBufferAttributeCounter++;
                }


                code.Append(@$"
        public void FillFrom({typeFullName} component)
        {{
");
                foreach (var property in type.GetProperties().Where(p=> p.CanWrite && p.CanRead))
                {
                    var propertyTypeName = GetPropertyTypeName(property, types);


                    if (property.PropertyType.IsEnum)
                    {
                        listOfEnums.Add(propertyTypeName);
                    }


                    if (property.PropertyType.IsEnum)
                    {
                        code.Append(@$"
            this.{property.Name} = ({propertyTypeName})component.{property.Name};
");
                    }
                    else if (property.PropertyType.IsGenericType)
                    {
                        var genericTypeDef = property.PropertyType.GetGenericTypeDefinition();
                        code.Append(@$"
            this.{property.Name} = new {propertyTypeName}(component.{property.Name}.Cast<{GetGenericTypeStringStorage(property.PropertyType)}>());
");
                    }
                    else
                    {
                        if (property.PropertyType.Name == "IEntity")
                        {
                            code.Append(@$"
            this.{property.Name}.FillFrom(component.{property.Name});
");
                        }
                        else
                        {
                            code.Append(@$"
            this.{property.Name} = component.{property.Name};
");
                        }
                    }
                }
                code.Append(@$"
        }}

        public void FillTo({typeFullName} component)
        {{
");
                foreach (var property in type.GetProperties().Where(p => p.CanWrite && p.CanRead))
                {
                    if (property.PropertyType.IsEnum)
                    {
                        code.Append(@$"
            component.{property.Name} = ({property.PropertyType.FullName})this.{property.Name};
");
                    }
                    else if (property.PropertyType.IsGenericType)
                    {
                        var genericTypeDef = property.PropertyType.GetGenericTypeDefinition();
                        code.Append(@$"
            component.{property.Name} = new {ToGenericTypeString(property.PropertyType, true)}(this.{property.Name}.Cast<{GetGenericArgsString(property.PropertyType, true)}>());
");
                    }
                    else
                    {
                        if (property.PropertyType.Name == "IEntity")
                        {
                            code.Append(@$"
            this.{property.Name}.FillTo(component.{property.Name});
");
                        }
                        else
                        {

                            code.Append(@$"
            component.{property.Name} = this.{property.Name};
");
                        }
                    }
                }
                code.Append(@$"

        }}

        public static implicit operator {typeFullName} ({storageName} thisType)
        {{
            {typeFullName} result = new {typeFullName}();
            thisType.FillTo(result);
            return result;
        }}

        public static implicit operator {storageName} ({typeFullName}  component)
        {{
            {storageName} result = new {storageName}();
            result.FillFrom(result);
            return result;
        }}

    }}
");
                code.Append(@$"
}}");

                filenameAndCodePairs.Add(new Tuple<string, string>(storageName, code.ToString()));
            }

            listOfEnums = listOfEnums.GroupBy(t => t).Select(g => g.First()).ToList();


            var enumTypes = AppDomain.CurrentDomain.GetAssemblies().First(a => a.ManifestModule.Name == "NamelessRogue.dll").GetTypes().Where(t => t.IsEnum && listOfEnums.Contains(t.Name)).ToList();

            enumTypes = enumTypes.GroupBy(t => t.FullName).Select(g => g.First()).ToList();


            foreach (Type enumType in enumTypes)
            {
                StringBuilder code = new StringBuilder();
                code.Append(commonHeader);
                var underType = enumType.GetEnumUnderlyingType();
                var enumValues = enumType.GetEnumNames();
                code.Append(@$"
    [FlatBufferEnum(typeof({underType.Name}))]
    public enum {enumType.Name}Storage : {underType.Name} 
    {{
    ");
                foreach (var enumName in enumValues)
                {
                    code.Append(@$"     {enumName},
    ");
                }
                code.Append(@$"
    }}
}}
");
                filenameAndCodePairs.Add(new Tuple<string, string>(enumType.Name + "Storage", code.ToString()));
            }

            Directory.CreateDirectory("AutogeneratedSerializationClasses");

            foreach (var pair in filenameAndCodePairs)
            {
                File.WriteAllText("AutogeneratedSerializationClasses\\" + pair.Item1+".cs", pair.Item2);
            }
        }
    }
}
