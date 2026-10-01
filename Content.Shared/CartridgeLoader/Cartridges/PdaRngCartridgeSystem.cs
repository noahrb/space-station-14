using Content.Shared.Administration.Logs;
using Content.Shared.CartridgeLoader.Cartridges.PdaRng;
using Content.Shared.Database;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.CartridgeLoader.Cartridges;

public sealed partial class PdaRngCartridgeSystem : EntitySystem
{
    // TEMP: unlimited rolls for local testing — set false / remove before merge.
    private static readonly bool UnlimitedRolls = true;

    [Dependency] private CartridgeLoaderSystem _cartridge = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private LabelSystem _label = default!;
    [Dependency] private PaperSystem _paper = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PdaRngCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
        SubscribeLocalEvent<PdaRngCartridgeComponent, CartridgeMessageEvent>(OnMessage);
    }

    private void OnUiReady(Entity<PdaRngCartridgeComponent> ent, ref CartridgeUiReadyEvent args)
    {
        UpdateUiState(ent, args.Loader);
    }

    private void OnMessage(Entity<PdaRngCartridgeComponent> ent, ref CartridgeMessageEvent args)
    {
        if (args is not PdaRngPrintMessage)
            return;

        TryPrintTicket(ent, args.User);
        UpdateUiState(ent, GetEntity(args.LoaderUid));
    }

    private void TryPrintTicket(Entity<PdaRngCartridgeComponent> ent, EntityUid user)
    {
        if (!UnlimitedRolls && ent.Comp.HasRolled)
        {
            _popup.PopupClient(Loc.GetString("pda-rng-already-rolled"), user, user);
            return;
        }

        if (!UnlimitedRolls)
        {
            // Consume the roll immediately so prediction cannot double-print.
            ent.Comp.HasRolled = true;
            Dirty(ent);
        }
        else
        {
            // Keep consecutive rolls unique even within the same tick.
            ent.Comp.DebugRollCount++;
            Dirty(ent);
        }

        IRobustRandom random;
        if (UnlimitedRolls)
        {
            var seed = HashCode.Combine(
                (int)_timing.CurTick.Value,
                GetNetEntity(ent.Owner).Id,
                GetNetEntity(user).Id,
                ent.Comp.DebugRollCount);
            var predicted = new RobustRandom();
            predicted.SetSeed(seed);
            random = predicted;
        }
        else
        {
            random = SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent.Owner), GetNetEntity(user));
        }

        var number = random.Next(PdaRngAnalyzer.MinNumber, PdaRngAnalyzer.MaxNumber + 1);
        var result = PdaRngAnalyzer.Analyze(number);

        var playerName = Identity.Name(user, EntityManager);
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = Loc.GetString("generic-unknown-title");

        var paper = PredictedSpawn(ent.Comp.PaperPrototype, _transform.GetMapCoordinates(user));
        _label.Label(paper, Loc.GetString("pda-rng-printout-label", ("number", result.NumberText)));
        _audio.PlayEntity(ent.Comp.PrintSound, user, paper);
        _hands.PickupOrDrop(user, paper, checkActionBlocker: false);

        var content = PdaRngAnalyzer.FormatTicket(result, playerName);
        var paperComp = Comp<PaperComponent>(paper);
        _paper.SetContent((paper, paperComp), content);

        _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low,
            $"{ToPrettyString(user):user} printed PDA RNG ticket {result.NumberText} ({paper})");
    }

    private void UpdateUiState(Entity<PdaRngCartridgeComponent> ent, EntityUid loaderUid)
    {
        var hasRolled = !UnlimitedRolls && ent.Comp.HasRolled;
        _cartridge.UpdateCartridgeUiState(loaderUid, new PdaRngUiState(hasRolled));
    }
}
