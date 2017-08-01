using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TableController : MonoBehaviour
{
    private DBManager manager;
    private TextMesh tableText;
    private string newText = "";

    // Use this for initialization
    void Start()
    {
        GameObject managerObject = GameObject.FindWithTag("Manager");
        if (managerObject != null)
        {
            manager = managerObject.GetComponent<DBManager>();
        }
        if (manager == null)
        {
            Debug.Log("Cannot find 'Manager' script");
        }
        tableText = this.GetComponent<TextMesh>();
        newText = manager.GetAllData("Player", 4);
        tableText.text = newText;
    }

    // Update is called once per frame
    void Update()
    {
        //tableText.text = newText;
    }
}
