# Consuming ProtocolAI

> **Your application owns meaning. ProtocolAI gives that meaning deterministic, addressable identity.**

This is the practical guide for someone who has a .NET application and wants to use ProtocolAI without becoming a ProtocolAI maintainer.

## 1. What you are adding

```
define vocabulary
      |
      v
encode known values
      |
      v
validate received identity
      |
      v
resolve identity into application values
```

Alpha 3 adds a fifth boundary:

```
ProtocolPayload
      |
      v
provider-neutral JSON
      |
      v
another process / service / application
```

It does not add an AI provider.

## 2. Install

For the current alpha:

```bash
dotnet add package TheSingularityWorkshop.ProtocolAi --version 0.1.0-alpha.3
```

Then:

```csharp
using TheSingularityWorkshop.ProtocolAi;
```

No LLM SDK, GrammarAI package, or FSM_COS package is required.

## 3. Define a vocabulary

```csharp
var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();
```

Think of it as a dictionary with addresses:

```
protocol 1001 = People
    2001 = Bob
    2002 = Jane
    2003 = Sara
```

The numbers are not global truth. They are addresses inside an application-owned vocabulary.

## 4. Encode

```csharp
var payload = people.Encode([
    "Bob",
    "Jane",
    "New Character"
]);
```

Conceptually:

```
Bob           -> [2001]
Jane          -> [2002]
New Character -> literal
```

The order is preserved. Unknown values are not silently assigned identities.

## 5. Validate external data

When a payload crosses a trust or process boundary:

```csharp
people.Validate(payload);
```

This asks:

> Does this payload belong to this vocabulary, and do its references exist?

It does **not** ask whether the sender is authenticated or whether an action is authorized.

Those are host concerns.

## 6. Resolve

```csharp
var values = people.Resolve(payload);
```

A useful mental model:

```
[2001]        -> known application identity
"Amelia"      -> unknown literal
[9999]        -> invalid reference if undefined
```

Unknown is not invalid.

## 7. Transport with Alpha 3 JSON

```csharp
var json = ProtocolPayloadJson.Serialize(payload);

// transport json however your application needs

var received = ProtocolPayloadJson.Deserialize(json);

people.Validate(received);

var values = people.Resolve(received);
```

The JSON layer is intentionally provider-neutral. It can travel through HTTP, a queue, a file, a database, WebSocket framing, or a clipboard artifact.

Read [PROTOCOL_PAYLOAD_JSON.md](PROTOCOL_PAYLOAD_JSON.md) for the exact representation.

## 8. Inspect a vocabulary

```csharp
Console.WriteLine(people.Describe());
```

Self-description is useful for diagnostics, documentation, generated context, and exchange tooling.

It is not authorization.

## 9. Explicit references

```csharp
var bob = people.Reference("bobId");

Console.WriteLine(bob.ProtocolId);
Console.WriteLine(bob.SymbolId);
```

Conceptually:

```
[1001 : 2001]
```

The protocol supplies the namespace. The symbol supplies the address.

## 10. Put AI outside the core

```
application
    |
    v
ProtocolAI
    |
    v
provider / clipboard / local model
    |
    v
model response
    |
    v
ProtocolAI
    |
    v
host policy
    |
    v
application state
```

The provider owns credentials, endpoints, model selection, and transport.

ProtocolAI owns semantic identity.

## 11. A complete example

```csharp
using TheSingularityWorkshop.ProtocolAi;

var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Build();

var outgoing = people.Encode(["Bob", "Jane", "New Character"]);

var json = ProtocolPayloadJson.Serialize(outgoing);

// Imagine another process received json.
var incoming = ProtocolPayloadJson.Deserialize(json);

people.Validate(incoming);

foreach (var value in people.Resolve(incoming))
{
    Console.WriteLine(value);
}
```

The result is:

```
Bob
Jane
New Character
```

## 12. Common mistakes

### Treating IDs as global

An integer such as 2001 means nothing by itself. It is meaningful in the context of its protocol.

### Treating unknown as failure

An unknown literal can be useful information.

### Resolving before validating

Validate externally received references first.

### Treating validation as authorization

A valid symbol does not grant permission.

### Putting provider code into ProtocolAI

Keep provider credentials and transport above the semantic foundation.

### Using ProtocolAI for every string

ProtocolAI is useful when deterministic vocabulary ownership matters. It is not a replacement for ordinary application data.

## 13. Where next?

- Want plain English? Read the [Idiot's Guide](IDIOTS_GUIDE.md).
- Want to build? Read the [Beginner's Guide](BEGINNERS_GUIDE.md).
- Want C# details? Read the [C# Developer's Guide](CSHARP_GUIDE.md).
- Want architecture? Read the [Developer's Guide](DEVELOPERS_GUIDE.md).
- Want AI exchange design? Read [AI Exchange](AI_EXCHANGE.md).
- Want the reasoning? Read [Theory](THEORY.md).
- Want Workshop boundaries? Read [Ecosystem Integration](ECOSYSTEM_INTEGRATION.md).

**You can use ProtocolAI without learning the whole Workshop.**