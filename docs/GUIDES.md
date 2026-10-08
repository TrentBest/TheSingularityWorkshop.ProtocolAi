# ProtocolAI Guides

ProtocolAI is intentionally small. The documentation should not be.

The goal is simple: a reader should never have to guess what ProtocolAI means, why a type exists, or which responsibility belongs somewhere else.

## Start here

| Guide | Audience | Purpose |
|---|---|---|
| [Idiot's Guide](IDIOTS_GUIDE.md) | Anyone | Plain-English explanation of the problem and the idea. |
| [Beginner's Guide](BEGINNERS_GUIDE.md) | New developers | Build a protocol, encode values, validate, resolve, and transport. |
| [C# Developer's Guide](CSHARP_GUIDE.md) | C#/.NET developers | Idiomatic APIs, exceptions, testing, packaging, and integration. |
| [Developer's Guide](DEVELOPERS_GUIDE.md) | Library/application developers | Protocol design, compatibility, ownership, and architecture. |
| [AI Exchange Guide](AI_EXCHANGE.md) | AI integrators | How ProtocolAI participates in an AI exchange without becoming an AI framework. |
| [Examples](EXAMPLES.md) | Everyone | Concrete scenarios and progressively larger examples. |
| [Reflection](REFLECTION.md) | Advanced users | Reflection-based vocabulary discovery and boundaries. |
| [Theory](THEORY.md) | Architects/researchers | The deeper model behind deterministic semantic identity. |
| [Ecosystem Integration](ECOSYSTEM_INTEGRATION.md) | Workshop developers | ProtocolAI with GrammarAI, FSM_UserIO, AnyApp, and other Workshop packages. |
| [Consumption](CONSUMING.md) | Package consumers | API-oriented consumption reference. |

## Documentation rule

Every public API should answer five questions:

1. What is it?
2. Why does it exist?
3. When should I use it?
4. What does it deliberately not do?
5. What happens when input is unknown, invalid, or owned by another protocol?

The guides explain concepts first; XML documentation and API reference support implementation details.
