<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# ProtocolAi Mathematical Foundations

## Visual companion

![Animated protocol lifecycle from an application-owned vocabulary to known, literal, and invalid outcomes.](images/protocol-ai-domain-lifecycle.svg)

The diagram is an intuition aid. The definitions, assumptions, and proof obligations below are the formal statement; the future-registration path is not part of the current API.

## 1. The claim we can prove

ProtocolAi does not make a language model deterministic, and it cannot prove that a model's answer matches a user's intent. Its narrower claim is both useful and formalizable:

> Given an application-owned finite vocabulary with unique integer identities, ProtocolAi can turn recognized values into stable, protocol-qualified addresses, preserve unrecognized values as literals, and let a host reject undefined references before they become application identity.

The model proposes a candidate. The application owns the address space. The host owns policy and execution.

## 2. Define the address space

Let a protocol be a pair

\[
P = (p, V)
\]

where:

- \(p\) is a nonzero protocol identifier;
- \(V\) is a finite vocabulary of application-defined entries;
- each entry \(v \in V\) has a semantic value \(m(v)\) and an integer symbol identifier \(i(v)\).

Require symbol identifiers to be unique within the protocol:

\[
v_1 \ne v_2 \implies i(v_1) \ne i(v_2).
\]

The address of an entry is the qualified pair

\[
a(v) = (p, i(v)).
\]

The protocol identifier matters: symbol ID 2001 in protocol 1001 is not automatically the same address as symbol ID 2001 in protocol 2002.

This is a *local address space*, not a universal dictionary. The integer is an address; the application-defined entry supplies its meaning.

## 3. Deterministic encoding

Let \(x\) be an input value. Define the vocabulary lookup:

\[
E_P(x) =
\begin{cases}
\operatorname{Ref}(p, i(v)), & \text{if } x = m(v) \text{ for an entry } v \in V,\\
\operatorname{Literal}(x), & \text{if no entry matches } x.
\end{cases}
\]

This definition assumes that the vocabulary's matching rule is specified and unambiguous. If matching is case-sensitive, for example, `Bob` and `bob` may be different values; if normalization is used, that normalization must be explicit and stable.

**Theorem 1 — Encoding determinism.** For a fixed protocol definition \(P\), fixed matching rule, and fixed input \(x\), \(E_P(x)\) has exactly one result, provided the vocabulary contains no conflicting matches.

**Proof.** If exactly one vocabulary entry matches \(x\), the definition selects its unique symbol ID and returns its qualified reference. If no entry matches, the definition returns a literal. The no-conflicting-matches condition excludes the case in which two entries would both claim the same input under the matching rule. These cases are mutually exclusive and cover all inputs. Therefore the result is unique. \(\square\)

This is a guarantee about the application-side mapping—not about which input the model chooses to emit.

## 4. Validation and resolution

Let \(I_P = \{i(v) : v \in V\}\) be the set of symbol IDs defined by protocol \(P\).

A reference \(\operatorname{Ref}(q,j)\) is valid for \(P\) exactly when

\[
q = p \quad \land \quad j \in I_P.
\]

**Theorem 2 — Reference safety.** A reference that passes this validation names an entry in the expected protocol definition. A reference with a different protocol ID or an undefined symbol ID does not pass.

**Proof.** The validation predicate is the conjunction of protocol equality and membership in the defined ID set. If both hold, \(j=i(v)\) for an entry in \(V\), and the qualified reference addresses that entry. If either fails, the predicate is false. \(\square\)

For every accepted reference, resolution uses the same immutable definition:

\[
R_P(\operatorname{Ref}(p,i(v))) = m(v).
\]

Therefore, for a recognized value and unchanged definition, encoding followed by resolution returns the value associated with that symbol. Literals remain literals; they are not silently promoted to known identities.

**Important limitation:** this is not authentication, authorization, or proof of intent. A malicious or mistaken sender can still choose a *different but valid* symbol. The host must check whether the requested operation is allowed in the current context.

## 5. From identities to an executable program

ProtocolAi provides the semantic primitives; it does not itself define a complete program or execute commands. A higher layer such as GrammarAi can define legal arrangements of those primitives. A host can then validate and interpret the resulting structure.

Model that pipeline as:

\[
\text{LLM output}
\xrightarrow{\text{parse}}
\text{candidate structure}
\xrightarrow{\text{grammar validation}}
\text{well-formed program}
\xrightarrow{\text{ProtocolAi validation}}
\text{defined identities}
\xrightarrow{\text{host policy}}
\text{authorized plan}
\xrightarrow{\text{interpreter}}
\text{effects}.
\]

Each arrow establishes a different property:

1. **Parsing** establishes that the response can be read in the agreed representation.
2. **Grammar validation** establishes that the pieces are arranged in an allowed form.
3. **Protocol validation** establishes that referenced identities belong to the expected vocabulary.
4. **Host policy** establishes that the requested actions are permitted for this actor and context.
5. **Interpretation** performs only the operations explicitly implemented by the host.

**Theorem 3 — Conditional execution boundary.** If the host executes a candidate only after all required parsing, grammar, identity, and authorization checks succeed, then malformed structures, undefined references, and unauthorized actions are rejected before the interpreter performs their requested effects.

**Proof sketch.** The host's execution gate requires each check to return success. Any candidate failing a required check cannot reach the interpreter. The conclusion depends on the gate being correctly implemented, on checks covering all relevant effects, and on there being no bypass path. It does not prove that an authorized program is harmless or that its result matches user intent. \(\square\)

This is the key distinction behind the larger idea: the LLM may *propose* an integral, address-based program; the ecosystem can decide whether that program is meaningful, well-formed, authorized, and executable. Integer IDs alone do not make arbitrary model output safe or executable.

## 6. What consistency means—and does not mean

For an unchanged definition, an address is stable:

\[
(p,i) \mapsto m
\]

as long as the protocol owner preserves the meaning associated with that address. Reassigning an existing ID to a different meaning breaks that invariant, even if the code still compiles. Versioning and vocabulary evolution therefore need explicit policy.

The model output itself is not guaranteed to be stable:

\[
\text{same prompt} \centernot\implies \text{same model response}.
\]

What the application controls is the interpretation boundary:

- recognized values map to application-owned references;
- unknown values remain visible as literals for host policy;
- undefined references fail validation;
- only a separate host policy and interpreter can authorize and perform effects.

This can reduce ambiguity *after mapping*. It does not establish that the LLM will always emit the correct ID, follow a supplied vocabulary, or preserve the user's intent.

## 7. Information and compression: no free theorem

Replacing a known string with an integer can make a payload more compact in some encodings, but this is not guaranteed by the mathematics above. The total cost includes protocol context, vocabulary delivery, serialization overhead, tokenization, and any explanatory text needed by the model. Token savings and reliability improvements must be measured with representative models and tasks.

Nor does an integer alone carry its meaning. The receiving side must have access to a compatible definition or another explicit resolution mechanism.

## 8. A testable experiment

A fair evaluation should compare a baseline string workflow with a ProtocolAi-based workflow over the same task set and model settings. Measure separately:

- exact semantic match to the intended action;
- unknown-value rate;
- invalid-reference and malformed-program rejection rates;
- unauthorized-action rejection rate;
- correct task completion after host validation;
- prompt and response token counts;
- latency and end-to-end cost.

Use fixed test cases, repeated trials where model variation matters, and report denominators and confidence intervals. Keep the model-generated candidate, validator decisions, and final effects as separate recorded outcomes.

Do not describe an architectural guarantee as a measured reduction in hallucinations until the experiment supports that conclusion.

## 9. Summary

\[
\boxed{\text{Probability proposes; owned identity addresses; grammar structures; policy authorizes; an interpreter executes.}}
\]

ProtocolAi's mathematical contribution is the deterministic address-and-validation boundary. The larger executable-program concept emerges only when that boundary is composed with grammar, host policy, and an explicitly bounded interpreter.

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
