﻿using System.Collections.Generic;

namespace Snowberry.Editor.Entities;

[Plugin("torch")]
public class Plugin_Torch : Entity {
    [Option("startLit")] public bool Lit;

    public override void Render() {
        base.Render();

        (Lit ? FromSprite("litTorch", "on") : FromSprite("torch", "off"))?.DrawCentered(Position);
    }

    public static void AddPlacements() {
        Placements.EntityPlacementProvider.Create("Torch", "torch", new Dictionary<string, object>() { { "red", false } });
        Placements.EntityPlacementProvider.Create("Torch (Lit)", "torch", new Dictionary<string, object>() { { "startLit", true } });
    }
}