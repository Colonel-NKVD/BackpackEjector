using System;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Core.Logging;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace BackpackEjector
{
    public class BackpackEjector : RocketPlugin<BackpackEjectorConfiguration>
    {
        protected override void Load()
        {
            Logger.Log("BackpackEjector: Система выброса вещей активирована!");
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        protected override void Unload()
        {
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            // Проверяем, включен ли плагин в конфиге
            if (Configuration.Instance == null || !Configuration.Instance.Enabled) return;

            // Индекс сумки (Backpack) в Unturned — это 2
            byte backpackIndex = 2;
            Items backpackItems = player.Inventory.getItemStack(backpackIndex);

            if (backpackItems != null && backpackItems.getItemCount() > 0)
            {
                Logger.Log($"Выбрасываем {backpackItems.getItemCount()} предметов из рюкзака игрока {player.CharacterName}");

                // Перебираем все вещи в рюкзаке
                for (byte i = 0; i < backpackItems.getItemCount(); i++)
                {
                    ItemJar itemJar = backpackItems.getItem(i);
                    if (itemJar != null)
                    {
                        // Создаем предмет в мире на месте смерти игрока
                        ItemManager.dropItem(itemJar.item, player.Position, false, true, true);
                    }
                }

                // Очищаем инвентарь рюкзака, чтобы вещи не дублировались
                for (byte i = 0; i < backpackItems.getItemCount(); i++)
                {
                    player.Inventory.removeItem(backpackIndex, 0);
                }
            }
        }
    }
}
