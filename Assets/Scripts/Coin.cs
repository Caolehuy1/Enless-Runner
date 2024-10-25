using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Coin : MonoBehaviour
{
    [SerializeField] private BoxCollider BoxCollider;
    [SerializeField] private Vector3 newSize;
    [SerializeField] private float speed;

    private Vector3 InnitialSize;
    private Player player;
    private bool IsFly = false;

    private void Awake()
    {
        InnitialSize = BoxCollider.size;
        player = FindObjectOfType<Player>();

        
    }
    public void SetNewSize()
    {
        BoxCollider.size = newSize;
        
    }
    public void SetOldSize()
    {
        BoxCollider.size = InnitialSize;
    }

    public void Update()
    {
        if (player!= null&& IsFly == true )
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position,speed*Time.deltaTime);
            if (Vector3.Distance(transform.position, player.transform.position) <= 0.1f)
            { gameObject.SetActive(false); }
            }

    }
    public void SetIsFly(bool isfly)
    {
        IsFly = isfly;
    }

}
