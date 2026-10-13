using ProtocolAi.Playground.Services;
using Xunit;

namespace ProtocolAi.Playground.Tests;

public sealed class DemoFlowTests
{
    [Fact]
    public void Known_value_moves_through_encode_validate_resolve_and_host_policy()
    {
        var flow = new DemoFlow();

        flow.Advance();
        Assert.Equal("Encoded", flow.Stage);
        Assert.Equal("[2001]", flow.EncodedValue);

        flow.Advance();
        Assert.Equal("Validated", flow.Stage);
        Assert.Null(flow.ValidationError);

        flow.Advance();
        Assert.Equal("Resolved", flow.Stage);
        Assert.Equal(["Bob"], flow.ResolvedValues);

        flow.Advance();
        Assert.Equal("HostDecision", flow.Stage);
        Assert.True(flow.IsTerminal);
        Assert.Contains("host still decides", flow.HostDecision, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Unknown_value_stays_a_literal_and_reaches_host_policy()
    {
        var flow = new DemoFlow { CandidateValue = "Amelia" };

        flow.Advance();
        Assert.Equal("LITERAL: Amelia", flow.EncodedValue);

        flow.Advance();
        Assert.Null(flow.ValidationError);

        flow.Advance();
        Assert.Equal(["Amelia"], flow.ResolvedValues);

        flow.Advance();
        Assert.Equal("HostDecision", flow.Stage);
        Assert.Contains("literal remains", flow.HostDecision, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Undefined_reference_is_rejected_instead_of_becoming_a_literal()
    {
        var flow = new DemoFlow { InjectInvalidReference = true };

        flow.Advance();
        Assert.Equal("[9999]", flow.EncodedValue);

        flow.Advance();
        Assert.NotNull(flow.ValidationError);

        flow.Advance();
        Assert.Equal("Rejected", flow.Stage);
        Assert.True(flow.IsTerminal);
        Assert.Empty(flow.ResolvedValues);
    }

    [Fact]
    public void Reset_returns_the_live_fsm_to_ready_and_clears_the_payload()
    {
        var flow = new DemoFlow();
        flow.Advance();
        Assert.NotNull(flow.Payload);

        flow.Reset();

        Assert.Equal("Ready", flow.Stage);
        Assert.Null(flow.Payload);
        Assert.False(flow.IsTerminal);

        flow.Advance();
        Assert.Equal("Encoded", flow.Stage);
        Assert.Equal("[2001]", flow.EncodedValue);
    }
}
