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

For a real consumer, check the [NuGet package page](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi) for the latest published version and follow its install instructions. The repository's version can be ahead of the latest published package while a release is being prepared.
