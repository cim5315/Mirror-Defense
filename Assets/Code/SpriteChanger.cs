using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] spriteFrames;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeFrame(int frameIndex)
    {
        if (frameIndex >= 0 && frameIndex < spriteFrames.Length)
        {
            spriteRenderer.sprite = spriteFrames[frameIndex];
        }

    }
}
