using Content.Shared.DrinkCoaster;
using Content.Shared.Paper;
using Robust.Client.GameObjects;

using static Content.Shared.Paper.PaperComponent;

namespace Content.Client.DrinkCoaster;

public sealed class DrinkCoasterVisualizerSystem : VisualizerSystem<DrinkCoasterComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, DrinkCoasterComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var faceUp = true;
        if (AppearanceSystem.TryGetData<bool>(uid, DrinkCoasterVisuals.IsFaceUp, out var isFaceUp, args.Component))
            faceUp = isFaceUp;

        SpriteSystem.LayerSetVisible((uid, args.Sprite), DrinkCoasterVisualLayers.Side, !faceUp);

        var showWriting = !faceUp
            && AppearanceSystem.TryGetData<PaperStatus>(uid, PaperVisuals.Status, out var status, args.Component)
            && status == PaperStatus.Written;

        SpriteSystem.LayerSetVisible((uid, args.Sprite), PaperVisualLayers.Writing, showWriting);
    }
}
