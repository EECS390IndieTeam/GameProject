using UnityEngine;

/// <summary>
/// Defines an interface for a transition from one menu canvas to another.
/// </summary>
public interface IMenuTransistion
{
    /// <summary>
    /// This function should move the camera from one menu to another.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Transistion source destination compbination are not supported.
    /// </exception>
    /// <param name="from">Point A</param>
    /// <param name="to">Point B</param>
    void Transistion(Canvas from, Canvas to);
}
