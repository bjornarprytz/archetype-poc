using Archetype.Build;
using Archetype.Build.Extensions;
using Archetype.Core;

// Two-player card combat prototype.
// Zones (per player): hand, draw-pile, discard-pile, board
// Keywords: strike (deal damage), heal (restore health)
// Cards: Swordsman (deal 3 damage to self), Healer (restore 2 health to self)
// Win condition: first player whose health accumulator drops to 0 loses.

var definition = new GameDefinitionBuilder()
    .WithId("archetype-poc")

    // ── Zone definitions ────────────────────────────────────────────────────
    // Instances are created per-player in the InitManifest below.
    .AddZone("hand",         b => b.WithStaticProperty("visible", true))
    .AddZone("draw-pile",    b => b.WithStaticProperty("visible", false))
    .AddZone("discard-pile", b => b.WithStaticProperty("visible", true))
    .AddZone("board",        b => b.WithStaticProperty("visible", true))

    // ── Turn structure ──────────────────────────────────────────────────────
    .AddPhase("main", _ => { })

    // ── Custom keywords ─────────────────────────────────────────────────────
    .RegisterKeyword("strike", b => b
        .WithParam("target", TypeName.Atom)
        .WithParam("power",  TypeName.Number)
        .WithReturnType(TypeName.Number)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("target"),
            Kw.Str("health"),
            Kw.Multiply(Kw.Param("power"), Kw.Num(-1))))
        .WithTextTemplate("Deal {power} damage to {target}"))

    .RegisterKeyword("heal", b => b
        .WithParam("target", TypeName.Atom)
        .WithParam("amount", TypeName.Number)
        .WithReturnType(TypeName.Number)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("target"),
            Kw.Str("health"),
            Kw.Param("amount")))
        .WithTextTemplate("Restore {amount} health to {target}"))

    // ── Win conditions ──────────────────────────────────────────────────────
    .AddStateBasedRule(new StateBasedRule(
        "alice-loses",
        Kw.AtMost(
            Kw.GetState(Kw.PlayerByName(Kw.Str("alice")), Kw.Str("health")),
            Kw.Num(0)),
        new EffectBlockDef([
            new EffectBlockStep("declare-winner", [Kw.PlayerByName(Kw.Str("bob"))]),
        ])))

    .AddStateBasedRule(new StateBasedRule(
        "bob-loses",
        Kw.AtMost(
            Kw.GetState(Kw.PlayerByName(Kw.Str("bob")), Kw.Str("health")),
            Kw.Num(0)),
        new EffectBlockDef([
            new EffectBlockStep("declare-winner", [Kw.PlayerByName(Kw.Str("alice"))]),
        ])))

    // ── Player definitions ──────────────────────────────────────────────────
    // health is an accumulator; starting value is set via PlayerStateSpec below.
    .AddPlayer("alice", b => b.WithStateField("health", StateFieldType.Number))
    .AddPlayer("bob",   b => b.WithStateField("health", StateFieldType.Number))

    // Cards may only be played from hand.
    .WithPlayableZones("hand")

    // ── Initial game state ──────────────────────────────────────────────────
    .WithInitManifest(new InitManifest(
        Zones: [
            new ZoneSpec("alice-hand",    "alice", "hand",         new Dictionary<string, double>(), []),
            new ZoneSpec("alice-draw",    "alice", "draw-pile",    new Dictionary<string, double>(), []),
            new ZoneSpec("alice-discard", "alice", "discard-pile", new Dictionary<string, double>(), []),
            new ZoneSpec("bob-hand",      "bob",   "hand",         new Dictionary<string, double>(), []),
            new ZoneSpec("bob-draw",      "bob",   "draw-pile",    new Dictionary<string, double>(), []),
            new ZoneSpec("bob-discard",   "bob",   "discard-pile", new Dictionary<string, double>(), []),
        ],
        Cards: [
            // Alice: 5 Swordsmen + 5 Healers in draw pile
            new CardSpec("alice", "alice-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-draw", "Healer",    new Dictionary<string, double>(), [], null),
            // Bob: same deck
            new CardSpec("bob", "bob-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Swordsman", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Healer",    new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-draw", "Healer",    new Dictionary<string, double>(), [], null),
        ],
        PlayerStates: [
            new PlayerStateSpec("alice", new Dictionary<string, double> { ["health"] = 20.0 }, []),
            new PlayerStateSpec("bob",   new Dictionary<string, double> { ["health"] = 20.0 }, []),
        ]))
    .Build();

// ── Card set ────────────────────────────────────────────────────────────────
var coreSet = new CardSet("core", 1, [
    new CardDefinitionBuilder("Swordsman")
        .WithStaticProperty("cost",   2.0)
        .WithStateField("health", StateFieldType.Number)
        .WithPrimaryEffect(b => b
            .Step("strike", Kw.Param("source"), Kw.Num(3)))
        .Build(),

    new CardDefinitionBuilder("Healer")
        .WithStaticProperty("cost",   1.0)
        .WithStateField("health", StateFieldType.Number)
        .WithPrimaryEffect(b => b
            .Step("heal", Kw.Param("source"), Kw.Num(2)))
        .Build(),
]);

var outputDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../..", "archetype-poc"));
Console.WriteLine($"Writing to: {outputDir}");
BuildRunner.Run(definition, [coreSet], outputDir: outputDir);
Console.WriteLine("Done.");
