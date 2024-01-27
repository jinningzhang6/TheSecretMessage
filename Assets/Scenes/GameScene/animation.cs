using UnityEngine;

public class animation : MonoBehaviour
{
    [SerializeField]
    private DrawCardAnimation _prefabDrawingCard;

    public GameObject imageClicker;

/*    public void MyAnimation()
    {
        var myObject = Instantiate(_prefabDrawingCard, imageClicker.transform.localPosition, Quaternion.identity);
        myObject.transform.SetParent(transform, false);
        StartCoroutine(myObject.AssigningCardAnimation(new Vector3(50, 61)));
    }*/

}
