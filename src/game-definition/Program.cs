using Archetype.Build;
using Archetype.Build.Extensions;
using Archetype.Core;

// Journey — a two-player card game about survival, enjoyment, and nature.
// Players balance stamina, supplies and morale while travelling toward a destination.
// First player to reach distance 12 wins. If stamina falls to 0 the traveller collapses.

var definition = new GameDefinitionBuilder()
    .WithId("journey")

    // ── Zones (per player) ───────────────────────────────────────────────────
    .AddZone("hand",    b => b.WithStaticProperty("visible", true))
    .AddZone("deck",    b => b.WithStaticProperty("visible", false))
    .AddZone("discard", b => b.WithStaticProperty("visible", true))
    .AddZone("camp",    b => b.WithStaticProperty("visible", true))

    // ── Turn structure ───────────────────────────────────────────────────────
    .AddPhase("day", _ => { })

    // ── Journey keywords ─────────────────────────────────────────────────────
    .RegisterKeyword("travel", b => b
        .WithParam("player", TypeName.Player)
        .WithParam("amount", TypeName.Number)
        .WithReturnType(TypeName.Void)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("player"), Kw.Str("distance"), Kw.Param("amount")))
        .WithTextTemplate("{player} travels {amount} miles."))

    .RegisterKeyword("exert", b => b
        .WithParam("player", TypeName.Player)
        .WithParam("amount", TypeName.Number)
        .WithReturnType(TypeName.Number)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("player"), Kw.Str("stamina"), Kw.Multiply(Kw.Param("amount"), Kw.Num(-1))))
        .WithTextTemplate("{player} loses {amount} stamina."))

    .RegisterKeyword("forage", b => b
        .WithParam("player", TypeName.Player)
        .WithParam("amount", TypeName.Number)
        .WithReturnType(TypeName.Number)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("player"), Kw.Str("supplies"), Kw.Param("amount")))
        .WithTextTemplate("{player} forages and gains {amount} supplies."))

    .RegisterKeyword("rest", b => b
        .WithParam("player", TypeName.Player)
        .WithParam("amount", TypeName.Number)
        .WithReturnType(TypeName.Number)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("player"), Kw.Str("stamina"), Kw.Param("amount")))
        .WithTextTemplate("{player} rests and recovers {amount} stamina."))

    .RegisterKeyword("enjoy", b => b
        .WithParam("player", TypeName.Player)
        .WithParam("amount", TypeName.Number)
        .WithReturnType(TypeName.Number)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("player"), Kw.Str("morale"), Kw.Param("amount")))
        .WithTextTemplate("{player} enjoys nature and gains {amount} morale."))

    .RegisterKeyword("consume-supplies", b => b
        .WithParam("player", TypeName.Player)
        .WithParam("amount", TypeName.Number)
        .WithReturnType(TypeName.Number)
        .WithBody(Kw.ModifyAccumulator(
            Kw.Param("player"), Kw.Str("supplies"), Kw.Multiply(Kw.Param("amount"), Kw.Num(-1))))
        .WithTextTemplate("{player} consumes {amount} supplies."))

    // ── State-based rules (collapse & arrival) ───────────────────────────────
    .AddStateBasedRule(new StateBasedRule(
        "alice-collapses",
        Kw.AtMost(Kw.GetState(Kw.PlayerByName(Kw.Str("alice")), Kw.Str("stamina")), Kw.Num(0)),
        new EffectBlockDef([
            new EffectBlockStep("declare-winner", [Kw.PlayerByName(Kw.Str("bob"))]),
        ])))

    .AddStateBasedRule(new StateBasedRule(
        "bob-collapses",
        Kw.AtMost(Kw.GetState(Kw.PlayerByName(Kw.Str("bob")), Kw.Str("stamina")), Kw.Num(0)),
        new EffectBlockDef([
            new EffectBlockStep("declare-winner", [Kw.PlayerByName(Kw.Str("alice"))]),
        ])))

    .AddStateBasedRule(new StateBasedRule(
        "alice-arrives",
        Kw.AtLeast(Kw.GetState(Kw.PlayerByName(Kw.Str("alice")), Kw.Str("distance")), Kw.Num(12)),
        new EffectBlockDef([
            new EffectBlockStep("declare-winner", [Kw.PlayerByName(Kw.Str("alice"))]),
        ])))

    .AddStateBasedRule(new StateBasedRule(
        "bob-arrives",
        Kw.AtLeast(Kw.GetState(Kw.PlayerByName(Kw.Str("bob")), Kw.Str("distance")), Kw.Num(12)),
        new EffectBlockDef([
            new EffectBlockStep("declare-winner", [Kw.PlayerByName(Kw.Str("bob"))]),
        ])))

    // ── Player definitions ───────────────────────────────────────────────────
    .AddPlayer("alice", b => b
        .WithStateField("stamina", StateFieldType.Number)
        .WithStateField("morale", StateFieldType.Number)
        .WithStateField("supplies", StateFieldType.Number)
        .WithStateField("distance", StateFieldType.Number))

    .AddPlayer("bob", b => b
        .WithStateField("stamina", StateFieldType.Number)
        .WithStateField("morale", StateFieldType.Number)
        .WithStateField("supplies", StateFieldType.Number)
        .WithStateField("distance", StateFieldType.Number))

    .WithPlayableZones("hand")

    // ── Initial state ───────────────────────────────────────────────────────
    .WithInitManifest(new InitManifest(
        Zones: [
            new ZoneSpec("alice-hand",    "alice", "hand",   new Dictionary<string, double>(), []),
            new ZoneSpec("alice-deck",    "alice", "deck",   new Dictionary<string, double>(), []),
            new ZoneSpec("alice-discard", "alice", "discard", new Dictionary<string, double>(), []),
            new ZoneSpec("alice-camp",    "alice", "camp",   new Dictionary<string, double>(), []),
            new ZoneSpec("bob-hand",      "bob",   "hand",   new Dictionary<string, double>(), []),
            new ZoneSpec("bob-deck",      "bob",   "deck",   new Dictionary<string, double>(), []),
            new ZoneSpec("bob-discard",   "bob",   "discard", new Dictionary<string, double>(), []),
            new ZoneSpec("bob-camp",      "bob",   "camp",   new Dictionary<string, double>(), []),
        ],
        Cards: [
            // Alice deck
            new CardSpec("alice", "alice-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Forage", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Forage", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Forage", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Camp", new Dictionary<string, double>(), [], null),
            new CardSpec("alice", "alice-deck", "Scenic View", new Dictionary<string, double>(), [], null),
            // Bob deck
            new CardSpec("bob", "bob-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Walk", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Forage", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Forage", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Forage", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Camp", new Dictionary<string, double>(), [], null),
            new CardSpec("bob", "bob-deck", "Scenic View", new Dictionary<string, double>(), [], null),
        ],
        PlayerStates: [
            new PlayerStateSpec("alice", new Dictionary<string, double> { ["stamina"] = 10.0, ["morale"] = 0.0, ["supplies"] = 3.0, ["distance"] = 0.0 }, []),
            new PlayerStateSpec("bob",   new Dictionary<string, double> { ["stamina"] = 10.0, ["morale"] = 0.0, ["supplies"] = 3.0, ["distance"] = 0.0 }, []),
        ]))
    .Build();

// ── Card set ────────────────────────────────────────────────────────────────
var coreSet = new CardSet("journey-core", 1, [
    new CardDefinitionBuilder("Walk")
        .WithStaticProperty("cost", 1.0)
        .WithPrimaryEffect(b => b
            .Step("travel", Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(2))
            .Step("exert",  Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(2)))
        .Build(),

    new CardDefinitionBuilder("Forage")
        .WithStaticProperty("cost", 1.0)
        .WithPrimaryEffect(b => b
            .Step("forage", Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(2))
            .Step("exert",  Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(1)))
        .Build(),

    new CardDefinitionBuilder("Camp")
        .WithStaticProperty("cost", 1.0)
        .WithPrimaryEffect(b => b
            .Step("rest", Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(3))
            .Step("consume-supplies", Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(1))
            .Step("enjoy", Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(1)))
        .Build(),

    new CardDefinitionBuilder("Scenic View")
        .WithStaticProperty("cost", 0.0)
        .WithPrimaryEffect(b => b
            .Step("enjoy", Kw.Invoke("owner-of", Kw.Param("source")), Kw.Num(2)))
        .Build(),
]);

var outputDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../..", "archetype-poc"));
Console.WriteLine($"Writing to: {outputDir}");
BuildRunner.Run(definition, [coreSet], outputDir: outputDir);
Console.WriteLine("Done.");
