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
            Rocket.Core.Logging.Logger.Log("BackpackEjector: СИСТЕМА ВЫБРОСА АКТИВИРОВАНА");
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (Configuration.Instance == null || !Configuration.Instance.Enabled) return;

            // Используем стандартный класс инвентаря из Unturned напрямую
            PlayerInventory inventory = player.Player.inventory;
            
            // Индекс рюкзака в современных версиях Unturned — это PlayerInventory.BACKPACK (6)
            byte backpackPage = PlayerInventory.BACKPACK; 

            // Проверяем, есть ли у игрока вообще страница рюкзака
            if (inventory.items == null || inventory.items.Length <= backpackPage) return;

            Items backpackItems = inventory.items[backpackPage];

            if (backpackItems != null && backpackItems.getItemCount() > 0)
            {
                Rocket.Core.Logging.Logger.Log($"[Ejector] Игрок {player.CharacterName} погиб. В рюкзаке предметов: {backpackItems.getItemCount()}");

                // Позиция для спавна предметов (чуть выше земли, чтобы не провалились)
                Vector3 dropPosition = player.Position + new Vector3(0, 0.5f, 0);

                // Цикл с конца (как в твоем InventoryGuard)
                for (int i = backpackItems.getItemCount() - 1; i >= 0; i--)
                {
                    ItemJar jar = backpackItems.getItem((byte)i);
                    if (jar != null && jar.item != null)
                    {
                        // ВЫБРАСЫВАЕМ ПРЕДМЕТ
                        // true в конце означает, что предмет будет лежать на земле как обычный лут
                        ItemManager.dropItem(jar.item, dropPosition, false, true, true);

                        // УДАЛЯЕМ ИЗ ИНВЕНТАРЯ ТРУПА
                        inventory.removeItem(backpackPage, (byte)i);
                    }
                }
                
                // Принудительное обновление инвентаря для сервера
                inventory.updateItems(backpackPage, backpackItems);
            }
            else 
            {
                Rocket.Core.Logging.Logger.Log($"[Ejector] У игрока {player.CharacterName} пустой рюкзак или его нет.");
            }
        }
    }
}
