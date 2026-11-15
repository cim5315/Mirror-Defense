using UnityEngine;
using TMPro;

public class Currency : MonoBehaviour
{
    private int shards;
    public static Currency mainCurrency;
    [SerializeField] private TMP_Text currencyText;
    void Awake()
    {
        mainCurrency = this;
    }
    public int Shards
    {
        get { return shards; }
        
        set { shards = value; }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shards = 50;
    }

    // Update is called once per frame
    void Update()
    {
        currencyText.text = "@X" + shards.ToString("D3");
    }

    /// <summary>
    /// Checks if you have enough shards and spends them if you do
    /// </summary>
    /// <param name="cost">The cost of the item</param>
    /// <returns>If you have enough shards</returns>
    public bool HaveEnoughShards(int cost)
    {
        if (cost > shards)
        {
            return false;
        }
        else
        {
            shards -= cost;
            return true;
        }
    }
}
