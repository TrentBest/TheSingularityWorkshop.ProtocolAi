using TheSingularityWorkshop.FSM_API;

namespace ProtocolAi.Playground.Services;

/// <summary>
/// The smallest visible FSM_API experiment: a two-state cycle that drives an SVG view.
/// This belongs to the playground; the ProtocolAi core package does not depend on FSM_API.
/// </summary>
public sealed class HelloWorldFlow
{
    private const string DefinitionName = "ProtocolAi.Playground.HelloWorld";
    private const string ProcessingGroup = "ProtocolAi.Playground.HelloWorld";
    private readonly HelloContext _context = new();
    private readonly FSMHandle _handle;

    public HelloWorldFlow()
    {
        FSM_API.Create.CreateFiniteStateMachine(DefinitionName, processRate: 0, processingGroup: ProcessingGroup)
            .State("Hello", onEnter: ctx => Enter(ctx, "Hello"), onUpdate: null, onExit: null)
            .State("Moving", onEnter: ctx => Enter(ctx, "Moving"), onUpdate: null, onExit: null)
            .WithInitialState("Hello")
            .Transition("Hello", "Moving", ctx => AsHello(ctx).AdvanceRequested)
            .Transition("Moving", "Hello", ctx => AsHello(ctx).AdvanceRequested)
            .BuildDefinition();

        _handle = FSM_API.Create.CreateInstance(DefinitionName, _context, ProcessingGroup);
        _context.Stage = "Hello";
    }

    public string Stage => _context.Stage;
    public double CircleX => Stage == "Moving" ? 255 : 65;
    public string Message => Stage == "Moving"
        ? "FSM_API changed the state: Moving"
        : "Hello, Workshop!";
    public string Instruction => Stage == "Moving"
        ? "Advance again to return to Hello."
        : "Advance the FSM to move the geometry.";

    public void Advance()
    {
        _context.AdvanceRequested = true;
        _handle.Update(ProcessingGroup);
    }

    public void Reset()
    {
        _handle.TransitionTo("Hello");
        _context.Stage = "Hello";
        _context.AdvanceRequested = false;
    }

    private static void Enter(IStateContext context, string stage)
    {
        var state = AsHello(context);
        state.Stage = stage;
        state.AdvanceRequested = false;
    }

    private static HelloContext AsHello(IStateContext context) => (HelloContext)context;

    private sealed class HelloContext : IStateContext
    {
        public bool IsValid { get; set; } = true;
        public string Name { get; set; } = "SVG Hello World";
        public string Stage { get; set; } = "Hello";
        public bool AdvanceRequested { get; set; }
    }
}
