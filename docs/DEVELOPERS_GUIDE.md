# ProtocolAI Developer's Guide

ProtocolAI separates **meaning**, **transport**, and **behavior**.

## The three boundaries

**ProtocolDefinition — meaning**

Defines the vocabulary owned by an application or domain.

**ProtocolPayload / ProtocolPayloadJson — transport**

Carries references and literals between boundaries.

**Application — behavior**

Decides what a resolved symbol actually causes the application to do.

This separation prevents ProtocolAI from becoming an AI framework or hidden application runtime.

## Compatibility

A protocol ID identifies the vocabulary namespace. Symbol IDs identify entries inside that namespace.

Changing a symbol's meaning without changing its identity is a semantic breaking change even if the C# API still compiles.

Prefer adding new symbols over silently redefining old ones.

## Unknown values are useful

A literal is not automatically a failure. AI systems, users, and other applications can introduce concepts the current vocabulary does not know.

The application can choose to reject the literal, display it, request clarification, map it to a new symbol, or pass it to another semantic layer.

ProtocolAI does not make that policy decision.

## ProtocolAI and GrammarAI

ProtocolAI answers:

> **WHAT does this identifier mean?**

GrammarAI answers:

> **HOW may those identifiers be structurally arranged?**

Keeping those responsibilities separate lets each package evolve without becoming a monolith.

## ProtocolAI and FSM_UserIO

FSM_UserIO can carry semantic intent through an application. ProtocolAI can provide stable identities for concepts referenced by that intent.

The two are complementary, not interchangeable.

## ProtocolAI and AI models

The model is one possible participant in the exchange.

A model may propose a human-readable value. ProtocolAI can resolve that value against application-owned vocabulary and preserve unknown values as literals.

The model does not become the authority for application meaning.

## Why this matters

A conventional C# application can add AI-mediated semantic interaction without moving its core domain into a Python service or binding its meaning to a particular AI provider.
