# Consuming ProtocolAI

This guide is for the person who has a working .NET application and wants to **use ProtocolAI**, not build ProtocolAI.

If you remember only one thing, remember this:

> **Your application owns meaning. ProtocolAI gives that meaning deterministic, addressable identity.**

The package does not call an LLM for you. It gives your application a semantic boundary that can sit before and after model interaction.

---

## 1. Install the package

For the current alpha:

```bash
dotnet add package TheSingularityWorkshop.ProtocolAi --version 0.1.0-alpha.2
```

Or:

```xml
<PackageReference Include="TheSingularityWorkshop.ProtocolAi" Version="0.1.0-alpha.2" />
```

Then:

```csharp
using TheSingularityWorkshop.ProtocolAi;
```

You do **not** need an LLM SDK to use ProtocolAI.

You do **not** need GrammarAI to use ProtocolAI.

You do **not** need FSM_COS to use ProtocolAI.

Those are higher-level concerns.

---

## 2. Define the vocabulary your application owns

Start with something your application already understands.

For example, a tool that works with people might define:

```csharp
var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();
```

The numbers are application-owned identities.

Conceptually:

```text
Protocol [1001] People
    |
    +-- [2001] bobId  = "Bob"
    +-- [2002] janeId = "Jane"
    +-- [2003] saraId = "Sara"
```

ProtocolAI does not decide what Bob means in your application.

Your application does.

---

## 3. Encode values

Give ProtocolAI ordinary application values:

```csharp
var payload = people.Encode([
    "Bob",
    "Jane",
    "Sara"
]);
```

Known values become references to the symbols you defined.

Conceptually:

```text
"Bob"  -> [2001]
"Jane" -> [2002]
"Sara" -> [2003]
```

The payload also carries the protocol identity, so the symbol IDs are not pretending to be globally meaningful numbers.

---

## 4. Unknown values stay visible

This is one of the most important behaviors.

Suppose the application knows Bob and Jane, but the incoming value is Amelia:

```csharp
var payload = people.Encode([
    "Bob",
    "Amelia"
]);
```

Conceptually:

```text
[2001] "Amelia"
```

ProtocolAI does **not** invent an identity for Amelia.

The host decides what an unknown value means.

For example, the host could:

1. create a new domain object;
2. propose a new protocol symbol;
3. reject the value;
4. ask for clarification;
5. retain it as transient data.

This distinction matters when the input originated from an LLM. A model producing a plausible new name does not automatically receive authority to create application identity.

---

## 5. Validate before consuming received data

When data crosses an application boundary, validate it before acting on it:

```csharp
people.Validate(payload);
```

Validation checks the protocol scope and the validity of references.

That gives the host a clean boundary:

```text
external/model data
        |
        v
    validation
        |
   +----+----+
   |         |
 valid     invalid
   |         |
   v         v
resolve    reject
   |
   v
host policy
```

**Validation is not authorization.**

A valid ProtocolAI reference means the reference exists in the vocabulary. It does not mean the current user, model, or operation is allowed to use it.

Authorization remains a host responsibility.

---

## 6. Resolve mixed values

Once validated, resolve the payload:

```csharp
var values = people.Resolve(payload);
```

Known references resolve back through the vocabulary. Literals remain literals.

That means the application can distinguish:

```text
known identity
    Bob -> [2001] -> "Bob"

new literal
    Amelia -> "Amelia"

invalid identity
    [9999] -> validation failure
```

Those are deliberately different states.

---

## 7. Decode a single identity

When you already have a symbol ID:

```csharp
var value = people.Decode(2001);
```

The integer is an address into the vocabulary owned by your application.

It is not a globally meaningful “Bob number.”

The protocol ID supplies the namespace; the symbol ID supplies the address within that namespace.

---

## 8. Carry an explicit reference

When a boundary needs both protocol and symbol identity:

```csharp
var reference = people.Reference("bobId");

Console.WriteLine(reference.ProtocolId);
Console.WriteLine(reference.SymbolId);
```

Conceptually:

```text
protocol = [1001]
symbol   = [2001]
```

This is useful when another subsystem needs to carry the identity without taking ownership of the domain object.

---

## 9. Inspect a definition

ProtocolAI definitions can describe themselves:

```csharp
Console.WriteLine(people.Describe());
```

Conceptually:

```text
[1001] People
  [2001] bobId = "Bob"
  [2002] janeId = "Jane"
  [2003] saraId = "Sara"
```

Self-description is useful for:

- diagnostics;
- debugging;
- generated documentation;
- AI-facing context;
- future exchange formats;
- inspecting what a protocol actually contains.

The description is **information, not authority**. Reading a protocol description does not grant permission to mutate the application.

---

## 10. Put the LLM outside the boundary

ProtocolAI does not need to know which model you use.

A host can build an AI-facing request from the vocabulary, send it through whatever model integration the user chooses, then validate and resolve the response.

```text
YOUR APPLICATION
      |
      | authoritative vocabulary
      v
  ProtocolAI
      |
      | semantic representation
      v
   AI host
      |
      v
     LLM
      |
      v
 model response
      |
      v
   AI host
      |
      v
  ProtocolAI
      |
      | validate + resolve
      v
 host policy
      |
      v
application state
```

The provider can change.

The model can change.

The application vocabulary remains application-owned.

---

## 11. Clipboard and connected AI are the same semantic boundary

ProtocolAI can participate in either:

**Clipboard mode**

1. Generate a semantic exchange.
2. Copy it into the LLM interface of your choice.
3. Copy the response back.
4. Validate and resolve it.

**Connected mode**

1. Generate the same semantic exchange.
2. Send it through a provider adapter.
3. Receive the response.
4. Validate and resolve it.

The transport changes. The semantic boundary does not.

Provider adapters should own:

- credentials;
- endpoints;
- model selection;
- HTTP or other transport;
- provider-specific request/response formatting.

ProtocolAI should not.

See **[AI Exchange](AI_EXCHANGE.md)** for the larger exchange design.

---

## 12. A complete small example

```csharp
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

people.Validate(request);

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

var resolved = people.Resolve(request);
```

The important result is:

```text
"Bob"           -> known identity
"Jane"          -> known identity
"New Character" -> literal
```

The model may be probabilistic.

The vocabulary resolution performed by your application is not.

---

## What ProtocolAI gives you

- application-owned integer identities;
- protocol-qualified references;
- known-value encoding;
- literal preservation;
- deterministic validation;
- deterministic resolution;
- single-value decoding;
- self-description.

## What you still provide

- model client;
- prompting;
- transport;
- validation policy beyond protocol validity;
- authorization;
- creation/registration policy;
- persistence;
- execution semantics;
- provider-specific constrained-generation behavior.

That division of responsibility is intentional.

---

## Where to go next

If you want practical patterns, read **[Examples](EXAMPLES.md)**.

If you want to understand the architectural argument, read **[Theory](THEORY.md)**.

If you are designing a provider-neutral AI exchange, read **[AI Exchange](AI_EXCHANGE.md)**.

If you are integrating ProtocolAI into the Workshop ecosystem, read **[Ecosystem Integration](ECOSYSTEM_INTEGRATION.md)**.

**You should be able to use ProtocolAI without reading the theory. The theory exists to explain why the boundary is shaped this way.**
