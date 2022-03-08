using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneLoader : MonoBehaviour
{
    public AssetReference mainSceneReference;

    private IEnumerator Start()
    {
        var handle = Addressables.LoadSceneAsync(mainSceneReference);
        yield return handle;
    }
}
