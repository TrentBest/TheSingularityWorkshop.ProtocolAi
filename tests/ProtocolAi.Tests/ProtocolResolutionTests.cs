using Xunit;
using TheSingularityWorkshop.ProtocolAi;

namespace ProtocolAi.Tests;

public sealed class ProtocolResolutionTests
{
    private static ProtocolDefinition People() =>
        new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Define(2002, "janeId", "Jane")
            .Build();

    [Fact]
    public void Validate_accepts_own_mixed_payload()
    {
        var protocol = People();
        var payload = protocol.Encode(["Bob", "New Character"]);

        protocol.Validate(payload);
    }

    [Fact]
    public void Validate_rejects_payload_owned_by_another_protocol()
    {
        var protocol = People();
        var payload = new ProtocolPayload(9001, [ProtocolValue.FromId(2001)]);

        var error = Assert.Throws<ArgumentException>(() => protocol.Validate(payload));

        Assert.Contains("belongs to protocol", error.Message);
    }

    [Fact]
    public void Validate_rejects_reference_to_undefined_symbol()
    {
        var protocol = People();
        var payload = new ProtocolPayload(1001, [ProtocolValue.FromId(9999)]);

        var error = Assert.Throws<ArgumentException>(() => protocol.Validate(payload));

        Assert.Contains("undefined symbol ID", error.Message);
    }

    [Fact]
    public void Resolve_decodes_references_and_preserves_literals_in_order()
    {
        var protocol = People();
        var payload = protocol.Encode(["Bob", "New Character", "Jane"]);

        var resolved = protocol.Resolve(payload);

        Assert.Equal(["Bob", "New Character", "Jane"], resolved);
    }
}