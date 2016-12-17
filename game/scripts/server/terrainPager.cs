// TerrainPager
// copyright 2015 Chris Calef

// (This does very little, do we really need it?)

function TerrainPager::UpdateSkybox()
{
   WS_SkyboxCubemap.updateFaces();//HERE: deal with different cubemap names?  Naw...  
   WS_SkyboxMaterial.reload();
}

function TerrainPager::onAdd(%this,%obj)
{
   %temp = new SimSet(StaticGroup);
   %temp = new SimSet(ActorGroup);
   %temp = new SimSet(TerrainGroup);
   
	///////////////////////////////////////
	//If/when sqlite gets added:
	//%sqlite = new SQLiteObject(sqlite);
   //if (%sqlite == 0)
   //{
      //echo("ERROR: Failed to create SQLiteObject. sqliteTest aborted.");
      //return;
   //} else {
      //sqlite.openDatabase("Ecopocalypse.db");
   //}
	///////////////////////////////////////
}

function TerrainPager::onRemove()
{
   echo("calling terrain pager script onRemove()");
	///////////////////////////////////////
	//If/when sqlite gets added:
   //sqlite.closeDatabase();
   //sqlite.delete();
	///////////////////////////////////////
}

function TerrainPager::onTick(%this,%obj)
{
   //echo("terrainPager onTick!!!!!!!!!!!!!!!!!!!!");
}

