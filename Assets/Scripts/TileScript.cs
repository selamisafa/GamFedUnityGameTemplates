using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool haveShipPart;
    public bool isTurned;
    public int length;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CheckShipPart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckShipPart()
    {
        //AmiralBattiGameManager.instance.AskQuestion(this);

        AmiralBattiGameManager.instance.RevealBlock(this);     
    }
}
