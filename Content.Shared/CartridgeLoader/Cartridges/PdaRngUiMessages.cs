using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class PdaRngPrintMessage : CartridgeMessageEvent;

[Serializable, NetSerializable]
public sealed class PdaRngUiState : BoundUserInterfaceState
{
    public bool HasRolled { get; }

    public PdaRngUiState(bool hasRolled)
    {
        HasRolled = hasRolled;
    }
}
