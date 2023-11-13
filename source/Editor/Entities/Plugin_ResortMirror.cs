﻿using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Snowberry.Editor.Entities;

[Plugin("resortmirror")]
public class Plugin_ResortMirror : Entity {
    private MTexture frame, mirror;

    public override void Initialize() {
        base.Initialize();
        frame = GFX.Game["objects/mirror/resortframe"];
        mirror = GFX.Game["objects/mirror/glassbreak00"].GetSubtexture(8, 0, frame.Width - 2, frame.Height);
    }

    public override void Render() {
        base.Render();
        mirror.DrawJustified(Position, new Vector2(0.5f, 0.75f));
        frame.DrawJustified(Position, new Vector2(0.5f, 1.0f));
    }

    protected override IEnumerable<Rectangle> Select() {
        yield return RectOnRelative(new(32, 37), justify: new(0.5f, 1));
    }

    public static void AddPlacements() {
        Placements.EntityPlacementProvider.Create("Resort Mirror", "resortmirror");
    }
}