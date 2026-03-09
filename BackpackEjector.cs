using System;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Unturned.Events;
using Rocket.Core.Logging;
using SDG.Unturned;
using Steamworks;

namespace BackpackEjector
{
    public class BackpackEjector : RocketPlugin<BackpackEjectorConfiguration>
    {
        protected override void Load()
        {
            Logger.Log("###############################");
            Logger.Log("BackpackEjector: Плагин успешно запущен!");
            Logger.Log("###############################");

            // Подписываемся на событие смерти через полный путь к классу
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        protected override void Unload()
        {
            // Отписываемся при выключении
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
            Logger.Log("BackpackEjector: Плагин выгружен.");
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (Configuration.Instance != null && Configuration.Instance.Enabled)
            {
                Logger.Log($"[BackpackEjector] Игрок {player.CharacterName} погиб. Событие обработано.");
                
                // Здесь будет логика появления рюкзака на земле, если нужно
            }
        }
    }
}
