using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;

namespace BackpackEjector
{
    public class BackpackEjector : RocketPlugin<BackpackEjectorConfiguration>
    {
        protected override void Load()
        {
            Logger.Log("###############################");
            Logger.Log("BackpackEjector запущен успешно!");
            Logger.Log("###############################");

            // Подписываемся на смерть игрока
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
            Logger.Log("BackpackEjector выгружен.");
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, Steamworks.CSteamID murderer)
        {
            if (Configuration.Instance.Enabled)
            {
                // Пока просто пишем в консоль, что событие сработало
                Logger.Log($"Игрок {player.CharacterName} умер. Система готова выкинуть рюкзак.");
            }
        }
    }
}
