using UnityEngine;

public class Singleton : MonoBehaviour
{
    private static Singleton Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject  );
            return;
        }
    }
}
