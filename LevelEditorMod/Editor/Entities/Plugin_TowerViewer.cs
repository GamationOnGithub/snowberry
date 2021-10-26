﻿using Celeste;
using Monocle;
using Microsoft.Xna.Framework;

namespace LevelEditorMod.Editor.Entities {
    [Plugin("towerviewer")]
    public class Plugin_Watchtower : Entity {
        [Option("onlyY")] public bool OnlyY = false;
        [Option("summit")] public bool Summit = false;

        public override void Render() {
            base.Render();

            MTexture tower = GFX.Game["objects/lookout/lookout05"];
            tower.DrawJustified(Position, new Vector2(0.5f, 1.0f));

            Vector2 prev = Position;
            foreach (Vector2 node in Nodes) {
                tower.DrawJustified(node, new Vector2(0.5f, 1.0f));
                Draw.Line(prev, node, Color.White * 0.5f);
                prev = node;
            }
        }
    }
}