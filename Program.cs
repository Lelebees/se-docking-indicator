using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.
        
        private readonly Color connectedGreen = new Color(0, 255, 0); // Nuclear Green
        private readonly Color disconnectingBlue = new Color(0, 255, 255); // Cyan
        private readonly Color connectingYellow = new Color(255, 255, 0); // Uranium Yellow
        private readonly Color errorRed = Color.Red; // It does not get redder then this.

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.
        }

        public void SetupDrawSurface(IMyTextSurface surface)
        {
            // Draw background color
            surface.ScriptBackgroundColor = new Color(0, 0, 0, 255);

            // Set content type
            surface.ContentType = ContentType.SCRIPT;

            // Set script to none
            surface.Script = "";
        }

        public void DrawSprites(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f)
        {
            DrawSprites(frame, centerPos, Color.White, scale);
        }

        public void DrawSprites(MySpriteDrawFrame frame, Vector2 centerPos, Color textColor, float scale = 1f)
        {
            frame.AddRange(DrawDockeeAnnouncement(centerPos, scale, Color.White));
            frame.AddRange(DrawRightConnector(centerPos, scale, connectedGreen));
            frame.AddRange(DrawLeftConnector(centerPos, scale, connectedGreen));
            frame.AddRange(DrawDockingPortName(centerPos, scale, Color.White));
            frame.AddRange(DrawDockingArrows(centerPos, scale));
            frame.AddRange(DrawUndockArrows(centerPos, scale));
            frame.AddRange(DrawConnectionChain(centerPos, scale));
        }

        #region Sprite Definitions

        private List<MySprite> DrawDockingPortName(Vector2 centerPos, float scale, Color color)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = "ALPHA",
                    Position = new Vector2(0f, -180f) * scale + centerPos,
                    Color = color,
                    FontId = "Debug",
                    RotationOrScale = 3.5f * scale
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

        private List<MySprite> DrawDockeeAnnouncement(Vector2 centerPos, float scale, Color color)
        {
            List<MySprite> textureSprites = new List<MySprite>
            {
                new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Alignment = TextAlignment.CENTER,
                    Data = "Large Grid 1234567",
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

        private List<MySprite> DrawRightConnector(Vector2 centerPos, float scale, Color color)
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

        private List<MySprite> DrawLeftConnector(Vector2 centerPos, float scale, Color color)
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

        private List<MySprite> DrawDockingArrows(Vector2 centerPos, float scale)
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
                    Color = connectingYellow,
                    RotationOrScale = 4.7124f
                }, // [Docking] indicatorRightArrow
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "Triangle",
                    Position = new Vector2(-25f, 0f) * scale + centerPos,
                    Size = new Vector2(25f, 25f) * scale,
                    Color = connectingYellow,
                    RotationOrScale = 1.5708f
                } // [Docking] indicatorLeftArrow
            };
            return textureSprites;
        }

        private List<MySprite> DrawUndockArrows(Vector2 centerPos, float scale)
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
                    Color = disconnectingBlue,
                    RotationOrScale = 1.5708f
                }, // [Undocking] indicatorRightArrow
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "Triangle",
                    Position = new Vector2(-25f, 0f) * scale + centerPos,
                    Size = new Vector2(25f, 25f) * scale,
                    Color = disconnectingBlue,
                    RotationOrScale = 4.7124f
                } // [Undocking] indicatorLeftArrow
            };
            return textureSprites;
        }

        private List<MySprite> DrawConnectionChain(Vector2 centerPos, float scale)
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
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical RightCenter
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-10f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 21f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical LeftCenter
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(40f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 21f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical RightEdge
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-40f, 0f) * scale + centerPos,
                    Size = new Vector2(5f, 21f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarVertical LeftEdge
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(25f, -8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal TopRight
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(25f, 8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal BottomRight
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-25f, 8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal BottomLeft
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(-25f, -8f) * scale + centerPos,
                    Size = new Vector2(30f, 5f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                }, // ChainBarHorizontal TopLeft
                new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Alignment = TextAlignment.CENTER,
                    Data = "SquareSimple",
                    Position = new Vector2(0f, 0f) * scale + centerPos,
                    Size = new Vector2(35f, 5f) * scale,
                    Color = connectedGreen,
                    RotationOrScale = 0f
                } // ChainBarHorizontal CenterLink
            };
            return textureSprites;
        }

        #endregion
    }
}