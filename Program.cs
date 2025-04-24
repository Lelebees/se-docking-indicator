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
        private const string DefaultCustomData = "[" + SectionIdentifier + "]\n";
        private const string SectionIdentifier = "PortIndicatorConfig";
        private const string IndicatorSectionIdentifier = "PortIndicator";
        private const string DockingPortSectionIdentifier = "DockingPort";
        private readonly MyIni configDataParser = new MyIni();
        private readonly Dictionary<string, DockingPort> ports = new Dictionary<string, DockingPort>();
        private readonly List<Indicator> indicatorPanels = new List<Indicator>();


        public Program()
        {
            if (Me.CustomData == String.Empty)
            {
                Me.CustomData = DefaultCustomData;
            }

            MyIniParseResult result;
            if (!configDataParser.TryParse(Me.CustomData, out result))
            {
                throw new Exception(result.ToString());
            }

            InitializeDockingPorts();
            InitializePortIndicators();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public static string GetDockingPortSectionIdentifier()
        {
            return DockingPortSectionIdentifier;
        }

        private void InitializeDockingPorts()
        {
            List<IMyShipConnector> configuredConnectors = new List<IMyShipConnector>();
            GridTerminalSystem.GetBlocksOfType(configuredConnectors,
                connector => MyIni.HasSection(connector.CustomData, DockingPortSectionIdentifier) &&
                             connector.IsSameConstructAs(Me));
            foreach (IMyShipConnector connector in configuredConnectors)
            {
                DockingPort port = new DockingPort(connector);
                ports.Add(port.GetName(), port);
            }
        }

        private void InitializePortIndicators()
        {
            List<IMyTerminalBlock> outputBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(outputBlocks,
                block => MyIni.HasSection(block.CustomData, IndicatorSectionIdentifier) && block.IsSameConstructAs(Me));
            MyIni blockConfigParser = new MyIni();
            foreach (IMyTerminalBlock block in outputBlocks)
            {
                MyIniParseResult configParseResult;
                if (!blockConfigParser.TryParse(block.CustomData, out configParseResult))
                {
                    throw new Exception(configParseResult.ToString());
                }

                IMyTextSurface outputSurface;
                IMyTextSurface surface = block as IMyTextSurface;
                if (surface != null)
                {
                    outputSurface = surface;
                }
                else if (block is IMyTextSurfaceProvider)
                {
                    int surfaceNumber = blockConfigParser.Get(IndicatorSectionIdentifier, "screen number")
                        .ToInt32();
                    outputSurface = ((IMyTextSurfaceProvider)block).GetSurface(surfaceNumber);
                }
                else
                {
                    continue;
                }

                string connectedDock = null;
                blockConfigParser.Get(IndicatorSectionIdentifier, "connected port").TryGetString(out connectedDock);
                if (connectedDock == null)
                {
                    Echo("connected port property not found!");
                    continue;
                }

                DockingPort dock = ports[connectedDock];
                if (dock == null)
                {
                    Echo($"Dock {connectedDock} not found for Indicator {block.CustomName}");
                    continue;
                }
                indicatorPanels.Add(new Indicator(outputSurface, dock));
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            foreach (Indicator panel in indicatorPanels)
            {
                panel.UpdateDockingPortIndication();
            }
        }
    }
}