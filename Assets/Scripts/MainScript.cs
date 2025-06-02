using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StreamingAssetsHelper.InitAssetManager();
        StreamingAssetsHelper.ReadAllBytes("hello.txt");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
