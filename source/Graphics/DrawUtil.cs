﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using Celeste;
using Snowberry.Triangulator;

namespace Snowberry;

public static class DrawUtil {

    public static Rectangle GetDrawBounds() {
        // TODO: cache this maybe
        RenderTargetBinding[] renderTargets = Draw.SpriteBatch.GraphicsDevice.GetRenderTargets();
        if (renderTargets.Length > 0 && renderTargets[0].RenderTarget is RenderTarget2D renderTarget)
            return renderTarget.Bounds;
        return new Rectangle(0, 0, Engine.Graphics.PreferredBackBufferWidth, Engine.Graphics.PreferredBackBufferHeight);
    }

    public static void WithinScissorRectangle(Rectangle rect, Action action, Matrix? matrix = null, bool nested = true, bool additive = false) {
        if (action != null) {
            Rectangle bounds = GetDrawBounds();
            if (!bounds.Intersects(rect))
                return;

            if (nested)
                Draw.SpriteBatch.End();

            Rectangle scissor = Draw.SpriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState rasterizerState = Engine.Instance.GraphicsDevice.RasterizerState;
            if (!Engine.Instance.GraphicsDevice.RasterizerState.ScissorTestEnable)
                Engine.Instance.GraphicsDevice.RasterizerState = new RasterizerState { ScissorTestEnable = true, CullMode = CullMode.None };
            Draw.SpriteBatch.GraphicsDevice.ScissorRectangle = rect.ClampTo(bounds);

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, additive ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Engine.Instance.GraphicsDevice.RasterizerState, null, matrix ?? Matrix.Identity);
            action();
            Draw.SpriteBatch.End();

            Engine.Instance.GraphicsDevice.RasterizerState = rasterizerState;
            Draw.SpriteBatch.GraphicsDevice.ScissorRectangle = scissor;

            if (nested)
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Engine.Instance.GraphicsDevice.RasterizerState);
        }
    }

    public static void DottedLine(Vector2 start, Vector2 end, Color color, float dot = 2f, float space = 2f) {
        float d = Vector2.Distance(start, end);
        Vector2 dir = (end - start).SafeNormalize();
        float step = dot + space;
        for (float x = 0f; x < d; x += step) {
            Vector2 a = start + dir * Math.Min(x, d);
            Vector2 b = start + dir * Math.Min(x + dot, d);
            Draw.Line(a, b, color);
        }
    }

    public static void Path(List<Vector2> points, Color color, float thickness = 1) {
        if (points.Count < 2)
            return;

        Vector2 current = points[0];
        for(var i = 1; i < points.Count; i++) {
            Draw.Line(current, points[i], color, thickness);
            current = points[i];
        }
    }

    public static void DrawVerticesWithScissoring<T>(
        Matrix matrix,
        T[] vertices,
        int vertexCount,
        Effect effect = null,
        BlendState blendState = null)
        where T : struct, IVertexType
    {
        effect ??= GFX.FxPrimitive;
        blendState ??= BlendState.AlphaBlend;
        Vector2 vector2 = Vector2.Zero;
        ref Vector2 local = ref vector2;
        Viewport viewport = Engine.Graphics.GraphicsDevice.Viewport;
        double width = viewport.Width;
        viewport = Engine.Graphics.GraphicsDevice.Viewport;
        double height = viewport.Height;
        local = new Vector2((float)width, (float)height);
        matrix *= Matrix.CreateScale((float)(1.0 / vector2.X * 2.0), (float)(-(1.0 / vector2.Y) * 2.0), 1f);
        matrix *= Matrix.CreateTranslation(-1f, 1f, 0.0f);
        Engine.Instance.GraphicsDevice.RasterizerState = new RasterizerState { ScissorTestEnable = true, CullMode = CullMode.None };
        Engine.Instance.GraphicsDevice.BlendState = blendState;
        effect.Parameters["World"].SetValue(matrix);
        foreach(EffectPass pass in effect.CurrentTechnique.Passes){
            pass.Apply();
            Engine.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertexCount / 3);
        }
    }

    // intended for gameplay
    public static void DrawPolygon(Vector2[] points, Color color) {
        // adapted from Frost Helper: https://github.com/JaThePlayer/FrostHelper/blob/aa09b0338c03b6a37921fe0a3d254eaec2bb675d/Code/FrostHelper/Helpers/ArbitraryShapeEntityHelper.cs#L30
        Triangulator.Triangulator.Triangulate(points, WindingOrder.Clockwise, null, out var verts, out var indices);
        var fill = new VertexPositionColor[indices.Length];
        for (int i = 0; i < indices.Length; i++)
            fill[i] = new VertexPositionColor {
                Position = new(verts[indices[i]], 0f),
                Color = color
            };
        GFX.DrawVertices(Editor.Editor.Instance.Camera.Matrix, fill, fill.Length);
    }

    // intended for UI
    public static void DrawGradientRectV(Vector2 position, float width, float height, Color top, Color bottom) {
        if (top == bottom) {
            Draw.Rect(position, width, height, top);
            return;
        }

        GFX.DrawVertices(Matrix.Identity, new VertexPositionColor[6] {
            new(new(position, 0), top),
            new(new(position + new Vector2(width, 0), 0), top),
            new(new(position + new Vector2(width, height), 0), bottom),
            new(new(position, 0), top),
            new(new(position + new Vector2(width, height), 0), bottom),
            new(new(position + new Vector2(0, height), 0), bottom),
        }, 6);
    }

    public static void DrawGuidelines(Rectangle bounds, Color c) {
        if (bounds.Width > 0) {
            string topTxt = (bounds.Width / 8).ToString();
            float topGap = Fonts.Regular.Measure(topTxt).X / 2f + 3;
            float topY = bounds.Top - 5;
            if (topGap * 2 <= bounds.Width - 2) {
                Draw.Line(new(bounds.Left, topY), new(bounds.Center.X - topGap, topY), c);
                Draw.Line(new(bounds.Center.X + topGap, topY), new(bounds.Right, topY), c);
            }
            Fonts.Regular.Draw(topTxt, new(bounds.Center.X, bounds.Top - 2), new(1), new(0.5f, 1), c);
        }

        if (bounds.Height > 0) {
            string leftTxt = (bounds.Height / 8).ToString();
            float leftGap = Fonts.Regular.Measure(leftTxt).Y / 2f + 3;
            if (leftGap * 2 <= bounds.Height - 2) {
                float leftX = bounds.Left - 5;
                Draw.Line(new(leftX, bounds.Top), new(leftX, bounds.Center.Y - leftGap), c);
                Draw.Line(new(leftX, bounds.Center.Y + leftGap), new(leftX, bounds.Bottom), c);
            }
            Fonts.Regular.Draw(leftTxt, new(bounds.Left - 2, bounds.Center.Y), new(1), new(1, 0.5f), c);
        }
    }
}