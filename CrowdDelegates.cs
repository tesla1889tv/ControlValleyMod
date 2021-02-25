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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StardewValley;

namespace ControlValley
{
    public delegate CrowdResponse CrowdDelegate(CrowdRequest req);

    public class CrowdDelegates
    {
        public static CrowdResponse DowngradeAxe(CrowdRequest req)
        {
            return DoDowngrade(req, "Axe");
        }

        public static CrowdResponse DowngradeHoe(CrowdRequest req)
        {
            return DoDowngrade(req, "Hoe");
        }

        public static CrowdResponse DowngradePickaxe(CrowdRequest req)
        {
            return DoDowngrade(req, "Pickaxe");
        }

        public static CrowdResponse Energize10(CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.1f);
        }

        public static CrowdResponse Energize25(CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.25f);
        }

        public static CrowdResponse Energize50(CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.5f);
        }

        public static CrowdResponse EnergizeFull(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                Game1.player.Stamina = max;
                UI.ShowInfo(String.Format("{0} fully energized {1}", req.GetReqViewer(), Game1.player.Name));
            }
            else
                status = CrowdResponse.Status.STATUS_FAILURE;

            return new CrowdResponse(req.GetReqID(), status);
        }

        public static CrowdResponse GiveMoney100(CrowdRequest req)
        {
            return DoGiveMoney(req, 100);
        }

        public static CrowdResponse GiveMoney1000(CrowdRequest req)
        {
            return DoGiveMoney(req, 1000);
        }

        public static CrowdResponse GiveMoney10000(CrowdRequest req)
        {
            return DoGiveMoney(req, 10000);
        }

        public static CrowdResponse GiveStardrop(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            int stamina = Game1.player.MaxStamina;
            if (stamina == 508)
                status = CrowdResponse.Status.STATUS_FAILURE;
            else
            {
                stamina += 34;
                Game1.player.MaxStamina = stamina;
                Game1.player.Stamina = stamina;
                UI.ShowInfo(String.Format("{0} gave {1} a Stardrop", req.GetReqViewer(), Game1.player.Name));
            }

            return new CrowdResponse(req.GetReqID(), status);
        }

        public static CrowdResponse Heal10(CrowdRequest req)
        {
            return DoHealBy(req, 0.1f);
        }

        public static CrowdResponse Heal25(CrowdRequest req)
        {
            return DoHealBy(req, 0.25f);
        }

        public static CrowdResponse Heal50(CrowdRequest req)
        {
            return DoHealBy(req, 0.5f);
        }

        public static CrowdResponse HealFull(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            if (Interlocked.Exchange(ref Game1.player.health, Game1.player.maxHealth) == 0)
                status = CrowdResponse.Status.STATUS_FAILURE;
            else
                UI.ShowInfo(String.Format("{0} fully healed {1}", req.GetReqViewer(), Game1.player.Name));

            return new CrowdResponse(req.GetReqID(), status);
        }

        public static CrowdResponse Hurt10(CrowdRequest req)
        {
            return DoHurtBy(req, 0.1f);
        }

        public static CrowdResponse Hurt25(CrowdRequest req)
        {
            return DoHurtBy(req, 0.25f);
        }

        public static CrowdResponse Hurt50(CrowdRequest req)
        {
            return DoHurtBy(req, 0.5f);
        }

        public static CrowdResponse Kill(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            if (Interlocked.Exchange(ref Game1.player.health, 0) == 0)
                status = CrowdResponse.Status.STATUS_FAILURE;
            else
                UI.ShowInfo(String.Format("{0} killed {1}", req.GetReqViewer(), Game1.player.Name));
            return new CrowdResponse(req.GetReqID(), status);
        }

        public static CrowdResponse PassOut(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            float stamina = Game1.player.Stamina;
            if (stamina > -15)
            {
                Game1.player.Stamina = -15;
                UI.ShowInfo(String.Format("{0} made {1} pass out", req.GetReqViewer(), Game1.player.Name));
            }
            else
                status = CrowdResponse.Status.STATUS_FAILURE;

            return new CrowdResponse(req.GetReqID(), status);
        }

        public static CrowdResponse RemoveMoney100(CrowdRequest req)
        {
            return DoRemoveMoney(req, 100);
        }

        public static CrowdResponse RemoveMoney1000(CrowdRequest req)
        {
            return DoRemoveMoney(req, 1000);
        }

        public static CrowdResponse RemoveMoney10000(CrowdRequest req)
        {
            return DoRemoveMoney(req, 10000);
        }

        public static CrowdResponse RemoveStardrop(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            int stamina = Game1.player.MaxStamina;
            if (stamina == 270)
                status = CrowdResponse.Status.STATUS_FAILURE;
            else
            {
                stamina -= 34;
                Game1.player.MaxStamina = stamina;
                if (Game1.player.Stamina > stamina)
                    Game1.player.Stamina = stamina;
                UI.ShowInfo(String.Format("{0} removed a Stardrop from {1}", req.GetReqViewer(), Game1.player.Name));
            }

            return new CrowdResponse(req.GetReqID(), status);
        }

        public static CrowdResponse Tire10(CrowdRequest req)
        {
            return DoTireBy(req, 0.1f);
        }

        public static CrowdResponse Tire25(CrowdRequest req)
        {
            return DoTireBy(req, 0.25f);
        }

        public static CrowdResponse Tire50(CrowdRequest req)
        {
            return DoTireBy(req, 0.5f);
        }

        public static CrowdResponse UpgradeAxe(CrowdRequest req)
        {
            return DoUpgrade(req, "Axe");
        }

        public static CrowdResponse UpgradeHoe(CrowdRequest req)
        {
            return DoUpgrade(req, "Hoe");
        }

        public static CrowdResponse UpgradePickaxe(CrowdRequest req)
        {
            return DoUpgrade(req, "Pickaxe");
        }

        public static CrowdResponse WarpBeach(CrowdRequest req)
        {
            return DoWarp(req, "Beach", 20, 4);
        }

        public static CrowdResponse WarpDesert(CrowdRequest req)
        {
            return DoWarp(req, "Desert", 35, 43);
        }

        public static CrowdResponse WarpFarm(CrowdRequest req)
        {
            return DoWarp(req, "Farm", 48, 7);
        }

        public static CrowdResponse WarpMountain(CrowdRequest req)
        {
            return DoWarp(req, "Mountain", 31, 20);
        }

        private static CrowdResponse DoDowngrade(CrowdRequest req, string toolName)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
                status = CrowdResponse.Status.STATUS_FAILURE;
            else
            {
                int level = tool.UpgradeLevel;
                if (level == 0)
                    status = CrowdResponse.Status.STATUS_FAILURE;
                else
                {
                    tool.UpgradeLevel = level - 1;
                    UI.ShowInfo(String.Format("{0} downgraded {1}'s {2}", req.GetReqViewer(), Game1.player.Name, toolName));
                }
            }

            return new CrowdResponse(req.GetReqID(), status);
        }

        private static CrowdResponse DoEnergizeBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                stamina += percent * max;
                Game1.player.Stamina = (stamina > max) ? max : stamina;
                UI.ShowInfo(String.Format("{0} energized {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));
            }
            else
                status = CrowdResponse.Status.STATUS_FAILURE;

            return new CrowdResponse(req.GetReqID(), status);
        }

        private static CrowdResponse DoGiveMoney(CrowdRequest req, int amount)
        {
            Game1.player.addUnearnedMoney(amount);
            UI.ShowInfo(String.Format("{0} gave {1} {2} gold", req.GetReqViewer(), Game1.player.Name, amount.ToString()));
            return new CrowdResponse(req.GetReqID());
        }

        private static CrowdResponse DoHealBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            int max = Game1.player.maxHealth;
            int health = (int)Math.Floor(percent * max) + Game1.player.health;
            if (Interlocked.Exchange(ref Game1.player.health, (health > max) ? max : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
            }
            else
                UI.ShowInfo(String.Format("{0} healed {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));

            return new CrowdResponse(req.GetReqID(), status);
        }

        private static CrowdResponse DoHurtBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            int health = Game1.player.health - (int)Math.Floor(percent * Game1.player.maxHealth);
            if (Interlocked.Exchange(ref Game1.player.health, (health < 0) ? 0 : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
            }
            else
                UI.ShowInfo(String.Format("{0} hurt {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));

            return new CrowdResponse(req.GetReqID(), status);
        }

        private static CrowdResponse DoRemoveMoney(CrowdRequest req, int amount)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            int money = Game1.player.Money;
            if (money > 0)
            {
                money -= amount;
                Game1.player.Money = (money < 0) ? 0 : money;
                UI.ShowInfo(String.Format("{0} removed {2} gold from {1}", req.GetReqViewer(), Game1.player.Name, amount.ToString()));
            }
            else
                status = CrowdResponse.Status.STATUS_FAILURE;
            
            return new CrowdResponse(req.GetReqID(), status);
        }

        private static CrowdResponse DoTireBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            float stamina = Game1.player.Stamina;
            if (stamina > 0)
            {
                stamina -= percent * Game1.player.MaxStamina;
                Game1.player.Stamina = (stamina < 0) ? 0 : stamina;
                UI.ShowInfo(String.Format("{0} tired {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));
            }
            else
                status = CrowdResponse.Status.STATUS_FAILURE;

            return new CrowdResponse(req.GetReqID(), status);
        }

        private static CrowdResponse DoUpgrade(CrowdRequest req, string toolName)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
                status = CrowdResponse.Status.STATUS_FAILURE;
            else
            {
                int level = tool.UpgradeLevel;
                if (level == 4)
                    status = CrowdResponse.Status.STATUS_FAILURE;
                else
                {
                    tool.UpgradeLevel = level + 1;
                    UI.ShowInfo(String.Format("{0} upgraded {1}'s {2}", req.GetReqViewer(), Game1.player.Name, toolName));
                }
            }

            return new CrowdResponse(req.GetReqID(), status);
        }

        private static CrowdResponse DoWarp(CrowdRequest req, string name, int targetX, int targetY)
        {
            Game1.warpFarmer(name, targetX, targetY, false);
            UI.ShowInfo(String.Format("{0} warped {1} to the {2}", req.GetReqViewer(), Game1.player.Name, name));
            return new CrowdResponse(req.GetReqID());
        }
    }
}
