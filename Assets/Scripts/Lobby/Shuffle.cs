using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    public static Type[] ShuffleList<Type>(Type[] source)
    {

        for (int t = 0; t < source.Length; t++)
        {
            Type tmp = source[t];
            int r = Random.Range(t, source.Length);
            source[t] = source[r];
            source[r] = tmp;
        }
        return source;
    }
}