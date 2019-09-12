using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePrefabController : MonoBehaviour {

    [SerializeField] private SpriteRenderer SpriteRenderer = null;

    [SerializeField] public Sprite SpriteNorth = null;
    [SerializeField] public Sprite SpriteEast = null;
    [SerializeField] public Sprite SpriteSouth = null;
    [SerializeField] public Sprite SpriteWest = null;

    public SpriteRenderer GetSpriteRenderer() {
        return SpriteRenderer;
    }

    public Sprite GetSpriteForRotation(int rotation) {
        rotation = rotation % 4;
        switch (rotation) {
            case 0:
                return SpriteNorth;
            case 1:
                return SpriteEast;
            case 2:
                return SpriteSouth;
            case 3:
                return SpriteWest;
        }

        return SpriteNorth;
    }

    public void SetRotation(int rotation) {
        SpriteRenderer.sprite = GetSpriteForRotation(rotation);
    }
}
