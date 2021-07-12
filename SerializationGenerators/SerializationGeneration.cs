using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SerializationGenerators;

namespace SerializationGeneration
{

    [Generator]
    public class SerilizationTypesGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            Debug.WriteLine("Execute code generator");



            var models = context.Compilation.SyntaxTrees.Select(s => context.Compilation.GetSemanticModel(s)).ToList();

            if (context.SyntaxReceiver is ComponentSyntaxReceiver receiver)
            {
                var allRevalentTypes = receiver.ComponentClasses.ToList();

          
                foreach (var t in receiver.ComponentClasses)
                {
                    TypeToStringHelper.FindTypes(context.Compilation, t, allRevalentTypes);
                }


                var semanticTypes = new List<ISymbol>();
                foreach (var type in allRevalentTypes)
                {
                    var model = context.Compilation.GetSemanticModel(type.GetLocation().SourceTree);
                    var symbol = model.GetDeclaredSymbol(type);
                    semanticTypes.Add(symbol);
                }

                //var mode1 = context.Compilation.GetSemanticModel(receiver.ComponentBaseNode.GetLocation().SourceTree);
                //var baseComponentType = model.GetDeclaredSymbol(receiver.ComponentBaseNode);

              



                var commonStart = new StringBuilder(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;

namespace NamelessRogue_updated.Engine.Serialization.SerializationClasses
{
");

                var commonEnd = new StringBuilder(@"
        }
    }
}");



                foreach (ClassDeclarationSyntax cds in allRevalentTypes)
                {
                    var sourceBuilder = new StringBuilder();

                    sourceBuilder.Append(commonStart);

                    sourceBuilder.Append($@"
                        [FlatBufferTable]
                        public class {cds.Identifier}Serializer
                        {{
");
                  
                    //foreach (var property in baseComponentProperties)
                    //{
                    //    var tupeActualName = ToGenericTypeString(type);
                    //    var fullName = ToGenericTypeString(property.PropertyType);
                    //    if (property.PropertyType.IsEnum)
                    //    {
                    //        listOfEnums.Add(fullName);
                    //    }
                    //}



                        context.AddSource("helloWorldGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                }


            }
          


            /*
            #>
                [FlatBufferItem(<#= flatBufferAttributeCounter#>)]  public <#= fullName #> <#= property.Name #> { get; set; }
            <#
                    flatBufferAttributeCounter++;
                }

            foreach (var property in type.GetProperties().Where(p => baseComponentProperties.Any(bp => bp.Name == p.Name) == false))
            {
                var fullName = ToGenericTypeString(property.PropertyType);
                if (property.PropertyType.IsEnum)
                {
                    listOfEnums.Add(fullName);
                }
            #>
                [FlatBufferItem(<#= flatBufferAttributeCounter#>)]  public <#= fullName #> <#= property.Name #> { get; set; }
            <#
                    flatBufferAttributeCounter++;
            }
            #>

            public void FillFrom(<#= type.FullName #> component)
                    {
            <#
                  foreach (var property in type.GetProperties())
            {


                if (property.PropertyType.IsEnum)
                {
            #>
                    this.<#= property.Name #> = (<#= property.PropertyType.Name #>)component.<#= property.Name #>;
            <#
                      }
                else if (property.PropertyType.IsGenericType && ienumerable.IsAssignableFrom(property.PropertyType))
                {
                    var genericTypeDef = property.PropertyType.GetGenericTypeDefinition();
            #>  
                    this.<#= property.Name #> = new <#= ToGenericTypeString(property.PropertyType) #>(component.<#= property.Name #>.Cast<<#= GetGenericArgsString(property.PropertyType) #>>());
            <#
                      }
                else
                {
            #>
                    this.<#= property.Name #> = component.<#= property.Name #>;
            <#
                      }
            }
            #>
                    }

                    public void FillTo(<#= type.FullName #> component)
                    {
            <#
                  foreach (var property in type.GetProperties())
            {
                if (property.PropertyType.IsEnum)
                {
            #>
                    component.<#= property.Name #> = (<#= property.PropertyType.FullName #>)this.<#= property.Name #>;
            <#
                      }
                else if (property.PropertyType.IsGenericType && ienumerable.IsAssignableFrom(property.PropertyType))
                {
                    var genericTypeDef = property.PropertyType.GetGenericTypeDefinition();
            #>  
                    component.<#= property.Name #> = new <#= ToGenericTypeString(property.PropertyType) #>(this.<#= property.Name #>.Cast<<#= GetGenericArgsString(property.PropertyType) #>>());
            <#
                      }
                else
                {
            #>
                    component.<#= property.Name #> = this.<#= property.Name #>;
            <#
                      }
            }
            #>

                    }

                    public static implicit operator <#= type.FullName #> (<#= type.Name #> thisType)
                    {
                        <#= type.FullName #> result = new <#= type.FullName #>();
                        thisType.FillTo(result);
                        return result;
                    }

                    public static implicit operator <#= type.Name #> ( <#= type.FullName #>  component)
                    {
                        <#= type.Name #> result = new <#= type.Name #>();
                        result.FillFrom(result);
                        return result;
                    }

                }
            <#
            } 
            #>
            */

           
        }

        public void Initialize(GeneratorInitializationContext context)
        {

#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            var componentSyntaxReceiver = new ComponentSyntaxReceiver();
            context.RegisterForSyntaxNotifications(()=>componentSyntaxReceiver);
        }
    }


    public sealed class ComponentSyntaxReceiver : ISyntaxReceiver
    {
        public List<SyntaxNode> ComponentClasses { get; private set; } = new List<SyntaxNode>();

        public SyntaxNode ComponentBaseNode { get; private set; }
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds)
            {

                if (cds.BaseList != null && cds.BaseList.Types.Any(t => t.ToString() == "Component")
                 )
                {
                    ComponentClasses.Add(cds);
                }
                else if (cds.Identifier.ValueText == "Component")
                {
                    ComponentBaseNode = cds;
                }
            }
        }
	}


}
