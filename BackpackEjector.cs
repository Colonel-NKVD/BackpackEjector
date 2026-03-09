using System;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Rocket.Core.Logging;

namespace BackpackEjector
{
    public class BackpackEjector : RocketPlugin<Rocket.API.IRocketPluginConfiguration>
    {
        protected override void Load()
        {
            PlayerLife.onPlayerDied += OnPlayerDied;
            Rocket.Core.Logging.Logger.Log("BackpackEjector для Iron & Mud успешно запущен!");
        }

        protected override void Unload()
        {
            PlayerLife.onPlayerDied -= OnPlayerDied;
        }

        private void OnPlayerDied(PlayerLife life, EDeathCause cause, ELimb limb, Steamworks.CSteamID killer)
        {
            if (life == null || life.player == null) return;

            Player player = life.player;
            PlayerInventory inventory = player.inventory;

            // Индекс страницы рюкзака в Unturned — 5
            byte backpackPage = PlayerInventory.BACKPACK;

            // Проверяем, есть ли вообще рюкзак и предметы в нем
            if (inventory.items[backpackPage] == null) return;

            var itemsInBackpack = inventory.items[backpackPage].items;
            
            // Если рюкзак пуст, ничего не делаем
            if (itemsInBackpack == null || itemsInBackpack.Count == 0) return;

            Vector3 deathPosition = player.transform.position;

            // Копируем список, так как мы будем удалять предметы в процессе цикла
            List<ItemJar> toDrop = new List<ItemJar>(itemsInBackpack);

            foreach (ItemJar jar in toDrop)
            {
                // 1. Создаем предмет в мире на месте смерти игрока
                ItemManager.dropItem(jar.item, deathPosition, false, true, true);
            }

            // 2. Очищаем страницу рюкзака, чтобы предметы не остались у игрока после возрождения
            // Мы делаем это через удаление каждого предмета из инвентаря
            for (int i = (int)inventory.items[backpackPage].getItemCount() - 1; i >= 0; i--)
            {
                inventory.items[backpackPage].removeItem((byte)i);
            }

            Rocket.Core.Logging.Logger.Log($"Игрок {player.channel.owner.playerID.playerName} погиб. Содержимое рюкзака выброшено.");
        }
    }
}
