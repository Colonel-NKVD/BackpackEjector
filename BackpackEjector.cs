using System;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Rocket.Core.Logging;
using Steamworks; // Добавили эту строку

namespace BackpackEjector
{
    public class BackpackEjector : RocketPlugin<Rocket.API.IRocketPluginConfiguration>
    {
        protected override void Load()
        {
            PlayerLife.onPlayerDied += OnPlayerDied;
            Rocket.Core.Logging.Logger.Log("BackpackEjector (Iron & Mud) успешно запущен!");
        }

        protected override void Unload()
        {
            PlayerLife.onPlayerDied -= OnPlayerDied;
        }

        private void OnPlayerDied(PlayerLife life, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            if (life == null || life.player == null) return;

            Player player = life.player;
            PlayerInventory inventory = player.inventory;

            byte backpackPage = PlayerInventory.BACKPACK;

            if (inventory.items[backpackPage] == null) return;

            var itemsInBackpack = inventory.items[backpackPage].items;
            
            if (itemsInBackpack == null || itemsInBackpack.Count == 0) return;

            Vector3 deathPosition = player.transform.position;
            List<ItemJar> toDrop = new List<ItemJar>(itemsInBackpack);

            foreach (ItemJar jar in toDrop)
            {
                ItemManager.dropItem(jar.item, deathPosition, false, true, true);
            }

            for (int i = (int)inventory.items[backpackPage].getItemCount() - 1; i >= 0; i--)
            {
                inventory.items[backpackPage].removeItem((byte)i);
            }

            Rocket.Core.Logging.Logger.Log($"Боец {player.channel.owner.playerID.playerName} пал. Содержимое рюкзака выброшено.");
        }
    }
}
