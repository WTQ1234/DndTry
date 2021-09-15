using EGamePlay;
using UnityEngine;


public class CardFlow_Finish : WorkFlow
{
    public override async void Startup()
    {
        base.Startup();
        Log.Debug("CombatFinishFlow Startup");

        GameObject.Destroy(GameObject.Find("CombatRoot"));

        await ET.TimeHelper.WaitAsync(100);

        Finish();
    }
}
