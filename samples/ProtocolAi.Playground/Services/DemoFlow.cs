using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.ProtocolAi;

namespace ProtocolAi.Playground.Services;

/// <summary>
/// Drives the documentation experiment through FSM_API while exercising the real ProtocolAi library.
/// The FSM is a sample-only dependency; the ProtocolAi package itself remains dependency-free.
/// </summary>
public sealed class DemoFlow
{
    private const string DefinitionName = "ProtocolAiLivingDocumentation";
    private const string ProcessingGroup = "ProtocolAiLivingDocumentation";
    private readonly FlowContext _context;
    private readonly FSMHandle _handle;

    public DemoFlow()
    {
        var protocol = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Define(2002, "janeId", "Jane")
            .Define(2003, "saraId", "Sara")
            .Build();

        _context = new FlowContext(protocol);
        FSM_API.Create.CreateFiniteStateMachine(DefinitionName, processRate: 0, processingGroup: ProcessingGroup)
            .State("Ready", onEnter: ctx => Enter(ctx, "Ready"), onUpdate: null, onExit: Encode)
            .State("Encoded", onEnter: ctx => Enter(ctx, "Encoded"), onUpdate: null, onExit: Validate)
            .State("Validated", onEnter: ctx => Enter(ctx, "Validated"), onUpdate: null, onExit: ResolveOrReject)
            .State("Resolved", onEnter: ctx => Enter(ctx, "Resolved"), onUpdate: null, onExit: Decide)
            .State("HostDecision", onEnter: ctx => Enter(ctx, "HostDecision"), onUpdate: null, onExit: null)
            .State("Rejected", onEnter: ctx => Enter(ctx, "Rejected"), onUpdate: null, onExit: null)
            .WithInitialState("Ready")
            .Transition("Ready", "Encoded", ctx => AsFlow(ctx).AdvanceRequested)
            .Transition("Encoded", "Validated", ctx => AsFlow(ctx).AdvanceRequested)
            .Transition("Validated", "Rejected", ctx => AsFlow(ctx).AdvanceRequested && AsFlow(ctx).ValidationError is not null)
            .Transition("Validated", "Resolved", ctx => AsFlow(ctx).AdvanceRequested && AsFlow(ctx).ValidationError is null)
            .Transition("Resolved", "HostDecision", ctx => AsFlow(ctx).AdvanceRequested)
            .BuildDefinition();

        _handle = FSM_API.Create.CreateInstance(DefinitionName, _context, ProcessingGroup);
        _context.Stage = "Ready";
    }

    public ProtocolDefinition Protocol => _context.Protocol;
    public string CandidateValue { get => _context.CandidateValue; set => _context.CandidateValue = value; }
    public bool InjectInvalidReference { get => _context.InjectInvalidReference; set => _context.InjectInvalidReference = value; }
    public string Stage => _context.Stage;
    public ProtocolPayload? Payload => _context.Payload;
    public string? ValidationError => _context.ValidationError;
    public IReadOnlyList<string> ResolvedValues => _context.ResolvedValues;
    public string HostDecision => _context.HostDecision;
    public bool IsTerminal => Stage is "HostDecision" or "Rejected";
    public bool IsKnownValue => Protocol.Symbols.Any(symbol => StringComparer.Ordinal.Equals(symbol.Value, CandidateValue));

    public string EncodedValue
    {
        get
        {
            if (Payload is null || Payload.Values.Count == 0) return "—";
            var value = Payload.Values[0];
            return value.IsReference ? $"[{value.SymbolId}]" : $"LITERAL: {value.Literal}";
        }
    }

    public (double X, double Y) BallPosition => Stage switch
    {
        "Ready" => (92, 55),
        "Encoded" => (205, 165),
        "Validated" => (335, 70),
        "Resolved" => (490, 165),
        "HostDecision" when IsKnownValue => (670, 78),
        "HostDecision" => (670, 250),
        "Rejected" => (490, 250),
        _ => (92, 55)
    };

    public void Advance()
    {
        if (IsTerminal) return;
        _context.AdvanceRequested = true;
        _handle.Update(ProcessingGroup);
    }

    public void Reset()
    {
        // ForceTransition invokes the outgoing state's exit action. Reset the data after
        // the transition so that any final outgoing action cannot leave stale display data.
        _handle.TransitionTo("Ready");
        _context.CandidateValue = string.IsNullOrWhiteSpace(_context.CandidateValue) ? "Bob" : _context.CandidateValue.Trim();
        _context.Stage = "Ready";
        _context.Payload = null;
        _context.ValidationError = null;
        _context.ResolvedValues = Array.Empty<string>();
        _context.HostDecision = "The host has not made a decision.";
        _context.AdvanceRequested = false;
    }

    private static void Encode(IStateContext context)
    {
        var state = AsFlow(context);
        var candidate = string.IsNullOrWhiteSpace(state.CandidateValue) ? "Bob" : state.CandidateValue.Trim();
        state.CandidateValue = candidate;
        var encoded = state.Protocol.Encode([candidate]);
        state.Payload = state.InjectInvalidReference
            ? new ProtocolPayload(state.Protocol.Id, [ProtocolValue.FromId(9999)])
            : encoded;
        state.Stage = "Encoded";
        state.AdvanceRequested = false;
    }

    private static void Validate(IStateContext context)
    {
        var state = AsFlow(context);
        try
        {
            state.Protocol.Validate(state.Payload!);
            state.ValidationError = null;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            state.ValidationError = ex.Message;
        }
        state.Stage = "Validated";
        state.AdvanceRequested = false;
    }

    private static void ResolveOrReject(IStateContext context)
    {
        var state = AsFlow(context);
        if (state.ValidationError is null)
        {
            state.ResolvedValues = state.Protocol.Resolve(state.Payload!);
            state.Stage = "Resolved";
        }
        else
        {
            state.Stage = "Rejected";
        }
        state.AdvanceRequested = false;
    }

    private static void Decide(IStateContext context)
    {
        var state = AsFlow(context);
        state.HostDecision = state.ResolvedValues.Any(value =>
            !state.Protocol.Symbols.Any(symbol => StringComparer.Ordinal.Equals(symbol.Value, value)))
            ? "A literal remains. The host must decide whether to accept it, ask for clarification, or propose a new identity."
            : "The value resolved to a known identity. The host still decides whether any requested action is allowed.";
        state.Stage = "HostDecision";
        state.AdvanceRequested = false;
    }

    private static void Enter(IStateContext context, string stage)
    {
        var state = AsFlow(context);
        state.Stage = stage;
        state.AdvanceRequested = false;
    }

    private static FlowContext AsFlow(IStateContext context) => (FlowContext)context;

    private sealed class FlowContext(ProtocolDefinition protocol) : IStateContext
    {
        public ProtocolDefinition Protocol { get; } = protocol;
        public bool IsValid { get; set; } = true;
        public string Name { get; set; } = "ProtocolAi living documentation flow";
        public string CandidateValue { get; set; } = "Bob";
        public bool InjectInvalidReference { get; set; }
        public bool AdvanceRequested { get; set; }
        public string Stage { get; set; } = "Ready";
        public ProtocolPayload? Payload { get; set; }
        public string? ValidationError { get; set; }
        public IReadOnlyList<string> ResolvedValues { get; set; } = Array.Empty<string>();
        public string HostDecision { get; set; } = "The host has not made a decision.";
    }
}
