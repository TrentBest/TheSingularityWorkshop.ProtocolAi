# Consuming ProtocolAI

ProtocolAI is designed to be consumed by the **tool that owns the vocabulary**.

The package does not call an LLM for you. It gives your application a small, deterministic vocabulary layer that an AI-facing host can use before and after model interaction.

## 1. Install the package

`bash
dotnet add package TheSingularityWorkshop.ProtocolAi
`

Or add it to a project file:

`xml
<PackageReference Include="TheSingularityWorkshop.ProtocolAi" Version="0.1.0-alpha.1" />
`

> The version above is the current alpha release. Use the version published on NuGet when consuming a later release.

## 2. Define the vocabulary your tool owns

Suppose a tool already knows about people.

`csharp
using TheSingularityWorkshop.ProtocolAi;

var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();
`

The important distinction is ownership:

`text
your application
      |
      | defines meaning
      v
ProtocolAI
      |
      +-- [1001] People
      +-- [2001] Bob
      +-- [2002] Jane
      +-- [2003] Sara
`

ProtocolAI supplies the form. Your application supplies the domain vocabulary.

## 3. Encode known values

The simplest consumption path is to resolve domain strings against the definition.

`csharp
var payload = people.Encode([
    "Bob",
    "Jane",
    "Sara"
]);

Console.WriteLine(payload);
`

Conceptually, the result is:

`text
[2001] [2002] [2003]
`

The application can now carry integer-backed references instead of repeatedly carrying the full domain values.

## 4. Unknown values remain visible

ProtocolAI does not pretend that every possible value is already registered.

`csharp
var payload = people.Encode([
    "Bob",
    "New Character"
]);

Console.WriteLine(payload);
`

Conceptually:

`text
[2001] "New Character"
`

That literal is the boundary where the **host** decides what to do.

For example, your application might:

1. recognize it as a creation request;
2. validate it;
3. assign a new symbol identity;
4. persist that identity;
5. add the resulting vocabulary entry to a future protocol definition.

The current alpha intentionally does not automate that lifecycle.

## 5. Decode an integer reference

When the application receives a known symbol ID:

`csharp
var value = people.Decode(2001);

Console.WriteLine(value);
`

Result:

`text
Bob
`

The integer is an address into the vocabulary owned by the application.

## 6. Ask for a stable reference

If you need the protocol and symbol identity together:

`csharp
var reference = people.Reference("bobId");

Console.WriteLine(reference.ProtocolId);
Console.WriteLine(reference.SymbolId);
`

That produces the explicit relationship:

`text
protocol = [1001]
symbol   = [2001]
`

## 7. Inspect the definition

ProtocolAI definitions are self-describing:

`csharp
Console.WriteLine(people.Describe());
`

Example:

`text
[1001] People
  [2001] bobId = "Bob"
  [2002] janeId = "Jane"
  [2003] saraId = "Sara"
`

That description is useful for diagnostics, protocol inspection, logging, documentation generation, and future serialization work.

## 8. Where the LLM fits

ProtocolAI deliberately stops before inference.

A host can construct a model-facing prompt or structured request using the definition, send it through the chosen model integration, then resolve the returned references against the same vocabulary.

`text
                    YOUR APPLICATION
                           |
                  defines vocabulary
                           |
                           v
                    +-------------+
                    | ProtocolAI  |
                    |    WHAT     |
                    +------+------+ 
                           |
                    protocol context
                           |
                           v
                    AI/model host
                           |
                           v
                         LLM
                           |
                    model response
                           |
                           v
                    AI/model host
                           |
                           v
                    ProtocolAI
                           |
                  resolve references
                           |
                           v
                 application state
`

ProtocolAI does **not** provide the AI client, prompt transport, inference, or execution layer.

## 9. A complete small example

`csharp
using TheSingularityWorkshop.ProtocolAi;

var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Build();

var request = people.Encode([
    "Bob",
    "Jane",
    "New Character"
]);

Console.WriteLine($"Request: {request}");

foreach (var value in request.Values)
{
    if (value.IsReference)
    {
        Console.WriteLine($"Known symbol: {value.SymbolId}");
    }
    else
    {
        Console.WriteLine($"Literal requires host handling: {value.Literal}");
    }
}
`

The important behavior is deterministic:

`text
"Bob"           -> [2001]
"Jane"          -> [2002]
"New Character" -> literal
`

The model may be probabilistic.

The vocabulary resolution performed by your application is not.

## What ProtocolAI gives you

- A self-defining integer-backed vocabulary.
- Explicit protocol and symbol identity.
- Known-value encoding.
- Literal fallback for unknown values.
- Integer decoding.
- Deterministic descriptions.

## What you still provide

- Model client.
- Prompting.
- Transport.
- Validation policy.
- Creation/registration policy.
- Persistence.
- Authorization.
- Execution semantics.
- Any provider-specific constrained-generation adapter.

That boundary is intentional.

## Next layer

Once your application has a vocabulary, GrammarAI can describe how those identities may be connected.

**ProtocolAI = WHAT.**

**GrammarAI = HOW.**

See [GrammarAI](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi) for the structural layer.
