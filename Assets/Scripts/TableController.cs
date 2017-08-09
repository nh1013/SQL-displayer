using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TableController : MonoBehaviour
{
    public GameObject tablePrefab;      // includes view port and scroll components
    public Text headerPrefab;           // used to update the header row
    public GameObject rowPrefab;        // used to store cells
    public Text cellPrefab;             // used to insert more data

    public RectTransform tableWindow;   // outer content tab
    public RectTransform headerRow;     // row which stores headers
    public RectTransform dataWindow;    // inner content tab of cells

    private DBManager manager;
    private int rowCount = 0;
    private string m_tableName;

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
        tableWindow = null;
    }

    public void SetName(string name) {
        m_tableName = name;
    }

    /// <summary>
    /// Destroy current header and remake with new fields
    /// </summary>
    public void SetupHeader(List<string> fields)
    {
        ClearHeader();
        foreach (string name in fields)
        {
            Text header = Instantiate(headerPrefab, headerRow);
            header.text = name;
        }
        UpdateWindow();
    }

    /// <summary>
    /// Add a single row to the table
    /// </summary>
    public void AddRow(List<string> data)
    {
        GameObject row = Instantiate(rowPrefab, dataWindow);
        foreach (string info in data)
        {
            Text cell = Instantiate(cellPrefab, row.transform);
            cell.text = info;
        }
        rowCount++;
        UpdateWindow();
    }

    /// <summary>
    /// Clear the header of fields
    /// </summary>
    public void ClearHeader()
    {
        for (int i = headerRow.childCount - 1; i >= 0; i--)
        {
            Destroy(headerRow.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Clear the table of data
    /// </summary>
    public void ClearData()
    {
        for (int i = dataWindow.childCount; i >= 0; i--)
        {
            Destroy(dataWindow.GetChild(i).gameObject);
        }
        rowCount = 0;
        UpdateWindow();
    }

    /// <summary>
    /// Update the size of windows
    /// </summary>
    public void UpdateWindow()
    {
        float headerWidth = 0;
        for (int i = headerRow.childCount - 1; i >= 0; i--)
        {
            headerWidth += headerRow.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
        }
        headerRow.sizeDelta = new Vector2(headerWidth, headerRow.sizeDelta.y);
        float dataHeight = 0;
        for (int i = dataWindow.childCount - 1; i >= 0; i--)
        {
            dataHeight += headerRow.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
        }
        dataWindow.sizeDelta  = new Vector2(headerWidth, dataHeight);
        tableWindow.sizeDelta = new Vector2(headerWidth, headerRow.sizeDelta.y + dataHeight);
        Debug.Log("header width = " + headerWidth + ", data height = " + dataHeight);
        Debug.Log("data window size = " + dataWindow.sizeDelta);
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
