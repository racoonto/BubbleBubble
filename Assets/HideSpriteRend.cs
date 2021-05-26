using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpriteRend : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}