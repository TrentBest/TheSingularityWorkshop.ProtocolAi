# ProtocolAI Beginner's Guide

This guide assumes you can read basic C#.

## 1. Install it

Add `TheSingularityWorkshop.ProtocolAi` to a .NET 8 application.

Then:

```csharp
using TheSingularityWorkshop.ProtocolAi;
```

## 2. Define a vocabulary

```csharp
var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Build();
```

The protocol ID identifies the vocabulary. Each symbol ID identifies one entry inside it.

## 3. Encode values

```csharp
var payload = people.Encode(["Bob", "New Character", "Jane"]);
```

The result contains two references and one literal.

## 4. Validate before trusting references

```csharp
people.Validate(payload);
```

Validation checks that the payload belongs to this protocol and that every referenced symbol exists.

## 5. Resolve

```csharp
var values = people.Resolve(payload);
```

The result is Bob, New Character, Jane in that order.

## 6. Put the payload on a wire

Alpha.3 adds a provider-neutral JSON representation:

```csharp
var json = ProtocolPayloadJson.Serialize(payload);

var received = ProtocolPayloadJson.Deserialize(json);
people.Validate(received);
var values = people.Resolve(received);
```

JSON is transport. The protocol remains the authority for meaning.

## 7. Show the vocabulary

```csharp
Console.WriteLine(people.Describe());
```

This produces a deterministic, human-readable vocabulary description suitable for application-owned AI exchange context.

## The key rule

Do not confuse **transport**, **vocabulary**, and **application behavior**.

ProtocolAI transports references.

Your protocol defines meaning.

Your application decides what those meanings do.
