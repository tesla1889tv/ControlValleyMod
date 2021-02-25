/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TheTexanTesla
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ControlValley
{
    class ControlClient
    {
        public static readonly string CV_HOST = "127.0.0.1";
        public static readonly int CV_PORT = 51337;

        private Dictionary<string, CrowdDelegate> Delegate { get; set; }
        private IPEndPoint Endpoint { get; set; }
        private bool Running { get; set; }
        private Socket Socket { get; set; }

        public ControlClient()
        {
            Endpoint = new IPEndPoint(IPAddress.Parse(CV_HOST), CV_PORT);
            Running = true;

            Delegate = new Dictionary<string, CrowdDelegate>()
            {
                {"downgrade_axe", CrowdDelegates.DowngradeAxe},
                {"downgrade_hoe", CrowdDelegates.DowngradeHoe},
                {"downgrade_pickaxe", CrowdDelegates.DowngradePickaxe},
                {"energize_10", CrowdDelegates.Energize10},
                {"energize_25", CrowdDelegates.Energize25},
                {"energize_50", CrowdDelegates.Energize50},
                {"energize_full", CrowdDelegates.EnergizeFull},
                {"give_money_100", CrowdDelegates.GiveMoney100},
                {"give_money_1000", CrowdDelegates.GiveMoney1000},
                {"give_money_10000", CrowdDelegates.GiveMoney10000},
                {"give_stardrop", CrowdDelegates.GiveStardrop},
                {"heal_10", CrowdDelegates.Heal10},
                {"heal_25", CrowdDelegates.Heal25},
                {"heal_50", CrowdDelegates.Heal50},
                {"heal_full", CrowdDelegates.HealFull},
                {"hurt_10", CrowdDelegates.Hurt10},
                {"hurt_25", CrowdDelegates.Hurt25},
                {"hurt_50", CrowdDelegates.Hurt50},
                {"kill", CrowdDelegates.Kill},
                {"passout", CrowdDelegates.PassOut},
                {"remove_money_100", CrowdDelegates.RemoveMoney100},
                {"remove_money_1000", CrowdDelegates.RemoveMoney1000},
                {"remove_money_10000", CrowdDelegates.RemoveMoney10000},
                {"remove_stardrop", CrowdDelegates.RemoveStardrop},
                {"tire_10", CrowdDelegates.Tire10},
                {"tire_25", CrowdDelegates.Tire25},
                {"tire_50", CrowdDelegates.Tire50},
                {"upgrade_axe", CrowdDelegates.UpgradeAxe},
                {"upgrade_hoe", CrowdDelegates.UpgradeHoe},
                {"upgrade_pickaxe", CrowdDelegates.UpgradePickaxe},
                {"warp_beach", CrowdDelegates.WarpBeach},
                {"warp_desert", CrowdDelegates.WarpDesert},
                {"warp_farm", CrowdDelegates.WarpFarm},
                {"warp_mountain", CrowdDelegates.WarpMountain},
            };
        }

        private void ClientLoop()
        {
            UI.ShowInfo("Connected to Crowd Control");

            try
            {
                while (Running)
                {
                    CrowdRequest req = CrowdRequest.Recieve(Socket);
                    if (req == null) continue;

                    string code = req.GetReqCode();
                    try
                    {
                        CrowdResponse res = Delegate[code](req);
                        if (res == null)
                        {
                            new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Request error for '" + code + "'").Send(Socket);
                        }

                        res.Send(Socket);
                    }
                    catch (KeyNotFoundException)
                    {
                        new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Invalid request '" + code + "'").Send(Socket);
                    }
                }
            }
            catch (Exception)
            {
                UI.ShowError("Disconnected from Crowd Control");
                Socket.Close();
            }
        }

        public void Loop()
        {
            while (Running)
            {
                UI.ShowInfo("Attempting to connect to Crowd Control");

                try
                {
                    Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                        ClientLoop();
                    else
                        UI.ShowError("Failed to connect to Crowd Control");
                    Socket.Close();
                }
                catch (Exception e)
                {
                    UI.ShowError(e.GetType().Name);
                    UI.ShowError("Failed to connect to Crowd Control");
                }

                Thread.Sleep(10000);
            }
        }

        public void Stop()
        {
            Running = false;
        }
    }
}
