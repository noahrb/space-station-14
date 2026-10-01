using Content.Shared.Examine;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared.DrinkCoaster;

/// <summary>
/// Handles flipping drink coasters and hiding their writing while face up.
/// </summary>
public sealed class SharedDrinkCoasterSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DrinkCoasterComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<DrinkCoasterComponent, GetVerbsEvent<Verb>>(OnGetVerbs);
        SubscribeLocalEvent<DrinkCoasterComponent, ActivatableUIOpenAttemptEvent>(OnUIOpenAttempt);
        SubscribeLocalEvent<DrinkCoasterComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<DrinkCoasterComponent, PaperWriteAttemptEvent>(OnPaperWriteAttempt);
    }

    private void OnInit(Entity<DrinkCoasterComponent> ent, ref ComponentInit args)
    {
        UpdateAppearance(ent);
    }

    private void OnGetVerbs(Entity<DrinkCoasterComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract)
            return;

        var text = ent.Comp.IsFaceUp
            ? "drink-coaster-verb-flip-down"
            : "drink-coaster-verb-flip-up";

        args.Verbs.Add(new Verb
        {
            Act = () => Flip(ent),
            Text = Loc.GetString(text),
            Category = VerbCategory.Rotate,
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/flip.svg.192dpi.png")),
            DoContactInteraction = true,
        });
    }

    public void Flip(Entity<DrinkCoasterComponent> ent)
    {
        ent.Comp.IsFaceUp = !ent.Comp.IsFaceUp;
        Dirty(ent);
        UpdateAppearance(ent);
    }

    private void UpdateAppearance(Entity<DrinkCoasterComponent> ent)
    {
        if (!TryComp<AppearanceComponent>(ent, out var appearance))
            return;

        _appearance.SetData(ent, DrinkCoasterVisuals.IsFaceUp, ent.Comp.IsFaceUp, appearance);
    }

    private void OnUIOpenAttempt(Entity<DrinkCoasterComponent> ent, ref ActivatableUIOpenAttemptEvent args)
    {
        if (!ent.Comp.IsFaceUp)
            return;

        if (!args.Silent)
            _popup.PopupClient(Loc.GetString("drink-coaster-flip-to-read"), ent, args.User);

        args.Cancel();
    }

    private void OnExamined(Entity<DrinkCoasterComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange || !ent.Comp.IsFaceUp)
            return;

        args.PushMarkup(Loc.GetString("drink-coaster-examine-face-up"));
    }

    private void OnPaperWriteAttempt(Entity<DrinkCoasterComponent> ent, ref PaperWriteAttemptEvent args)
    {
        if (!ent.Comp.IsFaceUp)
            return;

        args.FailReason = "drink-coaster-flip-to-write";
        args.Cancelled = true;
    }
}
