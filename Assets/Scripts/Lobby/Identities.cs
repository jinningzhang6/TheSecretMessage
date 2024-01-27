using System.Collections.Generic;
using UnityEngine;

public class Identities : MonoBehaviour
{
    private List<int> identities = new List<int>() {'B','R','G'};

    public void addIdentity(int identity)
    {
        identities.Add(identity);
    }

    public void removeIdentity(int identity)
    {
        identities.Remove(identity);
    }

    public int[] getShuffledIdentities()
    {
        return Shuffle.ShuffleList<int>(identities.ToArray());
    }
}
