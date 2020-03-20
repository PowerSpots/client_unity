using UnityEngine;

/// <summary>
/// Follow3DObject的依赖项，用于指定在哪些3D Layers下UI应该follow
/// </summary>
public class Follow3DLayers : MonoBehaviour
{
    public LayerMask[] followLayers;
    public LayerMask[] dontFollowLayers;
}
