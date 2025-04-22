using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public delegate void DockingPortStatusChangeEventHandler(DockingPort source, DockingPortStatusChangeEventArguments eventArguments);

    public class DockingPortStatusChangeEventArguments : EventArgs
    {
        
    }
        
    public class DockingPort
    {
        private readonly IMyShipConnector connector;
        private string dockName;
        private DockingPortState state;
        public event DockingPortStatusChangeEventHandler OnDockingPortStatusChange;
        

        public DockingPort(IMyShipConnector connector)
        {
            this.connector = connector;
            this.state = ConvertConnectorStatusToState(connector.Status);
            MyIni customData = new MyIni();
            if (!customData.TryParse(this.connector.CustomData))
            {
                dockName = this.connector.CustomName;
                return;
            }
            dockName = customData.Get(Program.GetDockingPortSectionIdentifier(), "port name").ToString(this.connector.CustomName);
        }

        public bool isDamagedOrDisabled()
        {
            // TODO: also check for superficial damage
            return connector.IsWorking;
        }

        public string getName()
        {
            return dockName;
        }

        public DockingPortState getState()
        {
            return this.state;
        }

        public void UpdateState()
        {
            DockingPortState newState = ConvertConnectorStatusToState(connector.Status);
            if (state == newState)
            {
                return;
            }

            state = newState;
            OnDockingPortStatusChange?.Invoke(this, new DockingPortStatusChangeEventArguments());
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
    }
}