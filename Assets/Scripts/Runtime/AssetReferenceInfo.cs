// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;

// [Serializable]
// public class AssetReferenceInfo
// {
//     public AssetReference reference = null;
//     public bool shouldInstantiate = false;
//     public Transform parent = null;
//     // [HideInInspector]
//     // public bool completed = false;

//     // private 

//     public void Load(Action callback)
//     {
//         if (this.shouldInstantiate)
//         {
//             Addressables.InstantiateAsync(this.reference, this.parent).Completed += (op) =>
//             {
//                 // this.completed = true;
//                 callback();
//             };
//         }
//         else
//         {
//             Addressables.LoadAssetAsync(this.reference).Completed += (op) =>
//             {
//                 // this.completed = true;
//                 callback();
//             }
//         }

//     }
// }

// [Serializable]
// public class AssetReferenceInfoGroup
// {
//     public List<AssetReferenceInfo> referenceInfoList;

//     public void LoadAll(Action groupCallback)
//     {
//         foreach (var referenceInfo in this.referenceInfoList)
//         {
//             referenceInfo.Load(() =>
//             {
//                 this.OnSingleCompleted(groupCallback);
//             });
//         }
//     }

//     private void OnSingleCompleted(Action groupCallback)
//     {

//     }

//     private void OnGroupComplete(Action groupCallback)
//     {

//     }
// }
