# ProtocolAI Guides

> **ProtocolAI is small enough to learn in an afternoon. The documentation is designed to make the ideas impossible to mystify.**

The library answers one narrow question:

> **How can application-owned meaning become deterministic, addressable identity at a boundary where language may be probabilistic?**

These guides approach that question from different distances. They are not duplicates.

## Choose your path

| Guide | You are... | Start here because... |
|---|---|---|
| [Idiot's Guide](IDIOTS_GUIDE.md) | Curious, non-technical, or skeptical | It explains the idea without assuming AI, protocol, or compiler knowledge. |
| [Beginner's Guide](BEGINNERS_GUIDE.md) | New to ProtocolAI | It gets you from package install to a working vocabulary. |
| [C# Developer's Guide](CSHARP_GUIDE.md) | A .NET/C# developer | It explains API contracts, errors, tests, and integration boundaries. |
| [Developer's Guide](DEVELOPERS_GUIDE.md) | Designing a system | It covers identity, compatibility, ownership, lifecycle, and architecture. |
| [AI Exchange](AI_EXCHANGE.md) | Connecting an AI participant | It explains clipboard and connected exchange without making ProtocolAI an AI framework. |
| [Examples](EXAMPLES.md) | Learning by doing | It shows small patterns before larger architectural examples. |
| [Theory](THEORY.md) | Wanting the “why” | It develops the semantic and deprobabilization argument. |
| [Reflection](REFLECTION.md) | Thinking about what comes next | It records limits, lifecycle, and unanswered questions. |
| [Ecosystem Integration](ECOSYSTEM_INTEGRATION.md) | Working in the Workshop ecosystem | It explains what ProtocolAI owns and what neighboring packages own. |
| [Consuming](CONSUMING.md) | Shipping an application | It is the practical consumer reference. |
| [Alpha 3 JSON](PROTOCOL_PAYLOAD_JSON.md) | Crossing a process/system boundary | It specifies the provider-neutral payload representation. |

## The learning ladder

```
plain language
     |
     v
Idiot's Guide
     |
     v
Beginner's Guide
     |
     v
C# Developer's Guide
     |
     v
Developer's Guide
     |
     +--------------------+
     |                    |
     v                    v
AI Exchange            Theory
     |                    |
     +---------+----------+
               |
               v
       Ecosystem Integration
               |
               v
          future design
```

You should be able to stop at the level you need.

You should **not** have to read the theory to use the package.

## Documentation standard

Every public API and major concept should eventually answer:

1. **What is it?**
2. **Why does it exist?**
3. **When should I use it?**
4. **What does it deliberately not do?**
5. **What happens when input is known, unknown, invalid, or owned elsewhere?**

The guides distinguish four layers that are easy to confuse:

```
MEANING
ProtocolDefinition
     |
     v
IDENTITY
ProtocolReference / ProtocolValue
     |
     v
TRANSPORT
ProtocolPayload / ProtocolPayloadJson
     |
     v
BEHAVIOR
host/application policy
```

Keeping those layers separate is one of the most important ProtocolAI lessons.

## The Workshop promise

The goal is not to document only the happy path.

A good ProtocolAI document should explain:

- the normal path;
- the boundary cases;
- the failure modes;
- the ownership boundary;
- the reason the design exists;
- the things the package intentionally refuses to do.

**Edify, don't mystify.**