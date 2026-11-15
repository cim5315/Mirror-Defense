using UnityEngine;

public class GrandSpeculumHealth : MonoBehaviour
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
        if (GrandSpeculum.playerHealth.HealthPercentage() <= 0.25)
        {
            ChangeFrame(3);
        }
        else if (GrandSpeculum.playerHealth.HealthPercentage() <= 0.5)
        {
            ChangeFrame(2);
        }
        if (GrandSpeculum.playerHealth.HealthPercentage() <= 0.75)
        {
            ChangeFrame(1);
        }
        else
        {
            ChangeFrame(0);
        }
    }
    public void ChangeFrame(int frameIndex)
    {
        if (frameIndex >= 0 && frameIndex < spriteFrames.Length)
        {
            spriteRenderer.sprite = spriteFrames[frameIndex];
        }

    }
}
