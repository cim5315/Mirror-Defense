using UnityEngine;

public class GrandSpeculum : MonoBehaviour
{
    private int health;
    private int maxHealth = 100;
    
    public int Health
    {
        get { return health; }
        
        set 
        { 
            health = value;
            if (health < 0)
            {
                health = 0;
            }
            else if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsBroken()
    {
        if (health == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
