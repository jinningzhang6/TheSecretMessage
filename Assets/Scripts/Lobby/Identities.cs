using System.Collections.Generic;
using UnityEngine;

public class Identities : MonoBehaviour
{
    private List<int> identities = new List<int>() {'B','R','G'};

    public void addIdentity(int identity)
    {
        //int[] inarr = new int[] { 1, 2, 3, };
        //inarr = Shuffle.ShuffleList<int>(inarr);
        //foreach(int i in inarr){
        //    Debug.Log(i);
        //}
        identities.Add(identity);
    } 

    public void removeIdentity(int identity)
    {
        identities.Remove(identity);
    }

    public void displayIdentities()
    {
        Debug.Log("----------------------");
        foreach(int c in identities)
        {
            Debug.Log("@@@@@@@@@@@@@@" + c);
        }
        Debug.Log("----------------------");
    }

    public int[] getShuffledIdentities()
    {
        return Shuffle.ShuffleList<int>(identities.ToArray());
    }
}
