using Rocket.API;

namespace BackpackEjector
{
    public class BackpackEjectorConfiguration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public float EjectForce;

        public void LoadDefaults()
        {
            Enabled = true;
            EjectForce = 15f;
        }
    }
}
