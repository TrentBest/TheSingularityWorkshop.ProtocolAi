# TheSingularityWorkshop.ProtocolAi

**Self-defining integer-backed protocol vocabularies for AI interaction.**

ProtocolAI defines the **lexicon layer** of a structured AI protocol.

Instead of repeatedly asking an LLM to emit a large body of descriptive strings, a tool can define a vocabulary once:

```text
[1001] People

[2001] bobId  = "Bob"
[2002] ryanId = "Ryan"
[2003] saraId = "Sara"
[2004] janeId = "Jane"
[2005] jackId = "Jack"
[2006] jillId = "Jill"
```

The model can then operate on the integer-backed representation:

```text
[2001] [2004] [2006]
```

When a value cannot be represented by the vocabulary — for example, a newly created object's name — the protocol can carry a literal string:

```text
[2001] [2004] "New Character"
```

ProtocolAI does **not** contain an LLM. It defines the vocabulary and representation that an LLM-facing host can provide to a model and decode from its response.

## Core model

```text
Tool defines vocabulary
        |
        v
+-------------------+
| ProtocolDefinition|
| [1001] People     |
+---------+---------+
          |
          +--> [2001] bobId  = "Bob"
          +--> [2002] ryanId = "Ryan"
          +--> ...
          |
          v
       LLM prompt
          |
          v
  integer-backed output
          |
          +--> [2001]
          +--> [2004]
          +--> "New Character"
```

## The API

```csharp
var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "ryanId", "Ryan")
    .Define(2003, "saraId", "Sara")
    .Define(2004, "janeId", "Jane")
    .Define(2005, "jackId", "Jack")
    .Define(2006, "jillId", "Jill")
    .Build();

var payload = people.Encode(["Bob", "Jane", "New Character"]);

Console.WriteLine(payload);
// [2001] [2004] New Character

Console.WriteLine(people.Decode(2001));
// Bob

Console.WriteLine(people.Describe());
```

The definition is deliberately **self-describing**. A tool defines its own nomenclature, symbols, and values rather than requiring ProtocolAI to know the domain beforehand.

## Design boundaries

ProtocolAI owns:

- protocol identity;
- symbol identity;
- symbolic names;
- string values;
- string-to-integer encoding;
- integer-to-string decoding;
- mixed integer/literal payloads;
- deterministic protocol description.

ProtocolAI does **not** own:

- LLM clients;
- model selection;
- tokenization;
- prompting policy;
- transport;
- MicroBundle hosting;
- grammar composition.

Those are separate composition concerns.

## Relationship to GrammarAI

ProtocolAI answers:

> **What does this symbol mean?**

GrammarAI answers the next question:

> **How may these symbols be connected?**

GrammarAI can reference ProtocolAI definitions without embedding the vocabularies themselves.

```text
ProtocolAI
    |
    | lexicon / values
    v
GrammarAI
    |
    | structure / composition
    v
AI-facing host
    |
    v
LLM
```

## Development

```bash
dotnet restore TheSingularityWorkshop.ProtocolAi.slnx
dotnet build TheSingularityWorkshop.ProtocolAi.slnx --configuration Release
dotnet test tests/ProtocolAi.Tests/ProtocolAi.Tests.csproj --configuration Release
dotnet pack TheSingularityWorkshop.ProtocolAi.csproj --configuration Release --output ./artifacts
```

## License

MIT. See LICENSE.txt.

---

## Resources & Support

- Core NuGet: https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi
- Source Code: https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi
- FSM_API: https://www.nuget.org/packages/TheSingularityWorkshop.FSM_API
- MicroBundleDomain: https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain

<p align="center">
  <a href="https://github.com/TrentBest/FSM_API">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" height="200">
  </a>
</p>

<p align="center">
  <em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br>
  <strong>Because state shouldn't be a mess.</strong>
</p>
