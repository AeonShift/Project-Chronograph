//From this guide: https://pixelnest.io/tutorials/2d-game-unity/parallax-scrolling/

using UnityEngine;

public static class RendererExtensions
{
    public static bool IsVisisbleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}