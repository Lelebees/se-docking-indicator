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

        public void DrawSprites(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f, float rotation = 0f, float colorScale = 1f)
        {
            float sin = (float)Math.Sin(rotation);
            float cos = (float)Math.Cos(rotation);
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(cos*-48f,sin*-48f)*scale+centerPos, new Vector2(75f,100f)*scale, new Color(1f*colorScale,1f*colorScale,1f*colorScale,1f), null, TextAlignment.CENTER, rotation)); // ConnectorBody
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(0f,0f)*scale+centerPos, new Vector2(50f,75f)*scale, new Color(1f*colorScale,1f*colorScale,1f*colorScale,1f), null, TextAlignment.CENTER, rotation)); // ConnectorBodyExtension
            frame.Add(new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(cos*12f-sin*-43f,sin*12f+cos*-43f)*scale+centerPos, new Vector2(25f,12f)*scale, new Color(1f*colorScale,1f*colorScale,1f*colorScale,1f), null, TextAlignment.CENTER, rotation)); // Connector ConnectUp
            frame.Add(new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(cos*12f-sin*44f,sin*12f+cos*44f)*scale+centerPos, new Vector2(12f,25f)*scale, new Color(1f*colorScale,1f*colorScale,1f*colorScale,1f), null, TextAlignment.CENTER, 1.5708f+rotation)); // Connector ConnectDown
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(cos*32f,sin*32f)*scale+centerPos, new Vector2(5f,75f)*scale, new Color(1f*colorScale,1f*colorScale,1f*colorScale,1f), null, TextAlignment.CENTER, rotation)); // Ring
        }

    }
}