using UnityEngine;

public class GrandSpeculumHealth : MonoBehaviour
{
    private SpriteChanger spriteChanger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteChanger = gameObject.GetComponentInParent<SpriteChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (GrandSpeculum.playerHealth.Health <= 50)
        //{
        spriteChanger.ChangeFrame(1);
        //}
    }
}
