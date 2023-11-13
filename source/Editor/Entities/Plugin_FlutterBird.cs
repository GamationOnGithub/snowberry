﻿using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Snowberry.Editor.Entities;

[Plugin("flutterbird")]
public class Plugin_FlutterBird : Entity {
    private static readonly Color[] colors = {
        Calc.HexToColor("89fbff"),
        Calc.HexToColor("f0fc6c"),
        Calc.HexToColor("f493ff"),
        Calc.HexToColor("93baff")
    };
    // TODO: per-entity randomness

    public override void Render() {
        base.Render();
        GFX.Game["scenery/flutterbird/idle00"].DrawJustified(Position, new Vector2(0.5f, 1), Calc.Random.Choose(colors));
    }

    protected override IEnumerable<Rectangle> Select() {
        yield return RectOnRelative(new(6, 6), justify: new(0.5f, 1));
    }

    public static void AddPlacements() {
        Placements.EntityPlacementProvider.Create("Flutterbird", "flutterbird");
    }
}