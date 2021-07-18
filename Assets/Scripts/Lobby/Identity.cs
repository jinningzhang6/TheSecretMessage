using UnityEngine;

public class Identity : MonoBehaviour
{
    // Start is called before the first frame update
    public int team;
    
    //private Identities identities;
       
    private void Start()
    {
        //identities = GameObject.FindObjectOfType<Identities>();
    }
    public void OnClick_IdentityToggle(bool ifChecked)
    {
        Identities identities = FindObjectOfType<Identities>();
        if (ifChecked)
        {
            identities.addIdentity(team);
        }
        else
        {
            identities.removeIdentity(team);
        }
    }
}
