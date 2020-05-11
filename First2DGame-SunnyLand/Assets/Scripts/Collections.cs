using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collections : MonoBehaviour
{
    public void Get()
    {
        FindObjectOfType<PlayerController>().Count();
        Destroy(gameObject);
    }
}
