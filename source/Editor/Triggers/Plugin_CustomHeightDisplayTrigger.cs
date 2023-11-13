﻿using Microsoft.Xna.Framework;

namespace Snowberry.Editor.Triggers;

[Plugin("everest/customHeightDisplayTrigger")]
public class Plugin_CustomHeightDisplayTrigger : Trigger {
    [Option("vanilla")] public bool Vanilla = false;
    [Option("target")] public float Target = 0;
    [Option("from")] public float From = 0;
    [Option("text")] public string text = "{x}m";
    [Option("progressAudio")] public bool ProgressAudio = false;
    [Option("displayOnTransition")] public bool DisplayOnTransition = false;

    public override void Render() {
        base.Render();

        string prefix = "";
        string postfix = "";
        bool valid = false;
        if (text.IndexOf("{x}") != -1) {
            if (text.LastIndexOf("{") > 0)
                prefix = text.Substring(0, text.LastIndexOf("{"));
            if (text.LastIndexOf("}") < text.Length - 1)
                postfix = text.Substring(text.LastIndexOf("}") + 1);
            valid = true;
        }

        var str = (valid) ? ((From == Target) ? $"({prefix}{Target}{postfix})" : $"({prefix}{From}{postfix} -> {prefix}{Target}{postfix})") : "(Invalid text field!)";
        Fonts.Pico8.Draw(str, Center + Vector2.UnitY * 6, Vector2.One, new(0.5f), Color.Black);
    }

    public new static void AddPlacements() {
        Placements.EntityPlacementProvider.Create("Custom Height Display Trigger (Everest)", "everest/customHeightDisplayTrigger", trigger: true);
    }
}