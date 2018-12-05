using UnityEngine;

//Credit to Michael Garforth for this code bit on the unity community wiki http://wiki.unity3d.com/index.php?title=IsVisibleFrom;
public static class RendererExtensions
{
    //checks if a renderer is visible from a specific camera. Note that this does NOT take into account obstacles, so if an object is obscured
    //by another object, it is still "visible" from the camera
    public static bool isVisibleFrom(this Renderer renderer, Camera camera)
    {
        //calculates view box 
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        //tests if renderer is within view box
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
