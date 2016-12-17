// SQLite integration script


function sqlInit()
{
   //echo("SQL INIT OPENING DB: " @ $ecstasy_dbname);
   %dbname = $ecstasy_dbname; 
   %sqlite = new SQLiteObject(sqlite);
   if (%sqlite == 0)
   {
      echo("ERROR: Failed to create SQLiteObject. sqliteTest aborted.");
      return;
   }
   
   // open database
   if (sqlite.openDatabase(%dbname) == 0)
   {
      echo("ERROR: Failed to open database: " @ %dbname);
      echo("       Ensure that the disk is not full or write protected.  sqliteTest aborted.");
      sqlite.delete();
      return;
   }
   
    sqlite.delete();
      
}

function sqlite::onQueryFailed(%this, %error)
{
   echo ("SQLite Query Error: " @ %error);
}


function sqlite::onQueryFinished(%this)
{
   echo ("SQLite Query Finished ");
}
