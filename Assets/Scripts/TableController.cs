using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TableController : MonoBehaviour
{
    public Text headerPrefab;           // used to update the header row
    public GameObject rowPrefab;        // used to store cells
    public Text cellPrefab;             // used to insert more data

    public RectTransform fullContent;   // outer content rect
    public RectTransform dataViewport;  // cells UI mask
    public RectTransform dataContent;   // cells rect
    public RectTransform headerRow;     // row which stores headers

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
        GameObject row = Instantiate(rowPrefab, dataContent);
        foreach (string info in data)
        {
            Text cell = Instantiate(cellPrefab, row.transform);
            cell.text = info;
        }
        rowCount++;

        // Increase the height of dataContent
        dataContent.sizeDelta = new Vector2(dataContent.sizeDelta.x, 
            dataContent.sizeDelta.y + row.GetComponent<RectTransform>().sizeDelta.y);
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
        for (int i = dataContent.childCount; i >= 0; i--)
        {
            Destroy(dataContent.GetChild(i).gameObject);
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
        // table should be as wide as the fields require
        headerRow.sizeDelta = new Vector2(headerWidth, headerRow.sizeDelta.y);
        fullContent.sizeDelta = new Vector2(headerWidth, fullContent.sizeDelta.y);
        dataViewport.sizeDelta = new Vector2(headerWidth, dataViewport.sizeDelta.y);
        float dataHeight = 0;
        for (int i = dataContent.childCount - 1; i >= 0; i--)
        {
            dataHeight += headerRow.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
        }
        // data content rect needs to adjust height based on amount of data
        dataContent.sizeDelta  = new Vector2(headerWidth, dataHeight);
        Debug.Log("header width = " + headerWidth + ", data height = " + dataHeight);
        Debug.Log("data window size = " + dataContent.sizeDelta);
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
