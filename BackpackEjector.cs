using System;
using System.Reflection;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Unturned.Events;
using SDG.Unturned;
using UnityEngine;
using Steamworks;

namespace BackpackEjector
{
    public class BackpackEjector : RocketPlugin<BackpackEjectorConfiguration>
    {
        // Метод из твоего InventoryGuard для корректной подгрузки библиотек
        static BackpackEjector()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.Contains("Assembly-CSharp-firstpass"))
                    return Assembly.GetAssembly(typeof(ItemJar));
                return null;
            };
        }

        protected override void Load()
        {
            Rocket.Core.Logging.Logger.Log("###############################");
            Rocket.Core.Logging.Logger.Log("BackpackEjector: МОНИТОРИНГ РЮКЗАКА ЗАПУЩЕН!");
            Rocket.Core.Logging.Logger.Log("###############################");

            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
            Rocket.Core.Logging.Logger.Log("BackpackEjector: Плагин выгружен.");
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (Configuration.Instance == null || !Configuration.Instance.Enabled) return;

            // В Unturned: 0-1 Оружие, 2 Руки, 3-5 Одежда, 6 РЮКЗАК, 7 ЖИЛЕТ
            const byte backpackPage = 6; 

            // Безопасная проверка: одет ли рюкзак вообще
            if (player.Inventory.items.Length <= backpackPage) return;

            var backpackItems = player.Inventory.items[backpackPage];

            if (backpackItems != null && backpackItems.getItemCount() > 0)
            {
                Rocket.Core.Logging.Logger.Log($"[Ejector] Игрок {player.CharacterName} погиб. Выбрасываем содержимое рюкзака.");

                // Обратный цикл удаления (как в твоем примере), чтобы не ломать индексы
                for (int i = backpackItems.getItemCount() - 1; i >= 0; i--)
                {
                    var jar = backpackItems.getItem((byte)i);
                    if (jar != null && jar.item != null)
                    {
                        // 1. Спавним предмет на земле в позиции игрока
                        ItemManager.dropItem(jar.item, player.Position, false, true, true);

                        // 2. Удаляем предмет из инвентаря трупа
                        backpackItems.removeItem((byte)i);
                    }
                }
            }
        }
    }
}
