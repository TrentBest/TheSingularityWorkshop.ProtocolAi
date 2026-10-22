<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# The Idiot's Guide to ProtocolAI

You do not need to know AI, machine learning, protocols, or compiler theory to understand this.

If you understand the idea of a **name** and an **address**, you understand most of ProtocolAI.

## Visual companion

![Animated comparison of known values, unknown literals, and invalid references.](images/protocol-ai-three-outcomes.svg)

**Watch for the distinction:** a known value has an owned identity, an unknown value remains literal data, and an invalid reference fails validation. The animation illustrates the rule; the library remains the source of truth.

## The problem

People communicate with words.

Software often needs stable identity.

Suppose your application knows:

@@@
Bob
Jane
Sara
@@@

The application can give those concepts addresses:

@@@
2001 = Bob
2002 = Jane
2003 = Sara
@@@

Now the application can accept human-readable language while retaining a deterministic identity underneath.

## What ProtocolAI adds

A ProtocolAI vocabulary gives those identities a namespace:

@@@
protocol 1001 = People

symbol 2001 = Bob
symbol 2002 = Jane
symbol 2003 = Sara
@@@

The namespace matters because 2001 in one vocabulary does not automatically mean the same thing as 2001 in another.

## Known values become references

If the application knows Bob:

@@@
"Bob" -> [1001 : 2001]
@@@

The important part is not the number.

The important part is:

> **The application already knows what this identity means.**

ProtocolAI gives that meaning an address.

## Unknown values stay visible

Suppose someone says:

@@@
"Amelia"
@@@

but the vocabulary does not contain Amelia.

ProtocolAI does not secretly invent a permanent identity.

It can preserve Amelia as a literal:

@@@
[2001] "Amelia"
@@@

The host can decide what to do:

- create something;
- ask a question;
- register a new identity;
- reject it;
- keep it as temporary information.

That is a feature.

## Invalid is different

If someone sends:

@@@
[9999]
@@@

and the expected vocabulary has no symbol 9999, that is not merely “unknown.”

It is an invalid reference.

So there are three useful states:

@@@
KNOWN
  -> deterministic identity

UNKNOWN
  -> literal
  -> host decides

INVALID
  -> validation failure
@@@

ProtocolAI keeps those states separate.

## Why this matters for AI

A language model can propose:

> “Bob”

That does not mean the model owns Bob.

The application can ask:

> “Does my vocabulary contain Bob?”

If yes, the application resolves the known identity.

If no, the value remains a literal.

This creates a clean boundary:

@@@
The model proposes.
       |
       v
ProtocolAI identifies.
       |
       v
The host decides.
@@@

## JSON does not change the idea

Alpha 3 lets a ProtocolPayload travel as ordinary JSON:

@@@json
{"protocolId":1001,"values":[{"symbolId":2001},{"literal":"Amelia"}]}
@@@

JSON is the suitcase.

ProtocolAI is the address system.

Your application still owns the meaning.

## What ProtocolAI does NOT do

ProtocolAI does not:

- call an AI provider;
- choose a model;
- store API keys;
- generate prompts;
- execute commands;
- authorize users;
- become a database;
- replace GrammarAI;
- run your application.

It is deliberately smaller than an AI framework.

## The simplest mental model

Think of ProtocolAI as:

> **a dictionary with addresses.**

The dictionary says what something means.

The address says where that meaning lives.

The payload carries the address.

The host decides what the meaning is allowed to do.

That's ProtocolAI.

## If you remember only one sentence

> **The model proposes. ProtocolAI identifies. Your application decides.**

For the next step, read the [Beginner's Guide](BEGINNERS_GUIDE.md).

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
  </a>
</p>

<p align="center">
  <a href="https://github.com/TrentBest">GitHub</a> ·
  <a href="https://coderlegion.com/">Coder Legion</a> ·
  <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a> ·
  <a href="https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J">PayPal</a>
  <br>
  <a href="https://github.com/TrentBest/FSM_API">FSM_API</a> ·
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization">FSM_Serialization</a> ·
  <a href="https://github.com/TrentBest/WebPage">WebPage</a>
</p>

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
