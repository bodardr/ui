using DG.Tweening;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    private static Transform cam;

    [SerializeField]
    private UpdateType updateType;

    private void Awake()
    {
        if (!cam && Camera.main)
            cam = Camera.main.transform;
    }

    void Update()
    {
        if (updateType != UpdateType.Normal)
            return;

        UpdateBillboard();
    }

    private void FixedUpdate()
    {
        if (updateType != UpdateType.Fixed)
            return;

        UpdateBillboard();
    }

    private void LateUpdate()
    {
        if (updateType != UpdateType.Late)
            return;

        UpdateBillboard();
    }

    private void UpdateBillboard()
    {
        transform.rotation = cam!.rotation;
    }
}