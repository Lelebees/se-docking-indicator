using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;
using VRageRender.Import;

namespace IngameScript
{
    public class Indicator
    {
        private readonly IMyTextSurface textSurface;
        private readonly DockingPort port;
        private static readonly Color ConnectedGreen = new Color(0, 255, 0); // Nuclear Green
        private static readonly Color DisconnectingBlue = new Color(0, 255, 255); // Cyan
        private static readonly Color ConnectingYellow = new Color(255, 255, 0); // Uranium Yellow
        private static readonly Color ErrorRed = Color.Red; // It does not get redder then this.

        public Indicator(IMyTextSurface textSurface, DockingPort port)
        {
            this.textSurface = textSurface;
            this.port = port;
            textSurface.ScriptBackgroundColor = new Color(0, 0, 0, 255);
            textSurface.ContentType = ContentType.SCRIPT;
            textSurface.Script = "";
        }

        public void UpdateDockingPortIndication()
        {
            MySpriteDrawFrame frame = textSurface.DrawFrame();
            RectangleF viewport = new RectangleF(
                (textSurface.TextureSize - textSurface.SurfaceSize) / 2f,
                textSurface.SurfaceSize
            );
            switch (port.GetState())
            {
                case DockingPortState.Disconnected:
                    RenderDisconnected(frame, viewport.Center);
                    break;
                case DockingPortState.Docked:
                    RenderDocked(frame, viewport.Center);
                    break;
                case DockingPortState.Docking:
                    RenderDocking(frame, viewport.Center);
                    break;
                case DockingPortState.Undocking:
                    RenderUndocking(frame, viewport.Center);
                    break;
            }

            frame.Dispose();
        }

        private static string FormatDockTime(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 60)
                return $"{(int)timeSpan.TotalSeconds} sec";
            else if (timeSpan.TotalMinutes < 10)
                return $"{(int)timeSpan.TotalMinutes} min {(int)timeSpan.Seconds} sec";
            else if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} min";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hr {(int)timeSpan.Minutes} min";
            else
                return $"{(int)timeSpan.TotalDays} d {(int)timeSpan.Hours} hr";
        }
        
        private void RenderDisconnected(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f)
        {
            Color leftPortColor = Color.White;
            if (port.IsDamagedOrDisabled())
            {
                leftPortColor = ErrorRed;
            }
            frame.AddRange(DrawDockingPortName(centerPos, scale, textSurface.ScriptForegroundColor, port));
            frame.AddRange(DrawLeftConnector(centerPos, scale, leftPortColor));
            frame.AddRange(DrawTimeSinceLastDock(centerPos, scale, port, textSurface.ScriptForegroundColor));
        }

        private void RenderDocked(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f)
        {
            frame.AddRange(DrawDockingPortName(centerPos, scale, textSurface.ScriptForegroundColor, port));
            frame.AddRange(DrawLeftConnector(centerPos, scale, ConnectedGreen));
            frame.AddRange(DrawConnectionChain(centerPos, scale));
            frame.AddRange(DrawRightConnector(centerPos, scale, ConnectedGreen));
            frame.AddRange(DrawDockeeAnnouncement(centerPos, scale, textSurface.ScriptForegroundColor, port));
        }

        private void RenderDocking(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f)
        {
            frame.AddRange(DrawDockingPortName(centerPos, scale, textSurface.ScriptForegroundColor, port));
            frame.AddRange(DrawLeftConnector(centerPos, scale, ConnectingYellow));
            frame.AddRange(DrawDockingArrows(centerPos, scale));
            frame.AddRange(DrawRightConnector(centerPos, scale, ConnectingYellow));
        }

        private void RenderUndocking(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f)
        {
            frame.AddRange(DrawDockingPortName(centerPos, scale, textSurface.ScriptForegroundColor, port));
            frame.AddRange(DrawLeftConnector(centerPos, scale, DisconnectingBlue));
            frame.AddRange(DrawUndockArrows(centerPos, scale));
            frame.AddRange(DrawRightConnector(centerPos, scale, DisconnectingBlue));
        }


        #region Sprite Definitions

        private static List<MySprite> DrawDockingPortName(Vector2 centerPos, float scale, Color color, DockingPort port)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = port.GetName(),
                    Position = new Vector2(0f, -180f) * scale + centerPos,
                    Color = color,
                    FontId = "Debug",
                    RotationOrScale = 3.2f * scale
                }, // labelPortName
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = "DOCKING PORT",
                    Position = new Vector2(0f, -235f) * scale + centerPos,
                    Color = color,
                    FontId = "Debug",
                    RotationOrScale = 1.5f * scale
                } // labelDockingPort
            };
            return textureSprites;
        }

        private static List<MySprite> DrawDockeeAnnouncement(Vector2 centerPos, float scale, Color color,
            DockingPort port)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = port.GetNameOfConnectedGrid(),
                    Position = new Vector2(0f, 125f) * scale + centerPos,
                    Color = color,
                    FontId = "Debug",
                    RotationOrScale = 2f * scale
                }, // labelDockee
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = "Currently docked with:",
                    Position = new Vector2(0f, 78f) * scale + centerPos,
                    Color = color,
                    FontId = "Debug",
                    RotationOrScale = 1f * scale
                } // labelDockeeIntroduction
            };
            return textureSprites;
        }

        private static List<MySprite> DrawRightConnector(Vector2 centerPos, float scale, Color color)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(145f, 0f) * scale + centerPos,
                    Size = new Vector2(75f, 100f) * scale,
                    Color = color,
                    RotationOrScale = 0f
                }, // [PortRight] Body
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(97f, 0f) * scale + centerPos,
                    Size = new Vector2(50f, 75f) * scale,
                    Color = color,
                    RotationOrScale = 0f
                }, // [PortRight] BodyWaist
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "RightTriangle",
                    Position = new Vector2(85f, 44f) * scale + centerPos,
                    Size = new Vector2(25f, 12f) * scale,
                    Color = color,
                    RotationOrScale = 3.1416f
                }, // [PortRight] SlantDown
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "RightTriangle",
                    Position = new Vector2(85f, -43f) * scale + centerPos,
                    Size = new Vector2(12f, 25f) * scale,
                    Color = color,
                    RotationOrScale = 4.7124f
                }, // [PortRight] SlantUp
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(65f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 75f) * scale,
                    Color = color,
                    RotationOrScale = 0f
                } // [PortRight] Ring
            };
            return textureSprites;
        }

        private static List<MySprite> DrawLeftConnector(Vector2 centerPos, float scale, Color color)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-145f, 0f) * scale + centerPos,
                    Size = new Vector2(75f, 100f) * scale,
                    Color = color,
                    RotationOrScale = 0f
                }, // [PortLeft] Body
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-97f, 0f) * scale + centerPos,
                    Size = new Vector2(50f, 75f) * scale,
                    Color = color,
                    RotationOrScale = 0f
                }, // [PortLeft] BodyWaist
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "RightTriangle",
                    Position = new Vector2(-85f, -43f) * scale + centerPos,
                    Size = new Vector2(25f, 12f) * scale,
                    Color = color,
                    RotationOrScale = 0f
                }, // [PortLeft] SlantUp
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "RightTriangle",
                    Position = new Vector2(-85f, 44f) * scale + centerPos,
                    Size = new Vector2(12f, 25f) * scale,
                    Color = color,
                    RotationOrScale = 1.5708f
                }, // [PortLeft] SlantDown
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-65f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 75f) * scale,
                    Color = color,
                    RotationOrScale = 0f
                } // [PortLeft] Ring
            };
            return textureSprites;
        }

        private static List<MySprite> DrawDockingArrows(Vector2 centerPos, float scale)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "Triangle",
                    Position = new Vector2(25f, 0f) * scale + centerPos,
                    Size = new Vector2(25f, 25f) * scale,
                    Color = ConnectingYellow,
                    RotationOrScale = 4.7124f
                }, // [Docking] indicatorRightArrow
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "Triangle",
                    Position = new Vector2(-25f, 0f) * scale + centerPos,
                    Size = new Vector2(25f, 25f) * scale,
                    Color = ConnectingYellow,
                    RotationOrScale = 1.5708f
                } // [Docking] indicatorLeftArrow
            };
            return textureSprites;
        }

        private static List<MySprite> DrawUndockArrows(Vector2 centerPos, float scale)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "Triangle",
                    Position = new Vector2(25f, 0f) * scale + centerPos,
                    Size = new Vector2(25f, 25f) * scale,
                    Color = DisconnectingBlue,
                    RotationOrScale = 1.5708f
                }, // [Undocking] indicatorRightArrow
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "Triangle",
                    Position = new Vector2(-25f, 0f) * scale + centerPos,
                    Size = new Vector2(25f, 25f) * scale,
                    Color = DisconnectingBlue,
                    RotationOrScale = 4.7124f
                } // [Undocking] indicatorLeftArrow
            };
            return textureSprites;
        }

        private static List<MySprite> DrawConnectionChain(Vector2 centerPos, float scale)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(10f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 21f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical RightCenter
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-10f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 21f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical LeftCenter
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(40f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 21f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical RightEdge
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-40f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 21f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical LeftEdge
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(25f, -8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal TopRight
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(25f, 8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal BottomRight
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-25f, 8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal BottomLeft
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-25f, -8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal TopLeft
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(0f, 0f) * scale + centerPos,
                    Size = new Vector2(35f, 5f) * scale,
                    Color = ConnectedGreen,
                    RotationOrScale = 0f
                } // ChainBarHorizontal CenterLink
            };
            return textureSprites;
        }

        private static List<MySprite> DrawTimeSinceLastDock(Vector2 centerPos, float scale, DockingPort port, Color color)
        {
            string dataText;
            Color textColor;
            if (port.GetTimeSinceLastDock() == TimeSpan.MinValue)
            {
                dataText = "No past dock data";
                textColor = ErrorRed;
            }
            else
            {
                dataText = FormatDockTime(port.GetTimeSinceLastDock());
                textColor = color;
            }
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = dataText,
                    Position = new Vector2(0f, 125f) * scale + centerPos,
                    Color = textColor,
                    FontId = "Debug",
                    RotationOrScale = 2f * scale
                }, // docktimer
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = "Undocked for:\n",
                    Position = new Vector2(0f, 78f) * scale + centerPos,
                    Color = color,
                    FontId = "Debug",
                    RotationOrScale = 1f * scale
                } // docktimerIntroduction
            };
            return textureSprites;
        }

        #endregion
    }
}