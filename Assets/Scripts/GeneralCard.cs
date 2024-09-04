using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Compiler;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class GeneralCard : MonoBehaviour
{
    public string Name, Faction;
    public float Power;
    public CardClass Type;
    public List<AttackClass> Attack;
    public RawImage Imag_Des;
    public GameObject Text_Des_Img;
    public TextMeshProUGUI Text_Des;
    public Deck Deck;
    public bool invoke,effects = false;
    public effect effect;
    public int players;
    public CardAdministrator Card;
    public bool InEscena = true;
    private void Start()
    {
        Imag_Des = GameObject.FindGameObjectWithTag("BigCard").GetComponent<RawImage>();
        Text_Des_Img = GameObject.FindGameObjectWithTag("TextD");
        Text_Des = GameObject.FindGameObjectWithTag("Text_Des").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(!InEscena)
        {
            Destroy(gameObject);
        }
        if(invoke && !effects && effect != null)
        {
            GameObject.FindGameObjectWithTag("Admin").GetComponent<GameManager>().DeterminateContext();
            effect.Action(GameObject.FindGameObjectWithTag("Admin").GetComponent<GameManager>().context);
            effects = true;
        }
    }
    public void Create(CardAdministrator carta,Deck player,int Player)
    {
        Deck = player;
        players = Player;
        Name = carta.Name;
        Faction = carta.Faction;
        Power = carta.Power;
        Type = carta.Type;
        Attack = carta.AttackClasses;
        effect = carta.effect;
        Card = carta;

        gameObject.name = Name;
        gameObject.transform.localScale = new Vector3(0.185000002f, 0.170000002f, 1);

        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<BoxCollider2D>();

        if(File.Exists(Application.dataPath + "/Resources/Images/" + Name + ".jpg"))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + Name);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Random");
        }
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(4f, 7f);
    }

    private void OnMouseEnter()
    {
        Imag_Des.transform.localScale = Vector3.one;
        Text_Des_Img.transform.localScale = Vector3.one;
        Imag_Des.texture = GetComponent<SpriteRenderer>().sprite.texture;
        string ataques = "";
        foreach(AttackClass s in Attack)
        {
            ataques += s.ToString() + " ";
        }
        Text_Des.text = "Name: " + Name + "\n" + "Type: " + Type + "\n" + "Faction: " + Faction + "\n" + "Power: " + Power + "\n" + "Attack: " + ataques;
    }

    private void OnMouseExit()
    {
        Imag_Des.transform.localScale = Vector3.zero;
        Text_Des_Img.transform.localScale = Vector3.zero;
    }

    private void OnMouseDown()
    {
        Deck.Select_Invoke(gameObject);
    }
}
