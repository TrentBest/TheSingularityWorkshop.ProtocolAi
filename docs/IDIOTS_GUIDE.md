# The Idiot's Guide to ProtocolAI

You do not need to know AI, machine learning, protocols, or compiler theory to understand ProtocolAI.

## The problem

Software needs to know what a word means, while an AI model usually communicates in words.

Suppose your application knows three people:

- Bob
- Jane
- Sara

Your program can assign stable identities:

- 2001 = Bob
- 2002 = Jane
- 2003 = Sara

Now the application can exchange human-readable words while retaining deterministic machine identity.

## What ProtocolAI does

ProtocolAI gives your application a vocabulary:

```text
[1001] People
  [2001] bobId = "Bob"
  [2002] janeId = "Jane"
```

It can encode:

```text
Bob
New Character
Jane
```

as a mixture of:

```text
[2001]
"New Character"
[2002]
```

Known concepts become references. Unknown concepts remain literals.

## What ProtocolAI does NOT do

ProtocolAI does not:

- call an AI provider;
- choose a model;
- authenticate with a provider;
- generate prompts;
- decide what your application should do;
- turn an AI model into a deterministic system;
- replace GrammarAI;
- replace your application's business rules.

**The model proposes. ProtocolAI identifies. Your application decides.**

## Why integers?

Strings are useful for people. Integer IDs are useful for software.

A stable ID lets multiple parts of a system say, "I mean symbol 2001 in protocol 1001," without requiring every participant to reinterpret the original text.

## Known, unknown, invalid

**Known:** the vocabulary contains the value and it can become a reference.

**Unknown:** the vocabulary does not contain the value, so it remains a literal.

**Invalid:** a payload claims to belong to one protocol but references a symbol that protocol does not define.

ProtocolAI deliberately keeps those states separate.

## The simplest mental model

Think of ProtocolAI as a dictionary with addresses.

The dictionary says what a symbol means.

The address says which dictionary and which entry.

The payload carries those addresses, plus any words the dictionary does not know.

That is ProtocolAI.
