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
        private readonly List<DockingPort> ports = new List<DockingPort>();
        private readonly List<Indicator> indicatorPanels = new List<Indicator>();
        private TimeSpan timeSinceLastUpdate = TimeSpan.Zero;


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

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
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
                ports.Add(new DockingPort(connector));
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
                }

                Indicator indicator = new Indicator(outputSurface, connectedDock);
                DockingPort dock = ports.Find(port => port.getName() == connectedDock);
                Echo("Dock name: " + connectedDock);
                if (dock == null)
                {
                    Echo($"Dock {connectedDock} not found for Indicator {block.CustomName}");
                    continue;
                }

                indicatorPanels.Add(indicator);
                dock.OnDockingPortStatusChange += indicator.UpdateDockingPortIndication;
            }

            foreach (DockingPort port in ports)
            {
                port.UpdateState();
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            timeSinceLastUpdate = timeSinceLastUpdate.Add(Runtime.TimeSinceLastRun);
            if (timeSinceLastUpdate.Milliseconds >= 50)
            {
                foreach (DockingPort port in ports)
                {
                    port.UpdateState();
                }

                timeSinceLastUpdate = TimeSpan.Zero;
            }

            foreach (Indicator panel in indicatorPanels)
            {
                DockingPort dock = ports.Find(port => port.getName() == panel.trackedPort);
                panel.UpdateDockingPortIndication(dock, new DockingPortStatusChangeEventArguments());
            }
        }
    }
}