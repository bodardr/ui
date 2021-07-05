using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Prompt Pool", menuName = "Object Pool/Prompt")]
public class PromptPool : ScriptableObjectPool<InteractionPrompt>
{
    private Canvas canvas;
    private CanvasScaler canvasScaler;
    private GameObject promptCanvasGO;

    [RuntimeInitializeOnLoadMethod]
    public static void InitializeOnLoad()
    {
        var all = Resources.FindObjectsOfTypeAll<PromptPool>();

        foreach (var promptPool in all)
        {
            promptPool.OnDisable();
            promptPool.OnEnable();
        }
    }

    protected override void InstantiatePool()
    {
        if (!Application.isPlaying)
            return;

        promptCanvasGO = new GameObject("Prompt Canvas", typeof(Canvas), typeof(CanvasScaler));

        canvas = promptCanvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasScaler = promptCanvasGO.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        DontDestroyOnLoad(promptCanvasGO);

        base.InstantiatePool();
    }

    protected override void DestroyPool()
    {
        base.DestroyPool();

#if UNITY_EDITOR
        DestroyImmediate(promptCanvasGO);
#else
        Destroy(promptCanvas);
#endif

        promptCanvasGO = null;
    }

    public override void Retrieve(PoolableObject<InteractionPrompt> poolable)
    {
        canvasScaler.StartCoroutine(poolable.Content.Hide().Then(RetrieveAfterAnimation(poolable)));
    }

    protected override PoolableObject<InteractionPrompt> InstantiateSingleObject(int index = 0)
    {
        var go = Instantiate(prefab, promptCanvasGO.transform);
        go.name = $"{prefab.name} {(index > 0 ? $"{index} " : "")}(Pooled)";
        go.SetActive(false);

        return new PoolableObject<InteractionPrompt>(go.GetComponent<InteractionPrompt>(), this);
    }

    private IEnumerator RetrieveAfterAnimation(PoolableObject<InteractionPrompt> poolable)
    {
        base.Retrieve(poolable);
        yield return null;
    }
}