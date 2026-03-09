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
        // Тот самый способ решения проблем с библиотеками через AssemblyResolve
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
            Rocket.Core.Logging.Logger.Log("BackpackEjector: Режим ВЫБРОСА рюкзака активен.");
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (Configuration.Instance == null || !Configuration.Instance.Enabled) return;

            // 2 — это индекс страницы рюкзака в инвентаре Unturned
            const byte backpackPage = 2;
            var backpackItems = player.Inventory.items[backpackPage];

            if (backpackItems != null && backpackItems.getItemCount() > 0)
            {
                Rocket.Core.Logging.Logger.Log($"[Ejector] Выбрасываем содержимое рюкзака игрока {player.CharacterName}");

                // Используем обратный цикл, как в твоем InventoryGuard, чтобы не сбить индексы
                for (int i = backpackItems.getItemCount() - 1; i >= 0; i--)
                {
                    var jar = backpackItems.getItem((byte)i);
                    if (jar != null)
                    {
                        // 1. Выбрасываем предмет на землю (как в твоем примере)
                        ItemManager.dropItem(jar.item, player.Position, false, true, true);

                        // 2. Удаляем из инвентаря
                        backpackItems.removeItem((byte)i);
                    }
                }
            }
        }
    }
}
