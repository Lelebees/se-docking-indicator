using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DockingPort
    {
        private readonly IMyShipConnector connector;
        private readonly string dockName;
        private DateTime lastDockTime = DateTime.MinValue;

        public DockingPort(IMyShipConnector connector)
        {
            this.connector = connector;
            MyIni customData = new MyIni();
            if (!customData.TryParse(this.connector.CustomData))
            {
                dockName = this.connector.CustomName;
                return;
            }

            dockName = customData.Get(Program.GetDockingPortSectionIdentifier(), "port name")
                .ToString(this.connector.CustomName);
        }

        public bool isDamagedOrDisabled()
        {
            // TODO: also check for superficial damage
            return connector.IsWorking;
        }

        public string GetName()
        {
            return dockName;
        }

        public DockingPortState GetState()
        {
            if (connector.Status == MyShipConnectorStatus.Connected)
            {
                lastDockTime = DateTime.Now;
            }
            return ConvertConnectorStatusToState(connector.Status);
        }

        private static DockingPortState ConvertConnectorStatusToState(MyShipConnectorStatus status)
        {
            switch (status)
            {
                case MyShipConnectorStatus.Connected:
                    return DockingPortState.Docked;
                case MyShipConnectorStatus.Connectable:
                    return DockingPortState.Docking;
                case MyShipConnectorStatus.Unconnected:
                default:
                    return DockingPortState.Disconnected;
            }
        }

        public string GetNameOfConnectedGrid()
        {
            if (connector.Status != MyShipConnectorStatus.Connected)
            {
                // Maybe throw error? idk :)
                return null;
            }

            return connector.OtherConnector.CubeGrid.CustomName;
        }

        public TimeSpan GetTimeSinceLastDock()
        {
            return DateTime.Now.Subtract(lastDockTime);
        }
    }
}