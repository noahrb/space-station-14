using Content.Shared.Paper;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(PdaRngCartridgeSystem))]
public sealed partial class PdaRngCartridgeComponent : Component
{
    /// <summary>
    /// Whether this PDA has already printed its one RNG ticket this round.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool HasRolled;

    /// <summary>
    /// TEMP testing counter so unlimited rolls get unique predicted seeds.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int DebugRollCount;

    /// <summary>
    /// Paper entity spawned for the ticket.
    /// </summary>
    [DataField]
    public EntProtoId PaperPrototype = "Paper";

    [DataField]
    public SoundSpecifier PrintSound = new SoundPathSpecifier("/Audio/Machines/diagnoser_printing.ogg");
}
