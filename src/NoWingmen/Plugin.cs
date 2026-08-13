using BepInEx;
using BepInEx.Logging;

namespace NoWingmen;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BaseUnityPlugin
{
    private static ManualLogSource _logger;

    private void Awake()
    {
        _logger = Logger;
    }
}