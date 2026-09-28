# ProtocolAI QuickStart

This is the shortest executable demonstration of the current ProtocolAI alpha.

From the repository root:

```bash
dotnet run --project examples/ProtocolAi.QuickStart/ProtocolAi.QuickStart.csproj
```

You will see:

1. a self-describing protocol vocabulary;
2. known values resolving to integer identities;
3. an unknown value remaining a literal;
4. an integer identity decoding back to its domain value.

The example uses a project reference so it exercises the repository source directly.

For a real consumer, install the package instead:

```bash
dotnet add package TheSingularityWorkshop.ProtocolAi --version 0.1.0-alpha.1
```

Then the same `ProtocolBuilder` / `Encode` / `Decode` flow works against the NuGet package.
