//----------------------------------------------
// Modified SQLiter
// Copyright © 2014 OuijaPaw Games LLC
// now open to public domain
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.IO;
using System.Text;


/// <summary>
/// The DBManager holds the database connection, and is the point of contact for all Table classes to run SQL statements
/// It is used to manage (create, manipulate, update, destroy) Tables of the DBManager's selected database
/// </summary>
public class DBManager : MonoBehaviour
{
	public bool DebugMode = false;
    public RectTransform tablePrefab;
    public Canvas screenCanvas;

    // Location of database - this will be set during Awake as to stop Unity 5.4 error regarding initialization before scene is set
    // file should show up in the Unity inspector after a few seconds of running it the first time
    private static string _sqlDBLocation = "";
    private string SQL_DB_NAME = "mondial";                // Table name and DB actual file location

	/// <summary>
	/// DB objects
	/// </summary>
	private IDbConnection _connection = null;
	private IDbCommand _command = null;
	private IDataReader _reader = null;
	private string _sqlString;
        

	/// <summary>
	/// Awake will initialize the connection.  
	/// RunAsyncInit is just for show.  You can do the normal SQLiteInit to ensure that it is
	/// initialized during the Awake() phase and everything is ready during the Start() phase
	/// </summary>
	void Awake()
	{
		if (DebugMode)
			Debug.Log("--- Awake ---");
	}

	/// <summary>
	/// Unity Start
	/// </summary>
	void Start()
	{
		if (DebugMode)
			Debug.Log("--- Start ---");
        
        // just for testing, comment/uncomment to play with it
        // note that it MUST be invoked after SQLite has initialized, 2-3 seconds later usually.  1 second is cutting it too close
        //Invoke("Test", 3);
    }

	/// <summary>
	/// Uncomment if you want to see the time it takes to do things
	/// </summary>
	//void Update()
	//{
	//    Debug.Log(Time.time);
	//}

    /// <summary>
    /// Clean up SQLite Connections, anything else
    /// </summary>
    void OnDestroy()
	{
		SQLiteClose();
	}

	/// <summary>
	/// Example using the Loom to run an asynchronous method on another thread so SQLite lookups
	/// do not block the main Unity thread
	/// </summary>
	public void RunAsyncInit()
	{
		LoomManager.Loom.QueueOnMainThread(() =>
		{
			SQLiteInit();
		});
	}

	/// <summary>
	/// Basic initialization of SQLite
	/// </summary>
	public void SQLiteInit()
	{
        // here is where we set the file location
        // ------------ IMPORTANT ---------
        // - during builds, this is located in the project root - same level as Assets/Library/obj/ProjectSettings
        // - during runtime (Windows at least), this is located in the SAME directory as the executable
        // you can play around with the path if you like, but build-vs-run locations need to be taken into account
        _sqlDBLocation = "URI=file:Databases/" + SQL_DB_NAME + ".db";
        Debug.Log("SQLiter - Opening SQLite Connection at " + _sqlDBLocation);

		_connection = new SqliteConnection(_sqlDBLocation);
		_command = _connection.CreateCommand();
		_connection.Open();

        // Some speed increasing settings
		// WAL = write ahead logging, very huge speed increase
		_command.CommandText = "PRAGMA journal_mode = WAL;";
		_command.ExecuteNonQuery();

		_command.CommandText = "PRAGMA journal_mode";
		_reader = _command.ExecuteReader();
		if (DebugMode && _reader.Read())
			Debug.Log("SQLiter - WAL value is: " + _reader.GetString(0));
		_reader.Close();
        _connection.Close();

        /*
		// more speed increases
		_command.CommandText = "PRAGMA synchronous = OFF";
		_command.ExecuteNonQuery();

		// and some more
		_command.CommandText = "PRAGMA synchronous";
		_reader = _command.ExecuteReader();
		if (DebugMode && _reader.Read())
			Debug.Log("SQLiter - synchronous value is: " + _reader.GetInt32(0));
		_reader.Close();
        */
    }

    /// <summary>
    /// Change the database the manager is connected to
    /// Closes all tables and renews all SQLite connections
    /// </summary>
    public void ChangeDatabase(string newDBName)
    {
        if (DebugMode)
            Debug.Log("--- Changing database from " + SQL_DB_NAME + " to " + newDBName + " ---");
        // close/delete any existing Tables
        GameObject[] tables;
        tables = GameObject.FindGameObjectsWithTag("Table");
        foreach (GameObject table in tables)
        {
            Destroy(table);
        }
        // close connection to current database
        SQLiteClose();
        // establish connection to new database
        SQL_DB_NAME = newDBName;
        SQLiteInit();
    }


    /// <summary>
    /// returns an array of tables in the current database
    /// </summary>
    public List<string> GetTables()
    {
        if (DebugMode)
            Debug.Log("--- Retrieving tables from " + SQL_DB_NAME + " ---");
        
        // executes query that select names of all tables in master table of the database
        _connection.Open();
        _command.CommandText =  "SELECT name FROM sqlite_master " +
                                "WHERE type = 'table'" +
                                "ORDER BY 1";
        _reader = _command.ExecuteReader();

        List<string> tables = new List<string> { };
        while (_reader.Read())
        {
            if (!_reader.IsDBNull(0))
            {
                tables.Add(_reader.GetString(0));
            }
            else
            {
                tables.Add("");
            }
        }
        if (DebugMode)
        {
            foreach (string table in tables)
            {
                Debug.Log(table);
            }
        }
        // close connections
        _reader.Close();
        _connection.Close();

        return tables;
    }

    /// <summary>
    /// Creates the target Table for display, if found in the database
    /// </summary>
    public RectTransform CreateTable(string tableName)
    {
        if (DebugMode)
            Debug.Log("--- Creating Table instance " + tableName + " ---");
        // check if the table exists in the database. If it doesn't exist we abort.
        _connection.Open();
        _command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
        _reader = _command.ExecuteReader();
        if (!_reader.Read())
        {
            Debug.Log("SQLiter - Could not find SQLite table " + tableName);
            _reader.Close();
            return null;
        }
        if (DebugMode)
            Debug.Log("SQLiter - SQLite table " + tableName + " was found");
        _reader.Close();
        _connection.Close();

        // Instantiate new Table
        Debug.Log("SQLiter - Creating new table: " + tableName);
        RectTransform table = Instantiate(tablePrefab, screenCanvas.transform);
        table.name = "table " + tableName;

        // give table its name, allow it to initialise
        TableController tabCon = table.GetComponent<TableController>();
        tabCon.tableName = tableName;

        // fill up the table
        FillAllData(tabCon);
        return table;
    }

    #region Test
    /// <summary>
    /// Clean up SQLite Connections, anything else
    /// </summary>
    void Test()
    {
        if (DebugMode)
            Debug.Log("--- Test Invoked ---");
        /*
        string res = "";
        res = GetAllData("Player", 4);
        if (DebugMode) {
            Debug.Log("--- Test Results ---");
            Debug.Log(res);
        }
        */
        CreateTable("Country");
    }
    #endregion

    #region Insert
    /// <summary>
    /// Inserts an entry into the database
    /// http://www.sqlite.org/lang_insert.html
    /// name must be unique, it's our primary key
    /// </summary>
    public void InsertWord(string table, string key, string columns, string values)
	{
		name = name.ToLower();

		// note - this will replace any item that already exists, overwriting them.  
		// normal INSERT without the REPLACE will throw an error if an item already exists
		_sqlString = "INSERT OR REPLACE INTO " + table
			+ " (" + columns +") VALUES ("
			+ "'" + key + "',"  // note that string values need quote or double-quote delimiters
			+ values + ");";

		if (DebugMode)
			Debug.Log(_sqlString);
		ExecuteNonQuery(_sqlString);
	}

	#endregion

	#region Get Whole Table

	/// <summary>
	/// Select and retrieve an entire table.
	/// </summary>
	public void FillAllData(TableController table)
    {
        _connection.Open();
        _command.CommandText = "PRAGMA table_info(" + table.tableName + ")";
        _reader = _command.ExecuteReader();
        List<string> fields = new List<string> { };
        while (_reader.Read())
        {
            fields.Add((string)_reader.GetValue(1));
        }
        _reader.Close();
        table.SetupHeader(fields);

        _command.CommandText = "SELECT * FROM " + table.tableName;
        _reader = _command.ExecuteReader();
        while (_reader.Read())
        {
            List<string> data = new List<string> { };
            for (int index = 0; index < _reader.FieldCount; ++index)
            {
                if (!_reader.IsDBNull(index))
                {
                    data.Add(_reader.GetString(index));
                }
                else
                {
                    data.Add("");
                }
            }

            table.AddRow(data);
        }
        // close connections
        _reader.Close();
        _connection.Close();
    }

	/// <summary>
	/// Supply the column and the value you're trying to find, and it will use the primary key to query the result
	/// </summary>
	/// <param name="column"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public string QueryString(string table, string column, string keyCol, string key)
	{
		string text = "Not Found";
		_connection.Open();
		_command.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + keyCol + "='" + key + "'";
		_reader = _command.ExecuteReader();
		if (_reader.Read())
			text = _reader.GetString(0);
		else
			Debug.Log("QueryString - nothing to read...");
		_reader.Close();
		_connection.Close();
		return text;
	}
		
	#endregion

	#region Update / Replace Values
	/// <summary>
	/// A 'Set' method that will set a column value for a specific entry, using their unique primary key
	/// Remember strings need single/double quotes around their values
	/// </summary>
	public void SetValue(string table, string keyCol, string key, string column, string value)
	{
		ExecuteNonQuery("UPDATE OR REPLACE " + table + " SET " + column + "='" + value + "' WHERE " + keyCol + "='" + key + "'");
	}

	#endregion

	#region Delete

	/// <summary>
	/// Basic delete
	/// </summary>
	/// <param name="wordKey"></param>
	public void DeleteEntries(string table, string criterion)
	{
		ExecuteNonQuery("DELETE FROM " + table + " WHERE " + criterion + "'");
	}
	#endregion

	/// <summary>
	/// Basic execute command - open, create command, execute, close
	/// </summary>
	/// <param name="commandText"></param>
	public void ExecuteNonQuery(string commandText)
	{
		_connection.Open();
		_command.CommandText = commandText;
		_command.ExecuteNonQuery();
		_connection.Close();
	}

	/// <summary>
	/// Clean up everything for SQLite
	/// </summary>
	private void SQLiteClose()
	{
		if (_reader != null && !_reader.IsClosed)
			_reader.Close();
		_reader = null;

		if (_command != null)
			_command.Dispose();
		_command = null;

		if (_connection != null && _connection.State != ConnectionState.Closed)
			_connection.Close();
		_connection = null;
	}
}
