using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed partial class PdaRngUi : UIFragment
{
    private PdaRngUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface ui, EntityUid? fragmentOwner)
    {
        _fragment = new PdaRngUiFragment();
        _fragment.OnPrintPressed += () =>
        {
            var message = new CartridgeUiMessage(new PdaRngPrintMessage());
            ui.SendPredictedMessage(message);
        };
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not PdaRngUiState cast || _fragment == null)
            return;

        _fragment.UpdateState(cast.HasRolled);
    }
}
