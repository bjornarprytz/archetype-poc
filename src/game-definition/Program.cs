using Archetype.Build;
using Archetype.Core;

// Goal 1:
// Create a game state with the following zones:
// - Hand, discard pile, draw pile
// One keyword: "Draw X cards"
// One card: Cantrip, no cost, Draw 1 card
// Starting deck (10 of those)
// 

var definition = new GameDefinitionBuilder()
    .WithId("archetype-poc")
    .AddZone("hand", new Dictionary<string, object>())
    .AddZone("board", new Dictionary<string, object>())
    .AddPhase(new PhaseDefinition(
        Name: "main"))
    .AddPlayer("alice", new PlayerDefinitionBuilder
    .Build();

var coreSet = new CardSet("core", 1, [
    new CardDefinition(
        Name: "Swordsman",
        StaticProperties: new Dictionary<string, object> { ["health"] = 5.0, ["cost"] = 2.0 },
        PrimaryEffect: new EffectBlockDef([
            new EffectBlockStep("strike", [Kw.Param("source"), Kw.Num(3)])
        ]),
        AdditionalEffects: [],
        StaticEffects: []),
    new CardDefinition(
        Name: "Healer",
        StaticProperties: new Dictionary<string, object> { ["health"] = 3.0, ["cost"] = 1.0 },
        PrimaryEffect: new EffectBlockDef([
            new EffectBlockStep("heal", [Kw.Param("source"), Kw.Num(2)])
        ]),
        AdditionalEffects: [],
        StaticEffects: []),
]);

var outputDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../..", "archetype-poc"));
Console.WriteLine($"Writing to: {outputDir}");
BuildRunner.Run(definition, [coreSet], outputDir: outputDir);
Console.WriteLine("Done.");
