using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class DeerSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup Deer Animation")]
    static void Setup()
    {
        string[] guids = {
            "9cf761462b783f949a9e375734f83a0c",
            "631226a66d39a06499c06c394df37a07",
            "b9c91f19d8672594b9de06fa1d45d7ec",
            "42b76fc37052b5d46bfde8986938af44",
            "6c0d5195975868c47893d19f9a1d5703",
            "9acb14a1ee3c3e24e97c16372d353b5c",
            "c1051137012d0704c93720c5bb5bf2a3",
            "5fbf8cba01a3db0468e27b18a29ba4aa"
        };

        Sprite[] sprites = new Sprite[guids.Length];
        for (int i = 0; i < guids.Length; i++)
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[i]));

        // Create AnimationClip
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 12f;

        EditorCurveBinding binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            keys[i] = new ObjectReferenceKeyframe();
            keys[i].time = i / 12f;
            keys[i].value = sprites[i];
        }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        if (!Directory.Exists("Assets/Animations")) Directory.CreateDirectory("Assets/Animations");
        AssetDatabase.CreateAsset(clip, "Assets/Animations/DeerRun.anim");

        // Create AnimatorController
        var ctrl = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/DeerController.controller");
        ctrl.AddMotion(clip);

        // Create deer GameObjects as children of Santa
        var santa = GameObject.Find("pixellab-Santa-claus-sleigh-1774719472748_0");
        if (santa == null) { Debug.LogError("Santa not found!"); return; }

        Sprite eastSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath("e8bcfe37d61bbab43afa476a3c6c3654"));

        for (int d = 0; d < 2; d++)
        {
            GameObject deer = new GameObject("Deer_" + (d + 1));
            deer.transform.SetParent(santa.transform, false);
            deer.transform.localPosition = new Vector3(-1.5f - d * 1.2f, -0.1f, 0f);
            deer.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

            SpriteRenderer sr = deer.AddComponent<SpriteRenderer>();
            sr.sprite = eastSprite;
            sr.sortingOrder = 2;

            Animator anim = deer.AddComponent<Animator>();
            anim.runtimeAnimatorController = ctrl;
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Deer setup complete!");
    }
}
