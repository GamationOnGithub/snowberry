﻿using Microsoft.Xna.Framework;

namespace LevelEditorMod.Editor {
    /// <summary>
    /// The class that you inherit from in order for your mod to be loaded into the editor.
    /// </summary>
    public abstract class EditorModule {
        internal readonly string Name;
        internal readonly Color Color;

        /// <summary>
        /// Base constructor, must be called from a parameterless constructor.
        /// </summary>
        /// <param name="name">The name of your mod, displayed in the editor.</param>
        /// <param name="color">Your mod's color, shown occasionally.</param>
        public EditorModule(string name, Color? color = null) {
            Name = name;
            Color = color ?? Color.Snow;
        }
    }
}
