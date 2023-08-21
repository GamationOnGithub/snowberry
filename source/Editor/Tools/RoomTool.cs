﻿using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using Snowberry.Editor.UI;
using System;

namespace Snowberry.Editor.Tools;

public class RoomTool : Tool {
    private Room lastSelected = null;
    private int lastFillerSelected = -1;
    public static bool ScheduledRefresh = false;

    private Vector2? lastRoomOffset = null;
    private static bool resizingX, resizingY, fromLeft, fromTop;
    private static int newWidth, newHeight;
    private static Rectangle oldRoomBounds;
    private static bool justSwitched = false;

    public static Rectangle? PendingRoom = null;

    public override UIElement CreatePanel(int height) {
        // room selection panel containing room metadata
        var ret = new UIRoomSelectionPanel {
            Width = 160,
            Height = height
        };
        ret.Refresh();
        return ret;
    }

    public override string GetName() => Dialog.Clean("SNOWBERRY_EDITOR_TOOL_ROOMTOOL");

    public override void Update(bool canClick) {
        // refresh the display
        var curRoom = Editor.SelectedRoom;
        if (lastSelected != curRoom || lastFillerSelected != Editor.SelectedFillerIndex || ScheduledRefresh) {
            justSwitched = true;
            ScheduledRefresh = false;
            lastSelected = curRoom;
            lastFillerSelected = Editor.SelectedFillerIndex;
            if (Editor.Instance.ToolPanel is UIRoomSelectionPanel selectionPanel)
                selectionPanel.Refresh();
            if (curRoom != null) {
                lastRoomOffset = curRoom.Position - (Editor.Mouse.World / 8);
                oldRoomBounds = curRoom.Bounds;
            }
        }

        // move, resize, add rooms
        if (canClick && curRoom != null && !justSwitched) {
            if (MInput.Mouse.PressedLeftButton) {
                lastRoomOffset = curRoom.Position - (Editor.Mouse.World / 8);
                // check if the mouse is 8 pixels from the room's borders
                fromLeft = Math.Abs(Editor.Mouse.World.X / 8f - curRoom.Position.X) < 1;
                resizingX = Math.Abs(Editor.Mouse.World.X / 8f - (curRoom.Position.X + curRoom.Width)) < 1
                            || fromLeft;
                fromTop = Math.Abs(Editor.Mouse.World.Y / 8f - curRoom.Position.Y) < 1;
                resizingY = Math.Abs(Editor.Mouse.World.Y / 8f - (curRoom.Position.Y + curRoom.Height)) < 1
                            || fromTop;
                oldRoomBounds = curRoom.Bounds;
            } else if (MInput.Mouse.CheckLeftButton) {
                Vector2 world = Editor.Mouse.World / 8;
                var offset = lastRoomOffset ?? Vector2.Zero;
                if (!resizingX && !resizingY) {
                    var newX = (int)(world + offset).X;
                    var newY = (int)(world + offset).Y;
                    var diff = new Vector2(newX - curRoom.Bounds.X, newY - curRoom.Bounds.Y);
                    curRoom.Bounds.X = (int)(world + offset).X;
                    curRoom.Bounds.Y = (int)(world + offset).Y;
                    foreach (var e in curRoom.AllEntities) {
                        e.Move(diff * 8);
                        for (int i = 0; i < e.Nodes.Count; i++) {
                            e.MoveNode(i, diff * 8);
                        }
                    }
                } else {
                    int dx = 0, dy = 0;
                    if (resizingX) {
                        // compare against the opposite edge
                        newWidth = (int)Math.Ceiling(fromLeft ? oldRoomBounds.Right - world.X : world.X - curRoom.Bounds.Left);
                        curRoom.Bounds.Width = Math.Max(newWidth, 1);
                        if (fromLeft) {
                            int newX = (int)Math.Floor(world.X);
                            dx = curRoom.Bounds.X - newX;
                            curRoom.Bounds.X = newX;
                        }
                    }

                    if (resizingY) {
                        newHeight = (int)Math.Ceiling(fromTop ? oldRoomBounds.Bottom - world.Y : world.Y - curRoom.Bounds.Top);
                        curRoom.Bounds.Height = Math.Max(newHeight, 1);
                        if (fromTop) {
                            int newY = (int)Math.Floor(world.Y);
                            dy = curRoom.Bounds.Y - newY;
                            curRoom.Bounds.Y = newY;
                        }
                    }

                    // TODO: dragging over tiles and then back removes the tiles
                    //  maybe fix alongside undo/redo/transactions?
                    if (dx != 0 || dy != 0)
                        curRoom.MoveTiles(dx, dy);
                }
            } else {
                lastRoomOffset = null;
                var newBounds = curRoom.Bounds;
                if (!oldRoomBounds.Equals(newBounds)) {
                    oldRoomBounds = newBounds;
                    Editor.SelectedRoom.UpdateBounds();
                }

                resizingX = resizingY = fromLeft = fromTop = false;
                newWidth = newHeight = 0;
            }
        }

        if (MInput.Mouse.ReleasedLeftButton) {
            justSwitched = false;
        }

        // room creation
        if (canClick) {
            if (curRoom == null) {
                if (MInput.Mouse.CheckLeftButton) {
                    var lastPress = (Editor.Instance.worldClick / 8).Ceiling() * 8;
                    var mpos = (Editor.Mouse.World / 8).Ceiling() * 8;
                    int ax = (int)Math.Min(mpos.X, lastPress.X);
                    int ay = (int)Math.Min(mpos.Y, lastPress.Y);
                    int bx = (int)Math.Max(mpos.X, lastPress.X);
                    int by = (int)Math.Max(mpos.Y, lastPress.Y);
                    var newRoom = new Rectangle(ax, ay, bx - ax, by - ay);
                    if (newRoom.Width > 0 || newRoom.Height > 0) {
                        newRoom.Width = Math.Max(newRoom.Width, 8);
                        newRoom.Height = Math.Max(newRoom.Height, 8);
                        if (!PendingRoom.HasValue)
                            ScheduledRefresh = true;
                        PendingRoom = newRoom;
                    } else {
                        ScheduledRefresh = true;
                        PendingRoom = null;
                    }
                }
            } else {
                if (PendingRoom.HasValue) {
                    PendingRoom = null;
                    ScheduledRefresh = true;
                }
            }
        }
    }

    public override void RenderWorldSpace() {
        base.RenderWorldSpace();
        if (PendingRoom.HasValue) {
            var prog = (float)Math.Abs(Math.Sin(Engine.Scene.TimeActive * 3));
            Draw.Rect(PendingRoom.Value, Color.Lerp(Color.White, Color.Cyan, prog) * 0.6f);
            Draw.HollowRect(PendingRoom.Value.X, PendingRoom.Value.Y, 40 * 8, 23 * 8, Color.Lerp(Color.Orange, Color.White, prog) * 0.6f);
        }
    }
}