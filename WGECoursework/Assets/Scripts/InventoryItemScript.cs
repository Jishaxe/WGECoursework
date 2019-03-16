using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemScript : MonoBehaviour
{
    public Text numberText;
    public RawImage image;
    public Texture2D tileTexture;

    public Block blockType;
    public int amount = 0;

    public void Start()
    {
        UpdateItem(Block.DIRT, 0);
    }

    public void UpdateItem(Block blockType, int amount)
    {
        this.amount = amount;
        this.blockType = blockType;

        if (amount == 0)
        {
            numberText.enabled = false;
            image.enabled = false;
        } else
        {
            numberText.enabled = true;
            numberText.text = amount.ToString();
            image.enabled = true;

            int x, y, width, height;

            Vector2 uvCoords = VoxelGenerator.Block2UV(blockType);

            x = (int)(uvCoords.x * tileTexture.width);
            y = (int)(uvCoords.y * tileTexture.height);
            width = (int)(tileTexture.width * 0.5f);
            height = (int)(tileTexture.height * 0.5f);

            Texture2D newTex = new Texture2D(width, height);

            Color[] pixels = tileTexture.GetPixels(x, y, width, height);
            newTex.SetPixels(pixels);
            newTex.Apply();

            image.texture = newTex;
        }
    }
}
