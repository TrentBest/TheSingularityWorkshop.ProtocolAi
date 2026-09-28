using TheSingularityWorkshop.ProtocolAi;

var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();

Console.WriteLine("PROTOCOL");
Console.WriteLine(people.Describe());
Console.WriteLine();

var payload = people.Encode([
    "Bob",
    "Jane",
    "Amelia"
]);

Console.WriteLine("PAYLOAD");
Console.WriteLine(payload);
Console.WriteLine();

foreach (var value in payload.Values)
{
    Console.WriteLine(value.IsReference
        ? $"KNOWN  [{value.SymbolId}]"
        : $"NEW    \"{value.Literal}\"");
}

Console.WriteLine();
Console.WriteLine($"Bob resolves back to: {people.Decode(2001)}");
