﻿using Celeste;
using Monocle;

namespace Snowberry.Editor.Entities;

[Plugin("floatingDebris")]
public class Plugin_FloatingDebris : Entity {
    private MTexture debris;

    public override void Initialize() {
        base.Initialize();
        debris = GFX.Game["scenery/debris"].GetSubtexture(Calc.Random.Next(0, 8) * 8, 0, 8, 8);
    }

    public override void Render() {
        base.Render();
        debris.DrawCentered(Position);
    }

    public static void AddPlacements() {
        Placements.EntityPlacementProvider.Create("Floating Debris", "floatingDebris");
    }
}