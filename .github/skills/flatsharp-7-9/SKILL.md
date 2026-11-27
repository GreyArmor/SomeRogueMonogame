---
name: flatsharp
description: Write, review, and debug C#/.NET code that uses FlatSharp 7.9.0, the FlatBuffers serialization library (FlatSharp.Runtime + FlatSharp.Compiler, .fbs schemas, fs_ attributes). Use this skill whenever the user mentions FlatSharp, FlatBuffers in C# or .NET, .fbs schema files, FlatSharpSchema, fs_serializer, Serializer.Parse/Write/GetMaxSize, deserialization modes (Lazy, Greedy, GreedyMutable, Progressive), or wants fast zero-copy binary serialization in .NET, even if they never say "FlatSharp" explicitly. Also use it when migrating older FlatSharp code (attribute-based [FlatBufferTable] classes, version 5/6) to version 7.
---

# FlatSharp 7.9.0

FlatSharp is a C# implementation of Google's FlatBuffers: a zero-copy binary serialization format. It is a serialization library, not an AI tool. Version 7.9.0 was published to NuGet on 2025-05-26 (`FlatSharp.Runtime` and `FlatSharp.Compiler`).

The reason this skill exists: FlatSharp 7 changed the workflow from earlier versions, and older tutorials, blog posts, and model memory often describe APIs that no longer apply. Follow the schema-first workflow below and check "Version 7 pitfalls" before reaching for anything else.

## Workflow in one view

1. Reference **both** `FlatSharp.Runtime` and `FlatSharp.Compiler` at the **same version**.
2. Describe data in an `.fbs` schema (standard FlatBuffers IDL plus `fs_` attributes).
3. Opt the schema into the build with `<FlatSharpSchema Include="..." />`.
4. The compiler generates partial C# classes at build time, including a static `Serializer` on tables marked `fs_serializer`.
5. Call `GetMaxSize`, `Write`, and `Parse` on that `Serializer`.

## Project setup

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <FlatSharpSchema Include="Schemas\Person.fbs" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="FlatSharp.Runtime" Version="7.9.0" />
    <PackageReference Include="FlatSharp.Compiler" Version="7.9.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

Keep the two versions identical, because the generated code is tied to the runtime's internals. Mark the compiler `PrivateAssets="all"` so it does not flow to consumers of the project.

Generated code runs on .NET Framework, .NET Standard 2.0+, and .NET Core 2.1+. The compiler itself needs a modern runtime to execute, but that does not constrain the target framework.

## Writing a schema

```fbs
// All FlatSharp-specific attributes start with fs_ and must be declared first.
attribute "fs_serializer";

namespace MyNamespace;

enum Color : ubyte { Red = 1, Green, Blue }

struct Location {
    x : float;
    y : float;
    z : float;
}

table Person (fs_serializer) {
    id : int;
    name : string;
    parent : Person (deprecated);
    children : [ Person ];
    favorite_color : Color = Blue;
    position : Location;
}
```

Rules that cause most schema errors:
- Declare every custom attribute at the top with `attribute "fs_xxx";`, otherwise the compiler rejects it.
- Attribute values need quotes: `(fs_serializer:"Lazy")`, not `(fs_serializer:Lazy)`.
- Define a type before you use it (like C without forward declarations). If `A` contains `B`, `B` comes first.
- Use snake_case for fields. FlatSharp 7 converts them to PascalCase by default (`favorite_color` becomes `FavoriteColor`).
- Add `fs_serializer` to every table you intend to serialize as a root object. Without it there is no generated `Serializer`.
- Structs are fixed-size and cannot have optional fields or be changed after release. Use tables for anything that may evolve.

## Serializing and parsing

```csharp
Person person = new Person
{
    Id = 1,
    Name = "Ada",
    FavoriteColor = Color.Green,
    Position = new Location { X = 1f, Y = 2f, Z = 3f },
};

int maxBytesNeeded = Person.Serializer.GetMaxSize(person);
byte[] buffer = new byte[maxBytesNeeded];
int bytesWritten = Person.Serializer.Write(buffer, person);

Person parsed = Person.Serializer.Parse(buffer);
```

`GetMaxSize` is an upper bound, so slice to `bytesWritten` before sending or storing the data (`buffer.AsSpan(0, bytesWritten)`).

Naming caveat: the current GitHub README uses `Serializer.Write(buffer, item)`, but the NuGet package readme text still shows `Serializer.Serialize(buffer, item)`. If one does not compile, inspect the members of `Person.Serializer` (IntelliSense or `go to definition`) and the repo's `samples` folder rather than guessing.

## Deserialization modes

The mode is the main performance lever. Choose it per table with `fs_serializer:"<mode>"`. Version 7 also generates every mode's parser, so you can restrict output with `<FlatSharpDeserializers>` (see Compiler options) or select a mode at parse time.

| Mode | Behavior | Choose it when |
|---|---|---|
| `GreedyMutable` (default) | Whole object graph read up front, buffer not needed afterward, objects are editable | You want behavior like other serializers, or you will modify parsed objects |
| `Greedy` | Same as above but setters throw `NotMutableException` | Read-only data, buffer is recycled immediately |
| `Lazy` | Nothing cached, each property read goes back to the buffer | Sparse, at-most-once access, or very large buffers |
| `Progressive` | Lazy plus memoization of each field | You cannot predict access patterns; the safe general choice for large data |

Practical guidance:
- Start with the default. FlatSharp is fast even in `GreedyMutable`, and the wiki recommends benchmarking before optimizing.
- `Lazy` and `Progressive` objects keep a reference to the input buffer. Never return that buffer to a pool, overwrite it, or dispose it while those objects are still in use. This is the most common source of corrupted reads. `Greedy` and `GreedyMutable` do not have this problem.
- Everything except `GreedyMutable` throws `NotMutableException` on writes, apart from fields using `fs_writeThrough`.
- `Greedy` on very large buffers allocates the whole graph at once and can cause GC spikes.

## Frequently used `fs_` annotations

| Annotation | Purpose |
|---|---|
| `fs_serializer` | Generate a static `Serializer` for the table; optional value picks the deserialization mode |
| `fs_vector:"..."` | Pick the vector type: `IList`, `IReadOnlyList`, `ReadOnlyMemory`; `[ubyte]` defaults to `Memory<byte>` |
| `fs_sortedVector` | Sort a vector of tables at serialization time; items need a single `key` field |
| `fs_valueStruct` | Generate a C# `struct` instead of a class for a FlatBuffer struct (fewer allocations) |
| `fs_writeThrough` | Allow in-place updates of a parsed buffer in `Lazy`/`Progressive` modes |
| `fs_sharedString` | Deduplicate repeated strings |
| `fs_setter:"..."` | Control setter shape: `None`, `Protected`, `ProtectedInternal`, `Init`, `ProtectedInit`, and so on |
| `fs_defaultCtor:"None"` | Suppress the generated default constructor so you can supply your own |
| `fs_literalName` | Keep the field name exactly as written, skipping PascalCase normalization |
| `fs_partialProperty` | Generate a property as `partial` |

Remember to declare each one with `attribute "fs_xxx";` at the top of the schema.

## Extending generated types

All generated classes are `partial`. Add constructors, helper methods, or computed properties in a separate file instead of editing generated output:

```csharp
public partial class Location
{
    public Location(float x, float y, float z) { X = x; Y = y; Z = z; }
}
```

For immutable models, combine `fs_setter:"Init"` (or `"None"`) with `fs_defaultCtor:"None"`. If the project uses C# 11, properties marked `required` in the schema are emitted with the `required` modifier.

## Unions

Version 7 removed the `Switch` methods on unions. Use the `Accept`/visitor pattern instead. If you see old code calling `union.Switch(...)`, rewrite it rather than looking for a package that still has it.

## Compiler options (MSBuild)

| Property | Effect |
|---|---|
| `<FlatSharpNullable>` | Override nullable handling; by default it follows the project's `<Nullable>` setting |
| `<FlatSharpNameNormalization>false</...>` | Turn off snake_case to PascalCase conversion for the whole project |
| `<FlatSharpDeserializers>Lazy;Progressive</...>` | Generate only the listed modes to cut generated code size |
| `<FlatSharpClassDefinitionsOnly>true</...>` | Emit only type definitions, so shared schemas can live in one library |
| `<FlatSharpInputFilesOnly>true</...>` | Emit classes only for files listed explicitly, not for `include`d ones |
| `<FlatSharpFileVisibility>true</...>` | Use C# 11 `file` visibility for generated internals |
| `<FlatSharpPrettyPrint>true</...>` | Format generated C#; slows the build noticeably |

Use `<IncludePath>` on a `<FlatSharpSchema>` item when the schema has `include "other.fbs";` statements. Command-line equivalent: `dotnet FlatSharp.Compiler.dll -i "a.fbs;b.fbs" -o <dir>`, with `--includes`, `--normalize-field-names`, `--deserializers`, and similar flags. Unity cannot edit the `.csproj`, so run the compiler manually in a pre-build step there.

## Version 7 pitfalls

Check these first when a user's code or an online example does not work:
- **No runtime-only (attribute) mode.** Classes decorated with `[FlatBufferTable]` / `[FlatBufferItem]` and compiled at runtime through `FlatBufferSerializer.Default` belong to older versions. For 7.x, move the model to an `.fbs` schema. If the user is on an older package on purpose, say so and keep to that version's APIs.
- **Name normalization is on by default**, so generated property names are PascalCase even when the schema is snake_case.
- **Array vectors (`fs_vector:"Array"`) and non-virtual properties (`fs_nonVirtual`) are deprecated.** They behave badly under `Lazy` (an array is reallocated on each access). Prefer `IList`, `IReadOnlyList`, or the memory types.
- **Mismatched package versions** between Runtime and Compiler produce confusing build errors.
- **Object pooling** exists in version 7 but was introduced as experimental; mention that when recommending it.
- **Security:** FlatSharp avoids unsafe code and bounds-checks reads, but this does not replace integrity checks. For untrusted input, still apply size limits and checksums or signatures as appropriate.

## How to respond to requests

- Ask for the schema (or the C# model) and the access pattern before recommending a deserialization mode.
- Show a complete minimal example (csproj lines, schema, usage) rather than fragments, since the build wiring is where people get stuck.
- When unsure of an exact API signature in 7.9.0, say so and point to the repo's `samples` solution or the wiki rather than inventing a member name.

## Sources

- Repository and samples: https://github.com/jamescourtney/FlatSharp
- Wiki pages worth reading for details: Compiler, Deserialization Modes, FBS Annotations, Defining Immutable Objects, Vectors, Object Pooling
- FlatBuffers schema authoring guide: https://google.github.io/flatbuffers/flatbuffers_guide_writing_schema.html
