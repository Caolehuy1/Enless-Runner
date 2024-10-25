using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
        
{
    public GameObject[] obstacles;
    public Vector2 numberOfobstacles;
    public GameObject coin;
    public Vector2 numberOfCoin;
    public List<GameObject> newObstacles;
    public List<GameObject> newCoins;

    // Start is called before the first frame update
    void Start()
    {
        int newNumberOfObstacles = (int)Random.Range(numberOfobstacles.x, numberOfobstacles.y);
        int newNumberOfCoin = (int)Random.Range(numberOfCoin.x, numberOfCoin.y);

        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            newObstacles[i].SetActive(false);
        }
        for (int i = 0; i < newNumberOfCoin; i++)
        {
            newCoins.Add(Instantiate(coin, transform));
            newCoins[i].SetActive(false);
        }

        PositionateObstacles();
        PositionateCoin();
    }

    void PositionateObstacles()
    {
        for (int i = 0; i < newObstacles.Count; i++)
        {
            float posZmin = (274f / newObstacles.Count) + (274f / newObstacles.Count) * i;
            float posZmax = (274f / newObstacles.Count) + (274f / newObstacles.Count) * i + 1 ;
            newObstacles[i].transform.localPosition = new Vector3(0,0, Random.Range(posZmin, posZmax));
            newObstacles[i].SetActive(true);
            if (newObstacles[i].GetComponent<Rigidbody>() != null)

                newObstacles[i].GetComponent<ChangeLane>().PositionLane();
            

        }
    }
    void PositionateCoin()
    {
        float minZpos = 10f;
        for (int i = 0;i < newCoins.Count;i++)
        {
            float maxZPos = minZpos + 5f;
            float randomZpos = Random.Range(minZpos, maxZPos);
            newCoins[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZpos);
            newCoins[i].SetActive(true);
            newCoins[1].GetComponent<ChangeLane>().PositionLane();
            minZpos = randomZpos + 1;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other .CompareTag("Player"))
        {
            other.GetComponent<Player>().IncreaseSpeed();
            transform.position = new Vector3(0, 0, transform.position.z+274 * 2);
            PositionateObstacles();
            PositionateCoin();

        }
    }


}
