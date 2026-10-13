# ProtocolAI Beginner's Guide

This guide assumes you can read basic C#, but you do not need to know ProtocolAI.

## What you will build

By the end, you will be able to:

1. define a vocabulary;
2. encode known and unknown values;
3. validate a received payload;
4. resolve it;
5. carry it as JSON;
6. understand where AI/provider code belongs.

## 1. Install

```bash
dotnet add package TheSingularityWorkshop.ProtocolAi --version 0.1.0-alpha.3
```

Then:

```csharp
using TheSingularityWorkshop.ProtocolAi;
```

## 2. Build a vocabulary

```csharp
var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Build();
```

You now have an immutable protocol definition.

Think:

```
1001 = People
2001 = Bob
2002 = Jane
```

The protocol ID is the namespace. The symbol ID is the address within that namespace.

## 3. Encode

```csharp
var payload = people.Encode([
    "Bob",
    "New Character",
    "Jane"
]);
```

Conceptually:

```
Bob           -> reference 2001
New Character -> literal
Jane          -> reference 2002
```

Order is preserved.

## 4. Validate

When data comes from outside your trusted application boundary:

```csharp
people.Validate(payload);
```

Validation checks that the payload belongs to the expected protocol and that its references are defined.

Validation is not authorization.

## 5. Resolve

```csharp
var values = people.Resolve(payload);
```

The application gets its values back in order.

The important distinction is:

```
known reference -> vocabulary-defined identity
literal         -> still a literal
invalid reference -> validation failure
```

## 6. Transport with Alpha 3

```csharp
var json = ProtocolPayloadJson.Serialize(payload);

// Send or store json.

var received = ProtocolPayloadJson.Deserialize(json);

people.Validate(received);

var values = people.Resolve(received);
```

The JSON representation is described in [PROTOCOL_PAYLOAD_JSON.md](PROTOCOL_PAYLOAD_JSON.md).

## 7. Inspect the definition

```csharp
Console.WriteLine(people.Describe());
```

This is useful when debugging or building an AI-facing context.

Description is information, not permission.

## 8. Explicit references

```csharp
var bob = people.Reference("bobId");

Console.WriteLine(bob.ProtocolId);
Console.WriteLine(bob.SymbolId);
```

A reference can cross another subsystem without transferring ownership of the domain object.

## 9. Where AI belongs

The application may place an LLM between encoding and validation:

```
application
    |
    v
ProtocolAI
    |
    v
LLM / human / another system
    |
    v
ProtocolAI
    |
    v
host policy
    |
    v
application
```

The provider adapter owns credentials and transport.

ProtocolAI owns semantic identity.

## 10. The complete mental model

```
WHAT
ProtocolAI
    |
    v
HOW
GrammarAI
    |
    v
WHAT SHOULD HAPPEN
host/application
    |
    v
EXECUTION
application runtime
```

These are separate responsibilities.

## Next

For C# contracts and testing, continue to the [C# Developer's Guide](CSHARP_GUIDE.md).

For architecture, continue to the [Developer's Guide](DEVELOPERS_GUIDE.md).