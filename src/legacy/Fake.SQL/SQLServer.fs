﻿[<AutoOpen>]
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
module Fake.SQL.SqlServer
 
open Fake
open System
open System.Data.SqlClient
open Microsoft.SqlServer.Management.Smo
open Microsoft.SqlServer.Management.Common
open System.IO

[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
type ServerInfo ={ 
    Server: Server
    ConnBuilder: SqlConnectionStringBuilder}
  
/// Gets a connection to the SQL server and an instance to the ConnectionStringBuilder
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let getServerInfo connectionString = 
    let connbuilder = new SqlConnectionStringBuilder(connectionString)
    let conn = new ServerConnection()
    if connbuilder.UserID <> "" then
        conn.LoginSecure <- false
        conn.Login <- connbuilder.UserID
    
    if connbuilder.Password <> "" then
        conn.LoginSecure <- false
        conn.Password <- connbuilder.Password
    
    conn.ServerInstance <- connbuilder.DataSource
    conn.Connect()

    {Server = new Server(conn)
     ConnBuilder = connbuilder}
  
/// gets the DatabaseNames from the server
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let getDatabasesFromServer (serverInfo:ServerInfo) = 
    seq {for db in serverInfo.Server.Databases -> db}

/// gets the DatabaseNames from the server
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let getDatabaseNamesFromServer (serverInfo:ServerInfo) = 
    getDatabasesFromServer serverInfo
      |> Seq.map (fun db -> db.Name)

/// Gets the initial catalog name
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let getDBName serverInfo = serverInfo.ConnBuilder.InitialCatalog 

/// Gets the name of the server
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let getServerName serverInfo = serverInfo.ConnBuilder.DataSource   
                  
/// Checks whether the given Database exists on the server
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let existDBOnServer serverInfo dbName = 
    let names = getDatabaseNamesFromServer serverInfo
    let searched = getDBName serverInfo
    tracefn "Searching for database %s on server %s. Found: " searched (getServerName serverInfo)
    names
      |> Seq.iter (tracefn "  - %s ")

    names
      |> Seq.exists ((=) dbName)

/// Gets the initial catalog as database instance
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let getDatabase serverInfo = new Database(serverInfo.Server, getDBName serverInfo)
    
/// Checks whether the given InitialCatalog exists on the server    
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let intitialCatalogExistsOnServer serverInfo =  
    getDBName serverInfo 
      |> existDBOnServer serverInfo  

/// Drops the given InitialCatalog from the server (if it exists)
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let DropDb serverInfo = 
    if intitialCatalogExistsOnServer serverInfo then
        logfn "Dropping database %s on server %s" (getDBName serverInfo) (getServerName serverInfo)
        (getDatabase serverInfo).DropBackupHistory |> ignore
        getDBName serverInfo |> serverInfo.Server.KillDatabase
    serverInfo

/// Kills all processes with the given server info
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let KillAllProcesses serverInfo =
    let dbName = getDBName serverInfo
    logfn "Killing all processes from database %s on server %s." dbName (getServerName serverInfo)
    serverInfo.Server.KillAllProcesses dbName
    serverInfo

/// Detaches a database
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let Detach serverInfo =
    serverInfo
      |> KillAllProcesses
      |> fun si -> 
            let dbName = getDBName si
            logfn "Detaching database %s on server %s." dbName (getServerName serverInfo)
            si.Server.DetachDatabase(dbName, true)
            si

/// Attach a database
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let Attach serverInfo (attachOptions:AttachOptions) files =
    let sc = new Collections.Specialized.StringCollection ()
    files |> Seq.iter (fun file ->         
        sc.Add file |> ignore
        checkFileExists file)

    let dbName = getDBName serverInfo

    logfn "Attaching database %s on server %s." dbName (getServerName serverInfo)
    serverInfo.Server.AttachDatabase(dbName,sc,attachOptions)
    serverInfo

/// Creates a new db on the given server
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let CreateDb serverInfo =     
    logfn "Creating database %s on server %s" (getDBName serverInfo) (getServerName serverInfo)
    (getDatabase serverInfo).Create()  
    serverInfo
  
/// <summary>Runs a sql script on the server.</summary>
/// <param name="serverInfo">Used as a connection to the database.</param>
/// <param name="sqlFile">The script which will be run.</param>
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let runScript serverInfo sqlFile =
    logfn "Executing script %s" sqlFile
    sqlFile
      |> StringHelper.ReadFileAsString
      |> (getDatabase serverInfo).ExecuteNonQuery
    
/// Closes the connection to the server
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let Disconnect serverInfo = 
    logfn "Disconnecting from server %s." (getServerName serverInfo)
    if serverInfo.Server = null then
        failwith "Server is not configured"
    if serverInfo.Server.ConnectionContext = null then
        failwith "Server.ConnectionContext is not configured"
    serverInfo.Server.ConnectionContext.Disconnect()

/// Replaces the database files
let internal replaceDatabaseFiles connectionString attachOptions copyF =
    connectionString
      |> getServerInfo
      |> fun si -> if existDBOnServer si (getDBName si) then Detach si else si
      |> fun si -> copyF() |> Attach si attachOptions
      |> Disconnect

/// Replaces the database files
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let ReplaceDatabaseFiles connectionString targetDir files attachOptions =
    replaceDatabaseFiles connectionString attachOptions
        (fun _ ->
            files 
              |> Seq.map (fun fileName ->     
                    let fi = new FileInfo(fileName)
                    CopyFile targetDir fileName
                    targetDir @@ fi.Name))

/// <summary>Replaces the database files from a cache.
/// If the files in the cache are not up to date, they will be refreshed.</summary>
/// <param name="connectionString">Used to open the connection to the database.</param>
/// <param name="targetDir">The directory where the attached files will live.</param>
/// <param name="cacheDir">The file cache. If the files in the cache are not up to date, they will be refreshed.</param>
/// <param name="files">The original database files.</param>
/// <param name="attachOptions">AttachOptions for Sql server.</param>
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let ReplaceDatabaseFilesWithCache connectionString targetDir cacheDir files attachOptions =
    replaceDatabaseFiles connectionString attachOptions
        (fun _ -> CopyCached targetDir cacheDir files)
 
/// <summary>Drops and creates the database (dropped if db exists. created nonetheless)</summary>
/// <param name="connectionString">Used to open the connection to the database.</param>
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let DropAndCreateDatabase connectionString = 
    connectionString 
      |> getServerInfo
      |> DropDb
      |> CreateDb
      |> Disconnect          

/// <summary>Runs the given sql scripts on the server.</summary>
/// <param name="connectionString">Used to open the connection to the database.</param>
/// <param name="scripts">The scripts which will be run.</param>
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let RunScripts connectionString scripts = 
    let serverInfo = getServerInfo connectionString
    scripts |> Seq.iter (runScript serverInfo)
    Disconnect serverInfo

/// <summary>Runs all sql scripts from the given directory on the server.</summary>
/// <param name="connectionString">Used to open the connection to the database.</param>
/// <param name="scriptDirectory">All *.sql files inside this directory and all subdirectories will be run.</param>
[<System.Obsolete("This function, type or module is obsolete. There is no alternative in FAKE 5 yet. If you need this functionality consider porting the module (https://fake.build/contributing.html#Porting-a-module-to-FAKE-5).")>]
let RunScriptsFromDirectory connectionString scriptDirectory =
    Directory.GetFiles(scriptDirectory, "*.sql", SearchOption.AllDirectories)
      |> RunScripts connectionString  
