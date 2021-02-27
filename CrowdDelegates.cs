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
using System.Threading;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace ControlValley
{
    public delegate CrowdResponse CrowdDelegate(CrowdRequest req);

    public class CrowdDelegates
    {
        private static readonly List<KeyValuePair<string, int>> downgradeFishingRods = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("Iridium Rod", 2),
            new KeyValuePair<string, int>("Fiberglass Rod", 0),
            new KeyValuePair<string, int>("Bamboo Pole", 1)
        };

        private static readonly List<KeyValuePair<string, int>> upgradeFishingRods = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("Training Rod", 0),
            new KeyValuePair<string, int>("Bamboo Pole", 2),
            new KeyValuePair<string, int>("Fiberglass Rod", 3)
        };

        public static CrowdResponse DowngradeAxe(CrowdRequest req)
        {
            return DoDowngrade(req, "Axe");
        }

        public static CrowdResponse DowngradeFishingRod(CrowdRequest req)
        {
            int id = req.GetReqID();

            foreach (KeyValuePair<string, int> downgrade in downgradeFishingRods)
            {
                Tool tool = Game1.player.getToolFromName(downgrade.Key);
                if (tool != null)
                {
                    tool.UpgradeLevel = downgrade.Value;
                    UI.ShowInfo(String.Format("{0} downgraded {1}'s Fishing Rod", req.GetReqViewer(), Game1.player.Name));

                    return new CrowdResponse(id);
                }
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Fishing Rod is already at the lowest upgrade level");
        }

        public static CrowdResponse DowngradeHoe(CrowdRequest req)
        {
            return DoDowngrade(req, "Hoe");
        }

        public static CrowdResponse DowngradePickaxe(CrowdRequest req)
        {
            return DoDowngrade(req, "Pickaxe");
        }

        public static CrowdResponse DowngradeTrashCan(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.trashCanLevel > 0)
            {
                Interlocked.Decrement(ref Game1.player.trashCanLevel);
                UI.ShowInfo(String.Format("{0} downgraded {1}'s Trash Can", req.GetReqViewer(), Game1.player.Name));
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Trash Can is already at the lowest upgrade level";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DowngradeWateringCan(CrowdRequest req)
        {
            return DoDowngrade(req, "Watering Can");
        }

        public static CrowdResponse DowngradeWeapon(CrowdRequest req)
        {
            int id = req.GetReqID();

            if (WeaponClass.Club.DoDowngrade() || WeaponClass.Sword.DoDowngrade() || WeaponClass.Dagger.DoDowngrade())
            {
                UI.ShowInfo(String.Format("{0} downgraded {1}'s Weapon", req.GetReqViewer(), Game1.player.Name));
                return new CrowdResponse(id);
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Weapon is already at the lowest upgrade level");
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
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                Game1.player.Stamina = max;
                UI.ShowInfo(String.Format("{0} fully energized {1}", req.GetReqViewer(), Game1.player.Name));
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at maximum energy";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 508)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at the highest energy maximum";
            }
            else
            {
                stamina += 34;
                Game1.player.MaxStamina = stamina;
                Game1.player.Stamina = stamina;
                UI.ShowInfo(String.Format("{0} gave {1} a Stardrop", req.GetReqViewer(), Game1.player.Name));
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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
            string message = "";

            if (Interlocked.Exchange(ref Game1.player.health, Game1.player.maxHealth) == 0)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo(String.Format("{0} fully healed {1}", req.GetReqViewer(), Game1.player.Name));

            return new CrowdResponse(req.GetReqID(), status, message);
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
            string message = "";

            if (Interlocked.Exchange(ref Game1.player.health, 0) == 0)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo(String.Format("{0} killed {1}", req.GetReqViewer(), Game1.player.Name));

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse PassOut(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > -16)
            {
                Game1.player.Stamina = -16;
                UI.ShowInfo(String.Format("{0} made {1} pass out", req.GetReqViewer(), Game1.player.Name));
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently passed out";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 270)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at lowest energy maximum";
            }
            else
            {
                stamina -= 34;
                Game1.player.MaxStamina = stamina;
                if (Game1.player.Stamina > stamina)
                    Game1.player.Stamina = stamina;
                UI.ShowInfo(String.Format("{0} removed a Stardrop from {1}", req.GetReqViewer(), Game1.player.Name));
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpawnBat(CrowdRequest req)
        {
            return DoSpawn(req, new Bat(GetRandomNear(), 20));
        }

        public static CrowdResponse SpawnFly(CrowdRequest req)
        {
            return DoSpawn(req, new Fly(GetRandomNear()));
        }

        public static CrowdResponse SpawnGhost(CrowdRequest req)
        {
            return DoSpawn(req, new Ghost(GetRandomNear()));
        }

        public static CrowdResponse SpawnLavaBat(CrowdRequest req)
        {
            return DoSpawn(req, new Bat(GetRandomNear(), 100));
        }

        public static CrowdResponse SpawnFrostBat(CrowdRequest req)
        {
            return DoSpawn(req, new Bat(GetRandomNear(), 60));
        }

        public static CrowdResponse SpawnSerpent(CrowdRequest req)
        {
            return DoSpawn(req, new Serpent(GetRandomNear()));
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

        public static CrowdResponse UpgradeFishingRod(CrowdRequest req)
        {
            int id = req.GetReqID();

            foreach (KeyValuePair<string, int> upgrade in upgradeFishingRods)
            {
                Tool tool = Game1.player.getToolFromName(upgrade.Key);
                if (tool != null)
                {
                    tool.UpgradeLevel = upgrade.Value;
                    UI.ShowInfo(String.Format("{0} upgraded {1}'s Fishing Rod", req.GetReqViewer(), Game1.player.Name));

                    return new CrowdResponse(id);
                }
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Fishing Rod is already at the highest upgrade level");
        }

        public static CrowdResponse UpgradeHoe(CrowdRequest req)
        {
            return DoUpgrade(req, "Hoe");
        }

        public static CrowdResponse UpgradePickaxe(CrowdRequest req)
        {
            return DoUpgrade(req, "Pickaxe");
        }

        public static CrowdResponse UpgradeTrashCan(CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.trashCanLevel < 4)
            {
                Interlocked.Increment(ref Game1.player.trashCanLevel);
                UI.ShowInfo(String.Format("{0} upgraded {1}'s Trash Can", req.GetReqViewer(), Game1.player.Name));
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Trash Can is already at the highest upgrade level";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeWeapon(CrowdRequest req)
        {
            int id = req.GetReqID();

            if (WeaponClass.Club.DoUpgrade() || WeaponClass.Sword.DoUpgrade() || WeaponClass.Dagger.DoUpgrade())
            {
                UI.ShowInfo(String.Format("{0} upgraded {1}'s Weapon", req.GetReqViewer(), Game1.player.Name));
                return new CrowdResponse(id);
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Weapon is already at the highest upgrade level");
        }

        public static CrowdResponse UpgradeWateringCan(CrowdRequest req)
        {
            return DoUpgrade(req, "Watering Can");
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
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = String.Format("{0}'s {1} is already at the lowest upgrade level", Game1.player.Name, toolName);
            }
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

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoEnergizeBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                stamina += percent * max;
                Game1.player.Stamina = (stamina > max) ? max : stamina;
                UI.ShowInfo(String.Format("{0} energized {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at maximum energy";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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
            string message = "";

            int max = Game1.player.maxHealth;
            int health = (int)Math.Floor(percent * max) + Game1.player.health;
            if (Interlocked.Exchange(ref Game1.player.health, (health > max) ? max : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo(String.Format("{0} healed {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoHurtBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int health = Game1.player.health - (int)Math.Floor(percent * Game1.player.maxHealth);
            if (Interlocked.Exchange(ref Game1.player.health, (health < 0) ? 0 : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already dead";
            }
            else
                UI.ShowInfo(String.Format("{0} hurt {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoRemoveMoney(CrowdRequest req, int amount)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int money = Game1.player.Money;
            if (money > 0)
            {
                money -= amount;
                Game1.player.Money = (money < 0) ? 0 : money;
                UI.ShowInfo(String.Format("{0} removed {2} gold from {1}", req.GetReqViewer(), Game1.player.Name, amount.ToString()));
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " currently has no money";
            }
            
            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoSpawn(CrowdRequest req, Monster monster)
        {
            Game1.player.currentLocation.addCharacter(monster);
            UI.ShowInfo(String.Format("{0} spawned a {1} near {2}", req.GetReqViewer(), monster.Name, Game1.player.Name));
            return new CrowdResponse(req.GetReqID());
        }

        private static CrowdResponse DoTireBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > 0)
            {
                stamina -= percent * Game1.player.MaxStamina;
                Game1.player.Stamina = (stamina < 0) ? 0 : stamina;
                UI.ShowInfo(String.Format("{0} tired {1} by {2}%", req.GetReqViewer(), Game1.player.Name, (int)Math.Floor(100 * percent)));
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already passed out";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoUpgrade(CrowdRequest req, string toolName, int max = 4)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = String.Format("{0}'s {1} is already at the highest upgrade level", Game1.player.Name, toolName);
            }
            else
            {
                int level = tool.UpgradeLevel;
                if (level == max)
                    status = CrowdResponse.Status.STATUS_FAILURE;
                else
                {
                    tool.UpgradeLevel = level + 1;
                    UI.ShowInfo(String.Format("{0} upgraded {1}'s {2}", req.GetReqViewer(), Game1.player.Name, toolName));
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoWarp(CrowdRequest req, string name, int targetX, int targetY)
        {
            try
            {
                Game1.warpFarmer(name, targetX, targetY, true);
            }
            catch (Exception e)
            {
                UI.ShowError(e.Message);
            }
            UI.ShowInfo(String.Format("{0} warped {1} to the {2}", req.GetReqViewer(), Game1.player.Name, name));
            return new CrowdResponse(req.GetReqID());
        }

        private static readonly float MAX_RADIUS = 400;

        private static Vector2 GetRandomNear()
        {
            Random random = new Random();
            return Game1.player.Position + new Vector2(
                (float)((random.NextDouble() * 2 * MAX_RADIUS) - MAX_RADIUS),
                (float)((random.NextDouble() * 2 * MAX_RADIUS) - MAX_RADIUS));
        }
    }
}
