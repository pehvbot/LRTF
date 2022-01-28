using TestFlightAPI;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_Resource : LRTFFailureBase
    {

        public ConfigNode resourceTitles = new ConfigNode();

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasNode("MODULE"))
                if (node.GetNode("MODULE").HasNode("RESOURCETITLES"))
                {
                    resourceTitles = node.GetNode("MODULE").GetNode("RESOURCETITLES");
                }
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
        }
    }
}