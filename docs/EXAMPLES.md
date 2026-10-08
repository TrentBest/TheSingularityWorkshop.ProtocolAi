# ProtocolAI Examples

These examples progress from a simple vocabulary to a complete semantic boundary.

## Example 1 — Commands

```csharp
var commands = new ProtocolBuilder(7100, "WorkshopCommands")
    .Define(7101, "inspect", "inspect")
    .Define(7102, "move", "move")
    .Define(7103, "create", "create")
    .Define(7104, "delete", "delete")
    .Build();

var request = commands.Encode(["inspect", "move"]);
```

Conceptually:

```
[7101] [7102]
```

ProtocolAI identifies the commands. It does not execute them.

## Example 2 — Existing objects

```csharp
var objects = new ProtocolBuilder(7200, "WorkshopObjects")
    .Define(7201, "forge", "Forge")
    .Define(7202, "laboratory", "Research Laboratory")
    .Build();

var request = objects.Encode([
    "Forge",
    "Research Laboratory"
]);
```

Known objects become application-owned identities.

## Example 3 — Existing versus new

```csharp
var request = objects.Encode([
    "Forge",
    "Avengers Landing Pad"
]);
```

Conceptually:

```
Forge
  -> [7201]

Avengers Landing Pad
  -> literal
```

The host can decide whether the new value should become a domain object, be rejected, or remain transient.

## Example 4 — Self-description

```csharp
Console.WriteLine(objects.Describe());
```

A human-readable description makes the vocabulary inspectable without reconstructing it from source code.

## Example 5 — Explicit references

```csharp
var forge = objects.Reference("forge");

Console.WriteLine(forge.ProtocolId);
Console.WriteLine(forge.SymbolId);
```

Conceptually:

```
[7200 : 7201]
```

This is useful when another subsystem needs identity without owning the domain object.

## Example 6 — JSON transport

```csharp
var payload = objects.Encode(["Forge", "New Building"]);

var json = ProtocolPayloadJson.Serialize(payload);

var received = ProtocolPayloadJson.Deserialize(json);

objects.Validate(received);

var values = objects.Resolve(received);
```

The transport boundary is:

```
ProtocolPayload
      |
      v
JSON
      |
      v
ProtocolPayload
      |
      v
validate
      |
      v
resolve
```

See [PROTOCOL_PAYLOAD_JSON.md](PROTOCOL_PAYLOAD_JSON.md) for the wire contract.

## Example 7 — Full WHAT boundary

```
DOMAIN
  |
  | "Forge", "Laboratory"
  v
ProtocolAI
  |
  | deterministic identity
  v
semantic representation
  |
  v
LLM / human / another application
  |
  v
returned data
  |
  v
ProtocolAI
  |
  | validation + resolution
  v
host policy
```

This is the core architectural pattern.

## Example 8 — Pairing with GrammarAI

ProtocolAI can own identities while GrammarAI owns relationships.

```
ProtocolAI
[1001] People
[2001] Bob
[2002] Jane
       |
       | external identity
       v
GrammarAI
relationship / structure
       |
       v
host policy
```

The two packages cooperate without needing to become one package.

## Example 9 — The non-AI case

ProtocolAI does not require an LLM.

A deterministic application can use the same vocabulary:

```csharp
var protocol = new ProtocolBuilder(8000, "EditorCommands")
    .Define(8001, "save", "save")
    .Define(8002, "publish", "publish")
    .Build();

var payload = protocol.Encode(["save"]);

protocol.Validate(payload);

var command = protocol.Resolve(payload).Single();
```

The semantic identity remains useful because the application owns it.

That is an important test of the architecture:

> **AI is a participant, not the reason the vocabulary exists.**