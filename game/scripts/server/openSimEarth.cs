

//=============================================================================
//
//                 OPEN SIM EARTH
//
//=============================================================================
$numScenes = 0;

function startSQL(%dbname)
{//Create the sqlite object that we will use in all the scripts.
   %temp = new SQLiteObject(sqlite);
   //HMMM... Maybe?? Radical new method: make a new SQLiteObject that copies m_pDatabase from existing one.
   
   //if (%sqlite.openDatabase(%dbname))
   //if (sqlite.loadOrSaveDb(%dbname,false))
   //   echo("Successfully loaded database: " @ %dbname );
   //else {
   //   echo("We had a problem involving database: " @ %dbname );
   //   return;
   //}
}

   //TESTING - SpatiaLite.  Exciting promise, disappointing failure... so far.
   //if (%sqlite.openDatabase("testDB.db"))
   //{
   //   echo("Successfully opened database: testDB.db" );
      //%query = "INSERT INTO testTable ( name, geom ) VALUES ('Test01',GeomFromText('POINT(1 2)'));";
      //%query = "";
      //%result = sqlite.query(%query, 0);
      //if (%result)
      //   echo("spatialite inserted into a table with a geom!");
      //else
      //   echo("spatialite failed to insert into a table with a geom!  "   );
   //   %sqlite.closeDatabase();
   //}   
   //NOW... apparently all we have to do is this, to gain access to all of SpatiaLite.
   //%query = "SELECT load_extension('libspatialite-2.dll');";
   //%result = sqlite.query(%query, 0);
   //echo( "Loaded SpatiaLite: " @ %result );
   //Except, maybe have to do this in the engine.
   
function stopSQL()
{
   //sqlite.closeDatabase();
   //sqlite.loadOrSaveDb(%dbname,true);
   //sqlite.delete();      
}

function openSimEarthTick()
{
   //OBSOLETE: Maybe come back to it when we come back to openSimEarth, but something tells me
   //MegaMotion is going to have a serious impact on how we load and manage scenes in OSE.
   /*
   if (($numScenes==0)&&($pref::MegaMotion::autoLoadScenes)) //first time through, unless DB is missing or corrupt.
   {
      %query = "SELECT s.id,p.x AS pos_x,p.y AS pos_y,p.z AS pos_z " @
               "FROM scene s LEFT JOIN vector3 p ON p.id=s.pos_id;";
      %result = sqlite.query(%query, 0);
      echo("query: " @ %query);
      %i=0;
      if (%result)
      {	   
         while (!sqlite.endOfResult(%result))
         {
            %id = sqlite.getColumn(%result, "id");     
            %x = sqlite.getColumn(%result, "pos_x");
            %y = sqlite.getColumn(%result, "pos_y");
            %z = sqlite.getColumn(%result, "pos_z");
            //DatabaseSceneList.add(%name,%id);
            echo("scene " @ %id  @ " " @ %x @ " " @ %y @ " " @ %z);
            
            $scenePos[%i] = %x @ " " @ %y @ " " @ %z;
            $sceneId[%i] = %id;
            $sceneLoaded[%i] = false;
            $sceneDist[%i] = 5.0;//TEMP, add this to scenes table
            
            %i++;
            sqlite.nextRow(%result);
         }
      } 
      $numScenes = %i;
      echo("Num scenes: " @ %numScenes);
   }
   sqlite.clearResult(%result);
   
   if (($myPlayer)&&($pref::MegaMotion::autoLoadScenes))
   {
      %pos = $myPlayer.getPosition();
      for (%i=0;%i<$numScenes;%i++)
      {
         %diff = VectorSub(%pos,$scenePos[%i]);
         
         if ((VectorLen(%diff)<$sceneDist[%i])&&($sceneLoaded[%i]==false))
         {
            loadScene($sceneId[%i]);
            $sceneLoaded[%i] = true;
         } 
         else if ((VectorLen(%diff)>$sceneDist[%i]*20)&&($sceneLoaded[%i]==true))//*20 completely arbitrary
         {
            unloadScene($sceneId[%i]);
            $sceneLoaded[%i] = false;              
         }
           
      }
      //echo("player position: " @ %pos );
   }
   
   schedule(60,0,"openSimEarthTick");
 */
}
   
///////////////////////////////////////////////////////////////////////////////////////

//MOVE: these should be in a behaviorTrees folder, or at least a single file.
function onStartup::precondition(%this, %obj)
{
   if (%obj.startedUp != true)
      return true;
   else
      return false;
}

//OBSOLETE THIS?
function onStartup::behavior(%this, %obj)
{
   //echo("calling onStartup!");   
   
   //Temp, store these in DB by shape and/or sceneShape
   /*
   %obj.setAmbientSeqByName("ambient");
   %obj.setIdleSeqByName("ambient");
   %obj.setWalkSeqByName("ambient");
   %obj.setRunSeqByName("run");
   %obj.setAttackSeqByName("power_punch_down");
   %obj.setBlockSeqByName("tpose");//TEMP, need block seq
   %obj.setFallSeqByName("ambient");
   %obj.setGetupSeqByName("rSideGetup");
   */
   //Possibly these should not be named actions but should all be included in 
   //a sequenceActions table so it can be infinitely expanded.
   
   //Should this be automatic here, 
   %obj.groundMove();
   //or wait until we find out if we're more than just a ragdoll?
   
   %obj.startedUp = true;
   
   return SUCCESS;   
}

////////////// BEHAVIORS ///////////////////////////////

///////////////////////////////////
//[behaviorName]::precondition()
//[behaviorName]::onEnter()
//[behaviorName]::onExit()

//Do a raycast, either torque or physx, and find the ground directly below me.
//if below some threshold, then just move/interpolate us there. If above that, go to
//falling animation and/or ragdoll until we hit the ground and stop, then go to getUp task.

/* // No longer necessary... this is now done during processTick.
function goToGround::behavior(%this, %obj)
{
   %start = VectorAdd(%obj.position,"0 0 1.0");//Add a tiny bit (or, a huge amount)
                // so we don't get an error when we're actually on the ground.
                
   %contact = physx3CastGroundRay(%start);
   
   %obj.setPosition(%contact);
   echo(%this @ " is going to ground!!!!!!");
   %obj.setAmbientSeqByName("ambient");
   %obj.setIdleSeqByName("ambient");
   %obj.setWalkSeqByName("walk");
   %obj.setRunSeqByName("run");
   %obj.setAttackSeqByName("power_punch_down");
   %obj.setBlockSeqByName("tpose");
   %obj.setFallSeqByName("ambient");
   %obj.setGetupSeqByName("rSideGetup");
   
   return SUCCESS;
}
*/



/////////////////////////////////////////////////

function osePullStatics(%simGroup)
{//So, here we need to remove objects from the MissionGroup and put them into another simGroup.
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="TSStatic")
         %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function osePullStaticsAndSave(%simGroup)
{   
   if (isDefined(theTP))   
      theTP.saveStaticShapes();
   
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="TSStatic")
      {
         %simGroup.add(%obj);
      }
   }
   
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.remove(%simGroup.getObject(%i));
   }
}

function osePushStatics(%simGroup)
{
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.add(%simGroup.getObject(%i));
   }
}

function osePullRoads(%simGroup)
{//So, here we need to remove objects from the MissionGroup and put them into another simGroup.
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="DecalRoad")// and/or MeshRoad
         %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function osePullRoadsAndSave(%simGroup)
{   
   theTP.saveRoads();
   
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="DecalRoad")// and/or MeshRoad
         %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function osePushRoads(%simGroup)
{
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.add(%simGroup.getObject(%i));
   }
}

function loadOSM()  // OpenStreetMap XML data
{
   //here, read lat/long for each node as we get to it, convert it to xyz coords,
   //and save it in an array, to be used in the DecalRoad declaration.    
   
   %beforeTime = getRealTime();
   
   theTP.loadOSM($pref::MegaMotion::OSM,$pref::MegaMotion::MapDB);     
   //theTP.loadOSM("min.osm");     
   //theTP.loadOSM("kincaid_map.osm");  
   //theTP.loadOSM("central_south_eug.osm");  
   //theTP.loadOSM("thirtieth_map.osm");
   //theTP.loadOSM("all_eugene.osm");  
   
   %loadTime = getRealTime() - %beforeTime;
   echo("OpenStreetMap file load time: " @ %loadTime );
}

function makeStreets()
{
   %mapDB = new SQLiteObject();
   %dbname = $pref::MegaMotion::MapDB;//HERE: need to find this in prefs or something.
   %result = %mapDB.openDatabase(%dbname);
   //echo("tried to open osmdb: " @ %result);
   
   %query = "SELECT osmId,type,name FROM osmWay;";  
	%result = %mapDB.query(%query, 0);
   if (%result)
   {	   
      while (!%mapDB.endOfResult(%result))
      {
         %wayId = %mapDB.getColumn(%result, "osmId");
         %wayType = %mapDB.getColumn(%result, "type");         
         %wayName = %mapDB.getColumn(%result, "name");
         echo("found a way: " @ %wayName @ " id " @ %wayId);
         if ((%wayType $= "residential")||
               (%wayType $= "tertiary")||
               (%wayType $= "trunk")||
               (%wayType $= "trunk_link")||
               (%wayType $= "motorway")||
               (%wayType $= "motorway_link")||
               (%wayType $= "service")||
               (%wayType $= "footway")||
               (%wayType $= "path")||
               (%wayType $= "track"))
         {   
            
            //Width
            %roadWidth = 10.0;       
            if ((%wayType $= "tertiary")||(%wayType $= "trunk_link"))
               %roadWidth = 18.0; 
            else if ((%wayType $= "trunk")||(%wayType $= "motorway_link"))
               %roadWidth = 32.0; 
            else if (%wayType $= "motorway")
               %roadWidth = 40.0; 
            else if (%wayType $= "footway")
               %roadWidth = 2.5; 
            else if ((%wayType $= "path")||(%wayType $= "track"))
               %roadWidth = 5.0; 
            
            //Material
            %roadMaterial = "DefaultDecalRoadMaterial";
            if (%wayType $= "footway")
               %roadMaterial = "DefaultRoadMaterialPath";
            else if ((%wayType $= "service")||(%wayType $= "path"))
               %roadMaterial = "DefaultRoadMaterialOther";
               
            //now, query the osmWayNode and osmNode tables to get the list of points
            %node_query = "SELECT wn.nodeId,n.latitude,n.longitude,n.type,n.name from " @ 
                           "osmWayNode wn JOIN osmNode n ON wn.nodeId = n.osmId " @
                           "WHERE wn.wayID = " @ %wayId @ ";";
            %result2 = %mapDB.query(%node_query, 0);
            if (%result2)
            {	   
               //echo("query2 results: " @ mapDB.numRows(%result2));
               %nodeString = "";
               while (!%mapDB.endOfResult(%result2))
               {
                  %nodeId = %mapDB.getColumn(%result2, "nodeId");
                  %latitude = %mapDB.getColumn(%result2, "latitude");
                  %longitude = %mapDB.getColumn(%result2, "longitude");
                  %pos = theTP.convertLatLongToXYZ(%longitude @ " " @ %latitude @ " 0.0");
                  %type = %mapDB.getColumn(%result2, "type");         
                  %name = %mapDB.getColumn(%result2, "name");               
                  echo("  Node " @ %nodeId @ " longitude " @ %longitude @ " latitude " @ %latitude @ 
                       " type " @ %type @ " name " @ %name );
                  //%nodeString = %nodeString @ " Node = \"" @ %pos @ " " @ %roadWidth @ " 2 0 0 1\";";//2 = road depth, fix                  
                  %nodeString = %nodeString @ " Node = \"" @ %pos @ " " @ %roadWidth @ "\";";                  
                  %mapDB.nextRow(%result2);
               }            
               %mapDB.clearResult(%result2);
            }
            //Node = "-2263.4 -2753.58 233.796 10 5 0 0 1";
           // " Node = \"0.0 0.0 300.0 30.000000\";" @
            echo( %nodeString );
            //Then, do the new DecalRoad, execed in order to get a loop into the declaration.
            
            %roadString = "      new DecalRoad() {" @
               " InternalName = \"" @ %wayId @ "\";" @
               " Material = \"" @ %roadMaterial @ "\";" @
               " textureLength = \"25\";" @
               " breakAngle = \"3\";" @
               " renderPriority = \"10\";" @
               " position = \"" @ %pos @ "\";" @ //Better position of last node than nothing, I guess.
               " rotation = \"1 0 0 0\";" @
               " scale = \"1 1 1\";" @
               " canSave = \"1\";" @
               " canSaveDynamicFields = \"1\";" @
               %nodeString @
            "};";
            /*
            %roadString = "      new MeshRoad() {" @
            " topMaterial = \"DefaultRoadMaterialTop\";" @
            " bottomMaterial = \"DefaultRoadMaterialOther\";" @
            " sideMaterial = \"DefaultRoadMaterialOther\";" @
            " textureLength = \"5\";" @
            " breakAngle = \"3\";" @
            " widthSubdivisions = \"0\";" @
            " position = \"-2263.4 -2753.58 233.796\";" @
            " rotation = \"1 0 0 0\";" @
            " scale = \"1 1 1\";" @
            " canSave = \"1\";" @
            " canSaveDynamicFields = \"1\";" @
             %nodeString @
            "};";
         */
            eval(%roadString); 
         }
         
         %mapDB.nextRow(%result);
      }
      %mapDB.clearResult(%result);
   } else echo ("no results.");
   
   %mapDB.closeDatabase();
   %mapDB.delete();
}


/*
function streetMap()
 {   
    %xml = new SimXMLDocument() {};
    %xml.loadFile( "only_kincaid_map.osm" );

    // "Get" inside of the root element, "Students".     
    %result = %xml.pushChildElement("osm");  
    %version = %xml.attribute("version");     
    %generator = %xml.attribute("generator");      
    // "Get" into the first child element    
    %xml.pushFirstChildElement("bounds"); 
    %minlat = %xml.attribute("minlat");
    %maxlat = %xml.attribute("maxlat");
    echo("result: " @ %result @ " version: " @ %version @ ", generator " @ %generator @" minlat " @ %minlat @ " maxlat " @ %maxlat );
    while  (%xml.nextSiblingElement("node"))     
    {     
       %id = %xml.attribute("id"); 
       %lat = %xml.attribute("lat");     
       %lon = %xml.attribute("lon");    
       echo("node " @ %id @ " lat " @ %lat @ " long " @ %lon);   
       //HERE: store data in sqlite, and then read it back in the makeStreets function. 
       //Need at least a "way" table and a "node" table, plus other decorators I'm sure.
    } 
    %xml.nextSiblingElement("way");    
    echo("way: " @ %xml.attribute("id"));
    %xml.pushFirstChildElement("nd");
    echo("ref: " @ %xml.attribute("ref"));
    while (%xml.nextSiblingElement("nd")) 
    {
       echo("ref: " @ %xml.attribute("ref"));
    }
    while (%xml.nextSiblingElement("tag"))
    {
       echo("k: " @ %xml.attribute("k") @ "  v: " @ %xml.attribute("v") );
    }
    
 }  */


