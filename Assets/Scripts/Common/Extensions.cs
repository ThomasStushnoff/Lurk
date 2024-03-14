using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Resets the transform's position and rotation.
    /// </summary>
    /// <param name="transform">The transform to reset.</param>
    /// <param name="resetPosition">Whether to reset the position.</param>
    /// <param name="resetRotation">Whether to reset the rotation.</param>
    public static void Reset(this Transform transform,
        bool resetPosition = true,
        bool resetRotation = false)
    {
        if (resetPosition) transform.position = Vector3.zero;

        if (resetRotation) transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Returns true if the collider is a player.
    /// </summary>
    /// <param name="collider">The collider to check.</param>
    public static bool IsPlayer(this Collider collider)
        => collider.CompareTag("Player") || collider.gameObject.CompareLayer("Player");

    /// <summary>
    /// Returns true if the collision is a player.
    /// </summary>
    /// <param name="collision">The collision to check.</param>
    public static bool IsPlayer(this Collision collision)
        => collision.gameObject.CompareTag("Player") || collision.gameObject.CompareLayer("Player");

    /// <summary>
    /// Returns true if the game object's layer matches the layer name.
    /// </summary>
    /// <param name="gameObject">The game object to check.</param>
    /// <param name="layerName">The layer name to compare.</param>
    /// <returns>true or false.</returns>
    public static bool CompareLayer(this GameObject gameObject, string layerName)
        => gameObject.layer == LayerMask.NameToLayer(layerName);
}