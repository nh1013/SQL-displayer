using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TableController : MonoBehaviour
{
    public Canvas m_table;
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

        // instantiate a Canvas
        // instantiate the panel/scroll prefab and all
        // instantiate the rowNum + 1 panels in vertical layout
        // extra panel is for field names
        // instantiate each panel with fieldNum text fields and fill with text
    }

    // Update is called once per frame
    void Update()
    {
        //tableText.text = newText;
    }

    /// <summary>
    /// Clean up all data
    /// </summary>
    void OnDestroy() { }

}
