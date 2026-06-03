using UnityEngine;

public interface Collectable
{
    Collider2D collider { get; }
    void Collect();
}
