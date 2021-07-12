using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerializationGenerators
{
	public static class TypeToStringHelper
	{
       public static string ToGenericTypeString(Type t)
        {
            if (!t.IsGenericType)
            {
                if (t.Name == "IEntity" || t.Name == "Entity")
                {
                    return "Guid";
                }
                return t.Name;
            }
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeString(ta)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        public static string GetGenericArgsString(Type t)
        {
            if (!t.IsGenericType)
                return "";
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeString(ta)).ToArray());
            return genericArgs;
        }

       public static void FindTypes(Compilation compilation, SyntaxNode n, List<SyntaxNode> nestedTypes)
        {
            
            if (n != null)
            {
                nestedTypes.Add(n);
                foreach (SyntaxNode childNode in n.ChildNodes())
                {
                    if (childNode is PropertyDeclarationSyntax pds)
                    {
                        var model = compilation.GetSemanticModel(pds.GetLocation().SourceTree);
                        var symbol = model.GetDeclaredSymbol(pds);
                        if (symbol != null && symbol.Kind == SymbolKind.Property && symbol is IPropertySymbol propertySymbol)
                        {
                            if (propertySymbol.Type is INamedTypeSymbol type)
                            {   
                                var declaringSyntaxRef = type.DeclaringSyntaxReferences.FirstOrDefault();
                                if (declaringSyntaxRef != null)
                                {
                                    var syntax = declaringSyntaxRef.GetSyntax();
                                    if (
                                      !(type.IsValueType) &&
                                      !(type.EnumUnderlyingType != null) &&
                                      !nestedTypes.Any(x => x == syntax) &&
                                      !(type.IsGenericType) &&
                                      !(type.IsAbstract))
                                    {
                                        if (declaringSyntaxRef != default)
                                        {
                                            FindTypes(compilation, syntax, nestedTypes);
                                        }
                                    }
                                    else if (type.IsGenericType)
                                    {
                                        foreach (var typeArgument in type.TypeArguments)
                                        {
                                            if (declaringSyntaxRef != default)
                                            {
                                                var declaringSyntaxGenericType = type.DeclaringSyntaxReferences.FirstOrDefault();
                                                FindTypes(compilation, declaringSyntaxGenericType.GetSyntax(), nestedTypes);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }            
        }

    }
}
