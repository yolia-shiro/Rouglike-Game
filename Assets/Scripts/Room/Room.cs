using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public int roomIndex;
    public int dis;
    public Vector3 position;
    public BitArray doorPos = new BitArray(4);
    public Text indexText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(int roomIndex, Vector3 position, int dis = 0)
    {
        this.roomIndex = roomIndex;
        this.dis = dis;
        this.position = position;
        indexText.text = dis.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //相机跟随
            CameraController.instance.SetTarget(transform);
        }
    }
}
