# ProtocolAI Examples

These examples focus on the practical consumption pattern: **define what your application already knows, encode references, preserve unknown literals, and resolve identities deterministically.**

## Example 1 — A command vocabulary

A tool can define commands as a local protocol:

`csharp
var commands = new ProtocolBuilder(7100, "WorkshopCommands")
    .Define(7101, "inspect", "inspect")
    .Define(7102, "move", "move")
    .Define(7103, "create", "create")
    .Define(7104, "delete", "delete")
    .Build();

var request = commands.Encode([
    "inspect",
    "move"
]);
`

Conceptually:

`text
[7101] [7102]
`

The command meaning remains owned by the tool.

## Example 2 — Existing objects

Suppose a host knows the IDs of objects in its own domain:

`csharp
var objects = new ProtocolBuilder(7200, "WorkshopObjects")
    .Define(7201, "forge", "Forge")
    .Define(7202, "laboratory", "Research Laboratory")
    .Build();

var request = objects.Encode([
    "Forge",
    "Research Laboratory"
]);
`

The model-facing representation can refer to known objects by identity:

`text
[7201] [7202]
`

ProtocolAI does not execute either object. It only gives the host a compact, explicit representation of the vocabulary.

## Example 3 — Creation is different from reference

Consider:

`csharp
var request = objects.Encode([
    "Forge",
    "Avengers Landing Pad"
]);
`

The result conceptually separates:

The payload also carries its owning protocol ID, so the integer references are not presented as globally meaningful numbers.

`text
existing object
    Forge
      |
      v
   [7201]

new value
    Avengers Landing Pad
      |
      v
   "Avengers Landing Pad"
`

The host can now distinguish an existing identity from a candidate new value.

The alpha does not decide how a new object receives its permanent identity. That belongs to the host or a future registration layer.

## Example 4 — Self-description

A tool can expose its protocol definition:

`csharp
Console.WriteLine(objects.Describe());
`

Output:

`text
[7200] WorkshopObjects
  [7201] forge = "Forge"
  [7202] laboratory = "Research Laboratory"
`

This makes the vocabulary inspectable without requiring the consumer to reconstruct it from source code.

## Example 5 — Explicit references

The protocol identity and symbol identity can be carried together:

`csharp
var forge = objects.Reference("forge");

Console.WriteLine($"Protocol: {forge.ProtocolId}");
Console.WriteLine($"Symbol:   {forge.SymbolId}");
`

Conceptually:

`text
[7200:7201]
`

This form is useful at boundaries where another subsystem needs to refer to the owned symbol without taking ownership of its meaning.

## Example 6 — The full WHAT boundary

A useful mental model for an AI-facing tool is:

`text
              DOMAIN
                |
        "Forge", "Laboratory"
                |
                v
        +---------------+
        |  ProtocolAI   |
        |     WHAT      |
        +-------+-------+
                |
         [7201] [7202]
                |
                v
          model-facing
         representation
                |
                v
               LLM
                |
                v
          returned data
                |
                v
        +---------------+
        |  ProtocolAI   |
        |    resolve    |
        +-------+-------+
                |
                v
        deterministic host
`

The package is most useful when the application already has authoritative meaning and wants an explicit identity layer around it.

## Example 7 — Pairing with GrammarAI

Once a vocabulary exists, GrammarAI can refer to its symbols:

`text
ProtocolAI

[1001] People
[2001] Bob
[2002] Jane

        |
        | externally owned references
        v

GrammarAI

[4001] Greeting
  [5001] [4001] -> [1001:2001]
  [5002] [4001] -> [1001:2002]
`

ProtocolAI still owns Bob and Jane.

GrammarAI owns the relationships.

That is the intended separation between the two packages.
