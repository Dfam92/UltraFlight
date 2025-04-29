using Fusion.Photon.Realtime;
using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class FusionInit : MonoBehaviour
{
    // ...

    // Start a NetworkRunner with a Game Mode and connect to the specified Region
    public Task<StartGameResult> StartSimulation(NetworkRunner runner, GameMode gameMode, string region)
    {

        var appSettings = BuildCustomAppSetting(region);

        return runner.StartGame(new StartGameArgs()
        {
            // ...
            GameMode = gameMode,
            CustomPhotonAppSettings = appSettings
        });
    }

    private FusionAppSettings BuildCustomAppSetting(string region, string customAppID = null, string appVersion = "1.0.0")
    {

        var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy(); ;

        appSettings.UseNameServer = true;
        appSettings.AppVersion = appVersion;

        if (string.IsNullOrEmpty(customAppID) == false)
        {
            appSettings.AppIdFusion = customAppID;
        }

        if (string.IsNullOrEmpty(region) == false)
        {
            appSettings.FixedRegion = region.ToLower();
        }

        // If the Region is set to China (CN),
        // the Name Server will be automatically changed to the right one
        // appSettings.Server = "ns.photonengine.cn";

        return appSettings;
    }
}
