using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text texto;
    public Text CoracaoText;
    public float Quantidade;

    public static GameController instance;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void UpdateQuantidade(float value)
    {
        Quantidade += value;
        texto.text = Quantidade.ToString();
    }

    public void Coracao(float value)
    {
        CoracaoText.text = "x " + value.ToString();
    }
}
