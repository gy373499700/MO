using UnityEngine;

public class sdRandomLight : MonoBehaviour
{
    public sdLightAnimation[] LigthAnimationList;
    public float minTime;
    public float maxTime;

    float nextTime = 0;
    int nextIndex = 0;
    void DoRandom()
    {
        nextTime = Random.Range(minTime, maxTime);
        nextIndex = Random.Range(0, LigthAnimationList.Length);
    }

    void Start()
    {
        if (LigthAnimationList != null)
            DoRandom();
    }

    void Update()
    {
        if (nextTime > 0)
        {
            nextTime -= Time.deltaTime;
            if (nextTime <= 0)
            {
                LigthAnimationList[nextIndex].enabled = true;
                DoRandom();
            }
        }
    }
}
