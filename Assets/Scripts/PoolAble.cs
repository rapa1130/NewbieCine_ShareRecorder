using UnityEngine;
using UnityEngine.Pool;

public class PoolAble : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    virtual public void ReleaseObject()
    {
        Pool.Release(gameObject);
    }
}