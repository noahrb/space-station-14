using Robust.Shared.GameStates;

namespace Content.Shared.DrinkCoaster;

/// <summary>
/// A writable drink coaster that can be flipped to hide or reveal its message.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DrinkCoasterComponent : Component
{
    /// <summary>
    /// Whether the branded side is face up, hiding any writing.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsFaceUp = true;
}

[Serializable, NetSerializable]
public enum DrinkCoasterVisuals : byte
{
    IsFaceUp,
}

[Serializable, NetSerializable]
public enum DrinkCoasterVisualLayers : byte
{
    Side,
}
