using FlatSharp.Attributes;
using Mono.TextTemplating;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.Stats;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NamelessRogue_updated.Engine.Serialization
{
	[FlatBufferTable]
	public static class SerializationCodeGenerator
	{
		public static void Generate()
		{	

			TemplateGenerator generator = new TemplateGenerator();
			var inputFile = @"C:\Users\Admin\source\repos\SomeRogueMonogame\NamelessRogue_updated\Engine\Serialization\FlatSharpSerializationGeneration.tt";
			var outputFile = "SerializationClasses.cs";

			generator.ProcessTemplate(inputFile, outputFile);
        }
      

	}
}
