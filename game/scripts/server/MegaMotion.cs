////////////////////////////////
//LICENSE INFO HERE
//The following source code is released  
////////////////////////////////


$MegaMotionScenesWindowID = 159;
$MegaMotionSequenceWindowID = 395;

$mmAddProjectWindowID = 241;
$mmAddSceneWindowID = 246;
$mmAddSceneShapeWindowID = 247;
$mmAddShapeGroupWindowID = 333;
$mmAddOpenSteerWindowID = 378;
$mmAddShapeMountWindowID = 594;
$mmAddShapeWindowID = 619;
$mmAddShapePartWindowID = 629;
$mmAddBVHProfileWindowID = 674;

$mmAddKeyframeWindowID = 524;
$mmAddKeyframeSeriesWindowID = 466;
$mmAddKeyframeSetWindowID = 544;

$mmScene_ActorBlock_16x16_ID = 13;
$mmScene_ActorBlock_4x4_ID = 8;
$mmScene_ActorBlock_3x40_ID = 10;
$mmScene_ActorBlock_2x2_ID = 11;

$mmLastProject = 0;
$mmLastScene = 0;  

$mmLoadedScenes = 0;
$mmLoadedShapes = 0;

$mmSelectedShape = 0;
$mmSelectedSceneShape = 0;//Use this to store selection between load/unload scenes.

$mmLoopDetecting = false;
$mmRotDeltaSumMin = 0;
$mmRotDeltaSumDescending = 0;
$mmRotDeltaSumLast = 0;

$mmShapeMountChildSceneShape = 0;

//$mmKeyframesRotation = 1;
//$mmKeyframesPosition = 0;

$mmAddKeyframeRotation = 1;
$mmAddKeyframePosition = 0;

$mmSequenceBlend = 0;
$mmSequenceLoop = 0;
$mmSequenceGroundAnimate = 0;

$mmKeyframeID = 0;

$mmPlayer = 0;
$mmCamera = 0;

$mmDebugRenderCollisions = 1;
$mmDebugRenderJointLimits = 0;
$mmDebugRenderBodyAxes = 0;

//Some of above should just be prefs, others should be local.

exec("MegaMotionScenes.gui");

/*

//AND, for tomorrow: EWorldEditor.getSelectionSize(), %obj = EWorldEditor.getSelectedObject( %i ).
//Multi-select, for the masses of sceneShapes. Wherever applicable, make it so.

function EcstasyToolsWindow::toggleGroundAnimate()
{
   if (EWorldEditor.getSelectionSize()>0)
   {
      for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
      {
         %obj = EWorldEditor.getSelectedObject( %i );
         if (%obj)
         {
           if (%obj.getClassName() $= "fxFlexBody")
           {
              //echo("playing sequence " @ SequencesList.getText() @ " on actor " @ %obj.gatActorID());
               %ghostID = LocalClientConnection.getGhostID(%obj);
               %client_bot = ServerConnection.resolveGhostID(%ghostID);
               %client_bot.setGroundAnimating($tweaker_ground_animate);           
           }
         }
      }
   } 
   else if ($actor) 
   {
      $actor.setGroundAnimating($tweaker_ground_animate);
   }
}
*/

function mmTabBook::onTabSelected(%this, %text, %index)
{
   echo("selecting tab!  " @ %text @ " index " @ %index);  
   
   if (%index == 0) // physics tab
   {
      //Have to be extra careful with debug draw, needs rewrite. Terrain crashes it, too much data(?)
      %haveTerrain = 0;
      for (%i=0;%i<MissionGroup.getCount();%i++)
      {
         if (MissionGroup.getObject(%i).getClassName() $= "TerrainBlock")
            %haveTerrain = 1;
      }
      if (!%haveTerrain)
         physicsDebugDraw(0);//(1)//Kill this for now, it causes major problems.
      else
         physicsDebugDraw(0);
   }
   else
      physicsDebugDraw(0);
      
   if (%index == 2)// sequence tab
      MegaMotionSequenceWindow.visible = true;
   else
      MegaMotionSequenceWindow.visible = false;
      
   if (%index == 3)// ai tab
      $Nav::Editor::renderMesh = true;
   else
      $Nav::Editor::renderMesh = false;
}

function mmToggleDebugRenderCollisions()
{
   echo("Toggling debug render collisions!!! " @ $mmDebugRenderCollisions);
   $physics::debugRenderCollisions = $mmDebugRenderCollisions;   
}

function mmToggleDebugRenderJointLimits()
{
   echo("Toggling debug render joint limits!!! " @ $mmDebugRenderJointLimits);
   $physics::debugRenderJointLimits = $mmDebugRenderJointLimits;   
}

function mmToggleDebugRenderBodyAxes()
{
   echo("Toggling debug render body axes!!! " @ $mmDebugRenderBodyAxes);
   $physics::debugRenderBodyAxes = $mmDebugRenderBodyAxes;   
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////

function numericTest(%testString)
{
   if ((strlen(%testString)==0)||(!isAllNumeric(%testString)))
      return false;
   else 
      return true;  
}

//$lastScene = 0; //Store selected scene, for when we re-expose form, but we're working on a scene
//and project that is not the first one.
function exposeMegaMotionScenesForm()
{
   %project_id = 0;
   %scene_id = 0;
   %sceneShape_id = 0;
   %sequence_id = 0;
   %tab_page = 4;//SceneShapeTab=4
   
   if (isDefined("MegaMotionScenes"))
   {
      %project_id = $mmProjectList.getSelected();
      %scene_id = $mmSceneList.getSelected();
      %sceneShape_id = $mmSceneShapeList.getSelected();
      %targetShape_id = $mmTargetShapeList.getSelected();
      %sequence_id = $mmSequenceList.getSelected();
      %tab_page = $mmTabBook.getSelectedPage();
      MegaMotionScenes.delete();
   }
   
   %dbRebuild = false;
   if (%dbRebuild)
      makeSqlGuiForm($MegaMotionScenesWindowID);
   else //Hmm, not working, not sure why.
   {
      exec("MegaMotionScenes.gui");//FIX: still searching for correct place to execute this.
      EWorldEditor.add(MegaMotionScenes); 
   }
      
   setupMegaMotionScenesForm();   
   
   if ((%project_id>0)&&(%project_id!=$mmProjectList.getSelected()))
      $mmProjectList.setSelected(%project_id);
   if ((%scene_id>0)&&(%scene_id!=$mmSceneList.getSelected()))
      $mmSceneList.setSelected(%scene_id);
   if ((%sceneShape_id>0)&&(%sceneShape_id!=$mmSceneShapeList.getSelected()))
      $mmSceneShapeList.setSelected(%sceneShape_id);
   if ((%targetShape_id>0)&&(%targetShape_id!=$mmTargetShapeList.getSelected()))
      $mmTargetShapeList.setSelected(%targetShape_id);
   if ((%sequence_id>0)&&(%sequence_id!=$mmSequenceList.getSelected()))
      $mmSequenceList.setSelected(%sequence_id);
      
   $mmTabBook.selectPage(%tab_page);
   
}

function openMegaMotionScenes()
{
   echo("calling openMegaMotionScenes");
}

function setupMegaMotionScenesForm()
{
   echo("calling setupMegaMotionScenesForm");
   
   if (!isDefined("MegaMotionScenes"))
      return;   
      
   $mmShapeId = 0;
   //$mmPosId = 0;
   //$mmRotId = 0;
   //$mmScaleId = 0;
  
   $mmProjectList = MegaMotionScenes.findObjectByInternalName("projectList");
   $mmSceneList = MegaMotionScenes.findObjectByInternalName("sceneList");
   $mmSceneShapeList = MegaMotionScenes.findObjectByInternalName("sceneShapeList"); 
   $mmShapeList = MegaMotionScenes.findObjectByInternalName("shapeList"); 
   $mmSequenceList = MegaMotionScenes.findObjectByInternalName("sequenceList");
   
   $mmTabBook = MegaMotionScenes.findObjectByInternalName("mmTabBook");    
   $mmTabBook.allowReorder = true;

   //maybe temporary   
   $mmSceneShapeTab = $mmTabBook.findObjectByInternalName("sceneShapeTab");
   $mmShapePartTab = $mmTabBook.findObjectByInternalName("shapePartTab");
   $mmSequenceTab = $mmTabBook.findObjectByInternalName("sequenceTab");
   $mmBvhTab = $mmTabBook.findObjectByInternalName("bvhTab");
   $mmAiTab = $mmTabBook.findObjectByInternalName("aiTab");
   $mmSceneTab = $mmTabBook.findObjectByInternalName("sceneTab");

   $mmShapePartTab.setTabIndex(0);
   $mmBvhTab.setTabIndex(1);
   $mmSequenceTab.setTabIndex(2); 
   $mmAiTab.setTabIndex(3); 
   $mmSceneShapeTab.setTabIndex(4);
   $mmSceneTab.setTabIndex(5);
   $mmTabBook.reArrange();   
   
   mmSetupSceneShapeTab();
   
   mmSetupPhysicsTab();
   
   mmSetupSequenceTab();

   mmSetupBvhTab();
   
   mmSetupAiTab();
   
   mmSetupSceneTab();
   
   mmRefreshShapeLists();
   
   //mmLoadOpenSteerProfiles();//Trying to avoid querying openSteer table for every bot, but screw it for now.
}

function mmSetupSceneShapeTab()
{
   %panel = $mmSceneShapeTab.findObjectByInternalName("sceneShapePanel");
   
   //Can we reduce the number of globals here, by defining them temporarily where needed?

   $mmSceneShapePositionX = %panel.findObjectByInternalName("sceneShapePositionX");
   $mmSceneShapePositionY = %panel.findObjectByInternalName("sceneShapePositionY");
   $mmSceneShapePositionZ = %panel.findObjectByInternalName("sceneShapePositionZ");
   $mmSceneShapeOrientationX = %panel.findObjectByInternalName("sceneShapeOrientationX");//quat
   $mmSceneShapeOrientationY = %panel.findObjectByInternalName("sceneShapeOrientationY");
   $mmSceneShapeOrientationZ = %panel.findObjectByInternalName("sceneShapeOrientationZ");
   $mmSceneShapeOrientationAngle = %panel.findObjectByInternalName("sceneShapeOrientationAngle");
   $mmSceneShapeScaleX = %panel.findObjectByInternalName("sceneShapeScaleX");
   $mmSceneShapeScaleY = %panel.findObjectByInternalName("sceneShapeScaleY");
   $mmSceneShapeScaleZ = %panel.findObjectByInternalName("sceneShapeScaleZ");  
   
   $mmShapeMountList = %panel.findObjectByInternalName("shapeMountList");  
   $mmShapeMountParentNodeList = %panel.findObjectByInternalName("shapeMountParentNodeList");
   $mmShapeMountChildShapeList = %panel.findObjectByInternalName("shapeMountChildShapeList"); 
   $mmShapeMountChildNodeList = %panel.findObjectByInternalName("shapeMountChildNodeList"); 
}

function mmSetupPhysicsTab()
{
   %panel = $mmShapePartTab.findObjectByInternalName("shapePartPanel");
   
   $mmShapePartList = %panel.findObjectByInternalName("shapePartList");   
   $mmShapePartTypeList = %panel.findObjectByInternalName("shapePartTypeList");
   $mmShapePartBaseNodeList = %panel.findObjectByInternalName("shapePartBaseNodeList");   
   $mmShapePartChildNodeList = %panel.findObjectByInternalName("shapePartChildNodeList");   
   
   $mmShapePartTypeList.add("Box","0");
   $mmShapePartTypeList.add("Capsule","1");
   $mmShapePartTypeList.add("Sphere","2");
   //From here down, currently unsupported.
   //$mmShapeTypeList.add("Convex","3");
   //$mmShapeTypeList.add("Collision","4");
   //$mmShapeTypeList.add("Trimesh","5");
   $mmShapePartTypeList.setSelected(0);
   
   $mmJointList = %panel.findObjectByInternalName("jointList");
   
   $mmJointTypeList = %panel.findObjectByInternalName("jointTypeList");
   $mmJointTypeList.add("Spherical","0");
   $mmJointTypeList.add("Revolute","1");
   $mmJointTypeList.add("Prismatic","2");
   $mmJointTypeList.add("Fixed","3");
   $mmJointTypeList.add("Distance","4");
   $mmJointTypeList.add("D6","5");
   $mmJointTypeList.setSelected(5);
   
   %query = "SELECT id,name FROM project ORDER BY name;";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmProjectList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if ($mmLastProject>0)
            $mmProjectList.setSelected($mmLastProject);
         else if (%firstID>0) 
            $mmProjectList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }
   
   %query = "SELECT id,name FROM px3Joint ORDER BY name;";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         $mmJointList.add("",0);  
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmJointList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
}

function mmSetupSequenceTab()
{
   %panel = $mmSequenceTab.findObjectByInternalName("sequencePanel");
   
   $mmSequenceActionList = %panel.findObjectByInternalName("sequenceActionList");
   $mmSequenceFileText = %panel.findObjectByInternalName("sequenceFileText");
   
   //Can we reduce the number of globals here, by defining them temporarily where needed?
   $mmSequenceNodeList = %panel.findObjectByInternalName("sequenceNodeList");
   $mmSequenceAllNodeList = %panel.findObjectByInternalName("sequenceAllNodeList");
   $mmSequenceKeyframeSetList = %panel.findObjectByInternalName("sequenceKeyframeSetList");
   $mmSequenceKeyframeSeriesList = %panel.findObjectByInternalName("sequenceKeyframeSeriesList");
   $mmSequenceKeyframeList = %panel.findObjectByInternalName("sequenceKeyframeList");
   
   $mmSequenceKeyframeValueX = %panel.findObjectByInternalName("sequenceKeyframeValueX");
   $mmSequenceKeyframeValueY = %panel.findObjectByInternalName("sequenceKeyframeValueY");
   $mmSequenceKeyframeValueZ = %panel.findObjectByInternalName("sequenceKeyframeValueZ");
   
   $mmSequenceKeyframeValueX.setText(0);
   $mmSequenceKeyframeValueY.setText(0);
   $mmSequenceKeyframeValueZ.setText(0);
   
   $mmLoopDetectorDelay = 10;
   $mmLoopDetectorMax   = 150;
   $mmLoopDetectorSmooth = 10;
   
   $mmGroundCaptureButton = %panel.findObjectByInternalName("groundCaptureButton");
   
   $mmSequenceKeyframeTypeLabel = %panel.findObjectByInternalName("sequenceKeyframeTypeLabel");
   $mmSequenceKeyframeTypeText = %panel.findObjectByInternalName("sequenceKeyframeTypeText");
   $mmSequenceKeyframeTypeLabel.setVisible(false);//For now, skip this detail.
   
   $mmSceneAutoplay = false;

   //These are currently unused and may never be useful, but for now just hiding them.
   //%addKeyframeSeriesButton = %panel.findObjectByInternalName("addKeyframeSeriesButton");
   //%addKeyframeSeriesButton.setVisible(false);
   
   //%deleteKeyframeSeriesButton = %panel.findObjectByInternalName("deleteKeyframeSeriesButton");
   //%deleteKeyframeSeriesButton.setVisible(false);
   
   //%addKeyframeButton = %panel.findObjectByInternalName("addKeyframeButton");
   //%addKeyframeButton.setVisible(false);
   
   //%groundAnimateCheckbox = %panel.findObjectByInternalName("sequenceGroundAnimate");
   //%groundAnimateCheckbox.setVisible(false);//Turning this off till it gets hooked up.
   
   %seqFrameLabel = %panel.findObjectByInternalName("sequenceKeyframeFrameLabel");
   %seqFrameLabel.setVisible(false);
   %seqFrame = %panel.findObjectByInternalName("sequenceKeyframeFrame");
   %seqFrame.setVisible(false);
   
   %query = "SELECT id,name FROM action ORDER BY name;";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmSequenceActionList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
      }
      sqlite.clearResult(%resultSet);
   }   
   
   if (!isObject(MegaMotionSequenceWindow))
   {
      makeSqlGuiForm($MegaMotionSequenceWindowID);
      setupMegaMotionSequenceWindow();
   }
   
}

function mmSetupBvhTab()
{
   %panel = $mmBvhTab.findObjectByInternalName("bvhPanel");
   
   $mmBvhImportProfileList = %panel.findObjectByInternalName("bvhImportProfileList");
   $mmBvhExportProfileList = %panel.findObjectByInternalName("bvhExportProfileList");
   $mmBvhProfileList = %panel.findObjectByInternalName("bvhProfileList");
   
   $mmBvhModelNodeList = %panel.findObjectByInternalName("bvhModelNodesList");
   $mmBvhBvhNodeList = %panel.findObjectByInternalName("bvhBvhNodesList");
   $mmBvhLinkedNodesList = %panel.findObjectByInternalName("bvhLinkedNodesList");
   
   %query = "SELECT id,name FROM bvhProfile;";
   %resultSet = sqlite.query(%query,0);
   if (%resultSet)
   {
      %firstID = sqlite.getColumn(%resultSet, "id");
      while (!sqlite.endOfResult(%resultSet))
      {
         %id = sqlite.getColumn(%resultSet, "id");
         %name = sqlite.getColumn(%resultSet, "name");
            
         $mmBvhImportProfileList.add(%name,%id);
         $mmBvhExportProfileList.add(%name,%id);
         $mmBvhProfileList.add(%name,%id);
         
         sqlite.nextRow(%resultSet);   
      }
      sqlite.clearResult(%resultSet);
   }
}

function mmSetupAiTab()
{   
   %panel = $mmAiTab.findObjectByInternalName("aiPanel");
   
   //Can we reduce the number of globals here, by defining them temporarily where needed?
   //$mmSceneShapeBehaviorTree = %panel.findObjectByInternalName("sceneShapeBehaviorTree");//AiBehaviorTree
   $mmSceneShapeBehaviorTreeList = %panel.findObjectByInternalName("sceneShapeBehaviorTreeList");
   $mmTargetShapeList = %panel.findObjectByInternalName("sceneShapeTargetShapeList");
   $mmShapeGroupList = %panel.findObjectByInternalName("sceneShapeGroupList");//RENAME: AiGroupList   
   $mmOpenSteerList = %panel.findObjectByInternalName("sceneShapeOpenSteerList");//AiOpenSteerList
   $mmSceneAutoplay = $pref::MegaMotion::SceneAutoplay;
   
   $mmSceneShapeBehaviorTreeList.add("",0);
   for (%i=0;%i<BehaviorTreeGroup.getCount();%i++)
   {
      %name = BehaviorTreeGroup.getObject(%i).getName();
      $mmSceneShapeBehaviorTreeList.add(%name,%i+1);//Can't start with zero, because that is blank line.
   }
   
   //behaviorList
   //Whoops, getting rid of the list for now, changing back to a text edit.
   
   //groupList
   $mmShapeGroupList.clear();
   %query = "SELECT id,name FROM shapeGroup ORDER BY name;";
   %resultSet = sqlite.query(%query, 0);
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         $mmShapeGroupList.add("",0);  
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmShapeGroupList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }
   
   %query = "SELECT id,name FROM openSteerProfile ORDER BY name;";
   %resultSet = sqlite.query(%query, 0);

   %id = "0";
   %name = "";
   //$mmOpenSteerList.add(%name @ "   " @ %id,%id);

   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         $mmOpenSteerList.add("",0);  
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmOpenSteerList.add(%name @ "   " @ %id,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }
}

function mmSetupSceneTab()
{
   //Nothing to do yet.   
}

function mmRefreshShapeLists()
{   
   %query = "SELECT id,name FROM physicsShape ORDER BY name;";
   %resultSet = sqlite.query(%query, 0);
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         $mmShapeList.add("",0);   
         $mmShapeMountChildShapeList.add("",0);         
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmShapeList.add(%name,%id);
            $mmShapeMountChildShapeList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
}

function mmLoadOpenSteerProfiles()  //NOPE!!! Using an array like this does not work, apparently.
{  //Now, this is surely better than querying the database each time, but now we're adding twelve
   //new global variables for every new openSteerProfile, which is hardly optimal either. Might want
   //to create a new object type in the engine and set its values here instead.
   %query = "SELECT * FROM openSteerProfile ORDER BY id;";
   %resultSet = sqlite.query(%query, 0);
   while (!sqlite.endOfResult(%resultSet))
   {
      %id = sqlite.getColumn(%resultSet, "id");
      %maxForce = sqlite.getColumn(%resultSet,"maxForce");
      $openSteerProfile[%id].mass = sqlite.getColumn(%resultSet,"mass");
      $openSteerProfile[%id].radius = sqlite.getColumn(%resultSet,"radius");
      $openSteerProfile[%id].maxForce = sqlite.getColumn(%resultSet,"maxForce");
      $openSteerProfile[%id].maxSpeed = sqlite.getColumn(%resultSet,"maxSpeed");
      $openSteerProfile[%id].wanderChance = sqlite.getColumn(%resultSet,"wanderChance");
      $openSteerProfile[%id].wanderWeight = sqlite.getColumn(%resultSet,"wanderWeight");
      $openSteerProfile[%id].seekTargetWeight = sqlite.getColumn(%resultSet,"seekTargetWeight");
      $openSteerProfile[%id].avoidTargetWeight = sqlite.getColumn(%resultSet,"avoidTargetWeight");
      $openSteerProfile[%id].seekNeighborWeight = sqlite.getColumn(%resultSet,"seekNeighborWeight");
      $openSteerProfile[%id].avoidNeighborWeight = sqlite.getColumn(%resultSet,"avoidNeighborWeight");
      $openSteerProfile[%id].avoidNavMeshEdgeWeight = sqlite.getColumn(%resultSet,"avoidNavMeshEdgeWeight");
      $openSteerProfile[%id].detectNavMeshEdgeRange = sqlite.getColumn(%resultSet,"detectNavMeshEdgeRange");      
      echo("setting up opensteer profile " @ %id @ ", maxForce " @ %maxForce);
      sqlite.nextRow(%resultSet);       
   }
}

//////////////////////////////////////////////////


function updateMegaMotionForm()
{
   //This is the big Commit Button at the top of the form. 
   //Making it commit whichever tab is on top.
   
   //NOTE: these need to be fixed anytime we add another tab.    
   %physicsTab = 0;
   %bvhTab = 1;
   %sequencesTab = 2;
   %aiTab = 3;
   %sceneShapeTab = 4;
   
   %selectedTab = $mmTabBook.getSelectedPage();
   
   echo("Tab book selected page: " @ %selectedTab);
   if (%selectedTab == %physicsTab)
   {
      mmUpdatePhysicsTab();      
   }
   else if (%selectedTab == %bvhTab)
   {
      mmUpdateBvhTab();    
   }
   else if (%selectedTab == %sequencesTab)
   {
      mmUpdateSequenceTab();    
   }
   else if (%selectedTab == %sceneShapeTab)
   {
      mmUpdateSceneShapeTab();
   }
   else if (%selectedTab == %aiTab)
   {
      mmUpdateAiTab();    
   }
   
   if ($mmLoadedScenes>0)
   {//FIX: need to track which scenes are currently loaded, and reload all of them and only them.)
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
}

function mmUpdatePhysicsTab()
{//First shape part properties, then joint properties...
 //FIX: All of these fields need to check for blank spaces, will crash query.
   if (!isObject($mmSelectedShape))
   {
      echo("Physics Tab Commit failed, no sceneShape selected!");
      return;
   }
      
   //%tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = $mmShapePartTab.findObjectByInternalName("shapePartPanel");
   %partID = $mmShapePartList.getSelected();
   %jointID = $mmJointList.getSelected();
   
   if (%partID<=0) 
   {      
      echo("Physics Tab Commit failed, partId <=0 ! partId " @ %partID);
      return;
   }
   %dimensionsX = %panel.findObjectByInternalName("shapePartDimensionsX").getText();
   %dimensionsY = %panel.findObjectByInternalName("shapePartDimensionsY").getText();
   %dimensionsZ = %panel.findObjectByInternalName("shapePartDimensionsZ").getText();   
   %orientationX = %panel.findObjectByInternalName("shapePartOrientationX").getText();
   %orientationY = %panel.findObjectByInternalName("shapePartOrientationY").getText();
   %orientationZ = %panel.findObjectByInternalName("shapePartOrientationZ").getText();   
   %offsetX = %panel.findObjectByInternalName("shapePartOffsetX").getText();
   %offsetY = %panel.findObjectByInternalName("shapePartOffsetY").getText();
   %offsetZ = %panel.findObjectByInternalName("shapePartOffsetZ").getText();  
   %jointRot1X = %panel.findObjectByInternalName("shapePartJointRot1X").getText();
   %jointRot1Y = %panel.findObjectByInternalName("shapePartJointRot1Y").getText();
   %jointRot1Z = %panel.findObjectByInternalName("shapePartJointRot1Z").getText();  
   %jointRot2X = %panel.findObjectByInternalName("shapePartJointRot2X").getText();
   %jointRot2Y = %panel.findObjectByInternalName("shapePartJointRot2Y").getText();
   %jointRot2Z = %panel.findObjectByInternalName("shapePartJointRot2Z").getText();
   
   %query = "UPDATE physicsShapePart SET ";
   %query = %query @ "dimensions_x=" @ %dimensionsX;
   %query = %query @ ",dimensions_y=" @ %dimensionsY;
   %query = %query @ ",dimensions_z=" @ %dimensionsZ;
   %query = %query @ ",orientation_x=" @ %orientationX;
   %query = %query @ ",orientation_y=" @ %orientationY;
   %query = %query @ ",orientation_z=" @ %orientationZ;   
   %query = %query @ ",offset_x=" @ %offsetX;
   %query = %query @ ",offset_y=" @ %offsetY;
   %query = %query @ ",offset_z=" @ %offsetZ;
   %query = %query @ ",joint_x=" @ %jointRot1X;
   %query = %query @ ",joint_y=" @ %jointRot1Y;
   %query = %query @ ",joint_z=" @ %jointRot1Z;
   %query = %query @ ",joint_x_2=" @ %jointRot2X;
   %query = %query @ ",joint_y_2=" @ %jointRot2Y;
   %query = %query @ ",joint_z_2=" @ %jointRot2Z;
   %query = %query @ " WHERE id=" @ %partID @ ";";
   sqlite.query(%query,0);
   
   if (%jointID<=0)
      return;
      
   %twistLimit = %panel.findObjectByInternalName("jointTwistLimit").getText();
   %swingLimit = %panel.findObjectByInternalName("jointSwingLimit").getText();
   %swingLimit2 = %panel.findObjectByInternalName("jointSwingLimit2").getText();
   %xLimit = %panel.findObjectByInternalName("jointXLimit").getText();
   %yLimit = %panel.findObjectByInternalName("jointYLimit").getText();
   %zLimit = %panel.findObjectByInternalName("jointZLimit").getText();
   %axisX = %panel.findObjectByInternalName("jointAxisX").getText();
   %axisY = %panel.findObjectByInternalName("jointAxisY").getText();
   %axisZ = %panel.findObjectByInternalName("jointAxisZ").getText();
   %normalX = %panel.findObjectByInternalName("jointNormalX").getText();
   %normalY = %panel.findObjectByInternalName("jointNormalY").getText();
   %normalZ = %panel.findObjectByInternalName("jointNormalZ").getText();
   %twistSpring = %panel.findObjectByInternalName("jointTwistSpring").getText();
   %swingSpring = %panel.findObjectByInternalName("jointSwingSpring").getText();
   %springDamper = %panel.findObjectByInternalName("jointSpringDamper").getText();
   %motorSpring = %panel.findObjectByInternalName("jointMotorSpring").getText();
   %motorDamper = %panel.findObjectByInternalName("jointMotorDamper").getText();
   %maxForce = %panel.findObjectByInternalName("jointMaxForce").getText();
   %maxTorque = %panel.findObjectByInternalName("jointMaxTorque").getText();
   
   %query = "UPDATE px3Joint SET ";
   %query = %query @ "twistLimit=" @ %twistLimit;
   %query = %query @ ",swingLimit=" @ %swingLimit;
   %query = %query @ ",swingLimit2=" @ %swingLimit2;
   %query = %query @ ",xLimit=" @ %xLimit;
   %query = %query @ ",yLimit=" @ %yLimit;
   %query = %query @ ",zLimit=" @ %zLimit;
   %query = %query @ ",localAxis_x=" @ %axisX;
   %query = %query @ ",localAxis_y=" @ %axisY;
   %query = %query @ ",localAxis_z=" @ %axisZ;
   %query = %query @ ",localNormal_x=" @ %normalX;
   %query = %query @ ",localNormal_y=" @ %normalY;
   %query = %query @ ",localNormal_z=" @ %normalZ;
   %query = %query @ ",twistSpring=" @ %twistSpring;
   %query = %query @ ",swingSpring=" @ %swingSpring;
   %query = %query @ ",springDamper=" @ %springDamper;
   %query = %query @ ",motorSpring=" @ %motorSpring;
   %query = %query @ ",motorDamper=" @ %motorDamper;
   %query = %query @ ",maxForce=" @ %maxForce;
   %query = %query @ ",maxTorque=" @ %maxTorque;
   %query = %query @ " WHERE id=" @ %jointID @ ";";
   sqlite.query(%query,0);  
   
   loadJointData();
   
   if ($mmLoadedScenes>0) //TEMP! Better ways, from worst to best:
   {   // 1) delete/load just this character 2) delete/load physics bodies on this character
       //(Except, actually all instance of this shape. Maybe whole scene reload is not so bad.)
      %sceneID = $mmSelectedShape.sceneID;
      %sceneShapeID = $mmSelectedShape.sceneShapeID;
      mmUnloadScene(%sceneID);//BUT, don't use scene list getSelected, because
      mmLoadScene(%sceneID);//that could have changed since you loaded this guy.
      $mmSceneShapeList.setSelected(%sceneShapeID);
      $mmShapePartList.setSelected(%partID);
      $mmJointList.setSelected(%jointID);
   }   
   
}

function mmUpdateBvhTab()
{
   %panel = $mmBvhTab.findObjectByInternalName("bvhPanel");   
   
   %skeletonNodeId = $mmBvhLinkedNodesList.getSelected();
   
   %poseRotAX = %panel.findObjectByInternalName("bvhPoseRotAX").getText();
   %poseRotAY = %panel.findObjectByInternalName("bvhPoseRotAY").getText();
   %poseRotAZ = %panel.findObjectByInternalName("bvhPoseRotAZ").getText();  
   %poseRotBX = %panel.findObjectByInternalName("bvhPoseRotBX").getText();
   %poseRotBY = %panel.findObjectByInternalName("bvhPoseRotBY").getText();
   %poseRotBZ = %panel.findObjectByInternalName("bvhPoseRotBZ").getText();   
   %fixRotAX = %panel.findObjectByInternalName("bvhFixRotAX").getText();
   %fixRotAY = %panel.findObjectByInternalName("bvhFixRotAY").getText();
   %fixRotAZ = %panel.findObjectByInternalName("bvhFixRotAZ").getText();   
   %fixRotBX = %panel.findObjectByInternalName("bvhFixRotBX").getText();
   %fixRotBY = %panel.findObjectByInternalName("bvhFixRotBY").getText();
   %fixRotBZ = %panel.findObjectByInternalName("bvhFixRotBZ").getText(); 
   
   %query = "UPDATE bvhProfileSkeletonNode SET ";
   %query = %query @ "poseRotA_x=" @ %poseRotAX;
   %query = %query @ ",poseRotA_y=" @ %poseRotAY;
   %query = %query @ ",poseRotA_z=" @ %poseRotAZ;
   %query = %query @ ",poseRotB_x=" @ %poseRotBX;
   %query = %query @ ",poseRotB_y=" @ %poseRotBY;
   %query = %query @ ",poseRotB_z=" @ %poseRotBZ;
   %query = %query @ ",fixRotA_x=" @ %fixRotAX;
   %query = %query @ ",fixRotA_y=" @ %fixRotAY;
   %query = %query @ ",fixRotA_z=" @ %fixRotAZ;
   %query = %query @ ",fixRotB_x=" @ %fixRotBX;
   %query = %query @ ",fixRotB_y=" @ %fixRotBY;
   %query = %query @ ",fixRotB_z=" @ %fixRotBZ;
   %query = %query @ " WHERE id=" @ %skeletonNodeId @ ";";
   //echo(%query);
   sqlite.query(%query,0);  //HMMM this seems to cause a crash, but not until we're off in the 
   // keyframes section on reload. (???)
}

function mmUpdateSequenceTab()
{
   //do nothing here yet (so far all sequence tab changes are instant, no "Commit" button changes.
}


//HERE: still need to fix all occurrences of vector3/rotation or new position/rotation/scale tables,
//to restore system of vectors included in the parent table instead.
function mmUpdateSceneShapeTab()
{
   %panel = $mmSceneShapeTab.findObjectByInternalName("sceneShapePanel");
   
   %sceneShapeID = $mmSceneShapeList.getSelected();
   if (%sceneShapeID<=0)
      return;

   %pos_x = $mmSceneShapePositionX.getText();
   %pos_y = $mmSceneShapePositionY.getText();
   %pos_z = $mmSceneShapePositionZ.getText();
   //%query = "UPDATE vector3 SET x=" @ %pos_x @ ",y=" @ %pos_y @ ",z=" @ %pos_z @ 
   //         " WHERE id=" @ MegaMotionScenes.pos_id @ ";";
   //sqlite.query(%query, 0); 
   
   %rot_x = $mmSceneShapeOrientationX.getText();
   %rot_y = $mmSceneShapeOrientationY.getText();
   %rot_z = $mmSceneShapeOrientationZ.getText();
   %rot_a = $mmSceneShapeOrientationAngle.getText();
   //%query = "UPDATE rotation SET x=" @ %rot_x @ ",y=" @ %rot_y @ ",z=" @ %rot_z @ 
   //          ",angle=" @ %rot_a @ " WHERE id=" @ MegaMotionScenes.rot_id @ ";";
   //sqlite.query(%query, 0); 
   
   %scale_x = $mmSceneShapeScaleX.getText();
   %scale_y = $mmSceneShapeScaleY.getText();
   %scale_z = $mmSceneShapeScaleZ.getText();
   //%query = "UPDATE vector3 SET x=" @ %scale_x @ ",y=" @ %scale_y @ ",z=" @ %scale_z @ 
   //         " WHERE id=" @ MegaMotionScenes.scale_id @ ";";
   //sqlite.query(%query, 0); 
   
   %query = "UPDATE sceneShape SET pos_x=" @ %pos_x @ ",pos_y=" @ %pos_y @ ",pos_z=" @ %pos_z @ 
            ",rot_x=" @ %rot_x @ ",rot_y=" @ %rot_y @ ",rot_z=" @ %rot_z @ ",rot_a=" @ %rot_a @ 
            ",scale_x=" @ %scale_x @ ",scale_y=" @ %scale_y @ ",scale_z=" @ %scale_z @
            " WHERE id=" @ %sceneShapeID @ ";";
   sqlite.query(%query,0);
   
   //Shape Mounts
   %shapeMountID = %panel.findObjectByInternalName("shapeMountList").getSelected();
   echo("Updated pos " @ MegaMotionScenes.pos_id @ ", rot " @ MegaMotionScenes.rot_id @ 
         " shapeMount " @ %shapeMountID @  " Stored child shape " @ MegaMotionScenes.mount_child_id);
         
   if ((%shapeMountID <= 0)&&($mmLoadedScenes>0))
   {
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
      return;
   }
   
   %parentNode = %panel.findObjectByInternalName("shapeMountParentNodeList").getSelected();
   %childShape = %panel.findObjectByInternalName("shapeMountChildShapeList").getSelected();
   %childNode = %panel.findObjectByInternalName("shapeMountChildNodeList").getSelected();
   
   if (MegaMotionScenes.mount_child_shape != %childShape)
   {//We've changed our shape, so we need to go set it in the sceneShape table.
      %query = "UPDATE sceneShape SET shape_id=" @ %childShape @ " WHERE id=" @ 
                  MegaMotionScenes.mount_child_id @ ";";
      sqlite.query(%query,0);
   }
   
   if ($mmLoadedScenes>0)
   {
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
   
   echo("Reloaded scene after updating sceneShapeTab!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
}

function mmUpdateAiTab()
{
   %multiselect = false;
   %panel = $mmAiTab.findObjectByInternalName("aiPanel");
   
   //if 
   %sceneShapeId = $mmSceneShapeList.getSelected();
   
   if (%openSteer_id>0)
   {
      %tab = $mmTabBook.findObjectByInternalName("sceneShapeTab");
      %panel = %tab.findObjectByInternalName("sceneShapePanel");
   
      %mass = %panel.findObjectByInternalName("sceneShapeOpenSteerMass");
      %radius = %panel.findObjectByInternalName("sceneShapeOpenSteerRadius");
      %maxForce = %panel.findObjectByInternalName("sceneShapeOpenSteerMaxForce");
      %maxSpeed = %panel.findObjectByInternalName("sceneShapeOpenSteerMaxSpeed");
      %wanderChance = %panel.findObjectByInternalName("sceneShapeOpenSteerWanderChance");
      %wanderWeight = %panel.findObjectByInternalName("sceneShapeOpenSteerWanderWeight");
      %seekTarget = %panel.findObjectByInternalName("sceneShapeOpenSteerSeekTarget");
      %avoidTarget = %panel.findObjectByInternalName("sceneShapeOpenSteerAvoidTarget");
      %seekNeighbor = %panel.findObjectByInternalName("sceneShapeOpenSteerSeekNeighbor");
      %avoidNeighbor = %panel.findObjectByInternalName("sceneShapeOpenSteerAvoidNeighbor");
      %avoidEdge = %panel.findObjectByInternalName("sceneShapeOpenSteerAvoidEdge");
      %detectEdge = %panel.findObjectByInternalName("sceneShapeOpenSteerDetectEdge");      
      
      %query = "UPDATE openSteerProfile SET " @
               "mass=" @ %mass.getText() @ ",radius=" @ %radius.getText() @ 
               ",maxForce=" @ %maxForce.getText() @ ",maxSpeed=" @ %maxSpeed.getText() @ 
               ",wanderChance=" @ %wanderChance.getText() @ ",wanderWeight=" @ %wanderWeight.getText() @ 
               ",seekTargetWeight=" @ %seekTarget.getText() @ ",avoidTargetWeight=" @ %avoidTarget.getText() @ 
               ",seekNeighborWeight=" @ %seekNeighbor.getText() @ ",avoidNeighborWeight=" @ %avoidNeighbor.getText() @ 
               ",avoidNavMeshEdgeWeight=" @ %avoidEdge.getText() @ ",detectNavMeshEdgeRange=" @ %detectEdge.getText() @ 
               " WHERE id=" @ %openSteer_id @ ";";
      echo("Changing openSteer data: \n " @ %query);
      sqlite.query(%query,0);
   }   
}

////////////////////////////////////////////////////////////////////////////////////
//REFACTOR: Still working out the MegaMotion vs openSimEarth division of labor.

function mmPullSceneShapes(%simGroup)
{//So, here we need to remove objects from the MissionGroup and put them into another simGroup.
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i);  
      %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function mmPullSceneShapesAndSave(%simGroup)
{
   MegaMotionSaveSceneShapes();
   
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i);  
      %simGroup.add(%obj);
   }
   
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.remove(%simGroup.getObject(%i));
   }
}

function mmPushSceneShapes(%simGroup)
{
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.add(%simGroup.getObject(%i));
   }
}
//Direct copy of EditorSaveMission from menuHandlers.ed.cs. This version exists
//because mission save is actually just SimObject::save, and that is way too deep 
//into T3D to be making application level changes. Instead, we just call this one,
//but we are still going to have problems with all the plugins until we keep them 
//from calling MissionGroup.save() on their own.

function MegaMotionSaveMission()
{
   echo("Calling MegaMotionSaveMission!!!!!!!!!!!!!!!!!!!!!!!!!!!");
   //if(isFunction("getObjectLimit") && MissionGroup.getFullCount() >= getObjectLimit())
   //{ //(Object limit in trial version, ossible way to nag licensing compliance?)
   //   MessageBoxOKBuy( "Object Limit Reached", "You have exceeded the object limit of " @ getObjectLimit() @ " for this demo. You can remove objects if you would like to add more.", "", "Canvas.showPurchaseScreen(\"objectlimit\");" );
   //   return;
   //}
   
   // first check for dirty and read-only files:
   if((EWorldEditor.isDirty || ETerrainEditor.isMissionDirty) && !isWriteableFileName($Server::MissionFile))
   {
      MessageBox("Error", "Mission file \""@ $Server::MissionFile @ "\" is read-only.  Continue?", "Ok", "Stop");
      return false;
   }
   if(ETerrainEditor.isDirty)
   {
      // Find all of the terrain files
      initContainerTypeSearch($TypeMasks::TerrainObjectType);

      while ((%terrainObject = containerSearchNext()) != 0)
      {
         if (!isWriteableFileName(%terrainObject.terrainFile))
         {
            if (MessageBox("Error", "Terrain file \""@ %terrainObject.terrainFile @ "\" is read-only.  Continue?", "Ok", "Stop") == $MROk)
               continue;
            else
               return false;
         }
      }
   }
  
   // now write the terrain and mission files out - for now, only one option, always do it.
   %tempSceneShapes = new SimSet();
   //if ($pref::MegaMotion::saveSceneShapes)//Do this regardless of terrainPager or not.
   mmPullSceneShapesAndSave(%tempSceneShapes);
   //else  //In the future perhaps provide a checkbox for whether to save scene shapes or not,
   //   mmPullSceneShapes(%tempSceneShapes); //but for now we do, so unload them or leave them 
                                             // where you wnat them before working on mission.
      
   
   ///////////////////////////   
   //For terrainPager/openSimEarth, we need to save many things to the database instead of to 
   //the mission. Do not do this if we don't have a TerrainPager.
   if (isObject(theTP))//FIX!!! Search for objects of type TerrainPager, regardless of name.
   {
      %tempStaticGroup = new SimSet();
      %tempRoadGroup = new SimSet();
      %tempForestGroup = new SimSet();
      
      if ($pref::MegaMotion::saveStatics)
         osePullStaticsAndSave(%tempStaticGroup);
      else
         osePullStatics(%tempStaticGroup);
      
      if ($pref::MegaMotion::saveRoads)
         osePullRoadsAndSave(%tempRoadGroup);
      else
         osePullRoads(%tempRoadGroup);
      
      //if ($pref::MegaMotion::saveForests==false)
      //   osePullForest($tempForestGroup);
   
   }
   
   if(EWorldEditor.isDirty || ETerrainEditor.isMissionDirty)
      MissionGroup.save($Server::MissionFile);
      
   mmPushSceneShapes(%tempSceneShapes);//Do this regardless of terrainPager or not.
   
   if (isObject(theTP))//FIX!!! Search for objects of type TerrainPager, regardless of name.
   {   
      osePushStatics(%tempStaticGroup);
      osePushRoads(%tempRoadGroup);
      //osePushForest($tempForestGroup);
   }
   
   ///////////////////////////   
   
   
   if(ETerrainEditor.isDirty)
   {
      // Find all of the terrain files
      initContainerTypeSearch($TypeMasks::TerrainObjectType);

      while ((%terrainObject = containerSearchNext()) != 0)
         %terrainObject.save(%terrainObject.terrainFile);
   }

   ETerrainPersistMan.saveDirty();
      
   // Give EditorPlugins a chance to save.
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject(%i);
      if ( %obj.isDirty() )
         %obj.onSaveMission( $Server::MissionFile );      
   } 
   
   EditorClearDirty();
   
   EditorGui.saveAs = false;
   
   //NOW: since we've moved to an all-cached DB context, save it out to disk every time we save mission, in order to 
   %dbname = $pref::MegaMotion::DB;   //help minimize data losses in case of a crash.
   sqlite.loadOrSaveDb(%dbname,true); 
   
   return true;
}

function MegaMotionSaveSceneShapes()
{
   %startTime = getTime();
   echo("Saving scene shapes, start time " @ %startTime);
   //NOW: find all physicsShapes, and save each of them 

   //BUT: let's try doing this in the engine
   saveSceneShapes();//AND, there we have it. Prepared statements in sqlite helped quite a bit, though
   //it will probably still be too slow until we implement a cached database. 
     
   /*
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i);  
      if ((%obj.sceneShapeID>0)&&(%obj.sceneID>0))//&&(%obj.isDirty)//isDirty isn't always being set - slower, but do them all for now.
      {
         %trans = %obj.getTransform();
         
         %p_x = getWord(%trans,0);
         %p_y = getWord(%trans,1);
         %p_z = getWord(%trans,2);
         
         %r_x = getWord(%trans,3);
         %r_y = getWord(%trans,4);
         %r_z = getWord(%trans,5);
         %r_a = mRadToDeg(getWord(%trans,6));
         
         %scale = %obj.getScale();
         %s_x = getWord(%scale,0);
         %s_y = getWord(%scale,1);
         %s_z = getWord(%scale,2);
         
         %query = "UPDATE sceneShape SET pos_x=" @ %p_x @ ",pos_y=" @ %p_y @ ",pos_z=" @ %p_z @
                  ",rot_x=" @ %r_x @ ",rot_y=" @ %r_y @ ",rot_z=" @ %r_z @ ",rot_a=" @ %r_a @ 
                  ",scale_x=" @ %s_x @ ",scale_y=" @ %s_y @ ",scale_z=" @ %s_z @
                  " WHERE id=" @ %obj.sceneShapeID @ ";";
         sqlite.query(%query,0);          
      }
   }
   */
   %endTime = getTime();
   %elapsed = %endTime - %startTime;
   echo("Done saving scene shapes, end time " @ %endTime @ " elapsed " @ %elapsed);
}
         
////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

function mmSelectProject()
{   
   if ($mmProjectList.getSelected()<=0)
      return;
    
   $mmLastProject = $mmProjectList.getSelected();
   $mmSceneList.clear();  
   
   %firstID = 0;
   %query = "SELECT id,name FROM scene WHERE project_id=" @ $mmProjectList.getSelected() @ " ORDER BY name;";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmSceneList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }         
         if ($mmLastScene>0)
            $mmSceneList.setSelected($mmLastScene);
         else if (%firstID>0)
            $mmSceneList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }      
}

function mmAddProject()
{
   makeSqlGuiForm($mmAddProjectWindowID);
}

function mmReallyAddProject()
{
   if (mmAddProjectWindow.isVisible())
   {
      %name = mmAddProjectWindow.findObjectByInternalName("nameEdit").getText(); 
      if ((strlen(%name)==0)||(substr(%name," ")>0))
      {
         MessageBoxOK("Name Invalid","Project name must be a unique character string with no spaces or special characters.","");
         return;  
      }
      %query = "SELECT id FROM project WHERE name='" @ %name @ "';";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)>0)
      {
         MessageBoxOK("Name Invalid","Project name must be unique.","");
         return;
      }
      %query = "INSERT INTO project (name) VALUES ('" @ %name @ "');";
      sqlite.query(%query,0);
      
      mmAddProjectWindow.delete();
      
      exposeMegaMotionScenesForm();
   }
}

function mmDeleteProject()
{
   %project_id = $mmProjectList.getSelected();
   if (%project_id<=0)
      return;
      
   MessageBoxOKCancel( "Warning", 
      "This will permanently delete this project and all of its scenes! Are you completely sure?", 
      "mmReallyDeleteProject();",
      "" ); 
      
}
   
function mmReallyDeleteProject()
{
   %project_id = $mmProjectList.getSelected();
      
   %query = "SELECT id FROM scene WHERE project_id=" @ %project_id @ ";";
   %resultSet = sqlite.query(%query,0);
   if (sqlite.numRows(%resultSet)>0)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         mmReallyDeleteScene(sqlite.getColumn(%resultSet,"id") );
         sqlite.nextRow(%resultSet);  
      }
      sqlite.clearResult(%resultSet);      
   }
   
   //(This could be accomplished with cascade deletes, but let's keep doing it manually for now.)
   %query = "DELETE FROM scene WHERE project_id=" @ %project_id @ ";";
   sqlite.query(%query,0);
   
   %query = "DELETE FROM project WHERE id=" @ %project_id @ ";";
   sqlite.query(%query,0);
   
   exposeMegaMotionScenesForm();
}


function mmLoadProject()
{
   //Maybe we will want to load shared static object props used by multiple scenes? 
    
}

function mmUnloadProject()
{
   //Remove project environment objects?
   
}

function mmSelectScene()
{
   echo("calling selectMegaMotionScene");
   
   if ($mmSceneList.getSelected()<=0)
      return;
      
   $mmLastScene = $mmSceneList.getSelected();
   $mmSceneShapeList.clear(); 
   $mmTargetShapeList.clear(); 
   $mmTargetShapeList.add("",0);
   echo("calling selectMegaMotionScene on scene " @ $mmSceneList.getSelected());
   
   %firstID = 0;
   %query = "SELECT ss.id,ps.id AS ps_id, ps.name FROM sceneShape ss " @
	         "JOIN physicsShape ps ON ps.id=ss.shape_id " @
            "WHERE scene_id=" @ $mmSceneList.getSelected() @ ";";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      //echo("adding " @ sqlite.numRows(%resultSet) @ " scene shapes!");
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name") @ " - " @ %id;
            $mmSceneShapeList.add(%name,%id);
            $mmTargetShapeList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) //Don't do this after all, because it fills up the sceneShape tab before the
         //   $mmSceneShapeList.setSelected(%firstID);// user has actually selected a shape.
      }
      sqlite.clearResult(%resultSet);
   }   
}

function mmAddScene()
{
   makeSqlGuiForm($mmAddSceneWindowID);   
}

function mmReallyAddScene()  //TO DO: Description, position.
{  
   if (mmAddSceneWindow.isVisible())
   {
      %name = mmAddSceneWindow.findObjectByInternalName("nameEdit").getText(); 
      if ((strlen(%name)==0)||(substr(%name," ")>0))
      {
         MessageBoxOK("Name Invalid","Scene name must be a unique character string with no spaces or special characters.","");
         return;  
      }
      %proj_id = $mmProjectList.getSelected();
      %query = "SELECT id FROM scene WHERE name='" @ %name @ "' AND project_id=" @ %proj_id @ ";";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)>0)
      {
         MessageBoxOK("Name Invalid","Scene name must be unique for this project.","");
         return;
      }
      sqlite.clearResult(%resultSet);
      
      //HERE: need scene position XYZ fields.
      //%query = "INSERT INTO vector3 (x,y,z) VALUES (0,0,0);";
      //sqlite.query(%query,0);      
      
      %query = "INSERT INTO scene (name,project_id,pos_x,pos_y,pos_z) VALUES ('" @ %name @ "'," @ %proj_id @ 
                  ",0,0,0);";//FIX!! add pos x,y,z to the Add Scene form.
      sqlite.query(%query,0);
            
      mmAddSceneWindow.delete();
      
      exposeMegaMotionScenesForm();
   }
}

function mmDeleteScene()
{
   %scene_id = $mmSceneList.getSelected();
   if (%scene_id<=0)
      return;
      
   MessageBoxOKCancel( "Warning", 
      "This will permanently delete this scene and all of its sceneShapes! Are you completely sure?", 
      "mmReallyDeleteScene(" @ %scene_id @ ");",
      "" );    
}

function mmReallyDeleteScene(%id)
{
   mmUnloadScene(%id);
   
   //(This could be accomplished with cascade deletes, but let's keep doing it manually for now.)
   //Except, DERP, now that everything is in the one table, there is a much simpler way than this...
   /*
   %query = "SELECT id FROM sceneShape WHERE scene_id=" @ %id @ ";";
   %resultSet = sqlite.query(%query,0);
   %id_list = "";
   if (sqlite.numRows(%resultSet)>0)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         //NOPE! Now we will simply list all the ids, and do one query here to delete them all,
         //mmReallyDeleteSceneShape(%shape_id); //instead of calling this function and doing many queries.
         
         %shape_id = sqlite.getColumn(%resultSet,"id");
         if (strlen(%id_list)==0)
            %id_list =  %shape_id;
         else
            %id_list = %id_list @ "," @ %shape_id;
     
         sqlite.nextRow(%resultSet); 
      }
      sqlite.clearResult(%resultSet);
   }
   
   %query = "DELETE FROM sceneShape WHERE id IN (" @ %id_list @ ");";
   sqlite.query(%query,0);   
   */
   %query = "DELETE FROM sceneShape WHERE scene_id=" @ %id @ ";";
   sqlite.query(%query,0);   
   
   %query = "DELETE FROM scene WHERE id=" @ %id @ ";";
   sqlite.query(%query,0);
   
   exposeMegaMotionScenesForm();
}

function mmLoadScene(%id)
{      
   if (%id<=0)
      return;
      
   //FIRST: check to see if we've already loaded this scene! Which we are going
   //to do by checking if there are any existing sceneShapes from this scene.
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.sceneID==%id)
      {
         MessageBoxOK("Scene already loaded.","Shapes from this scene are already present. Unload scene before adding again.","");
         return;  
      }
   }
   
   //HERE: all major SQL query loops need to be done in the engine, they take too long in script. (benchmark this btw)
   //loadSceneShapes(%id);//FIX: doing this later, this is a placeholder. 
   //EXCEPT - this really isn't an issue now, because it is now only one query, and really quite fast even in script.
   
   %dyn = false;
   %grav = true;
   %ambient = true;

	%query = "SELECT ss.id AS ss_id,ss.name AS ss_name,shape_id,shapeGroup_id," @ 
	         "behavior_tree,openSteerProfile_id,actionProfile_id,target_shape_id," @ 
	         "ss.pos_x,ss.pos_y,ss.pos_z," @ 
	         "ss.rot_x,ss.rot_y,ss.rot_z,ss.rot_a," @ 
	         "ss.scale_x,ss.scale_y,ss.scale_z," @ 
	         "s.pos_x AS scene_pos_x,s.pos_y AS scene_pos_y,s.pos_z AS scene_pos_z," @ 
	         "sh.datablock AS datablock, sh.skeleton_id AS skeleton_id " @ 
	         "FROM sceneShape ss " @ 
	         "JOIN scene s ON s.id=scene_id " @
	         "JOIN physicsShape sh ON ss.shape_id=sh.id " @ 
	         "WHERE scene_id=" @ %id @ ";";  
	%resultSet = sqlite.query(%query, 0);
	
	echo("calling loadScene, result " @ %resultSet);
   echo( "Query: " @ %query );	
	
   if (%resultSet)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         %sceneShape_id = sqlite.getColumn(%resultSet, "ss_id");   
         %shape_id = sqlite.getColumn(%resultSet, "shape_id");
         %name = sqlite.getColumn(%resultSet, "ss_name");
         %shapeGroup_id = sqlite.getColumn(%resultSet, "shapeGroup_id");//not used yet
         %behavior_tree = sqlite.getColumn(%resultSet, "behavior_tree");
         %openSteer_id = sqlite.getColumn(%resultSet, "openSteerProfile_id");
         %actionProfile_id = sqlite.getColumn(%resultSet, "actionProfile_id");
         %target_shape = sqlite.getColumn(%resultSet, "target_shape_id");
         
         %pos_x = sqlite.getColumn(%resultSet, "pos_x");
         %pos_y = sqlite.getColumn(%resultSet, "pos_y");
         %pos_z = sqlite.getColumn(%resultSet, "pos_z");
         
         %rot_x = sqlite.getColumn(%resultSet, "rot_x");
         %rot_y = sqlite.getColumn(%resultSet, "rot_y");
         %rot_z = sqlite.getColumn(%resultSet, "rot_z");
         %rot_a = sqlite.getColumn(%resultSet, "rot_a");
         
         %scale_x = sqlite.getColumn(%resultSet, "scale_x");
         %scale_y = sqlite.getColumn(%resultSet, "scale_y");
         %scale_z = sqlite.getColumn(%resultSet, "scale_z");
         
         %scene_pos_x = sqlite.getColumn(%resultSet, "scene_pos_x");
         %scene_pos_y = sqlite.getColumn(%resultSet, "scene_pos_y");
         %scene_pos_z = sqlite.getColumn(%resultSet, "scene_pos_z");
         
         %datablock = sqlite.getColumn(%resultSet, "datablock");
         %skeleton_id = sqlite.getColumn(%resultSet, "skeleton_id");
         
         //echo("Found a sceneShape: " @ %sceneShape_id @ " pos " @ %pos_x @ " " @ %pos_y @ " " @ %pos_z @
         //       " scale " @ %scale_x @ " " @ %scale_y @ " " @ %scale_z );
                
         %position = (%pos_x + %scene_pos_x) @ " " @ (%pos_y + %scene_pos_y) @ " " @ (%pos_z + %scene_pos_z);
         %rotation = %rot_x @ " " @ %rot_y @ " " @ %rot_z @ " " @ %rot_a;
         %scale = %scale_x @ " " @ %scale_y @ " " @ %scale_z;
         
         echo("loading sceneShape id " @ %shape_id @ " position " @ %position @ " rotation " @ 
               %rotation @ " scale " @ %scale);
         
         //TEMP -- use name from sceneShape table
         //%name = "";          
         //if (%shape_id==4)
         //   %name = "ka50";   
         //else if (%shape_id==3)
         //   %name = "bo105";
         //else if (%shape_id==2)
         //   %name = "dragonfly";
         //TEMP
            
         %pShape =  new PhysicsShape(%name) {
            playAmbient = %ambient;
            dataBlock = %datablock;
            position = %position;
            rotation = %rotation;
            scale = %scale;
            canSave = "1";
            canSaveDynamicFields = "1";
            areaImpulse = "0";
            damageRadius = "0";
            invulnerable = "0";
            minDamageAmount = "0";
            radiusDamage = "0";
            hasGravity = %grav;
            isDynamic = %dyn;
            shapeID = %shape_id;
            sceneShapeID = %sceneShape_id;
            sceneID = %id;
            skeletonID = %skeleton_id;
            openSteerID = %openSteer_id;
            actionProfileID = %actionProfile_id;
            shapeGroupID = %shapeGroup_id;
            targetShapeID = %target_shape;
            behaviorTreeName = %behavior_tree;
            targetType = "Health";//"AmmoClip" "Player" //Obsolete?
            targetItem = 0;
            isDirty = false;
         };
         //skeletonID = %skeleton_id;
         
         %pShape.setScale(%scale);//Maybe? Scale above is broken. 
         
         MissionGroup.add(%pShape);   
         SceneShapes.add(%pShape);  
         

         
         if ($mmSelectedSceneShape>0)
         {
            if ($mmSelectedSceneShape==%sceneShape_id)
            {
               $mmSelectedShape = %pShape;
               echo("reselecting selected shape! " @ %temp @ " sceneShape " @ %sceneShape_id);
            }
         }
         
         //echo("Adding a scene shape: " @ %sceneShape_id @ ", position " @ %position );
         
         if ((strlen(%behavior_tree)>0)&&(%behavior_tree!$="NULL")&&($mmSceneAutoplay))
         {
            %pShape.schedule(300,"setBehavior",%behavior_tree);
            //echo(%pShape.getId() @ " assigning behavior tree: " @ %behavior_tree );
         }
         %pShape.schedule(2000,"shapeSpecifics");
         sqlite.nextRow(%resultSet);
      }
   }   
   sqlite.clearResult(%resultSet);
   
   mmMountShapes(%id);   
   
   mmSceneSpecifics(%id);
   
   $mmLoadedScenes++;
   
   //schedule(40, 0, "mmLoadKeyframeSets");
} 

function mmSceneSpecifics(%id)
{  
   //One example of specific things one can do, on a per scene basis: grab all of the sceneShapes using 
   //a particular shape, in this case the Cube model, and put them in a special group for later access.
   if ((%id==$mmScene_ActorBlock_16x16_ID)||(%id==$mmScene_ActorBlock_3x40_ID)||(%id==$mmScene_ActorBlock_2x2_ID))
   {
      %temp = new SimSet(CubeShapes);
      for (%i = 0; %i < SceneShapes.getCount();%i++)
      {
         %obj = SceneShapes.getObject(%i); 
         if (%obj.shapeID==11) // Cube
         {
            CubeShapes.add(%obj);            
         }
      }
   }
}

function mmSetBehaviors(%id)
{
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.sceneID==%id)
      {
         if ((strlen(%obj.behaviorTreeName)>0)&&(%obj.behaviorTreeName!$="NULL"))
         {
            %obj.setBehavior(%obj.behaviorTreeName);   
            //%obj.getClientObject().setBehavior(%obj.behaviorTree);        
            //echo(%obj.getId() @ " assigning behavior tree: " @ %obj.behaviorTreeName @ " is server " @ %obj.isServerObject() );
         }         
      }
   }
}

//These are for custom code, whatever your needed action of the moment.
function mmSceneAction1()
{
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.shapeGroupID==1)
      {
         %client = %obj.getClientObject();
         %delay = 10 + getRandom(2600);
         %client.schedule(%delay,"applyImpulseToPart","2","0 0 0","0 -0.008 0.005"); 
         %obj.setBehavior("FallingTree");                    
      }
   }
}

function mmSceneAction2()
{
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.shapeGroupID==1)
      {
         %client = %obj.getClientObject();
         %delay = 10 + getRandom(2600);
         %client.schedule(%delay,"applyImpulseToPart","2","0 0 0","0 -0.02 0.015"); 
         %obj.setBehavior("FallingTree");                    
      }
   }
}

function mmSceneAction3()
{
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.shapeGroupID==1)
      {
         %client = %obj.getClientObject();
         %delay = 10 + getRandom(4000);
         %client.schedule(%delay,"applyImpulseToPart","2","0 0 0","0 -0.045 0.025"); 
         %obj.setBehavior("FallingTree");                    
      }
   }
}

function mmSceneAction4()
{
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.shapeGroupID==1)
      {
         %client = %obj.getClientObject();
         %delay = 10 + getRandom(4000);
         %client.schedule(%delay,"applyImpulseToPart","2","0 0 0","0 0.045 0.025"); 
         %obj.setBehavior("FallingTree");                    
      }
   }
}

function mmSceneAction5()
{
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.shapeGroupID==1)
      {
         %client = %obj.getClientObject();
         %delay = 10 + getRandom(2600);
         %client.schedule(%delay,"applyImpulseToPart","2","0 0 0","0 0.02 0.015"); 
         %obj.setBehavior("FallingTree");                    
      }
   } 
}

function mmSceneAction6()
{
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.shapeGroupID==1)
      {
         %client = %obj.getClientObject();
         %delay = 10 + getRandom(2600);
         %client.schedule(%delay,"applyImpulseToPart","2","0 0 0","0 0.008 0.005"); 
         %obj.setBehavior("FallingTree");                    
      }
   } 
}

function mmSceneAction7()
{
   echo("sceneAction7!");   
}

function mmSceneAction8()
{
   echo("sceneAction8!");   
}

function mmMountShapes(%id)
{
   //Now, do all shapeMounts - except only for shapes from this scene, so we don't do it twice.
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i); 
      if (%obj.sceneID==%id)
      {
         %obj.mountShapes();
      }     
   }
}

function mmUnloadScene(%id)
{
   if (%id<=0)
      return;
      
   if ($mmSelectedShape>0)
      $mmSelectedSceneShape = $mmSelectedShape.sceneShapeID;
      
   //HERE: look up all the sceneShapes from the scene in question, and drop them all from the current mission.
   %shapesCount = SceneShapes.getCount();
   for (%i=0;%i<%shapesCount;%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      //echo("shapesCount " @ %shapesCount @ ", sceneShape id " @ %shape.sceneShapeID @ 
      //         " scene " @ %shape.sceneID ); 
      if (%shape.sceneID==%id)
      {       
         //Whoops - *first*, we need to delete all physics shapes! (and joints? or is that automatic?)           
         //%shape.deletePhysicsBodies?
         
         MissionGroup.remove(%shape);
         SceneShapes.remove(%shape);//Wuh oh... removing from SceneShapes shortens the array...
         %shape.delete();//Maybe??
         
         %shapesCount = SceneShapes.getCount();
         if (%shapesCount>0)
            %i=-1;//So start over every time we remove one, until we loop through and remove none.
         else 
            %i=1;//Or else we run out of shapes, and just need to exit the loop.   
            
         $mmLoadedScenes--;
      }
   }   
}


function mmSelectSceneShape()
{
   //if (($mmSceneShapeList.getSelected()<=0)||(SceneShapes.getCount()==0))
   if ($mmSceneShapeList.getSelected()<=0)
      return;
      
   %scene_shape_id = $mmSceneShapeList.getSelected();
   
   $mmSequenceList.clear();
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i);  
      if (%obj.sceneShapeID==%scene_shape_id)
      {
         $mmSelectedShape = %obj;       
         %numSeqs = $mmSelectedShape.getNumSeqs();
         for (%j=0;%j<%numSeqs;%j++)
         {
            %name = $mmSelectedShape.getSeqName(%j);
            $mmSequenceList.add(%name,%j);         
         }
      }
   }
   
   //%query = "SELECT * FROM sceneShape WHERE id=" @ %sceneShapeId @ ";";
	%query = "SELECT shape_id,name,shapeGroup_id,behavior_tree,openSteerProfile_id," @ 
	         "pos_x,pos_y,pos_z," @ 
	         "rot_x,rot_y,rot_z,rot_a," @ 
	         "scale_x,scale_y,scale_z " @ 
	         "FROM sceneShape " @ 
	         "WHERE id=" @ %scene_shape_id @ ";";

   %resultSet = sqlite.query(%query, 0); 
   if (sqlite.numRows(%resultSet)==1)
   {
      %name = sqlite.getColumn(%resultSet, "name");
      %behavior_tree = sqlite.getColumn(%resultSet, "behavior_tree");
      $mmSceneShapeBehaviorTreeOrig = %behavior_tree;
      %shape_id = sqlite.getColumn(%resultSet, "shape_id");
      $mmShapeId = %shape_id;
      %group_id = sqlite.getColumn(%resultSet, "shapeGroup_id");
      $mmShapeGroupId = %group_id;
      %openSteer_id = sqlite.getColumn(%resultSet, "openSteerProfile_id");
      $mmOpenSteerId = %openSteer_id;
      %pos_x = sqlite.getColumn(%resultSet, "pos_x");
      %pos_y = sqlite.getColumn(%resultSet, "pos_y");
      %pos_z = sqlite.getColumn(%resultSet, "pos_z");
      %rot_x = sqlite.getColumn(%resultSet, "rot_x");
      %rot_y = sqlite.getColumn(%resultSet, "rot_y");
      %rot_z = sqlite.getColumn(%resultSet, "rot_z");
      %rot_a = sqlite.getColumn(%resultSet, "rot_a");
      %scale_x = sqlite.getColumn(%resultSet, "scale_x");
      %scale_y = sqlite.getColumn(%resultSet, "scale_y");
      %scale_z = sqlite.getColumn(%resultSet, "scale_z");
      //$mmScaleId = sqlite.getColumn(%resultSet, "scale_id");
      MegaMotionScenes.scale_id = sqlite.getColumn(%resultSet, "scale_id");
      
      $mmShapeList.setSelected(%shape_id);
      
      echo("selecting sceneShape! shapeGroup " @ %group_id @ " behavior " @ %behavior_tree);
      
      $mmSceneShapeBehaviorTreeList.setSelected($mmSceneShapeBehaviorTreeList.findText(%behavior_tree));
      $mmShapeGroupList.setSelected(%group_id);
      $mmOpenSteerList.setSelected(%openSteer_id);
      
      $mmSceneShapePositionX.setText(%pos_x);
      $mmSceneShapePositionY.setText(%pos_y);
      $mmSceneShapePositionZ.setText(%pos_z);
      
      $mmSceneShapeOrientationX.setText(%rot_x);
      $mmSceneShapeOrientationY.setText(%rot_y);
      $mmSceneShapeOrientationZ.setText(%rot_z);
      $mmSceneShapeOrientationAngle.setText(%rot_a);
      
      $mmSceneShapeScaleX.setText(%scale_x);
      $mmSceneShapeScaleY.setText(%scale_y);
      $mmSceneShapeScaleZ.setText(%scale_z);
      
      sqlite.clearResult(%resultSet);
      
      $mmShapeMountList.clear();
      %first_id = 0;
      %query = "SELECT * FROM shapeMount WHERE parent_shape_id=" @ $mmSelectedShape.sceneShapeID @ ";";
      %resultSet2 = sqlite.query(%query, 0); 
      if (sqlite.numRows(%resultSet2)>0)
      {         
         //$mmShapeMountList.add("",0);   
         while (!sqlite.endOfResult(%resultSet2))
         { //Next, get a human friendly child shape name.
            %id = sqlite.getColumn(%resultSet2, "id");
            %child = sqlite.getColumn(%resultSet2, "child_shape_id");
            %name = %child @ " - " @ %id;
            if (%first_id == 0)
               %first_id = %id;
            $mmShapeMountList.add(%name,%id);
            sqlite.nextRow(%resultSet2);         
         }       
         sqlite.clearResult(%resultSet2);
      }
      if (%first_id>0)
         $mmShapeMountList.setSelected(%first_id);
   }
}

function mmAddSceneShape()
{
   makeSqlGuiForm($mmAddSceneShapeWindowID);   
   mmSetupAddSceneShapeWindow();
}

function mmDeleteSceneShape()
{   
   if ($mmSceneShapeList.getSelected()<=0)
      return;

   //Don't bother with warning message for every scene shape.
   mmReallyDeleteSceneShape($mmSceneShapeList.getSelected());
   
   exposeMegaMotionScenesForm();   
   
   if ($mmLoadedScenes>0)
   {
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
}

function mmReallyDeleteSceneShape(%id)
{//Now, this can get called from the delete button, OR from inside a loop in another function.
   //EXCEPT: we're going to stop calling it from loops, because it is TOO SLOW. For deleting 
   //whole scenes, we need to collect the ids and use "WHERE id IN (...)"
   //%query = "SELECT pos_id,rot_id,scale_id FROM sceneShape WHERE id=" @ %id @ ";";
   //%resultSet = sqlite.query(%query,0);
   //if (%resultSet==0)
   //   return;

   //%pos_id = sqlite.getColumn(%resultSet,"pos_id");
   //%rot_id = sqlite.getColumn(%resultSet,"rot_id");
   //%scale_id = sqlite.getColumn(%resultSet,"scale_id");
   //sqlite.clearResult(%resultSet);
   
   //%query = "DELETE FROM vector3 WHERE id=" @ %pos_id @ " OR id=" @ %scale_id @ ";";
   //sqlite.query(%query,0);
   //%query = "DELETE FROM rotation WHERE id=" @ %rot_id @ ";";
   //sqlite.query(%query,0);
   
   %query = "DELETE FROM sceneShape WHERE id=" @ %id @ ";";
   sqlite.query(%query,0);
}

//Assign this behavior tree to all selected sceneShapes.
function mmSelectBehaviorTree()
{
   if (EWorldEditor.getSelectionSize()==0)
      return;
 
   %tree = $mmSceneShapeBehaviorTreeList.getText();
   %id_list = "";
   for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
   {
      %obj = EWorldEditor.getSelectedObject( %i );
      if ((%obj.getClassName() $= "PhysicsShape")&&(%obj.sceneShapeID>0)&&
            (%obj.behaviorTree !$= %tree))
      {
         if (strlen(%id_list)==0)
            %id_list =  %obj.sceneShapeID;
         else
            %id_list = %id_list @ "," @ %obj.sceneShapeID;
      }
   }
   %query = "UPDATE sceneShape SET behavior_tree='" @ %tree @ "'" @ 
                  " WHERE id IN (" @ %id_list @ ");";
   sqlite.query(%query,0);
}

//Assign this shape group to all selected sceneShapes.
function mmSelectShapeGroup()
{
   echo("calling mmSelectShapeGroup! select size " @ EWorldEditor.getSelectionSize() @ "!!!!!!!!!!!!!!!!!!!!!");
   
   if (EWorldEditor.getSelectionSize()==0)
      return;
   %group_id = $mmShapeGroupList.getSelected();
   %id_list = "";
   for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
   {
      %obj = EWorldEditor.getSelectedObject( %i );
      if ((%obj.getClassName() $= "PhysicsShape")&&(%obj.sceneShapeID>0))
      {
         if (strlen(%id_list)==0)
            %id_list =  %obj.sceneShapeID;
         else
            %id_list = %id_list @ "," @ %obj.sceneShapeID;
      }
   }
   %query = "UPDATE sceneShape SET shapeGroup_id='" @ %group_id @ "'" @ 
                  " WHERE id IN (" @ %id_list @ ");";  
   echo("QUERY: " @ %query);
   sqlite.query(%query,0);
}

//Assign this shape group to all selected sceneShapes.
function mmSelectTargetShape()
{
   echo("calling mmSelectShapeGroup! select size " @ EWorldEditor.getSelectionSize() @ "!!!!!!!!!!!!!!!!!!!!!");
   
   if (EWorldEditor.getSelectionSize()==0)
      return;
      
   %target_shape_id = $mmTargetShapeList.getSelected();
   %id_list = "";
   for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
   {
      %obj = EWorldEditor.getSelectedObject( %i );
      if ((%obj.getClassName() $= "PhysicsShape")&&(%obj.sceneShapeID>0))
      {
         if (strlen(%id_list)==0)
            %id_list =  %obj.sceneShapeID;
         else
            %id_list = %id_list @ "," @ %obj.sceneShapeID;
      }
   }
   %query = "UPDATE sceneShape SET target_shape_id='" @ %target_shape_id @ "'" @ 
                  " WHERE id IN (" @ %id_list @ ");";  
   echo("QUERY: " @ %query);
   sqlite.query(%query,0);
}

function mmAddPhysicsShape()
{   
   makeSqlGuiForm($mmAddShapeWindowID); 
}

function mmBrowsePhysicsShape()
{
   if (strlen($Pref::MegaMotion::DtsDir))
      %openFileName = mmGetOpenFilename($Pref::MegaMotion::DtsDir,"dts");
   else
      %openFileName = mmGetOpenFilename("art/shapes","dts");
   
   if (strlen(%openFileName))
   {     
      %openFileName = strreplace(%openFileName,"'","''");//Escape single quotes in the name.
      
      %executablePath = getExecutablePath();
      strreplace(%executablePath,"\\","/");//Replace backslashes with forward slashes.
      
      %executableName = getExecutableName();
      %namePos = strstr(%executablePath,%executableName);
      %execPath = getSubStr(%executablePath,0,%namePos);
      %localFilePath = getSubStr(%openFileName,%namePos,strlen(%openFileName));
      
      if (strlen(%localFilePath)>0)
      {         
         %query = "SELECT id,name FROM physicsShape WHERE path='" @ %localFilePath @ "';";
         %resultSet = sqlite.query(%query, 0); 
         if (sqlite.numRows(%resultSet)>0)
         {
            %shapeID = sqlite.getColumn(%resultSet,"id");
            %shapeName = sqlite.getColumn(%resultSet,"name");
            %msg = "This model is already in the database as '" @ %shapeName @ "', would you like add " @ 
                  "it again?"; 
            
            MessageBoxOKCancel( "Shape Exists",%msg,"","addPhysicsShapeWindow.visible=false;");
         } 
         %fileText = addPhysicsShapeWindow.findObjectByInternalName("fileText");
         %fileText.setText(%localFilePath);
         //%query = "INSERT INTO physicsShape (path) VALUES ('" @ %localFilePath @ "');";
         //sqlite.query(%query,0);
      }
      //echo("trying to open local path: " @ %localFilePath @ ", global path " @ %execPath);
   } 
}

function mmReallyAddPhysicsShape()
{
   %shapeName = addPhysicsShapeWindow.findObjectByInternalName("nameField").getText();
   %shapePath = addPhysicsShapeWindow.findObjectByInternalName("fileText").getText();
   
   //HERE: we need to open up art/datablocks/physicsShape.cs and add another datablock to the end!
   %datablockFile = "art/datablocks/physicsShape.cs";//Maybe put this in prefs?
   
   %file = new FileObject();
   %file.openForAppend(%datablockFile);
   
   %query = "INSERT INTO physicsShape (name,datablock,path) VALUES ('" @ %shapeName @ "','" @ 
            %shapeName @ "Physics','" @  %shapePath @ "');";
   sqlite.query(%query,0);
   
   %query = "SELECT last_insert_rowid() AS id;";
   %resultSet = sqlite.query(%query,0);
   %id = sqlite.getColumn(%resultSet,"id");
   sqlite.clearResult(%resultSet);
   
   //sqlite.getLastRowId();//Maybe?
   
   
   %file.writeLine("");
   %file.writeLine("datablock PhysicsShapeData( " @ %shapeName @ "Physics )");
   %file.writeLine("{");   
   %file.writeLine("   category = \"PhysicsShape\";"); 
   %file.writeLine("   shapeName = \"" @ %shapePath @ "\";"); 
   %file.writeLine("   mass = 1.0;");
   %file.writeLine("   shapeID = " @ %id @ ";");
   %file.writeLine("};");
   
   %file.close();
   

   //AH, but now we're going to have to re-exec that file, or else restart the engine before that 
   //datablock will exist. Maybe it can't even be reloaded after startup time. Hm.
   //Maybe we can write all of the above to one string and include it in a single call to writeLine,
   //and then also exec it on the fly. So it will work this time, and then every other time be loaded
   //normally at startup.
      
   
   addPhysicsShapeWindow.visible = false;
}

/*
//Still not sure how many things I can cut out, but this minimal datablock is working so far...
datablock PhysicsShapeData( ka50tubeRocketPhysics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/ka50/Models/weapons/tuberocket.dts";
   isArticulated = false;
   mass = "0";   
   integration = 4;
   shapeID = 8;    
};
*/

function mmDeletePhysicsShape()
{   //First, delete all the shapeParts, then the shape.
   if ($mmShapeList.getSelected()<=0)
      return;
      
   MessageBoxOKCancel( "Warning", 
      "This will permanently delete this shape and all shapeParts referencing it. Are you completely sure?", 
      "mmReallyDeleteShape();",
      "" );     
}

function mmReallyDeleteShape()
{
   if ($mmShapeList.getSelected()<=0)
      return;
      
   %shape_id = $mmShapeList.getSelected();
   
   %query = "DELETE FROM physicsShapePart WHERE physicsShape_id=" @ %shape_id @ ";";
   sqlite.query(%query);
   
   %query = "DELETE FROM shapeMount WHERE parent_shape_id=" @ %shape_id @ " OR child_shape_id=" @
                %shape_id @ ";";
   sqlite.query(%query);
   
   %query = "DELETE FROM physicsShape WHERE id=" @ %shape_id @ ";";
   sqlite.query(%query);
   
   exposeMegaMotionScenesForm();   
}

///////////////////////////////////////////////////////////////////////////////

function mmSelectShape()
{
   echo("calling selectMegaMotionShape");
   
   if ($mmShapeList.getSelected()<=0)
      return;
      
   $mmShapePartList.clear();   
   %firstID = 0;
   %query = "SELECT id,name FROM physicsShapePart WHERE physicsShape_id=" @ $mmShapeList.getSelected() @ ";";  
   echo("\n" @ %query @ "\n");   
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            $mmShapePartList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0)
          //  $mmShapePartList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }

   $mmShapePartBaseNodeList.clear();
   $mmShapePartChildNodeList.clear();
   $mmSequenceAllNodeList.clear();
   $mmShapeMountParentNodeList.clear();
   $mmBvhModelNodeList.clear();
   for (%i=0;%i<$mmSelectedShape.getNumNodes();%i++)
   {
      %node_name = $mmSelectedShape.getNodeName(%i);
      $mmShapePartBaseNodeList.add(%node_name,%i);
      $mmShapePartChildNodeList.add(%node_name,%i);
      $mmSequenceAllNodeList.add(%node_name,%i);
      $mmShapeMountParentNodeList.add(%node_name,%i);
      $mmBvhModelNodeList.add(%node_name,%i);
   }
   
   $mmShapeMountChildShapeList.setSelected(0);
   $mmShapeMountChildNodeList.setSelected(0);
   
   //Whoops, gotta be much more careful about not doing this on selecting scene shape, only on changing shape.
   //Finally, see if we want to change the shape of the currently selected sceneShape.
   %sceneShapeId = $mmSceneShapeList.getSelected();
   %shape_id = $mmShapeList.getSelected();
   if ((%sceneShapeId>0)&&(%shape_id!=$mmShapeId))
   {
      MessageBoxYesNo("","Really assign sceneShape " @ %sceneShapeId @ " to shape " @ 
         $mmShapeList.getText() @ "?","mmReassignShape();","");
   }
   
   
}

//Not currently hooked in, can't associate it with selectShape() above until we remove all the times
//we select the shapelist automatically, or really any time we select the shape we're already using.
function mmReassignShape()
{
   echo("REASSIGNING SHAPE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
   if (($mmSceneShapeList.getSelected()<=0)||($mmShapeList.getSelected()<=0))
      return;
      
   %sceneShapeId = $mmSceneShapeList.getSelected();
   %shape_id = $mmShapeList.getSelected();
   %query = "UPDATE sceneShape SET shape_id=" @ %shape_id @ " WHERE id=" @ %sceneShapeId @ ";";
   echo("reassigning shape: " @ %query );
   sqlite.query(%query,0);
   
   if ($mmLoadedScenes>0)
   {
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
   
   $mmSceneShapeList.setSelected(%sceneShapeId);
}

function mmSelectShapePart()
{
   if ($mmShapePartList.getSelected()<=0)
   { //We need clear functions for all tabs and panels / select lists, ie every time you 
      //mmClearShapePartTab();//select null on a list it should clear all related controls. 
      return;
   }
   %partId = $mmShapePartList.getSelected();
   if (%partId<=0)
      return;
	
   %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = %tab.findObjectByInternalName("shapePartPanel");
	
   %baseNodeList = %panel.findObjectByInternalName("baseNodeList");
   %childNodeList = %panel.findObjectByInternalName("childNodeList");   
   
   //Actually, let's not make these ones globals... going down to selectShapePart
   %dimensionsX = %panel.findObjectByInternalName("shapePartDimensionsX");
   %dimensionsY = %panel.findObjectByInternalName("shapePartDimensionsY");
   %dimensionsZ = %panel.findObjectByInternalName("shapePartDimensionsZ");         
   %orientationX = %panel.findObjectByInternalName("shapePartOrientationX");//eulers
   %orientationY = %panel.findObjectByInternalName("shapePartOrientationY");
   %orientationZ = %panel.findObjectByInternalName("shapePartOrientationZ");         
   %offsetX = %panel.findObjectByInternalName("shapePartOffsetX");
   %offsetY = %panel.findObjectByInternalName("shapePartOffsetY");
   %offsetZ = %panel.findObjectByInternalName("shapePartOffsetZ");   
   %jointRot1X = %panel.findObjectByInternalName("shapePartJointRot1X");
   %jointRot1Y = %panel.findObjectByInternalName("shapePartJointRot1Y");
   %jointRot1Z = %panel.findObjectByInternalName("shapePartJointRot1Z");  
   %jointRot2X = %panel.findObjectByInternalName("shapePartJointRot2X");
   %jointRot2Y = %panel.findObjectByInternalName("shapePartJointRot2Y");
   %jointRot2Z = %panel.findObjectByInternalName("shapePartJointRot2Z");
	      
	%query = "SELECT * FROM physicsShapePart " @ 
	         "WHERE id=" @ %partId @ ";"; 
	%resultSet = sqlite.query(%query, 0);
	if (%resultSet)
	{
	   if (sqlite.numRows(%resultSet)==1)
	   {	               
         %dimensionsX.setText(sqlite.getColumn(%resultSet, "dimensions_x"));
         %dimensionsY.setText(sqlite.getColumn(%resultSet, "dimensions_y"));
         %dimensionsZ.setText(sqlite.getColumn(%resultSet, "dimensions_z"));         
         %orientationX.setText(sqlite.getColumn(%resultSet, "orientation_x"));
         %orientationY.setText(sqlite.getColumn(%resultSet, "orientation_y"));
         %orientationZ.setText(sqlite.getColumn(%resultSet, "orientation_z"));         
         %offsetX.setText(sqlite.getColumn(%resultSet, "offset_x"));
         %offsetY.setText(sqlite.getColumn(%resultSet, "offset_y"));
         %offsetZ.setText(sqlite.getColumn(%resultSet, "offset_z"));
         %jointRot1X.setText(sqlite.getColumn(%resultSet, "joint_x"));
         %jointRot1Y.setText(sqlite.getColumn(%resultSet, "joint_y"));
         %jointRot1Z.setText(sqlite.getColumn(%resultSet, "joint_z"));
         %jointRot2X.setText(sqlite.getColumn(%resultSet, "joint_x_2"));
         %jointRot2Y.setText(sqlite.getColumn(%resultSet, "joint_y_2"));
         %jointRot2Z.setText(sqlite.getColumn(%resultSet, "joint_z_2"));
         %jointId = sqlite.getColumn(%resultSet, "px3Joint_id");
         if (%jointId > 0)
            $mmJointList.setSelected(%jointId);
         
         $mmShapePartTypeList.setSelected(sqlite.getColumn(%resultSet, "shapeType"));
	   } else {
	      echo("shape part num rows: " @ sqlite.numRows(%resultSet));
	   }
	} else { 
	   echo("shape part query failed!  \n " @ %query);
	} 
}

function mmUpdateShapePart()
{
   if ($mmShapePartList.getSelected()<=0)
      return;
      
   %part_id = $mmShapePartList.getSelected();
   
   %query = "";
   
   %query = %query @ "UPDATE physicsShapePart SET "; 
   %query = %query @ "shapeType=" @ $mmShapePartTypeList.getSelected();
   
   if (strlen($mmShapePartDimensionsX.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_x=" @ $mmShapePartDimensionsX.getText();
   if (strlen($mmShapePartDimensionsY.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_y=" @ $mmShapePartDimensionsY.getText();
   if (strlen($mmShapePartDimensionsZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_z=" @ $mmShapePartDimensionsZ.getText();
   if (strlen($mmShapePartOrientationX.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_x=" @ $mmShapePartOrientationX.getText();
   if (strlen($mmShapePartOrientationY.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_y=" @ $mmShapePartOrientationY.getText();
   if (strlen($mmShapePartOrientationZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_z=" @ $mmShapePartOrientationZ.getText();
   if (strlen($mmShapePartOffsetX.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_x=" @ $mmShapePartOffsetX.getText();
   if (strlen($mmShapePartOffsetY.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_y=" @ $mmShapePartOffsetY.getText();
   if (strlen($mmShapePartOffsetZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_z=" @ $mmShapePartOffsetZ.getText();
      
   %query = %query @ " WHERE id=" @ %part_id @ ";"; 
   echo("\n" @ %query @ "\n"); 
	sqlite.query(%query, 0);
}


function mmSelectJoint()
{
   %jointId = $mmJointList.getSelected();
   if (%jointId<=0)
      return;
      
   %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = %tab.findObjectByInternalName("shapePartPanel");

   %jointCount = $mmJointList.size();   
   echo("selecting joint " @ %jointId @ "!  numJoints " @ %jointCount ); 
   
   %twistLimit = %panel.findObjectByInternalName("jointTwistLimit");
   %swingLimit = %panel.findObjectByInternalName("jointSwingLimit");
   %swingLimit2 = %panel.findObjectByInternalName("jointSwingLimit2");
   %xLimit = %panel.findObjectByInternalName("jointXLimit");
   %yLimit = %panel.findObjectByInternalName("jointYLimit");
   %zLimit = %panel.findObjectByInternalName("jointZLimit");
   %axisX = %panel.findObjectByInternalName("jointAxisX");
   %axisY = %panel.findObjectByInternalName("jointAxisY");
   %axisZ = %panel.findObjectByInternalName("jointAxisZ");
   %normalX = %panel.findObjectByInternalName("jointNormalX");
   %normalY = %panel.findObjectByInternalName("jointNormalY");
   %normalZ = %panel.findObjectByInternalName("jointNormalZ");
   %twistSpring = %panel.findObjectByInternalName("jointTwistSpring");
   %swingSpring = %panel.findObjectByInternalName("jointSwingSpring");
   %springDamper = %panel.findObjectByInternalName("jointSpringDamper");
   %motorSpring = %panel.findObjectByInternalName("jointMotorSpring");
   %motorDamper = %panel.findObjectByInternalName("jointMotorDamper");
   %maxForce = %panel.findObjectByInternalName("jointMaxForce");
   %maxTorque = %panel.findObjectByInternalName("jointMaxTorque");
   
   %query = "SELECT * FROM px3Joint WHERE id=" @ %jointId @ ";";
   %resultSet = sqlite.query(%query,0);
   if (%resultSet)
	{
	   if (sqlite.numRows(%resultSet)==1)
	   {	               
         %twistLimit.setText(sqlite.getColumn(%resultSet, "twistLimit"));
         %swingLimit.setText(sqlite.getColumn(%resultSet, "swingLimit"));
         %swingLimit2.setText(sqlite.getColumn(%resultSet, "swingLimit2"));
         %xLimit.setText(sqlite.getColumn(%resultSet, "xLimit"));
         %yLimit.setText(sqlite.getColumn(%resultSet, "yLimit"));
         %zLimit.setText(sqlite.getColumn(%resultSet, "zLimit"));
         %axisX.setText(sqlite.getColumn(%resultSet, "localAxis_x"));
         %axisY.setText(sqlite.getColumn(%resultSet, "localAxis_y"));
         %axisZ.setText(sqlite.getColumn(%resultSet, "localAxis_z"));
         %normalX.setText(sqlite.getColumn(%resultSet, "localNormal_x"));
         %normalY.setText(sqlite.getColumn(%resultSet, "localNormal_y"));
         %normalZ.setText(sqlite.getColumn(%resultSet, "localNormal_z"));
         %twistSpring.setText(sqlite.getColumn(%resultSet, "twistSpring"));
         %swingSpring.setText(sqlite.getColumn(%resultSet, "swingSpring"));
         %springDamper.setText(sqlite.getColumn(%resultSet, "springDamper"));
         %motorSpring.setText(sqlite.getColumn(%resultSet, "motorSpring"));
         %motorDamper.setText(sqlite.getColumn(%resultSet, "motorDamper"));
         %maxForce.setText(sqlite.getColumn(%resultSet, "maxForce"));
         %maxTorque.setText(sqlite.getColumn(%resultSet, "maxTorque"));
         $mmJointTypeList.setSelected(sqlite.getColumn(%resultSet, "jointType"));
	   }
	}
}


//////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////

function mmSetupAddSceneShapeWindow()
{
   
   %shapeList = mmAddSceneShapeWindow.findObjectByInternalName("shapeList"); 
   %groupList = mmAddSceneShapeWindow.findObjectByInternalName("groupList"); 
   %behaviorTree = mmAddSceneShapeWindow.findObjectByInternalName("behaviorTree"); 
   if ((!isDefined(%shapeList))||(!isDefined(%groupList)))
      return;
   
   //%shapeList
   %query = "SELECT id,name FROM physicsShape ORDER BY name;";
   %resultSet = sqlite.query(%query, 0);
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            %shapeList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }
   if ($mmShapeList.getSelected()>0)
      %shapeList.setSelected($mmShapeList.getSelected());
   
   //groupList
   %query = "SELECT id,name FROM shapeGroup ORDER BY name;";
   %resultSet = sqlite.query(%query, 0);
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            %groupList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }    
   if ($mmShapeGroupList.getSelected()>0)
      %groupList.setSelected($mmShapeGroupList.getSelected());
      
   if (strlen($mmSceneShapeBehaviorTreeList.getText())>0)
      %behaviorTree.setText($mmSceneShapeBehaviorTreeList.getText());
      
   %posX = mmAddSceneShapeWindow.findObjectByInternalName("shapePositionX"); 
   %posY = mmAddSceneShapeWindow.findObjectByInternalName("shapePositionY"); 
   %posZ = mmAddSceneShapeWindow.findObjectByInternalName("shapePositionZ"); 
   
   %oriX = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationX"); 
   %oriY = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationY"); 
   %oriZ = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationZ"); 
   %oriAngle = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationAngle"); 
   
   %scaleX = mmAddSceneShapeWindow.findObjectByInternalName("shapeScaleX"); 
   %scaleY = mmAddSceneShapeWindow.findObjectByInternalName("shapeScaleY"); 
   %scaleZ = mmAddSceneShapeWindow.findObjectByInternalName("shapeScaleZ"); 
   
   %posX.setText(0);
   %posY.setText(0);
   %posZ.setText(0);
   
   %oriX.setText(0);
   %oriY.setText(0);
   %oriZ.setText(1);
   %oriAngle.setText(0);
   
   %scaleX.setText(1);
   %scaleY.setText(1);
   %scaleZ.setText(1);
     
   %blockX = mmAddSceneShapeWindow.findObjectByInternalName("blockCountX"); 
   %blockY = mmAddSceneShapeWindow.findObjectByInternalName("blockCountY"); 
   %blockPaddingX = mmAddSceneShapeWindow.findObjectByInternalName("blockPaddingX"); 
   %blockPaddingY = mmAddSceneShapeWindow.findObjectByInternalName("blockPaddingY"); 
   %blockVariationX = mmAddSceneShapeWindow.findObjectByInternalName("blockVariationX"); 
   %blockVariationY = mmAddSceneShapeWindow.findObjectByInternalName("blockVariationY"); 
   %blockRotX = mmAddSceneShapeWindow.findObjectByInternalName("blockRotationX"); 
   %blockRotY = mmAddSceneShapeWindow.findObjectByInternalName("blockRotationY"); 
   %blockRotZ = mmAddSceneShapeWindow.findObjectByInternalName("blockRotationZ"); 
   %blockRotAngle = mmAddSceneShapeWindow.findObjectByInternalName("blockRotationAngle"); 
   
   %blockX.setText(2);
   %blockY.setText(2);
   %blockPaddingX.setText(2);
   %blockPaddingY.setText(2);
   %blockVariationX.setText(0);
   %blockVariationY.setText(0);
   %blockRotX.setText(0);
   %blockRotY.setText(0);
   %blockRotZ.setText(1);
   %blockRotAngle.setText(0);
   
}

function mmReallyAddSceneShape() 
{
   %name = mmAddSceneShapeWindow.findObjectByInternalName("nameEdit").getText(); 
   //if (substr(%name," ")>0)
   //{
   //   MessageBoxOK("Name Invalid","Scene name must be a unique character string with no spaces or special characters.","");
   //   return;  
   //}
   %scene_id = $mmSceneList.getSelected();
   %shape_id = mmAddSceneShapeWindow.findObjectByInternalName("shapeList").getSelected();
   %group_id = mmAddSceneShapeWindow.findObjectByInternalName("groupList").getSelected();
   %behavior_tree = mmAddSceneShapeWindow.findObjectByInternalName("behaviorTree").getText();
   
   %pos_x = mmAddSceneShapeWindow.findObjectByInternalName("shapePositionX").getText();
   %pos_y = mmAddSceneShapeWindow.findObjectByInternalName("shapePositionY").getText();
   %pos_z = mmAddSceneShapeWindow.findObjectByInternalName("shapePositionZ").getText(); 
   
   %rot_x = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationX").getText();
   %rot_y = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationY").getText();
   %rot_z = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationZ").getText(); 
   %rot_a = mmAddSceneShapeWindow.findObjectByInternalName("shapeOrientationAngle").getText();
   
   %scale_x = mmAddSceneShapeWindow.findObjectByInternalName("shapeScaleX").getText();
   %scale_y = mmAddSceneShapeWindow.findObjectByInternalName("shapeScaleY").getText();
   %scale_z = mmAddSceneShapeWindow.findObjectByInternalName("shapeScaleZ").getText(); 
   
   if (strlen(%name)>0)
   {
      %query = "SELECT id FROM sceneShape WHERE name='" @ %name @ "' AND scene_id=" @ %scene_id @ ";";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)>0)
      {
         MessageBoxOK("Name Invalid","Scene shape name must be unique for this scene.","");
         return;
      }
   }
   
   //
   //HERE: do some sanity testing before we commit!
   //
   
   //And, insert pos, rot & scale, and get the ids back. Q: what is the most efficient way to do this?
   //For now, I'm inserting the other stuff, grabbing an id, and then inserting the pos/rot/scale and
   //using last_insert_rowid() in update statements.
   %query = "INSERT INTO sceneShape (name,scene_id,shape_id,shapeGroup_id,behavior_tree) " @
            " VALUES ('" @ %name @ "'," @ %scene_id @ "," @ %shape_id @ "," @ %group_id @
             ",'" @ %behavior_tree @ "');";
   sqlite.query(%query,0);


   %query = "SELECT last_insert_rowid() AS id;";
   %resultSet = sqlite.query(%query,0);
   %ss_id = sqlite.getColumn(%resultSet,"id");
   sqlite.clearResult(%resultSet);
   if (%ss_id==0)
      return;
      
   %query = "UPDATE sceneShape SET pos_x=" @ %pos_x @ ",pos_y="  @ %pos_y @ ",pos_z="  @ %pos_z @
            ",rot_x="  @ %rot_x @ ",rot_y="  @ %rot_y @ ",rot_z="  @ %rot_z @ ",rot_a="  @ %rot_a @ 
            ",scale_x="  @ %scale_x @ ",scale_y="  @ %scale_y @ ",scale_z="  @ %scale_z @
            " WHERE id=" @ %ss_id @ ";";
            
            
            
/*
   //For first pass at least, just add default values and spawn the character at scene origin.
   %query = "INSERT INTO vector3 (x,y,z) VALUES (" @ %pos_x @ "," @ %pos_y @ "," @ %pos_z @ ");";
   sqlite.query(%query,0);
   %query = "UPDATE sceneShape SET pos_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
   sqlite.query(%query, 0);  
         
   %query = "INSERT INTO rotation (x,y,z,angle) VALUES (" @ %ori_x @ "," @ %ori_y @ "," @ 
                  %ori_z @  "," @ %ori_a @ ");";      
   sqlite.query(%query,0);
   %query = "UPDATE sceneShape SET rot_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
   sqlite.query(%query, 0);  
   
   %query = "INSERT INTO vector3 (x,y,z) VALUES (" @ %scale_x @ "," @ %scale_y @ "," @ %scale_z @ ");";      
   sqlite.query(%query,0);
   %query = "UPDATE sceneShape SET scale_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
   sqlite.query(%query, 0);  
  */
   
   mmAddSceneShapeWindow.delete();
      
   exposeMegaMotionScenesForm();
   
   if ($mmLoadedScenes>0)
   {
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
}



//////////////////////////////////////////////////////////////////////


function mmAddShapeGroup() 
{
   makeSqlGuiForm($mmAddShapeGroupWindowID);
}

function mmReallyAddShapeGroup() 
{
   if (mmAddShapeGroupWindow.isVisible())
   {
      %name = mmAddShapeGroupWindow.findObjectByInternalName("nameEdit").getText(); 
      if (strlen(%name)==0)//TEST FOR UNIQUE
      {
         MessageBoxOK("Name Invalid","Group name must exist!","");
         return;  
      }
      %query = "SELECT id FROM shapeGroup WHERE name='" @ %name @ "';";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)>0)
      {
         MessageBoxOK("Name Invalid","Group name must be unique.","");
         return;
      }
      %query = "INSERT INTO shapeGroup (name) VALUES ('" @ %name @ "');";
      sqlite.query(%query,0);
      
      mmAddShapeGroupWindow.delete();
      
      exposeMegaMotionScenesForm();
   }
}

function mmDeleteShapeGroup()
{
   //nothing here yet  
}

function mmAddSceneShapeBlock() 
{   
   %scene_id = $mmSceneList.getSelected();
   %shape_id = mmAddSceneShapeWindow.findObjectByInternalName("shapeList").getSelected();

   %groupList = mmAddSceneShapeWindow.findObjectByInternalName("groupList");
   %group_id = %groupList.getSelected();
   %group_name = %groupList.getText();
   
   if ((%scene_id<=0)||(%shape_id<=0)||(%group_id<=0))
   {
      echo("Please select a scene, shape, and group before creating a block.");
      return;
   }
   
   echo("calling addSceneShapeBlock" );
   %lastClock = getClock();
   
   //AND... new way! In engine, SQL queries go much faster.
   addSceneShapeBlock(%scene_id,%shape_id,%group_id);

   echo("finished adding characters, clock  " @ getClock() @ " elapsed " @ getClock()-%lastClock);
   %lastClock = getClock();
   
   if ($mmLoadedScenes>0)
   {
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
  
   mmAddSceneShapeWindow.delete();
      
   exposeMegaMotionScenesForm();
}

//////////////////////////////////////////////////////////////

function mmSetupAddShapePartWindow()
{   
   addShapePartWindow.findObjectByInternalName("dimensionsX").setText("0.1");
   addShapePartWindow.findObjectByInternalName("dimensionsY").setText("0.1");
   addShapePartWindow.findObjectByInternalName("dimensionsZ").setText("0.1");
   
   addShapePartWindow.findObjectByInternalName("rotationX").setText("0.0");
   addShapePartWindow.findObjectByInternalName("rotationY").setText("0.0");
   addShapePartWindow.findObjectByInternalName("rotationZ").setText("0.0");
   
   addShapePartWindow.findObjectByInternalName("offsetX").setText("0.0");
   addShapePartWindow.findObjectByInternalName("offsetY").setText("0.0");
   addShapePartWindow.findObjectByInternalName("offsetZ").setText("0.0");
   
   %typeList = addShapePartWindow.findObjectByInternalName("typeList");
   %baseNodeList = addShapePartWindow.findObjectByInternalName("baseNodeList");
   %jointList = addShapePartWindow.findObjectByInternalName("jointList");
   
   %typeList.clear();
   %typeList.add("Box","0");
   %typeList.add("Capsule","1");
   %typeList.add("Sphere","2");
   //%typeList.add("Convex","3");
   //%typeList.add("Collision","4");
   //%typeList.add("Trimesh","5");
   %typeList.setSelected(0);
   
   %baseNodeList.clear();
   %query = "SELECT node_index,name FROM shapeNode WHERE physicsShape_id=" @ $mmShapeList.getSelected() @ ";";
   %resultSet = sqlite.query(%query,0);
   while (!sqlite.endOfResult(%resultSet))
   {
      %id = sqlite.getColumn(%resultSet, "node_index");
      %name = sqlite.getColumn(%resultSet, "name");
            
      %baseNodeList.add(%name,%id);
      sqlite.nextRow(%resultSet); 
   }
   sqlite.clearResult(%resultSet);
      
   %jointList.clear();
   %query = "SELECT id,name FROM px3Joint;";
   %resultSet = sqlite.query(%query,0);
   while (!sqlite.endOfResult(%resultSet))
   {
      %id = sqlite.getColumn(%resultSet, "id");
      %name = sqlite.getColumn(%resultSet, "name");
            
      %jointList.add(%name,%id);
      sqlite.nextRow(%resultSet); 
   }
   sqlite.clearResult(%resultSet);
   
}

function mmAddShapePart()
{
   makeSqlGuiForm($mmAddShapePartWindowID);
   
   mmSetupAddShapePartWindow();
}

function mmReallyAddShapePart()
{
   //And, NOW, really send the insert query!
   %name = addShapePartWindow.findObjectByInternalName("nameEdit").getText();
   
   if (strlen(%name)<=0)//FIX: check for unique, and give this a visible window not a console echo.
   {
      echo("Shape part name must exist and be unique per shape.");      
      return;
   }
   
   %dimensionsX = addShapePartWindow.findObjectByInternalName("dimensionsX").getText();
   %dimensionsY = addShapePartWindow.findObjectByInternalName("dimensionsY").getText();
   %dimensionsZ = addShapePartWindow.findObjectByInternalName("dimensionsZ").getText();
   
   %rotationX = addShapePartWindow.findObjectByInternalName("rotationX").getText();
   %rotationY = addShapePartWindow.findObjectByInternalName("rotationY").getText();
   %rotationZ = addShapePartWindow.findObjectByInternalName("rotationZ").getText();
   
   %offsetX = addShapePartWindow.findObjectByInternalName("offsetX").getText();
   %offsetY = addShapePartWindow.findObjectByInternalName("offsetY").getText();
   %offsetZ = addShapePartWindow.findObjectByInternalName("offsetZ").getText();
   
   %type = addShapePartWindow.findObjectByInternalName("typeList").getSelected();
   %baseNode = addShapePartWindow.findObjectByInternalName("baseNodeList").getText();
   %joint = addShapePartWindow.findObjectByInternalName("jointList").getSelected();
   
   %shape = $mmShapeList.getSelected();
   
   //FIX: sanity check the data! And, although it is a pain in the butt, I really should modify 
   //  physicsShapePart table to include offset_id etc., and use vector3 table for consistency.
   %query = "INSERT INTO physicsShapePart (physicsShape_id,px3Joint_id,name,baseNode,shapeType," @
            "dimensions_x,dimensions_y,dimensions_z,orientation_x,orientation_y,orientation_z," @
            "offset_x,offset_y,offset_z) VALUES (" @ 
            %shape @ "," @ %joint @ ",'" @ %name @ "','" @ %baseNode @ "'," @ %type @ "," @ 
            %dimensionsX @ "," @ %dimensionsY @ "," @ %dimensionsZ @ "," @
            %rotationX @ "," @ %rotationY @ "," @ %rotationZ @ "," @
            %offsetX @ "," @ %offsetY @ "," @ %offsetZ @ ");";
   sqlite.query(%query,0);
   
   $mmShapeList.setSelected(%shape);
   
}

//FIX: add a sanity check "are you sure" window before this.
function mmReallyDeleteShapePart()
{      
   %panel = $mmSceneShapeTab.findObjectByInternalName("shapePartPanel");
   %shapePart = %panel.findObjectByInternalName("shapePartList").getSelected();
   
   if (%shapePart>0)
   {
      %query = "DELETE FROM physicsShapePart WHERE id=" @ %shapePart @ ";";
      sqlite.query(%query,0);
   }
}


//////////////////////////////////////////////////////////////


function mmAddOpenSteer()
{
   makeSqlGuiForm($mmAddOpenSteerWindowID);
}

function mmReallyAddOpenSteer()
{
   if (mmAddOpenSteerWindow.isVisible())
   {
      %name = mmAddOpenSteerWindow.findObjectByInternalName("nameEdit").getText(); 
      
      %query = "INSERT INTO openSteerProfile (name) VALUES ('" @ %name @ "');";
      sqlite.query(%query,0);
      
      mmAddOpenSteerWindow.delete();
      
      exposeMegaMotionScenesForm();
   }
}

function mmDeleteOpenSteer()
{
   %openSteer_id = $mmOpenSteerList.getSelected();
   if (%openSteer_id<=0)
      return;
      
   MessageBoxOKCancel( "Warning", 
      "This will permanently delete this OpenSteer Profile! Are you completely sure?", 
      "mmReallyDeleteOpenSteer();",
      "" ); 
      
}
   
function mmReallyDeleteOpenSteer()
{
   %openSteer_id = $mmOpenSteerList.getSelected();
      
   %query = "DELETE FROM openSteerProfile WHERE id=" @ %openSteer_id @ ";";
   sqlite.query(%query,0);
   
   exposeMegaMotionScenesForm();
}


function mmSelectOpenSteer()
{   
   %openSteerID = $mmOpenSteerList.getSelected();
   if (%openSteerID<=0)
      return;
      
   %panel = $mmAiTab.findObjectByInternalName("aiPanel");
   
   %mass = %panel.findObjectByInternalName("sceneShapeOpenSteerMass");
   %radius = %panel.findObjectByInternalName("sceneShapeOpenSteerRadius");
   %maxForce = %panel.findObjectByInternalName("sceneShapeOpenSteerMaxForce");
   %maxSpeed = %panel.findObjectByInternalName("sceneShapeOpenSteerMaxSpeed");
   %wanderChance = %panel.findObjectByInternalName("sceneShapeOpenSteerWanderChance");
   %wanderWeight = %panel.findObjectByInternalName("sceneShapeOpenSteerWanderWeight");
   %seekTarget = %panel.findObjectByInternalName("sceneShapeOpenSteerSeekTarget");
   %avoidTarget = %panel.findObjectByInternalName("sceneShapeOpenSteerAvoidTarget");
   %seekNeighbor = %panel.findObjectByInternalName("sceneShapeOpenSteerSeekNeighbor");
   %avoidNeighbor = %panel.findObjectByInternalName("sceneShapeOpenSteerAvoidNeighbor");
   %avoidEdge = %panel.findObjectByInternalName("sceneShapeOpenSteerAvoidEdge");
   %detectEdge = %panel.findObjectByInternalName("sceneShapeOpenSteerDetectEdge");
   
   %query = "SELECT * FROM openSteerProfile WHERE id=" @ %openSteerID @ ";";
   %resultSet = sqlite.query(%query,0);
   
   if (%resultSet)
   {
      %mass.setText(sqlite.getColumn(%resultSet,"mass"));
      %radius.setText(sqlite.getColumn(%resultSet,"radius"));
      %maxForce.setText(sqlite.getColumn(%resultSet,"maxForce"));
      %maxSpeed.setText(sqlite.getColumn(%resultSet,"maxSpeed"));
      %wanderChance.setText(sqlite.getColumn(%resultSet,"wanderChance"));
      %wanderWeight.setText(sqlite.getColumn(%resultSet,"wanderWeight"));
      %seekTarget.setText(sqlite.getColumn(%resultSet,"seekTargetWeight"));
      %avoidTarget.setText(sqlite.getColumn(%resultSet,"avoidTargetWeight"));
      %seekNeighbor.setText(sqlite.getColumn(%resultSet,"seekNeighborWeight"));
      %avoidNeighbor.setText(sqlite.getColumn(%resultSet,"avoidNeighborWeight"));
      %avoidEdge.setText(sqlite.getColumn(%resultSet,"avoidNavMeshEdgeWeight"));
      %detectEdge.setText(sqlite.getColumn(%resultSet,"detectNavMeshEdgeRange")); 
   }
   
   //HERE, AS ELSEWHERE: insert an OK/CANCEL message before making changes!   
   if (EWorldEditor.getSelectionSize()==0)
      return;
 
   %id_list = "";
   for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
   {
      %obj = EWorldEditor.getSelectedObject( %i );
      if ((%obj.getClassName() $= "PhysicsShape")&&(%obj.sceneShapeID>0))
      {
         if (strlen(%id_list)==0)
            %id_list =  %obj.sceneShapeID;
         else
            %id_list = %id_list @ "," @ %obj.sceneShapeID;
      }
   }
   %query = "UPDATE sceneShape SET openSteerProfile_id='" @ %openSteerID @ "'" @ 
                  " WHERE id IN (" @ %id_list @ ");";
   sqlite.query(%query,0);
}

////////////////////////////////////////////////////////////////

function mmSelectShapeMount()
{
   echo("selecting shapeMount: " @ $mmShapeMountList.getSelected());
   %shapeMount = $mmShapeMountList.getSelected();   
   
   %panel = $mmSceneShapeTab.findObjectByInternalName("sceneShapePanel");
   
   %offsetX = %panel.findObjectByInternalName("shapeMountOffsetX");
   %offsetY = %panel.findObjectByInternalName("shapeMountOffsetY");
   %offsetZ = %panel.findObjectByInternalName("shapeMountOffsetZ");   
   %orientationX = %panel.findObjectByInternalName("shapeMountRotationX");
   %orientationY = %panel.findObjectByInternalName("shapeMountRotationY");
   %orientationZ = %panel.findObjectByInternalName("shapeMountRotationZ");   
   %scaleX = %panel.findObjectByInternalName("shapeMountScaleX");
   %scaleY = %panel.findObjectByInternalName("shapeMountScaleY");
   %scaleZ = %panel.findObjectByInternalName("shapeMountScaleZ");  
   
   if (%shapeMount<=0)
   {
      $mmShapeMountParentNodeList.setSelected(0);
      $mmShapeMountChildShapeList.setSelected(0);
      $mmShapeMountChildNodeList.setSelected(0);
      
      %offsetX.setText(""); %offsetY.setText(""); %offsetZ.setText("");
      %orientationX.setText(""); %orientationY.setText(""); %orientationZ.setText("");
      %scaleX.setText(""); %scaleY.setText(""); %scaleZ.setText("");
      
      return;
   }
   
   
   //(all of these are globals for now...)
   //%childShapeList =  %panel.findObjectByInternalName("shapeMountChildShapeList");
   //($mmShapeMountParentNodeList is already defined - do so many need to be global though?)
   //%childNodeList =  %panel.findObjectByInternalName("shapeMountChildNodeList");
   
   
   %query = "SELECT parent_shape_id,child_shape_id,parent_node,child_node," @ 
   "offset_x,offset_y,offset_z," @
   "orient_x,orient_y,orient_z," @
   "scale_x,scale_y,scale_z," @
   "joint_id FROM shapeMount sm " @
	
   "WHERE sm.id=" @ %shapeMount @ ";";
   %resultSet = sqlite.query(%query,0);
   
   if (%resultSet)
   {
      //%parentShape = sqlite.getColumn(%resultSet,"parent_shape_id");
      %childSceneShape = sqlite.getColumn(%resultSet,"child_shape_id");
      %parentNode = sqlite.getColumn(%resultSet,"parent_node");
      %childNode = sqlite.getColumn(%resultSet,"child_node");

      $mmShapeMountParentNodeList.setSelected(%parentNode);
      
      %query2 = "SELECT shape_id FROM sceneShape WHERE id=" @ %childSceneShape @ ";";
      %resultSet2 = sqlite.query(%query2,0);
      %childShape = sqlite.getColumn(%resultSet2,"shape_id");
      sqlite.clearResult(%resultSet2);

      echo("Parent node: " @ %parentNode @ " child shape: " @ %childShape @ " child node " @ %childNode );
      
      MegaMotionScenes.mount_parent_node = %parentNode;      
      MegaMotionScenes.mount_child_shape = %childShape;
      MegaMotionScenes.mount_child_node = %childNode;    
      MegaMotionScenes.mount_child_id = %childSceneShape;      
      
      //echo("selecting child shape: " @ %childShape );
      $mmShapeMountChildShapeList.setSelected(%childShape);
      
      //echo("Setting child shape node: " @ %childNode);
      $mmShapeMountChildNodeList.setSelected(%childNode);
      
      %offsetX.setText(sqlite.getColumn(%resultSet,"offset_x"));
      %offsetY.setText(sqlite.getColumn(%resultSet,"offset_y"));
      %offsetZ.setText(sqlite.getColumn(%resultSet,"offset_z"));
      %orientationX.setText(sqlite.getColumn(%resultSet,"orient_x"));
      %orientationY.setText(sqlite.getColumn(%resultSet,"orient_y"));
      %orientationZ.setText(sqlite.getColumn(%resultSet,"orient_z"));
      %scaleX.setText(sqlite.getColumn(%resultSet,"scale_x"));
      %scaleY.setText(sqlite.getColumn(%resultSet,"scale_y"));
      %scaleZ.setText(sqlite.getColumn(%resultSet,"scale_z"));      
   }
}

/*
"LEFT JOIN position o ON sm.offset_id=o.id " @ 
	"LEFT JOIN euler r ON sm.orientation_id=r.id " @ 
	"LEFT JOIN scale s ON sm.scale_id=s.id " @  
*/
	
function mmAddShapeMount()
{
   echo("calling addShapeMount!!!");
   
   makeSqlGuiForm($mmAddShapeMountWindowID);
   
   mmSetupAddShapeMountWindow();
   
}

function mmSetupAddShapeMountWindow()
{
   //HERE: fill in the parent nodes and child shape list. And then have a    

   %parentNodeList = addShapeMountWindow.findObjectByInternalName("parentNodeList");
   %childShapeList = addShapeMountWindow.findObjectByInternalName("childShapeList");
   %childNodeList = addShapeMountWindow.findObjectByInternalName("childNodeList");
   
   //Fill parent node list from active parent, because you have to have a shape selected in 
   //order to use any of the other UI controls anyway. But fill the child node list from the
   //database, because you won't instantiate the child shape until commit and reload.
   
   %parentNodeList.clear();
   for (%i=0;%i<$mmSelectedShape.getNumNodes();%i++)
   {
      %node_name = $mmSelectedShape.getNodeName(%i);
      %parentNodeList.add(%node_name,%i);
   }
   
   %childNodeList.clear();
   %childShapeList.clear();
   %query = "SELECT id,name FROM physicsShape ORDER BY name;";
   %resultSet = sqlite.query(%query, 0);
   if (%resultSet)
   {
      %childShapeList.add("",0);         
      while (!sqlite.endOfResult(%resultSet))
      {
         %id = sqlite.getColumn(%resultSet, "id");
         %name = sqlite.getColumn(%resultSet, "name");
            
         %childShapeList.add(%name,%id);
         sqlite.nextRow(%resultSet);         
      }         
   }
   sqlite.clearResult(%resultSet);
   
   //FIX! remember values from last use.
   %offsetX = addShapeMountWindow.findObjectByInternalName("offsetX");
   %offsetY = addShapeMountWindow.findObjectByInternalName("offsetY");
   %offsetZ = addShapeMountWindow.findObjectByInternalName("offsetZ");
   
   %offsetX.setText("0.0"); %offsetY.setText("0.0"); %offsetZ.setText("0.0"); 
   
   %rotX = addShapeMountWindow.findObjectByInternalName("rotationX");
   %rotY = addShapeMountWindow.findObjectByInternalName("rotationY");
   %rotZ = addShapeMountWindow.findObjectByInternalName("rotationZ");
   
   
   %rotX.setText("0.0"); %rotY.setText("0.0"); %rotZ.setText("0.0"); 
   
   %scaleX = addShapeMountWindow.findObjectByInternalName("scaleX");
   %scaleY = addShapeMountWindow.findObjectByInternalName("scaleY");
   %scaleZ = addShapeMountWindow.findObjectByInternalName("scaleZ");
   
   %scaleX.setText("1.0"); %scaleY.setText("1.0"); %scaleZ.setText("1.0"); 
}

function mmSelectAddShapeMountChildShape()
{
   //When child shape is selected, fill the child nodes list. *OR* just make them
   //take care of nodes on the main form. But why not do it here as well? Thinking
   //I should probably expand more of the add forms to allow more to be done there.
   
   %childShapeList = addShapeMountWindow.findObjectByInternalName("childShapeList");
   %childShape = %childShapeList.getSelected();
   
   %childNodeList = addShapeMountWindow.findObjectByInternalName("childNodeList");
   %childNodeList.clear();
   %query = "SELECT name,node_index FROM shapeNode WHERE physicsShape_id=" @ %childShape @ ";";
   %resultSet = sqlite.query(%query,0);
   while (!sqlite.endOfResult(%resultSet))
   {
      %name = sqlite.getColumn(%resultSet,"name");
      %index = sqlite.getColumn(%resultSet,"node_index");
      %childNodeList.add(%name,%index);
      sqlite.nextRow(%resultSet);
   }
   sqlite.clearResult(%resultSet);
}

function mmDeleteShapeMount()
{
   
   %shapeMount_id = $mmShapeMountList.getSelected();
   if (%shapeMount_id<=0)
      return;
      
   MessageBoxOKCancel( "Warning", 
      "Are you sure you want to delete this shapeMount?", 
      "mmReallyDeleteShapeMount();",
      "" ); 
}

function mmReallyDeleteShapeMount()
{
   //SO: to delete a shapeMount:
   // 1) get the child sceneShape id, because we are going to need to delete it from the scene (stored in selectShapeMount)
   // 2) //unmount the child shape from the parent shape - NOPE! We're just gonna unload/reload scene.
   // 3) delete the child sceneShape
   // 4) delete the shapeMount
   // 5) unload and reload the scene
   %shapeMount_id = $mmShapeMountList.getSelected();
   if (%shapeMount_id<=0)
      return;
      
   if (MegaMotionScenes.mount_child_id<=0)
      return;
   
   //%sceneShape_id = $mmSelectedSceneShape.sceneShapeID;
   %sceneShape_id = $mmSceneShapeList.getSelected();
   
   %query = "DELETE FROM sceneShape WHERE id=" @ MegaMotionScenes.mount_child_id @ ";";
   sqlite.query(%query,0);
   
   %query = "DELETE FROM shapeMount WHERE id=" @ %shapeMount_id @ ";";
   sqlite.query(%query,0);
      
   if ($mmLoadedScenes>0)
   {
      //echo("refreshing scene! " @ $mmSceneList.getSelected() );
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
   
   //echo("mmReallyDeleteShapeMount - SELECTING SCENE SHAPE " @ %sceneShape_id );
   $mmSceneShapeList.setSelected(%sceneShape_id);
}

function mmSelectMountChildShape()
{
   //When child shape is selected, fill the child nodes list. *OR* just make them
   //take care of nodes on the main form. But why not do it here as well? Thinking
   //I should probably expand more of the add forms to allow more to be done there.
   
   %childShape = $mmShapeMountChildShapeList.getSelected();
   $mmShapeMountChildNodeList.clear();
   %query = "SELECT name,node_index FROM shapeNode WHERE physicsShape_id=" @ %childShape @ ";";
   %resultSet = sqlite.query(%query,0);
   while (!sqlite.endOfResult(%resultSet))
   {
      %name = sqlite.getColumn(%resultSet,"name");
      %index = sqlite.getColumn(%resultSet,"node_index");
      $mmShapeMountChildNodeList.add(%name,%index);
      sqlite.nextRow(%resultSet);
   }
   sqlite.clearResult(%resultSet);
}

function mmReallyAddShapeMount()
{
   if (($mmSelectedShape<=0)||($mmSelectedShape.sceneID<=0))
      return;
   
   %sceneShape_id = $mmSelectedShape.sceneShapeID;
   
   %parentNodeList = addShapeMountWindow.findObjectByInternalName("parentNodeList");
   %childShapeList = addShapeMountWindow.findObjectByInternalName("childShapeList");
   %childNodeList = addShapeMountWindow.findObjectByInternalName("childNodeList");
   
   %parentNode = %parentNodeList.getSelected();
   %childShapeID = %childShapeList.getSelected();
   %childNode = %childNodeList.getSelected();
   
   %offsetX = addShapeMountWindow.findObjectByInternalName("offsetX").getText();
   %offsetY = addShapeMountWindow.findObjectByInternalName("offsetY").getText();
   %offsetZ = addShapeMountWindow.findObjectByInternalName("offsetZ").getText();
   
   %rotationX = addShapeMountWindow.findObjectByInternalName("rotationX").getText();
   %rotationY = addShapeMountWindow.findObjectByInternalName("rotationY").getText();
   %rotationZ = addShapeMountWindow.findObjectByInternalName("rotationZ").getText();
   
   %scaleX = addShapeMountWindow.findObjectByInternalName("scaleX").getText();
   %scaleY = addShapeMountWindow.findObjectByInternalName("scaleY").getText();
   %scaleZ = addShapeMountWindow.findObjectByInternalName("scaleZ").getText();
   
   //NOW: insert the actual values! Mount a new shape! But first, add the child shape
   //to sceneShapes! Since it's going to be a mount, using default pos and rot values.
   %query = "INSERT INTO sceneShape (name,scene_id,shape_id,shapeGroup_id," @
            "pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,rot_a,scale_x,scale_y,scale_z)" @
            " VALUES ('" @ %childShapeList.getText() @ "'," @ $mmSelectedShape.sceneID @ "," @ 
            %childShapeList.getSelected() @ "," @ $mmSelectedShape.shapeGroupID @ "," @ 
            "0,0,0,0,0,1,0,1,1,1);";//In fact, ignoring scale for now too, since it's stored in shapeMount.
   sqlite.query(%query,0);
   %query = "SELECT last_insert_rowid() AS id;";
   %resultSet = sqlite.query(%query,0);
   %child_id = sqlite.getColumn(%resultSet,"id");
   
   %query = "INSERT INTO shapeMount (parent_shape_id,child_shape_id,parent_node,child_node," @ 
            "offset_x,offset_y,offset_z,orient_x,orient_y,orient_z,scale_x,scale_y,scale_z) " @
            " VALUES (" @ $mmSelectedShape.sceneShapeID @ "," @ %child_id @ "," @
            %parentNodeList.getSelected() @ "," @ %childNodeList.getSelected() @ "," @
            %offsetX @ "," @ %offsetY @ "," @ %offsetZ @ "," @ 
            %rotationX @ "," @ %rotationY @ "," @ %rotationZ @ "," @ 
            %scaleX @ "," @ %scaleY @ "," @ %scaleZ @ ");";
   sqlite.query(%query,0);
   
   %shapeMount_id = sqlite.getLastRowId();
   
   //%query = "SELECT last_insert_rowid() AS id;";
   //%resultSet = sqlite.query(%query,0);
   //%sm_id = sqlite.getColumn(%resultSet,"id");
   
   //And, WHEW! Really gotta take a look at optimizations here... 
   
   if ($mmLoadedScenes>0)
   {   
      mmUnloadScene($mmSceneList.getSelected());
      mmLoadScene($mmSceneList.getSelected());
   }
   
   $mmSceneShapeList.setSelected(%sceneShape_id);
   $mmShapeMountList.setSelected(%shapeMount_id);
   
   addShapeMountWindow.visible = false;
}

/*

   //For first pass at least, just add default values and spawn the character at scene origin.
   %query = "INSERT INTO vector3 (x,y,z) VALUES (0,0,0);";
   sqlite.query(%query,0);
   %query = "UPDATE sceneShape SET pos_id=last_insert_rowid() WHERE id=" @ %child_id @ ";";
   sqlite.query(%query, 0); 
         
   %query = "INSERT INTO rotation (x,y,z,angle) VALUES (0,0,1,0);";      
   sqlite.query(%query,0);
   %query = "UPDATE sceneShape SET rot_id=last_insert_rowid() WHERE id=" @ %child_id @ ";";
   sqlite.query(%query, 0);  
   
   %query = "INSERT INTO vector3 (x,y,z) VALUES (1,1,1);";      
   sqlite.query(%query,0);
   %query = "UPDATE sceneShape SET scale_id=last_insert_rowid() WHERE id=" @ %child_id @ ";";
   sqlite.query(%query, 0); 
   
   
   
   //For first pass at least, just add default values and spawn the character at scene origin.
   %query = "INSERT INTO position (x,y,z) VALUES (" @ %offsetX @ "," @ %offsetY @ "," @ %offsetZ @ ");";
   sqlite.query(%query,0);
   %query = "UPDATE shapeMount SET offset_id=last_insert_rowid() WHERE id=" @ %sm_id @ ";";
   sqlite.query(%query, 0); 
         
   %query = "INSERT INTO euler (x,y,z) VALUES (" @ %rotationX @ "," @ %rotationY @ "," @ 
                  %rotationZ @ ");";      
   sqlite.query(%query,0);
   %query = "UPDATE shapeMount SET orientation_id=last_insert_rowid() WHERE id=" @ %sm_id @ ";";
   sqlite.query(%query, 0);  
   
   %query = "INSERT INTO scale (x,y,z) VALUES (" @ %scaleX @ "," @ %scaleY @ "," @ %scaleZ @ ");";      
   sqlite.query(%query,0);
   %query = "UPDATE shapeMount SET scale_id=last_insert_rowid() WHERE id=" @ %sm_id @ ";";
   sqlite.query(%query, 0);  
   
   


	%query = "SELECT shape_id,ss.name,shapeGroup_id,behavior_tree,openSteerProfile_id," @ 
	         "ss.pos_id AS pos_id,p.x AS pos_x,p.y AS pos_y,p.z AS pos_z," @ 
	         "ss.rot_id AS rot_id,r.x AS rot_x,r.y AS rot_y,r.z AS rot_z,r.angle AS rot_angle," @ 
	         "ss.scale_id AS scale_id,sc.x AS scale_x,sc.y AS scale_y,sc.z AS scale_z " @ 
	         "FROM sceneShape ss " @ 
	         "LEFT JOIN vector3 p ON ss.pos_id=p.id " @ 
	         "LEFT JOIN rotation r ON ss.rot_id=r.id " @ 
	         "LEFT JOIN vector3 sc ON ss.scale_id=sc.id " @ 
	         "WHERE ss.id=" @ %scene_shape_id @ ";";
*/

//////////////////////////////////////////////////////////////////////////////////
//Sequence Tab
function mmSelectSequence()
{
   echo("selecting sequence! " @ $mmSequenceList.getSelected());
   
   $mmSequenceNodeList.clear();    
   $mmSequenceKeyframeSetList.clear(); 
   $mmSequenceKeyframeSeriesList.clear();  
   $mmSequenceKeyframeList.clear();
   mmResetKeyframeValues();
   
   if (($mmSequenceList.getSelected()<=0)||($mmSelectedShape<=0) || (!isObject($mmSelectedShape)))
   {
      $mmSequenceFileText.setText("");
      return;
   }
   
   %seq_id = $mmSequenceList.getSelected();
   $mmSequenceFileText.setText($mmSelectedShape.getSeqFilename(%seq_id));
   
   $mmSequenceBlend = $mmSelectedShape.getSeqBlend(%seq_id);
   $mmSequenceCyclic = $mmSelectedShape.getSeqCyclic(%seq_id);
   
   echo("selecting sequence on shape! " @ $mmSequenceList.getSelected() @ " selected shape " @ $mmSelectedShape);

   $mmSelectedShape.playSeqByNum($mmSequenceList.getSelected());
   $mmSelectedShape.pauseSeq();
   $mmSelectedShape.setSeqPos(0);
   
   
   //echo("setting up node list!");
   %numNodes = $mmSelectedShape.getNumMattersNodes(%seq_id);
   for (%i=0;%i<%numNodes;%i++)
   {
      //echo("sequence " @ %seq_id @ " nodes, mattersNode " @ $mmSelectedShape.getMattersNodeIndex(%seq_id,%i) @
      //       " nodeMatters " @ $mmSelectedShape.getNodeMattersIndex(%seq_id,%i)); 
      %node_index = $mmSelectedShape.getMattersNodeIndex(%seq_id,%i);
      %node_name = $mmSelectedShape.getNodeName(%node_index);
      $mmSequenceNodeList.add(%node_name,%node_index);
   }
   
   %typenames[0]="adjust_pos";
   %typenames[1]="set_pos";
   %typenames[2]="adjust_rot";
   %typenames[3]="set_rot";
   
   $mmSequenceKeyframeSetList.add("",0);   
   %firstID = 0;
   %query = "SELECT id,name FROM keyframeSet WHERE shape_id=" @ 
            $mmSelectedShape.shapeID @ " AND sequence_name='" @ 
            $mmSequenceList.getText() @ "' ORDER BY name;"; 
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         echo("found " @ sqlite.numRows(%resultSet) @ " rows, first id " @ %firstID );
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            $mmSequenceKeyframeSetList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
      }
      sqlite.clearResult(%resultSet);
   }
   
   
   
   MegaMotionSequenceWindow.visible = true;//Why not?
   
   if (MegaMotionSequenceWindow.isVisible())
   {      
      %frames = $mmSelectedShape.getSeqFrames(%seq_id);
      $mmSequenceSlider.range = "0 " @ %frames;
      $mmSequenceSlider.value = 0;
      $mmSequenceInFrame.setText("0");
      $mmSequenceOutFrame.setText(%frames);
      
      //Above is redundantly repeated in the function below, for testing, problems occurred with them only in the function.
      mmSequenceResetInOutBars();
   }
}

/*
////(FROM EM) //////0
      
toolsWindow::selectSequence()
{
   ...
   
      if ($actor.getSeqNumKeyframes(%seqnum) == $actor.getSeqNumGroundFrames(%seqnum))
      {
         $sequence_ground_animate = true;
         groundCaptureSeqButton.setText("Un Ground Capture");
         GroundAnimateCheckbox.visible = true;
      } else {
         $sequence_ground_animate = false;
         groundCaptureSeqButton.setText("Ground Capture");
         GroundAnimateCheckbox.visible = false;
      }

   ...     
}
      
function EcstasyToolsWindow::toggleGroundAnimate()
{
   if (EWorldEditor.getSelectionSize()>0)
   {
      for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
      {
         %obj = EWorldEditor.getSelectedObject( %i );
         if (%obj)
         {
           if (%obj.getClassName() $= "fxFlexBody")
           {
              //echo("playing sequence " @ SequencesList.getText() @ " on actor " @ %obj.gatActorID());
               %ghostID = LocalClientConnection.getGhostID(%obj);
               %client_bot = ServerConnection.resolveGhostID(%ghostID);
               %client_bot.setGroundAnimating($tweaker_ground_animate);           
           }
         }
      }
   } 
   else if ($actor) 
   {
      $actor.setGroundAnimating($tweaker_ground_animate);
   }
}



*/

   //Nice thought, but nope:
   //%query = "SELECT id,frame,x,y,z FROM keyframe WHERE series_id IN" @ 
   //         " (SELECT id FROM keyframeSeries WHERE set_id IN" @ 
   //         " (SELECT id FROM keyframeSet" @ 
   //         " WHERE shape_id=" @ $mmSelectedShape.shapeID @ 
   //         " AND sequence_name='" @ $mmSequenceList.getText() @ "'));";  
   
function mmAddSequence()
{
   if (!isObject($mmSelectedShape))
      return;
         
   if (strlen($Pref::MegaMotion::DsqLoadDir))
      %openFileName = mmGetOpenFilename($Pref::MegaMotion::DsqLoadDir,"dsq");
   else
      %openFileName = mmGetOpenFilename($mmSelectedShape.getPath(),"dsq");
   
   if (strlen(%openFileName))
   {     
      %openFileName = strreplace(%openFileName,"'","''");//Escape single quotes in the name.
      if (!$mmSelectedShape.loadSequence(%openFileName))
         return;   	  
   } else return;

   mmSelectShape();
   
   $mmSceneShapeList.setSelected($mmSelectedShape.sceneShapeID);
   
   //Now, add it to the sequenceList (and any others?) and then select it.
   //%name = $mmSelectedShape.getSeqName($mmSequenceList.size()-1);
   //$mmSequenceList.add(%name,$mmSequenceList.size()-1);
   $mmSequenceList.setSelected($mmSequenceList.size()-1);
   
   echo("loaded sequence! " @ %openFileName @ ", sequence list: " @ $mmSequenceList.getText());
   return;
}

function mmAddSequenceByFilename(%filename)
{   
   if (!isObject($mmSelectedShape))
      return;      
   
   if (strlen(%filename))
   {       
     %filename = strreplace(%filename,"'","''");//Escape single quotes in the name.
      if (!$mmSelectedShape.loadSequence(%filename))
         return;   //possible repetition of addSequence taking place now...	  
   } else return;

   mmSelectShape();

   $mmSequenceList.setSelected($mmSequenceList.size()-1);
   
   return;   
}

function mmDropSequence()
{
   if (!isObject($mmSelectedShape))
      return;     
       
   $mmSelectedShape.dropSequence($mmSequenceList.getSelected());
    
}
   
function mmSaveSequence()
{
   if (!isObject($mmSelectedShape))
      return;      
       
   if (strlen($Pref::MegaMotion::DsqSaveDir))
      %saveFileName = mmGetSaveFileName($Pref::MegaMotion::DsqSaveDir,"dsq");
   else
      %saveFileName = mmGetSaveFileName($actor.getPath(),"dsq");
         
   $mmSelectedShape.saveSequence($mmSequenceList.getSelected(),%saveFileName);
}
   
function mmSelectSequenceNode()
{   
   /*
   %typenames[0]="adjust_pos";
   %typenames[1]="set_pos";
   %typenames[2]="adjust_rot";
   %typenames[3]="set_rot";
   
   $mmSequenceKeyframeSeriesList.clear();   
   %firstID = 0;
   %node_id = $mmSequenceNodeList.getSelected();
   %query = "SELECT id,type FROM keyframeSeries WHERE node=" @ %node_id @ 
            " AND set_id IN (SELECT id FROM keyframeSet" @ 
            " WHERE shape_id=" @ $mmSelectedShape.shapeID @ 
            " AND sequence_name='" @ $mmSequenceList.getText() @ "');"; 
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %type = sqlite.getColumn(%resultSet, "type");
            %typename = %typenames[%type];
            %nodename =  $mmSelectedShape.getNodeName(%node_id);    
            %name =  %nodename @ " - " @ %typename @ " " @ %id;
            $mmSequenceKeyframeSeriesList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if (%firstID>0) 
            $mmSequenceKeyframeSeriesList.setSelected(%firstID);
      } 
      else 
      {
         $mmSequenceKeyframeSeriesList.clear();
         $mmSequenceKeyframeList.clear();     
         $mmKeyframeID = 0;    
      }
      sqlite.clearResult(%resultSet);
   }
   */
}

function mmSelectKeyframeSet()
{
 //HERE: call addKeyframeSet() with the sequence name!
 
   %typenames[0]="adjust_pos";
   %typenames[1]="set_pos";
   %typenames[2]="adjust_rot";
   %typenames[3]="set_rot";
   
   
   $mmSelectedShape.clearKeyframeSet();
   //echo("APPLYING ULTRAFRAME SET!!!!!!");
   $mmSelectedShape.applyKeyframeSet();//Hmm, sometimes this crashes. Other times it helps us reset. Hmmm.
         
   //If resetting to a set with no frames, need to restore starting position.
   $mmSelectedShape.playSeqByNum($mmSequenceList.getSelected());
   $mmSelectedShape.pauseSeq();
   $mmSelectedShape.setSeqPos(0);   
   
   //If null option is chosen, simply restore sequence and exit.
   %set_id = $mmSequenceKeyframeSetList.getSelected();
   if (%set_id==0)
   {

      return;
   }
   
   %seq_name = $mmSequenceList.getText();
   echo("ADDING ULTRAFRAME SET!!!!!! " @ %seq_name);
   $mmSelectedShape.addKeyframeSet(%seq_name);   
   
   $mmSequenceKeyframeSeriesList.clear();   
   $mmSequenceKeyframeSeriesList.add("",0);   
   %firstID = 0;
   %query = "SELECT id,type,node FROM keyframeSeries WHERE set_id=" @ 
               %set_id @ " ORDER BY node;"; 
   %resultSet = sqlite.query(%query, 0);   
   if (sqlite.numRows(%resultSet)==0)
   { //Bail if no keyframeSeries exist, or else we'll crash when we try to apply.
      sqlite.clearResult(%resultSet);
      return;
   }
   //echo("SELECTING KEYFRAME SET, numSeries " @ sqlite.numRows(%resultSet));
   %first_id = 0;
   while (!sqlite.endOfResult(%resultSet))
   {
      %series_id = sqlite.getColumn(%resultSet, "id");
      if (%first_id==0) %first_id = %series_id;
      %type = sqlite.getColumn(%resultSet, "type");
      %typename = %typenames[%type];
      %node = sqlite.getColumn(%resultSet, "node"); 
      //echo("filling keyframeSeries list, series id " @ %series_id @ " node " @ %node);
      %nodename =  $mmSelectedShape.getNodeName(%node); 
      %name =  %nodename @ " - " @ %typename @ " " @ %series_id;
      $mmSequenceKeyframeSeriesList.add(%name,%series_id);
      
      //Now, due to change in plans, we need to actually add all the keyframes and apply them      
      %query2 = "SELECT * FROM keyframe WHERE series_id=" @ %series_id @ " ORDER BY frame;"; 
      %resultSet2 = sqlite.query(%query2, 0);  
      if (sqlite.numRows(%resultSet2)==0)
      { //Bail if no keyframeSeries exist, or else we'll crash when we try to apply.
         sqlite.clearResult(%resultSet2);
         return;
      }
      
      $mmSelectedShape.addKeyframeSeries(%type,%node);
      while (!sqlite.endOfResult(%resultSet2))
      {
         %frame = sqlite.getColumn(%resultSet2,"frame");
         %x = sqlite.getColumn(%resultSet2,"x");
         %y = sqlite.getColumn(%resultSet2,"y");
         %z = sqlite.getColumn(%resultSet2,"z");
         
         $mmSelectedShape.addKeyframe(%frame,%x,%y,%z);
         echo("adding a keyframe! " @ %frame);
         
         sqlite.nextRow(%resultSet2); 
      } 
      sqlite.nextRow(%resultSet); 
   }
   echo("APPLYING KEYFRAME SET!!!!!!");
   $mmSelectedShape.applyKeyframeSet();
   
   if ($mmSequenceKeyframeSeriesList.size()>1)
      $mmSequenceKeyframeSeriesList.setSelected(%first_id);
   
   
   //Now, we need to play the first frame again to show the change.
   $mmSelectedShape.playSeqByNum($mmSequenceList.getSelected());
   $mmSelectedShape.pauseSeq();
   $mmSelectedShape.setSeqPos(0);
}

function mmSelectKeyframeSeries()
{   
   $mmSequenceKeyframeList.clear();
   mmResetKeyframeValues();
   
   %series_id = $mmSequenceKeyframeSeriesList.getSelected();
   if (%series_id==0)
      return;
      
   %firstID = 0;
   %query = "SELECT id,frame,x,y,z FROM keyframe WHERE series_id=" @ %series_id @ " ORDER BY frame;";
   %resultSet = sqlite.query(%query, 0); 
   
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %frame = sqlite.getColumn(%resultSet, "frame");
            %x = sqlite.getColumn(%resultSet, "x");
            %y = sqlite.getColumn(%resultSet, "y");
            %z = sqlite.getColumn(%resultSet, "z");              
            %name = "frame " @  %frame ; // @ " value " @ %x @ " " @ %y @ " " @ %z  ;
            $mmSequenceKeyframeList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if (%firstID>0) 
            $mmSequenceKeyframeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
}

function mmSelectKeyframe()
{   
   mmResetKeyframeValues();
   
   if (!isObject($mmSelectedShape))
      return;     
   
   $mmKeyframeID = $mmSequenceKeyframeList.getSelected();
   
   if ($mmKeyframeID==0)
      return;
      
   %panel = $mmSequenceTab.findObjectByInternalName("sequencePanel");
   %sumX = %panel.findObjectByInternalName("sequenceKeyframeSumX");
   %sumY = %panel.findObjectByInternalName("sequenceKeyframeSumY");
   %sumZ = %panel.findObjectByInternalName("sequenceKeyframeSumZ");
   //%frame = %panel.findObjectByInternalName("sequenceKeyframeFrame");
   
   %query = "SELECT id,frame,x,y,z FROM keyframe WHERE id=" @ $mmKeyframeID @ ";";
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      %frame = sqlite.getColumn(%resultSet, "frame");
      %x = sqlite.getColumn(%resultSet, "x");
      %y = sqlite.getColumn(%resultSet, "y");
      %z = sqlite.getColumn(%resultSet, "z"); 
       
      %sumX.setText(%x);
      %sumY.setText(%y);
      %sumZ.setText(%z);  
      
      //%frame.setText(%frame);      
      $mmSequenceSlider.setValue(%frame);
      $mmSelectedShape.setSeqPos(%frame/$mmSequenceSlider.range.y);
      $mmSelectedShape.pauseSeq();
   }   
   
   //Here: set $mmSequenceKeyframeTypeText to "Rotation" or "Position" based on series.
   
}

function mmResetKeyframeValues()
{   
   %panel = $mmSequenceTab.findObjectByInternalName("sequencePanel");
   
   %keyframeSumX = %panel.findObjectByInternalName("sequenceKeyframeSumX");
   %keyframeSumY = %panel.findObjectByInternalName("sequenceKeyframeSumY");
   %keyframeSumZ = %panel.findObjectByInternalName("sequenceKeyframeSumZ");
   
   %keyframeSumX.setText(0);
   %keyframeSumY.setText(0);
   %keyframeSumZ.setText(0);
   
   $mmSequenceKeyframeValueX.setText(0);
   $mmSequenceKeyframeValueY.setText(0);
   $mmSequenceKeyframeValueZ.setText(0);
   
   $mmSequenceKeyframeTypeText.setText("");
}

function mmAddSequenceAction()
{
   //nothing here yet   
}

function mmDeleteSequenceAction()
{
   //nothing here yet   
}

function mmAssignSequenceAction()
{   
   %profile_id = $mmSelectedShape.actionProfileID;
   %seqFile = $mmSequenceFileText.getValue();
   %action_id = $mmSequenceActionList.getSelected();
   
   if ((%profile_id<=0)||(strlen(%seqFile)==0)||(%action_id<=0))
      return;
   
   %query = "SELECT id FROM actionProfileSequence WHERE " @
            "profile_id=" @ %profile_id @ " AND action_id=" @ 
            %action_id @ ";";
   %resultSet = sqlite.query(%query,0);
   if (%resultSet)
   {
      %actionSeq_id = sqlite.getColumn(%resultSet,"id");
      %query = "UPDATE actionProfileSequence SET sequence_name='" @ 
               %seqFile @ "' WHERE id=" @ %actionSeq_id @ ";";
      sqlite.query(%query,0);
   }
   else
   {
      %query = "INSERT INTO actionProfileSequence (profile_id,action_id,sequence_name) " @
               "VALUES (" @ %profile_id @ "," @ %action_id @ ",'" @ %seqFile @ "');";
      sqlite.query(%query,0);      
   }   
}

function mmUnassignSequenceAction()
{
   echo("mmUnassignSequenceAction is currently unavailable.");   
}

function mmAddMattersNode()
{
   if (!isObject($mmSelectedShape))
      return;     
      
   $mmSelectedShape.addMattersNode($mmSequenceList.getSelected(),$mmSequenceAllNodeList.getSelected());
   
   exposeMegaMotionScenesForm();
}

function mmDropMattersNode()
{
   if (!isObject($mmSelectedShape))
      return;   
  
   $mmSelectedShape.dropMattersNode($mmSequenceList.getSelected(),$mmSequenceAllNodeList.getSelected());
   
   exposeMegaMotionScenesForm();
}

//////////////////////////////////////////////////////////////////////////////////////
function mmSetupAddKeyframeSetWindow()
{
   //nothing to do
}

function mmAddKeyframeSet()
{ 
   if (!isObject($mmSelectedShape)||($mmSelectedShape.shapeID<=0)||($mmSequenceList.getSelected()<=0))
   {
      echo("Either shape or sequence is not selected., shape " @ $mmSelectedShape @ " seq id " @ $mmSequenceList.getSelected() );
      return;
   }
   
   if (isObject(mmAddKeyframeSetWindow))
      mmAddKeyframeSetWindow.delete();
   
   //FIX! Create the gui once at the beginning, from the .gui file, and make it visible 
   makeSqlGuiForm($mmAddKeyframeSetWindowID); // or not from here! For all guis!
   //And put them somewhere reasonable while you're at it!
     
   mmSetupAddKeyframeSetWindow();
}

function mmReallyAddKeyframeSet()
{ 
   echo("calling reallyAddKeyframeSet!");
   %shape_id = $mmSelectedShape.shapeID;
   %seq_name = $mmSequenceList.getText();
   %name = mmAddKeyframeSetWindow.findObjectByInternalName("nameEdit").getText();
   
   %query = "INSERT INTO keyframeSet (shape_id,sequence_name,name) VALUES (" @ %shape_id @
            ",'" @ %seq_name @ "','" @ %name @ "');";   
   sqlite.query(%query,0);
   
   %set_id = sqlite.getLastRowId();
   
   mmAddKeyframeSetWindow.delete();
      
   %seq = $mmSequenceList.getSelected();
   $mmSequenceList.setSelected(%seq);   
   
   $mmSequenceKeyframeSetList.setSelected(%set_id);
}

function mmDeleteKeyframeSet()
{   
   //Get a list of all series in this set, then loop through that deleting all keyframes.
   //If possible do this in one compound query.
   %set_id = $mmSequenceKeyframeSetList.getSelected(); 
   
   %query = "DELETE FROM keyframe WHERE series_id IN (SELECT id FROM keyframeSeries WHERE set_id=" @  
      %set_id @ ");";   
   sqlite.query(%query,0);
   
   %query = "DELETE FROM keyframeSeries WHERE set_id =" @ %set_id @ ");";   
   sqlite.query(%query,0);
   
   %query = "DELETE FROM keyframeSet WHERE id =" @ %set_id @ ");";   
   sqlite.query(%query,0);
   
   %seq = $mmSequenceList.getSelected();
   $mmSequenceList.setSelected(%seq);
}

/////////////////////////////////////////

function mmSetupAddKeyframeSeriesWindow()
{
   %nodeList = mmAddKeyframeSeriesWindow.findObjectByInternalName("nodeList");
   %seq_id = $mmSequenceList.getSelected();
   %numNodes = $mmSelectedShape.getNumMattersNodes(%seq_id);
   for (%i=0;%i<%numNodes;%i++)
   {
      %node_index = $mmSelectedShape.getMattersNodeIndex(%seq_id,%i);
      %node_name = $mmSelectedShape.getNodeName(%node_index);
      %nodeList.add(%node_name,%node_index);
   }  
}

function mmAddKeyframeSeries()
{ 
   if (!isObject($mmSelectedShape)||($mmSelectedShape.shapeID<=0)||($mmSequenceList.getSelected()<=0))
   {
      echo("Either shape or sequence is not selected., shape " @ $mmSelectedShape @ " seq id " @ $mmSequenceList.getSelected() );
      return;
   }
   
   if (isObject(mmAddKeyframeSeriesWindow))
      mmAddKeyframeSeriesWindow.delete();
   
   makeSqlGuiForm($mmAddKeyframeSeriesWindowID); 
     
   mmSetupAddKeyframeSeriesWindow();
}

function mmReallyAddKeyframeSeries()
{ 
      
   %set_id = $mmSequenceKeyframeSetList.getSelected(); 
   %node = mmAddKeyframeSeriesWindow.findObjectByInternalName("nodeList").getSelected();
    
   %type = 2;
   if ($mmAddKeyframeRotation)
      %type = 2;
   else //if ($mmAddKeyframePosition)
      %type = 0;
   
   %query = "INSERT INTO keyframeSeries (set_id,type,node) VALUES (" @ %set_id @
            ",'" @ %type @ "','" @ %node @ "');";   
   sqlite.query(%query,0);
      
   %series_id = sqlite.getLastRowId();
   
   mmAddKeyframeSeriesWindow.delete();
   
   %set = $mmSequenceKeyframeSetList.getSelected();
   $mmSequenceKeyframeSetList.setSelected(%set);  
   
   $mmSequenceKeyframeSeriesList.setSelected(%series_id);
}

function mmDeleteKeyframeSeries()
{      
   %series_id = $mmSequenceKeyframeSeriesList.getSelected();
   
   %query = "DELETE FROM keyframe WHERE series_id=" @ %series_id @ ";";
   sqlite.query(%query,0);
   
   %query = "DELETE FROM keyframeSeries WHERE id=" @ %series_id @ ";";
   sqlite.query(%query,0);
   
   %set_id = $mmSequenceKeyframeSetList.getSelected();
   $mmSequenceKeyframeSetList.setSelected(%set_id);//Re select set to refresh series list.
}


/////////////////////////////////////////

function mmSetupAddKeyframeWindow()
{   
   %valueX = mmAddKeyframeWindow.findObjectByInternalName("valueX"); 
   %valueY = mmAddKeyframeWindow.findObjectByInternalName("valueY"); 
   %valueZ = mmAddKeyframeWindow.findObjectByInternalName("valueZ"); 
   
   %valueX.setText("0");   
   %valueY.setText("0");
   %valueZ.setText("0");
   
   %frame = mmAddKeyframeWindow.findObjectByInternalName("frameEdit"); 
   %frame.setText($mmSequenceFrame.getText());
}

function mmAddKeyframe()
{ 
   if (!isObject($mmSelectedShape)||($mmSelectedShape.shapeID<=0)||($mmSequenceList.getSelected()<=0))
   {
      echo("Either shape or sequence is not selected., shape " @ $mmSelectedShape @ " seq id " @ $mmSequenceList.getSelected() );
      return;
   }
   
   if (isObject(mmAddKeyframeWindow))
      mmAddKeyframeWindow.delete();
   
   makeSqlGuiForm($mmAddKeyframeWindowID); 
     
   mmSetupAddKeyframeWindow();
}

function mmReallyAddKeyframe()
{ 
   echo("calling reallyAddKeyframe!");
   
   if (!isObject($mmSelectedShape)||($mmSelectedShape.shapeID<=0)||($mmSequenceList.getSelected()<=0))
   {
      echo("Either shape or sequence is not selected., shape " @ $mmSelectedShape @ " seq id " @ $mmSequenceList.getSelected() );
      return;
   }
   
   //First check the slider frame.
   //Make a new form for this, with XYZ values and node list. 
   
   %shape_id = $mmSelectedShape.shapeID;
   %series_id = $mmSequenceKeyframeSeriesList.getSelected();
   %set_id = $mmSequenceKeyframeSetList.getSelected();
   if (%series_id<=0)
   {
      echo("Please select the keyframe series before adding a keyframe!");
      return;  
   }
   
   //%node = $mmSequenceNodeList.getSelected();
   //%frame = mFloor($mmSequenceSlider.getValue());
   //%x = $mmSequenceKeyframeValueX.getText();
   //%y = $mmSequenceKeyframeValueY.getText();
   //%z = $mmSequenceKeyframeValueZ.getText(); 
   
   %frame = mmAddKeyframeWindow.findObjectByInternalName("frameEdit").getText();
   %x = mmAddKeyframeWindow.findObjectByInternalName("valueX").getText();
   %y = mmAddKeyframeWindow.findObjectByInternalName("valueY").getText();
   %z = mmAddKeyframeWindow.findObjectByInternalName("valueZ").getText();
   
   
   %query = "INSERT INTO keyframe (series_id,frame,x,y,z) VALUES (" @ %series_id @ "," @
               %frame @ "," @ %x @ "," @ %y @ "," @ %z @ ");";
   sqlite.query(%query,0);
    
   echo("finished mmAddKeyframeset()!");
   
   %keyframe_id = sqlite.getLastRowId();
   
   //if ($mmLoadedScenes>0)
   //{//FIX: would be better to load/unload just this shape, but then we need all instances of this
   //shape actually, so hell with it. Do your keyframe work on scenes with only one guy, ideally.
      //mmUnloadScene($mmSceneList.getSelected());
      //mmLoadScene($mmSceneList.getSelected());
   //}
   
   mmAddKeyframeWindow.delete();
   
   $mmSelectedShape.applyKeyframeSet();//HERE: we need to set mUltraframeSet on selectKeyframeSet
   
   $mmSequenceKeyframeSetList.setSelected(%set_id);    
   
   $mmSequenceKeyframeSeriesList.setSelected(%series_id); 
   
   $mmSequenceKeyframeList.setSelected(%keyframe_id);
}



/*

   %typenames[0]="adjust_pos";
   %typenames[1]="set_pos";
   %typenames[2]="adjust_rot";
   %typenames[3]="set_rot";
   
   $mmSequenceKeyframeSeriesList.clear();   
   %firstID = 0;
   %node_id = $mmSequenceNodeList.getSelected();
   %query = "SELECT id,type FROM keyframeSeries WHERE node=" @ %node_id @ 
            " AND set_id IN (SELECT id FROM keyframeSet" @ 
            " WHERE shape_id=" @ $mmSelectedShape.shapeID @ 
            " AND sequence_name='" @ $mmSequenceList.getText() @ "');"; 
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %type = sqlite.getColumn(%resultSet, "type");
            %typename = %typenames[%type];
            %nodename =  $mmSelectedShape.getNodeName(%node_id);    
            %name =  %nodename @ " - " @ %typename @ " " @ %id;
            $mmSequenceKeyframeSeriesList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if (%firstID>0) 
            $mmSequenceKeyframeSeriesList.setSelected(%firstID);
      } 
      else 
      {
         $mmSequenceKeyframeSeriesList.clear();
         $mmSequenceKeyframeList.clear();     
         $mmKeyframeID = 0;    
      }
      sqlite.clearResult(%resultSet);
   }
*/

/* 
//Removed remains of overthinking the problem: now we force creation of set, series 
//and keyframe independently, so we always know where we stand.
} else {
      %query = "SELECT * FROM keyframeSet WHERE shape_id=" @ %shape_id @ 
               " AND sequence_name='" @ $mmSequenceList.getText() @ "';";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)==1)
      {
         %set_id = sqlite.getColumn(%resultSet,"id");
         sqlite.clearResult(%resultSet);
      }
      if (%set_id>0)
      { //Here: we are not looking for any existing series, we just always create a new one...?
         %query = "INSERT INTO keyframeSeries (set_id,type,node) VALUES (" @ %set_id @ "," @
                  %type @ "," @ %node @ ");";
         sqlite.query(%query,0);      
      
         %query = "INSERT INTO keyframe (series_id,frame,x,y,z) VALUES (last_insert_rowid()" @ 
                  "," @ %frame @ "," @ %x @ "," @ %y @ "," @ %z @ ");";
         sqlite.query(%query,0);
      } 
      else 
      {      
         %query = "INSERT INTO keyframeSet (shape_id,sequence_name) VALUES (" @ %shape_id @ ",'" @
               $mmSequenceList.getText() @ "');";
         sqlite.query(%query,0);
         
         %query = "INSERT INTO keyframeSeries (set_id,type,node) VALUES (last_insert_rowid()," @  
                   %type @ "," @ %node @ ");";
         sqlite.query(%query,0);
         
         %query = "INSERT INTO keyframe (series_id,frame,x,y,z) VALUES (last_insert_rowid()" @ 
                  "," @ %frame @ "," @ %x @ "," @ %y @ "," @ %z @ ");";
         sqlite.query(%query,0);  
      }
   }  
*/

function mmDeleteKeyframe()
{
   %shape_id = $mmSelectedShape.shapeID;
   %series_id = $mmSequenceKeyframeSeriesList.getSelected();
   %keyframe_id = $mmSequenceKeyframeList.getSelected();
   if (%keyframe_id>0)
   {
      %query = "DELETE FROM keyframe WHERE id=" @ %keyframe_id @ ";";
      sqlite.query(%query,0);
      
      //See if this series has other keyframes, if so we're done. If not delete it, and possibly the set.
      %query = "SELECT id FROM keyframe WHERE series_id=" @ %series_id @ ";";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)>0)
         return; //didn't clear result before returning... but is it really necessary? 
      sqlite.clearResult(%resultSet);
      
      //Now, see if there are any other keyframeSeries sharing this set_id, and if not, delete the set.
      //FIX: we should really start doing this on the SQLite side with foreign keys and cascading deletes.
      %query = "SELECT set_id FROM keyframeSeries WHERE id=" @ %series_id @ ";";
      %resultSet = sqlite.query(%query,0);
      %set_id = sqlite.getColumn(%resultSet,"set_id");            
      sqlite.clearResult(%resultSet);
      
      %query = "SELECT id FROM keyframeSeries WHERE set_id=" @ %set_id @ " AND id!=" @ %series_id @ ";";
      %resultSet2 = sqlite.query(%query,0);  
      
      %query = "DELETE FROM keyframeSeries WHERE id=" @ %series_id @ ";";
      sqlite.query(%query);
      if (sqlite.numRows(%resultSet2)>0)
      {
         echo("Found more keyframeSeries in this set, not deleting it.");
         return;
      }         
      sqlite.clearResult(%resultSet2);

      %query = "DELETE FROM keyframeSet WHERE id=" @ %set_id @ ";";
      sqlite.query(%query,0);
   }   
   //exposeMegaMotionScenesForm();
}
///////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
// +/-/Set : search for a keyframe on the current frame for the current series. If found update it, if
//not found either create a new one on the fly, or call up a gui and ask nicely. (Decide after testing.)
//If doing it on the fly, allow unselecting all series on the series list, and then when you hit 
//the +/-/set buttons it will automatically make a new series, starting with a single frame.
 
function mmSetNode()
{
   %keyframe_id = $mmSequenceKeyframeList.getSelected();
   if (%keyframe_id<=0)
      return;
   
   %x = $mmSequenceKeyframeValueX.getText();
   %y = $mmSequenceKeyframeValueY.getText();
   %z = $mmSequenceKeyframeValueZ.getText();
   if (strlen(%x)==0) %x=0;
   if (strlen(%y)==0) %y=0;
   if (strlen(%z)==0) %z=0;
   
      
   %query = "UPDATE keyframe SET x=" @ %x @ ",y=" @ %y @ ",z=" @ %z @ " WHERE id=" @ %keyframe_id @ ";";
   sqlite.query(%query,0);
   
   //mmLoadKeyframeSets();
   $mmSelectedShape.applyKeyframeSet();
   mmSelectKeyframe();    
}

function mmAdjustNode()
{
   %keyframe_id = $mmSequenceKeyframeList.getSelected();
   if (%keyframe_id<=0)
      return;
   
   %query = "SELECT x,y,z FROM keyframe WHERE id=" @ %keyframe_id @ ";";
   %resultSet = sqlite.query(%query,0);
   %sumX = %sumY = %sumZ = 0;
   if (sqlite.numRows(%resultSet)==1)
   {
      %sumX = sqlite.getColumn(%resultSet,"x");
      %sumY = sqlite.getColumn(%resultSet,"y");
      %sumZ = sqlite.getColumn(%resultSet,"z");
      sqlite.clearResult(%resultSet);
   }   
   
   %x = $mmSequenceKeyframeValueX.getText();
   %y = $mmSequenceKeyframeValueY.getText();
   %z = $mmSequenceKeyframeValueZ.getText();
   
   if (strlen(%x)==0) %x=0;
   if (strlen(%y)==0) %y=0;
   if (strlen(%z)==0) %z=0;
         
   %query = "UPDATE keyframe SET x=" @ %sumX + %x @ ",y=" @ %sumY + %y @ ",z=" @ %sumZ + %z @ 
            " WHERE id=" @ %keyframe_id @ ";";
   sqlite.query(%query,0);
   
   //mmLoadKeyframeSets(); 
   $mmSelectedShape.applyKeyframeSet();  
   mmSelectKeyframe(); 
}

function mmUnadjustNode()
{
   %keyframe_id = $mmSequenceKeyframeList.getSelected();
   if (%keyframe_id<=0)
      return;
   
   %query = "SELECT x,y,z FROM keyframe WHERE id=" @ %keyframe_id @ ";";
   %resultSet = sqlite.query(%query,0);
   %sumX = %sumY = %sumZ = 0;
   if (sqlite.numRows(%resultSet)==1)
   {
      %sumX = sqlite.getColumn(%resultSet,"x");
      %sumY = sqlite.getColumn(%resultSet,"y");
      %sumZ = sqlite.getColumn(%resultSet,"z");
      sqlite.clearResult(%resultSet);
   }   
   
   %x = $mmSequenceKeyframeValueX.getText();
   %y = $mmSequenceKeyframeValueY.getText();
   %z = $mmSequenceKeyframeValueZ.getText();
   
   if (strlen(%x)==0) %x=0;
   if (strlen(%y)==0) %y=0;
   if (strlen(%z)==0) %z=0;
   
   %query = "UPDATE keyframe SET x=" @ %sumX - %x @ ",y=" @ %sumY - %y @ ",z=" @ %sumZ - %z @ 
            " WHERE id=" @ %keyframe_id @ ";";
   sqlite.query(%query,0);
   
   //mmLoadKeyframeSets();    
   $mmSelectedShape.applyKeyframeSet(); 
   mmSelectKeyframe(); 
   
}

function mmStartCentered()
{
   if (!isObject($mmSelectedShape))
      return;
      
   %seq = $mmSequenceList.getSelected();
   if (%seq > -1) 
   {  
      %initialPos = $mmSelectedShape.getNodeTrans(%seq,0);//returns frame zero
      %deltaX = -1 * getWord(%initialPos,0);
      %deltaY = -1 * getWord(%initialPos,1);
      %deltaZ = 0;
      %deltaPos = %deltaX @ " " @ %deltaY @ " " @ %deltaZ;
      $mmSelectedShape.adjustBaseNodePosRegion(%seq,%deltaPos,0.0,1.0);
   }
}

function mmFaceForward()
{
   if (!isObject($mmSelectedShape))
      return;
      
   %seq = $mmSequenceList.getSelected();
   if (%seq > -1)
   {  
      %initialRot = $mmSelectedShape.getNodeRot(%seq,0,0);//returns frame zero
      %deltaX = 0;//-1 * getWord(%initialRot,0); //See how this works...
      %deltaY = 0;//-1 * getWord(%initialRot,1);
      %deltaZ = getWord(%initialRot,2);
      %deltaRot = %deltaX @ " " @ %deltaY @ " " @ %deltaZ;
      $mmSelectedShape.adjustNodeRotRegion(%seq,0,%deltaRot,0.0,1.0);
   }
}

function mmMoveForward()
{
   if (!isObject($mmSelectedShape))
      return;
      
   %seq = $mmSequenceList.getSelected();
   if (%seq > -1)
   {
      %startPos = $mmSelectedShape.getNodeTrans(%seq,0);
      %finalPos = $mmSelectedShape.getNodeTrans(%seq,$mmSelectedShape.getSeqNumKeyframes(%seq)-1);
      %diff = VectorNormalize(VectorSub(%finalPos,%startPos));

      %forward = "0 1 0";
      %eulerArc = "0 0 0";
      if (VectorDot(%diff,%forward) < -0.999)//(ie, within small tolerance of 180 degrees opposite)
         %eulerArc = "0 0 180";
      else
      {
         //HERE: find the Z rotation!
         %eulerArc = rotationArcDegrees(%diff,%forward);//Actually, all rotations...
         echo("anim diff: " @ %diff @ ", euler arc: " @ %eulerArc);         
      }
      //%deltaRot = "0 0 " @ %deltaZ;
      $mmSelectedShape.adjustNodeRotRegion(%seq,0,%eulerArc,0.0,1.0); 
   }
}


function mmGroundCaptureSeq(%this)
{
   if (!isObject($mmSelectedShape))
      return;
      
   %seq = $mmSequenceList.getSelected();
   if ($mmSelectedShape.getSeqFrames(%seqnum) == $mmSelectedShape.getSeqGroundFrames(%seq))
   {
      $mmSelectedShape.unGroundCaptureSeq(%seq); 
      //GroundAnimateCheckbox.visible = false;
      $mmGroundCaptureButton.setText("Ground Capture");
      //SequenceNumGroundframes.setText($actor.getSeqNumGroundFrames(%seq));
   } else {
      $mmSelectedShape.groundCaptureSeq(%seq);
      //GroundAnimateCheckbox.visible = true;
      $mmGroundCaptureButton.setText("Un Ground Capture");
      //SequenceNumGroundframes.setText($actor.getSeqNumGroundFrames(%seq));
   }
   //EcstasyToolsWindow::updateSeqForm(%this);
}

function mmSequenceBlendToggle()
{
   %seq = $mmSequenceList.getSelected();
   if ((!isObject($mmSelectedShape))||(!(%seq>=0)))
      return;
      
   $mmSelectedShape.setSeqBlend(%seq,$mmSequenceBlend);
}

function mmSequenceCyclicToggle()
{
   %seq = $mmSequenceList.getSelected();
   if ((!isObject($mmSelectedShape))||(!(%seq>=0)))
      return;
   
   $mmSelectedShape.setSeqCyclic(%seq,$mmSequenceCyclic);
}

function mmSequenceGroundAnimateToggle()
{
   %seq = $mmSequenceList.getSelected();
   if ((!isObject($mmSelectedShape))||(!(%seq>=0)))
      return;
   
}




/////////////////////////////////////////////////////////
//BVH Tab
function mmImportBvhSequence()
{
   if (!isObject($mmSelectedShape))
      return;
      
   if (strlen($Pref::MegaMotion::BvhLoadDir))
      %openFileName = mmGetOpenFilename($Pref::MegaMotion::BvhLoadDir,"bvh");
   else
      %openFileName = mmGetOpenFilename($mmSelectedShape.getPath(),"bvh");
        
   
   $mmSelectedShape.importBvh(false,%openFileName,$mmBvhImportProfileList.getText(),false);
   
}

function mmImportBvhDirectory()
{
   
}

function mmBvhImportScene()
{
   
}

function mmExportBvhSequence()
{
   if (strlen($Pref::MegaMotion::BvhSaveDir))
      %saveFileName = mmGetSaveFilename($Pref::MegaMotion::BvhSaveDir,"bvh");
   else
      %saveFileName = mmGetSaveFilename($mmSelectedShape.getPath(),"bvh");
       
   $mmSelectedShape.saveBvh($mmSequenceList.getSelected(),%saveFileName,$mmBvhExportProfileList.getText(),$pref::MegaMotion::BvhGlobal);
   
}

function mmExportBvhDirectory()
{
   
}

function mmBvhExportScene()
{
   
}

function mmAddBvhProfile()
{   
   if (!isObject($mmSelectedShape))
      return;
         
         
   makeSqlGuiForm($mmAddBVHProfileWindowID);         
}

function mmReallyAddBvhProfile()
{  
   if (!isObject($mmSelectedShape))
      return;
      
   %profileName = mmAddBVHProfileWindow.findObjectByInternalName("nameEdit").getText();
   
   %profileSample = mmAddBVHProfileWindow.findObjectByInternalName("sampleBVHEdit").getText();
   
   $mmSelectedShape.importBvhSkeleton(%profileSample,%profileName);
   
   mmAddBVHProfileWindow.delete();
   
   return;
}

function mmBrowseBVHProfileSample()
{

   if (strlen($Pref::MegaMotion::BvhLoadDir))
      %openFileName = mmGetOpenFilename($Pref::MegaMotion::BvhLoadDir,"bvh");
   else
      %openFileName = mmGetOpenFilename($mmSelectedShape.getPath(),"bvh");
   
   if (strlen(%openFileName)>0)
   {
      mmAddBVHProfileWindow.findObjectByInternalName("sampleBVHEdit").setText(%openFileName);
   }
}
   
   
function mmDeleteBvhProfile()
{
   
}

function mmSelectBvhProfile()
{
   if (!isObject($mmSelectedShape))
      return;
      
   $mmBvhBvhNodeList.clear();
   $mmBvhLinkedNodesList.clear();
   
   %profile_id = $mmBvhProfileList.getSelected();
   if (%profile_id<=0)
      return;
      
   %query = "SELECT id,name FROM bvhProfileNode WHERE profile_id=" @ %profile_id @ ";";
   %resultSet = sqlite.query(%query,0);
   if (%resultSet)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         %id = sqlite.getColumn(%resultSet, "id");
         %name = sqlite.getColumn(%resultSet, "name");            
         $mmBvhBvhNodeList.add(%name,%id);
         sqlite.nextRow(%resultSet);         
      }
      sqlite.clearResult(%resultSet);
   }   
   
   %query = "SELECT id FROM bvhProfileSkeleton WHERE profile_id=" @ %profile_id @ 
   " AND skeleton_id=" @ $mmSelectedShape.skeletonID @ ";";
   %resultSet = sqlite.query(%query,0);
   %id = sqlite.getColumn(%resultSet, "id");
   sqlite.clearResult(%resultSet);
   
   if (%id>0)
   {      
      %query = "SELECT id,bvhNodeName,skeletonNodeName FROM bvhProfileSkeletonNode " @ 
               "WHERE bvhProfileSkeleton_id=" @ %id @ ";";
      %resultSet = sqlite.query(%query,0);
      while (!sqlite.endOfResult(%resultSet))
      {
         %id = sqlite.getColumn(%resultSet, "id");
         %bvhName = sqlite.getColumn(%resultSet, "bvhNodeName");            
         %skelName = sqlite.getColumn(%resultSet, "skeletonNodeName");   
         %text = %skelName @ " - " @ %bvhName;         
         $mmBvhLinkedNodesList.add(%text,%id);
         sqlite.nextRow(%resultSet);
      }
      sqlite.clearResult(%resultSet);
   }   
}

function mmBvhLinkNode()
{
   if (!isObject($mmSelectedShape))
      return;
      
   %node_id = 0;
   %skeleton_id = $mmSelectedShape.skeletonID;
   if ((%skeleton_id<=0)||(numericTest(%skeleton_id)==false))
      return;
   
   %query = "SELECT id FROM bvhProfileSkeleton WHERE profile_id=" @
             $mmBvhProfileList.getSelected() @ " AND skeleton_id = " @ %skeleton_id @";";
   %resultSet = sqlite.query(%query, 0); 
   if (sqlite.numRows(%resultSet)==1)
   {
      %bvhProfileSkeleton_id = sqlite.getColumn(%resultSet, "id");
      sqlite.clearResult(%resultSet);
   }
   else 
   {
      echo("ERROR: there are " @ sqlite.numRows(%resultSet) @ " bvhProfileSkeletons for bvh " @
         $mmBvhProfileList.getSelected() @ " and skeleton " @ %skeleton_id @ ", should be one.");
      sqlite.clearResult(%resultSet);      
      return;
   }
   %query = "INSERT INTO bvhProfileSkeletonNode (bvhProfileSkeleton_id,bvhNodeName,skeletonNodeName) " @ 
            "VALUES (" @ %bvhProfileSkeleton_id @ ",'" @ $mmBvhBvhNodeList.getText() @ "','" @
             $mmBvhModelNodeList.getText() @ "');"; 
   sqlite.query(%query, 0); 
   echo("Inserted new bvhProfileNode! " @ $mmBvhBvhNodeList.getText());
   /*
   //Maybe do some error checking at some point, at least enough to remind them to delete existing
   //links, if we don't do it automatically.
   
   %query = "SELECT id FROM bvhProfileSkeletonNode WHERE bvhProfileSkeleton_id=" @
            %bvhProfileSkeleton_id @ " AND bvhNodeName = '" @ $mmBvhBvhNodeList.getText() @ "';";
   %resultSet = sqlite.query(%query, 0); 
   
   %query = "UPDATE bvhProfileSkeletonNode SET skeletonNodeName='" @ 
            $mmBvhModelNodeList.getText() @ "' WHERE bvhProfileSkeleton_id=" @ 
            %bvhProfileSkeleton_id @ " AND bvhNodeName = '" @ 
            $mmBvhBvhNodeList.getText() @ "';";
   sqlite.query(%query, 0); 
   */
   
   
   //EcstasyToolsWindow::refreshBvhNodesList();
}

function mmBvhUnlinkNode()
{
   if (!isObject($mmSelectedShape))
      return;
      
   %skeleton_id = $mmSelectedShape.skeletonID;
   if ((%skeleton_id<=0)||(numericTest(%skeleton_id)==false))
      return;
   
   %query = "SELECT id FROM bvhProfileSkeleton WHERE profile_id=" @
             $mmBvhProfileList.getSelected() @ " AND skeleton_id = " @ %skeleton_id @";";
   %resultSet = sqlite.query(%query, 0); 
   if (sqlite.numRows(%result)==1)
   {
      %bvhProfileSkeleton_id = sqlite.getColumn(%result, "id");
      sqlite.clearResult(%resultSet);
   }
   else 
   {
      echo("ERROR: there are " @ sqlite.numRows(%result) @ " bvhProfileSkeletons for bvh " @
         $mmBvhProfileList.getSelected() @ " and skeleton " @ %skeleton_id @ 
         ", should be one.");
      return;
   }

   %query = "DELETE FROM bvhProfileSkeletonNode WHERE bvhProfileSkeleton_id=" @
            %bvhProfileSkeleton_id @ " AND bvhNodeName='" @ $mmBvhBvhNodeList.getText() @
            "' AND skeletonNodeName='" @ $mmBvhModelNodeList.getText() @ "';";
   sqlite.query(%query,0);
   
   //EcstasyToolsWindow::refreshBvhNodesList();   
}

/*
function EcstasyToolsWindow::unlinkBvhNode()
{
   //if(!EcstasyToolsWindow::StartSQL())
      //return;
   if (!$actor)
      return;
   %skeleton_id = $actor.getSkeletonId();
   if (numericTest(%skeleton_id)==false) %skeleton_id = 0;
   
   %query = "SELECT id FROM bvhProfileSkeleton WHERE bvhProfile_id=" @
             BvhImportProfilesList.getSelected() @ " AND skeleton_id = " @ %skeleton_id @";";
   %result = sqlite.query(%query, 0); 
   if (sqlite.numRows(%result)==1)
      %bvhProfileSkeleton_id = sqlite.getColumn(%result, "id");
      if (numericTest(%bvhProfileSkeleton_id)==false) %bvhProfileSkeleton_id = 0;
   else 
      echo("ERROR: there are " @ sqlite.numRows(%result) @ " bvhProfileSkeletons for bvh " @
         BvhImportProfilesList.getSelected() @ " and skeleton " @ %skeleton_id @ 
         ", should be one.");

   %query = "UPDATE bvhProfileSkeletonNode SET skeletonNodeName='' " @ 
            " WHERE bvhProfileSkeleton_id=" @ 
            %bvhProfileSkeleton_id @ " AND bvhNodeName = '" @ 
            BvhNodesList.getText() @ "';";
   %result = sqlite.query(%query, 0); 
   
   sqlite.clearResult(%result);
   //EcstasyToolsWindow::CloseSQL();
   
   EcstasyToolsWindow::refreshBvhNodesList();
}
*/

function mmSelectBvhLinkedNode()
{
   %panel = $mmBvhTab.findObjectByInternalName("bvhPanel");
   
   %posRotAX = %panel.findObjectByInternalName("bvhPoseRotAX");
   %posRotAY = %panel.findObjectByInternalName("bvhPoseRotAY");
   %posRotAZ = %panel.findObjectByInternalName("bvhPoseRotAZ");
   %posRotBX = %panel.findObjectByInternalName("bvhPoseRotBX");
   %posRotBY = %panel.findObjectByInternalName("bvhPoseRotBY");
   %posRotBZ = %panel.findObjectByInternalName("bvhPoseRotBZ");
   %fixRotAX = %panel.findObjectByInternalName("bvhFixRotAX");
   %fixRotAY = %panel.findObjectByInternalName("bvhFixRotAY");
   %fixRotAZ = %panel.findObjectByInternalName("bvhFixRotAZ");
   %fixRotBX = %panel.findObjectByInternalName("bvhFixRotBX");
   %fixRotBY = %panel.findObjectByInternalName("bvhFixRotBY");
   %fixRotBZ = %panel.findObjectByInternalName("bvhFixRotBZ");
   
   %id = $mmBvhLinkedNodesList.getSelected();
   
   %query = "SELECT * FROM bvhProfileSkeletonNode WHERE id=" @ %id @ ";";
   %resultSet = sqlite.query(%query,0);
   if (sqlite.numRows(%resultSet)==1)
   {
      %posRotAX.setText(sqlite.getColumn(%resultSet, "poseRotA_x"));
      %posRotAY.setText(sqlite.getColumn(%resultSet, "poseRotA_y"));
      %posRotAZ.setText(sqlite.getColumn(%resultSet, "poseRotA_z"));
      %posRotBX.setText(sqlite.getColumn(%resultSet, "poseRotB_x"));
      %posRotBY.setText(sqlite.getColumn(%resultSet, "poseRotB_y"));
      %posRotBZ.setText(sqlite.getColumn(%resultSet, "poseRotB_z"));
      %fixRotAX.setText(sqlite.getColumn(%resultSet, "fixRotA_x"));
      %fixRotAY.setText(sqlite.getColumn(%resultSet, "fixRotA_y"));
      %fixRotAZ.setText(sqlite.getColumn(%resultSet, "fixRotA_z"));
      %fixRotBX.setText(sqlite.getColumn(%resultSet, "fixRotB_x"));
      %fixRotBY.setText(sqlite.getColumn(%resultSet, "fixRotB_y"));
      %fixRotBZ.setText(sqlite.getColumn(%resultSet, "fixRotB_z"));
   }
   sqlite.clearResult(%resultSet);
   
}


function mmSelectMountShape()
{
   echo("selecting a mount shape! " @ $mmMountShapeList.getText());
   
   //%shapeMountID = $mmMountShapeList.getSelected();
   //%query = "SELECT * FROM shapeMount WHERE id=" @ %shapeMountID @ ";";
   //%resultSet = sqlite.query(%query,0);
   //if (sqlite.numRows(%resultSet)==1)
   //{
   //}
}


function mmSelectedRagdoll()
{   
   if (EWorldEditor.getSelectionSize()>0)
   {
      for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
      {
         %obj = EWorldEditor.getSelectedObject( %i );
         if (%obj)
         {
           if (%obj.getClassName() $= "PhysicsShape")
           {
              %obj.setDynamic(1);
           }
         }
      }
   }
   //$mmSelectedShape.setDynamic(1);
}

function mmSelectedRelaxAll()
{
   
   if (EWorldEditor.getSelectionSize()>0)
   {
      for (%i=0;%i<EWorldEditor.getSelectionSize();%i++)
      {
         %obj = EWorldEditor.getSelectedObject( %i );
         if (%obj)
         {
           if (%obj.getClassName() $= "PhysicsShape")
           {
              %obj.relaxAll();
           }
         }
      }
   }
   //$mmSelectedShape.relaxAll();   
}













//////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////
//  Sequence Timeline Window     /////////////////////////////////////////////////

function setupMegaMotionSequenceWindow()
{   
   if (!isDefined("MegaMotionSequenceWindow"))
      return;   
      
   $mmSequenceSliderIn = MegaMotionSequenceWindow.findObjectByInternalName("sequenceInBar");
   $mmSequenceSliderOut = MegaMotionSequenceWindow.findObjectByInternalName("sequenceOutBar");
   $mmSequenceSlider = MegaMotionSequenceWindow.findObjectByInternalName("sequenceSlider");
   $mmSequenceSlider.ticks = 0;   
   
   $mmSequenceInFrame = MegaMotionSequenceWindow.findObjectByInternalName("sequenceInFrame");
   $mmSequenceOutFrame = MegaMotionSequenceWindow.findObjectByInternalName("sequenceOutFrame");
   $mmSequenceFindLoopDelta = MegaMotionSequenceWindow.findObjectByInternalName("sequenceFindLoopDelta");
   $mmSequenceFrame = MegaMotionSequenceWindow.findObjectByInternalName("sequenceFrame");
   
   $mmSequenceInFrame.setText("");
   $mmSequenceOutFrame.setText("");
   $mmSequenceFindLoopDelta.setText("");
   
   $mmSequenceFrame.setText("");
}

function mmSequenceSetInBar()
{
   %outPos = $mmSequenceSliderOut.getPosition();
   
   %frame = $mmSequenceSlider.value;
   %frame = mFloor(%frame);
   
   $mmSelectedShape.getClientObject().recordCropStartPositions();//Used for loop detecting.
   
   %numFrames = $mmSequenceSlider.range.y;
   %pos_x = $mmSequenceSlider.position.x ;
   %slider_value = $mmSequenceSlider.getValue() / %numFrames;
   %extent_x = $mmSequenceSlider.extent.x;
   %newPos_x = %pos_x + (%slider_value * %extent_x);
   %newPos_y = $mmSequenceSlider.position.y - 16;
   %newPos = %newPos_x @ " " @ %newPos_y;
   //%newPos = $mmSequenceSlider.getPosition() + $mmSequenceSlider.getValue() ;
   //%newPos.x += (%frame / %numFrames) * ($mmSequenceSlider.extent.x - 12);//Slider can't go to end.
   //%newPos.y -= 16;
   
   echo("setting In position " @ %newPos @ " out position is " @ %outPos @ " posX " @ %pos_x @
         " value " @ %slider_value @ " extent x " @ %extent_x);
   
   if (%outPos.x > %newPos.x)
   {
      $mmSequenceSliderIn.setPosition(%newPos.x,%newPos.y);
      $mmSequenceInFrame.setText(%frame);
   }
   else if (%outPos.x < %newPos.x)
   {
      $mmSequenceSliderOut.setPosition(%newPos.x,%newPos.y);
      $mmSequenceOutFrame.setText(%frame);
      $mmSequenceSliderIn.setPosition(%outPos.x,%newPos.y);
      %sliderPos = $mmSequenceSlider.getPosition().x;
      %frame = mCeil(((%outPos.x - %sliderPos)/$mmSequenceSlider.extent.x)*$mmSequenceSlider.range.y);
      $mmSequenceInFrame.setText(%frame);
   }
   else if (%outPos.x == %newPos.x)
   {      
      $mmSequenceSliderIn.setPosition(%newPos.x,%newPos.y);
      $mmSequenceInFrame.setText(%frame);
      %frame += 1;
      %newPos = $mmSequenceSlider.getPosition();
      %newPos.x += (%frame / %numFrames) * ($mmSequenceSlider.extent.x - 12);
      $mmSequenceSliderOut.setPosition(%newPos.x,%newPos.y - 16);
      $mmSequenceOutFrame.setText(%frame);
   }      
}

function mmSequenceSetOutBar()
{   
   %inPos = $mmSequenceSliderIn.getPosition();
   
   %frame = $mmSequenceSlider.value;
   %frame = mCeil(%frame);
   %numFrames = $mmSequenceSlider.range.y;
   %newPos = $mmSequenceSlider.getPosition();
   %newPos.x += (%frame / %numFrames) * ($mmSequenceSlider.extent.x - 12);
   %newPos.y -= 16;
   
   if (%inPos.x > %newPos.x)
   {
      $mmSequenceSliderIn.setPosition(%newPos.x,%newPos.y);
      $mmSequenceInFrame.setText(%frame);
      $mmSequenceSliderOut.setPosition(%inPos.x,%inPos.y);
      %sliderPos = $mmSequenceSlider.getPosition().x;
      %frame = mCeil(((%inPos.x - %sliderPos)/$mmSequenceSlider.extent.x)*$mmSequenceSlider.range.y);
      $mmSequenceOutFrame.setText(%frame);
   }
   else if (%inPos.x == %newPos.x)
   {      
      $mmSequenceSliderIn.setPosition(%newPos.x,%newPos.y);
      $mmSequenceInFrame.setText(%frame);
      %frame += 1;
      %newPos = $mmSequenceSlider.getPosition();
      %newPos.x += (%frame / %numFrames) * ($mmSequenceSlider.extent.x - 12);
      %newPos.y -= 16;
      $mmSequenceSliderOut.setPosition(%newPos.x,%newPos.y);
      $mmSequenceOutFrame.setText(%frame);
   }
   else
   {
      $mmSequenceSliderOut.setPosition(%newPos.x,%newPos.y);
      $mmSequenceOutFrame.setText(%frame);
   }
   //echo("out bar, frame " @ %frame @ " numFrames " @ %numFrames @ " pos " @
   //       %newPos.x @ " startpos " @ $mmSequenceSlider.getPosition().x @ " extent " 
   //       @ $mmSequenceSlider.extent.x);

}

function mmSequenceResetInOutBars()
{
   if ($mmSelectedShape)
   {
      //%frames = $mmSelectedShape.getSeqFrames(%seq_id);
      //$mmSequenceSlider.range = "0 " @ %frames;
      //$mmSequenceSlider.value = 0;
      $mmSequenceInFrame.setText("0");
      $mmSequenceOutFrame.setText($mmSequenceSlider.range.y);
   }
   
   %newPos = $mmSequenceSlider.getPosition();
   %newPos.y -= 16;
   $mmSequenceSliderIn.setPosition((%newPos.x),%newPos.y);
   %newPos.x += ($mmSequenceSlider.extent.x - 10);
   $mmSequenceSliderOut.setPosition(%newPos.x,%newPos.y);
}

function mmSequenceBackwardToIn()
{
   $mmSelectedShape.pauseSeq();
   
   %inPos = $mmSequenceSliderIn.getPosition().x;
   %sliderPos = $mmSequenceSlider.getPosition().x;
   
   //Marker position as percentage of total slider width, -12 because marker can't get to the end.
   %inSeqPos = ((%inPos - %sliderPos)/($mmSequenceSlider.extent.x - 12));
   $mmSelectedShape.setSeqPos(%inSeqPos); 
}

function mmSequenceStepBackward()
{
   $mmSelectedShape.pauseSeq();
   
   //%seqFrameStep = 1.0/$mmSelectedShape.getSeqFrames($mmSelectedShape.getAmbientSeq());
   %seqFrameStep = 1.0/$mmSelectedShape.getSeqFrames($mmSequenceList.getSelected());
   //%seqFrameStep = 1.0/getWord($mmSequenceSlider.getRange(),1);//Hm, NOPE.
   %pos = $mmSelectedShape.getSeqPos();
   %newPos = %pos - %seqFrameStep;
   if (%newPos < 0.0)
      %newPos = 0.0;
      
   $mmSelectedShape.setSeqPos(%newPos);  
}

function mmSequencePlayBackward()
{
   %pos = $mmSelectedShape.getSeqPos();
   if (%pos==0) %pos = 1.0;
   //$mmSelectedShape.playSeqByNum($mmSelectedShape.getAmbientSeq());
   $mmSelectedShape.playSeqByNum($mmSequenceList.getSelected());
   $mmSelectedShape.reverseSeq();
   $mmSelectedShape.setSeqPos(%pos);   
}

function mmSequencePause()
{ 
   $mmSelectedShape.pauseSeq();
}

function mmSequencePlayForward()
{
   %pos = $mmSelectedShape.getSeqPos();
   //$mmSelectedShape.playSeqByNum($mmSelectedShape.getAmbientSeq());
   $mmSelectedShape.playSeqByNum($mmSequenceList.getSelected());
   $mmSelectedShape.forwardSeq();
   $mmSelectedShape.setSeqPos(%pos);
}

function mmSequenceStepForward()
{
   $mmSelectedShape.pauseSeq();
   
   //%seqFrameStep = 1.0/$mmSelectedShape.getSeqFrames($mmSelectedShape.getAmbientSeq());
   %seqFrameStep = 1.0/$mmSelectedShape.getSeqFrames($mmSequenceList.getSelected());
   %pos = $mmSelectedShape.getSeqPos();
   %newPos = %pos + %seqFrameStep;
   if (%newPos > 1.0)
      %newPos = 1.0;
      
   $mmSelectedShape.setSeqPos(%newPos); 
}

function mmSequenceForwardToOut()
{
   $mmSelectedShape.pauseSeq();
   
   %outPos = $mmSequenceSliderOut.getPosition().x;
   %sliderPos = $mmSequenceSlider.getPosition().x;
   
   //Marker position as percentage of total slider width.
   %outSeqPos = ((%outPos - %sliderPos)/($mmSequenceSlider.extent.x - 12));
   $mmSelectedShape.setSeqPos(%outSeqPos); 
}



/*
   %sliderPos = $mmSequenceSlider.getPosition();
   %numFrames = $mmSequenceSlider.range.y;
   %inPos = "12 " @ %sliderPos.y;
   %outPos = ($mmSequenceSlider.extent.x - 12) @ " " @ (%sliderPos.y - 12);
   
   $mmSequenceSliderIn.setPosition(%inPos.x,%inPos.y);
   $mmSequenceInFrame.setText("0");
   
   $mmSequenceSliderOut.setPosition(%outPos.x,%outPos.y);
   $mmSequenceOutFrame.setText(%numFrames);
*/

function mmSequenceSliderClick()
{
   $mmSelectedShape.pauseSeq();
   
   %value = $mmSequenceSlider.value; 
   %newPos = %value / $mmSequenceSlider.range.y;   
   $mmSelectedShape.setSeqPos(%newPos); 
}

function mmSequenceSliderDrag()
{
   $mmSelectedShape.pauseSeq();
      
   %value = $mmSequenceSlider.value; 
   %newPos = %value / $mmSequenceSlider.range.y;   
   $mmSelectedShape.setSeqPos(%newPos); 
}

function mmRefreshSequenceList()
{
   $mmSequenceList.clear();
   %numSeqs = $mmSelectedShape.getNumSeqs();
   for (%j=0;%j<%numSeqs;%j++)
   {
      %name = $mmSelectedShape.getSeqName(%j);
      $mmSequenceList.add(%name,%j);         
   }
}

function mmCrop()
{
   if (!isObject($mmSelectedShape))
      return;

   if (strlen($Pref::MegaMotion::DsqSaveDir))
      %saveFileName = mmGetSaveFileName($Pref::MegaMotion::DsqSaveDir,"dsq");
   else
      %saveFileName = mmGetSaveFileName($actor.getPath(),"dsq");

   %crop_start = $mmSequenceInFrame.getText()/$mmSequenceSlider.range.y;
   %crop_stop = $mmSequenceOutFrame.getText()/$mmSequenceSlider.range.y;
  
   //HERE: crop from crop_start to crop_stop, into a new sequence, append it to list.
   %seqnum = $mmSelectedShape.getSeqNum($mmSequenceList.getText());
   
   $mmSelectedShape.cropSequence(%seqnum,%crop_start,%crop_stop ,%saveFileName);
   echo("cropping sequence " @ $mmSequenceList.getText() @ " start " @ %crop_start @ " stop " @ %crop_stop);
   $mmSelectedShape.dropSequence($mmSelectedShape.getNumSeqs()-1);  
   if (strstr(%saveFileName,".dsq")<0)
      %saveFileName = %saveFileName @ ".dsq"; 
   $mmSelectedShape.loadSequence(%saveFileName);//maybe?
   
   //$mmSequenceSliderIn.setPosition();//Need a function for these, isolate the logic.
   //$mmSequenceSliderOut.setPosition();
   
   //mmRefreshSequenceList();
   $mmSceneShapeList.setSelected($mmSelectedShape.sceneShapeID);
   $mmSequenceList.setSelected($mmSequenceList.size()-1);
   
   echo("loaded new crop sequence: " @ %saveFileName);
   //$mmSequenceList.setSelected($mmSequenceList.size()-1);
   //EcstasyToolsWindow::selectSequence();
}

function mmFindLoop()
{
   //FIX: let's make these dynamic script members of each sceneShape.
   //$mmRotDeltaSumMin = 999.0;
   //$mmRotDeltaSumDescending = false;
   //$mmRotDeltaSumLast = 0;
   //$mmRotDeltaSumLastMinusOne = 0;
   //$mmRotDeltaSumLastMinusTwo = 0;
   //$mmLoopDetecting = true;
   
   $mmSelectedShape.rotDeltaSumDescending = false;
   $mmSelectedShape.rotDeltaSumMin = 999.0;
   $mmSelectedShape.rotDeltaSumCurrentFrame = 0;
   $mmSelectedShape.rotDeltaSumLastFrame = 0;
   $mmSelectedShape.rotDeltaSumLast = 0;
   $mmSelectedShape.rotDeltaSumLastMinusOne = 0;
   $mmSelectedShape.rotDeltaSumLastMinusTwo = 0;
   $mmSelectedShape.rotDeltaSumLastMinusThree = 0;
   $mmSelectedShape.rotDeltaSumLastMinusFour = 0;//A value from (four?) frames ago, to estimate overall slope.
         //But of course we have to save all frames from here to there in order to bump the values as we go.
   $mmSelectedShape.loopDetecting = true;
   
   EcstasySequenceSlider.paused = false;
   $mmSelectedShape.forwardSeq();
   echo("Find Loop! deltaSumMin " @ $mmRotDeltaSumMin);
   //$actor.startAnimatingAtPos(SequencesList.getText(),EcstasySequenceSlider.value);
}

function mmSmoothLoop()
{
   if (!isObject($mmSelectedShape))
      return;
   
   %seq = $mmSequenceList.getSelected();
   $mmSelectedShape.smoothLoopTransition(%seq,$mmLoopDetectorSmooth);
}

////////////////////////////////////////////////////
function mmGetOpenFileName(%defaultFilePath,%type)
{
   echo("GetOpenFileName: " @ %defaultFilePath @ " type " @ %type );
   if (%type$="dts")
      %filter = "DTS Files (*.dts)|*.dts|Collada Files (*.dae)|*.dae|FBX Files (*.fbx)|*.fbx|All Files (*.*)|*.*|";
   else if (%type$="dsq")
      %filter = "DSQ Files (*.dsq)|*.dsq|All Files (*.*)|*.*|";
   else if (%type$="bvh")
      %filter = "BVH Files (*.bvh)|*.bvh|All Files (*.*)|*.*|";    
   //else if ...
      
   %dlg = new OpenFileDialog()
   {
      Filters        = %filter;
      DefaultPath    = %defaultFilePath;
      //DefaultFile    = %defaultFileName;
      ChangePath     = false;
      MustExist      = true;
   };
   if(%dlg.Execute())
   {
      if (%type$="dts")
           $Pref::MegaMotion::DtsLoadDir = filePath( %dlg.FileName );
      else if (%type$="dsq")
           $Pref::MegaMotion::DsqLoadDir = filePath( %dlg.FileName );
      else if (%type$="bvh")
           $Pref::MegaMotion::BvhLoadDir = filePath( %dlg.FileName );
      %filename = %dlg.FileName;      
      %dlg.delete();
      return %filename;
   }
   %dlg.delete();
   return "";      
}

function mmGetSaveFileName(%defaultFilePath,%type)
{
   if (%type$="dts")
      %filter = "DTS Files (*.dts)|*.dts|All Files (*.*)|*.*|";
   else if (%type$="dsq")
      %filter = "DSQ Files (*.dsq)|*.dsq|All Files (*.*)|*.*|";
   else if (%type$="bvh")
      %filter = "BVH Files (*.bvh)|*.bvh|All Files (*.*)|*.*|";
   //else if ...
      
   %dlg = new SaveFileDialog()
   {
      Filters        = %filter;
      DefaultPath    = %defaultFilePath;
      ChangePath     = false;
      OverwritePrompt   = true;
   };
   if(%dlg.Execute())
   {
      $Pref::MegaMotion::DsqDir = filePath( %dlg.FileName );
      %filename = %dlg.FileName;      
      %dlg.delete();
      return %filename;
   }
   %dlg.delete();
   return "";   
   
}

//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////

function MegaMotionTick()
{  
   //General function for anything in MegaMotion that needs a tick but doesn't do it itself.
   
   //Sequence slider needs to keep up with selected shape's animation.
   if ((isObject($mmSelectedShape))&&($mmSequenceSlider)&&(MegaMotionSequenceWindow.isVisible()))
   {
      %threadPos = $mmSelectedShape.getSeqPos();
      %range = $mmSequenceSlider.range;
      %numFrames = %range.y;//range is a Point2F, so begin and end are x and y.
      %frame = %threadPos * %numFrames;
      $mmSequenceSlider.setValue(%frame);  
      //%frame = mRound(%frame * 100)/100;//(if we wanted "round to n decimals", n=100)      
      %frame = mRound(%frame);    
      $mmSequenceFrame.setText(%frame);
      
      /*
   $mmSelectedShape.rotDeltaSumDescending = false;
   $mmSelectedShape.rotDeltaSumMin = 999.0;
   $mmSelectedShape.rotDeltaSumCurrentFrame = 0;
   $mmSelectedShape.rotDeltaSumLastFrame = 0;
   $mmSelectedShape.rotDeltaSumLast = 0;
   $mmSelectedShape.rotDeltaSumLastMinusOne = 0;
   $mmSelectedShape.rotDeltaSumLastMinusTwo = 0;
   $mmSelectedShape.rotDeltaSumLastMinusThree = 0;
   $mmSelectedShape.rotDeltaSumLastMinusFour = 0;
   */
      //Loop Detection - checking for best animation cycle. 
      if ($mmSelectedShape.loopDetecting)
      {
         if ($mmSelectedShape.rotDeltaSumCurrentFrame == 0)
         {
            echo("starting loop detection!");
         }
         //TimelineRotDeltaSum.visible = 1;
         //cropStopCyclicButton.visible = 1;
         %seq = $mmSequenceList.getSelected();
         %current_frame = mFloor(%threadpos * $mmSequenceSlider.range.y);
         %start_frame = $mmSequenceInFrame.getText();
         //if (%start_frame==%current_frame)    
         //{//Somehow these starting values are getting lost between mmFindLoop and here. (??)
         //   $mmRotDeltaSumMin = 999.0;
         //   $mmRotDeltaSumDescending = 0;
         //   $mmRotDeltaSumLast = 0;
         //}
         
         %seqDeltaSum = $mmSelectedShape.getClientObject().getSeqDeltaSum(%seq,%current_frame,%start_frame);
         $mmSequenceFindLoopDelta.setText(%seqDeltaSum);
         //TimelineRotDeltaSum.setText(mFloatLength(%seqDeltaSum,3));
         %frame_from_start = %current_frame-%start_frame;
         
         %seqDeltaMinusOne = 0;
         %seqDeltaMinusTwo = 0;
         %seqDeltaMinusThree = 0;
         %seqDeltaMinusFour = 0;         
         
         //This is ugly, but we need a rolling array of last five values.
         if ($mmSelectedShape.rotDeltaSumCurrentFrame > 0)
         {
            if ($mmSelectedShape.rotDeltaSumCurrentFrame > 3) //if we're on frame four or more, move all four stored values.
            {
               //echo("Last four diff: " @ (%seqDeltaSum - $mmSelectedShape.rotDeltaSumLastMinusFour));
               $mmSelectedShape.rotDeltaSumLastMinusFour = $mmSelectedShape.rotDeltaSumLastMinusThree;
               $mmSelectedShape.rotDeltaSumLastMinusThree = $mmSelectedShape.rotDeltaSumLastMinusTwo;
               $mmSelectedShape.rotDeltaSumLastMinusTwo = $mmSelectedShape.rotDeltaSumLastMinusOne;
               $mmSelectedShape.rotDeltaSumLastMinusOne = $mmSelectedShape.rotDeltaSumLast; 
               
               %seqDeltaMinusOne = %seqDeltaSum - $mmSelectedShape.rotDeltaSumLastMinusOne;
               %seqDeltaMinusTwo = %seqDeltaSum - $mmSelectedShape.rotDeltaSumLastMinusTwo;
               %seqDeltaMinusThree = %seqDeltaSum - $mmSelectedShape.rotDeltaSumLastMinusThree;
               %seqDeltaMinusFour = %seqDeltaSum - $mmSelectedShape.rotDeltaSumLastMinusFour;  
            } 
            else if ($mmSelectedShape.rotDeltaSumCurrentFrame == 3)
            {
               $mmSelectedShape.rotDeltaSumLastMinusThree = $mmSelectedShape.rotDeltaSumLastMinusTwo;
               $mmSelectedShape.rotDeltaSumLastMinusTwo = $mmSelectedShape.rotDeltaSumLastMinusOne;
               $mmSelectedShape.rotDeltaSumLastMinusOne = $mmSelectedShape.rotDeltaSumLast;
            } 
            else if ($mmSelectedShape.rotDeltaSumCurrentFrame == 2)
            {
               $mmSelectedShape.rotDeltaSumLastMinusTwo = $mmSelectedShape.rotDeltaSumLastMinusOne;
               $mmSelectedShape.rotDeltaSumLastMinusOne = $mmSelectedShape.rotDeltaSumLast;
            }
            else if ($mmSelectedShape.rotDeltaSumCurrentFrame == 1)
            {
               $mmSelectedShape.rotDeltaSumLastMinusOne = $mmSelectedShape.rotDeltaSumLast;
            }
         }      
            
         echo(" frame: " @ %current_frame @ ", deltaSum " @ %seqDeltaSum @ ", -4 " @  %seqDeltaMinusFour @
               ", -3 " @ %seqDeltaMinusThree @ ", -2 " @ %seqDeltaMinusTwo @ ", -1 " @ %seqDeltaMinusOne );
            
         $mmSelectedShape.rotDeltaSumCurrentFrame++;
            
         if ((%seqDeltaSum < $mmSelectedShape.rotDeltaSumLast)&&(%frame_from_start > $mmLoopDetectorDelay))
            $mmSelectedShape.rotDeltaSumDescending = true;
         else 
         {
            if ($mmSelectedShape.rotDeltaSumDescending)
            {
               
               if ($mmSelectedShape.rotDeltaSumLast < $mmSelectedShape.rotDeltaSumMin)
               {
                  echo("loop detector found out pos: " @ %current_frame @ " frame-from-start " @ %frame_from_start );
                  $mmSelectedShape.rotDeltaSumMin = $mmSelectedShape.rotDeltaSumLast;
                  $mmSelectedShape.rotDeltaSumLastFrame = %current_frame-1;  //Are we sure about -1?                
                  //SequencesCropStopKeyframeText.setText($rotDeltaSumLastFrame);
                  %markOutPos = mFloatLength($mmSelectedShape.rotDeltaSumLastFrame / $mmSelectedShape.getSeqFrames(%seq),3);
                  $mmSequenceOutFrame.setText(%markOutPos);
                  $mmSequenceSlider.setValue($mmSelectedShape.rotDeltaSumLastFrame);
                  mmSequenceSetOutBar();
                  echo("found a minimum: " @ $mmSelectedShape.rotDeltaSumMin @ ", frame " @ $mmSelectedShape.rotDeltaSumLastFrame);           
                  $mmSelectedShape.loopDetecting = false;
                  $mmSelectedShape.pauseSeq();
               }
               $mmSelectedShape.rotDeltaSumDescending = false;
            }
         }
         $mmSelectedShape.rotDeltaSumLast = %seqDeltaSum;
         if (((%current_frame==0)&&($mmSelectedShape.rotDeltaSumMin<999.0))||(%frame_from_start > $mmLoopDetectorMax))//Hmmm
         {//FIX: not at all sure about any of this...
            echo("ending loop detection: current frame " @ %current_frame @ " frame-from-start " @ %frame_from_start @ 
               ", loop detector max: " @ $loopDetectorMax);
            //SequencesCropStopKeyframeText.setText($rotDeltaSumLastFrame);
            %markOutPos = mFloatLength($mmSelectedShape.rotDeltaSumLastFrame / $mmSelectedShape.getSeqFrames(%seq),3);
            //SequencesCropStopText.setText(%markOutPos);
            //$crop_stop = %markOutPos;//Note: this sucks, we should just check SequencesCropStopText.getText instead.
            //TimelineRotDeltaSum.setText(mFloatLength($rotDeltaSumMin,3));
            $mmSequenceOutFrame.setText(%markOutPos);            
            $mmSelectedShape.loopDetecting = false;
            $mmSequenceSlider.setValue($mmSequenceOutFrame.getText());
            mmSequenceSetOutBar();
            //EcstasySequenceSlider::setSliderToPos(SequencesCropStartText.getText());
            //$showRotDeltaSum = 0;
         }          
      }
   }
   
   schedule(30,0,"MegaMotionTick");//30 MS =~ 32 times per second.
}

function startSceneRecording()
{
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %shape.setIsRecording(true);
   }
}

function stopSceneRecording()
{
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %shape.setIsRecording(false);
   }   
   makeSceneSequences();
}

function makeSceneSequences()
{
   //OKAY... here we go. We now need to:
   // a) find our model's home directory   
   // b) in that directory, create a new directory with a naming protocol
   //       "scene_[%scene_id].[timestamp]"?
   // c) fill it with sequences
   
   echo("Making scene sequences!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
   %sceneDirPath = "art/shapes/MegaMotionScenes/" @ $mmProjectList.getText() @ "/" 
               @ $mmSceneList.getText() @ "/" @ getTime() @ "/";
   createDirectory(%sceneDirPath);
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %dsq_name = %shape.getSceneShapeID();
      %shape.makeSequence(%sceneDirPath @ "/" @ %dsq_name);
      echo("making scene sequence: " @ %sceneDirPath @ "/" @ %dsq_name);
   }
}

function mmLoadSceneSequences()
{
   %path = "";
   %dlg = new OpenFolderDialog()
   {
      DefaultPath    = $Pref::MegaMotion::SceneDsqDir;
      Filters        = "DSQ Files (*.dsq)|*.dsq|All Files (*.*)|*.*|";
   };
   
   if(%dlg.Execute())
   {
      $Pref::MegaMotion::SceneDsqDir = %dlg.FileName ;
      %path = %dlg.FileName;     
   }
   echo("dialog executed! path = " @ %path);
   
   %dlg.delete();
   
   
   if (strlen(%path)>0)
      mmReallyLoadSceneSequences(%path);
}
/*
function testFolderDialog()
{
   %dlg = new OpenFolderDialog()
   {
      DefaultPath    = "";
      Filters        = "";
   };
   
   if(%dlg.Execute())
   {
      %path = %dlg.FileName;     
      echo("dialog executed! path = " @ %path);
   } else {
      echo("dialog failed to execute! path = " @ %path);
   }
   %dlg.delete();
}*/

function mmReallyLoadSceneSequences(%path)
{
   echo("Really loading scene sequences!! path = " @ %path @ "!!!!!!!!!!!!!!!!");
   
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %dsq_name = %shape.getSceneShapeID() @ ".dsq";
      %index = %shape.findSeq(%shape.getSceneShapeID());
      if (%index>-1)
      {
         %shape.dropSequence(%index);
         echo("Dropping loading scene sequence!! index = " @ %index);
      }
      
      %shape.loadSequence(%path @ "/" @ %dsq_name);
      echo("Loaded sequence: " @ %path @ "/" @ %dsq_name);
      //No need to store this name, just make it again at play time from sceneShape ID.
   }   
}

function mmPlaySceneSequences()
{
   echo("Playing scene sequences!!!!!!!!!!!!!!!!!!");

   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);
      if (%shape.findSeq(%shape.getSceneShapeID())>-1)
      {
         //%shape.clearGroundMove();
         %shape.playSeq(%shape.getSceneShapeID());
      } else {
         echo("Could not find sequence: " @ %shape.getSceneShapeID());
      }
   }
}


function mmExportSceneBVH()
{
   %path = "";
   %dlg = new OpenFolderDialog()
   {
      DefaultPath    = $Pref::MegaMotion::SceneDsqDir;
      Filters        = "DSQ Files (*.dsq)|*.dsq|All Files (*.*)|*.*|";
   };
   
   if(%dlg.Execute())
   {
      $Pref::MegaMotion::SceneDsqDir = %dlg.FileName ;
      %path = %dlg.FileName;     
   }
   %dlg.delete();
   
   if (strlen(%path)>0)
      mmReallyExportSceneBVH(%path);
}

function mmReallyExportSceneBVH(%path)
{
   echo("Really loading scene sequences!! path = " @ %path @ "!!!!!!!!!!!!!!!!");
   
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %bvh_name = %shape.getSceneShapeID() @ ".bvh";
      %index = %shape.findSeq(%shape.getSceneShapeID());
      
      %shape.saveBvh(%index,%path @ "/" @ %bvh_name,"OldTruebones",$pref::MegaMotion::BvhGlobal);
      //echo("Loaded sequence: " @ %path @ "/" @ %dsq_name);
      //No need to store this name, just make it again at play time from sceneShape ID.
   }   
}
//////////////////////////////////////////////////////////////////////

function shapesAct()
{
   //pdd(1);
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i); 
      
      //%clientShape = %shape.getClientObject();//SINGLE PLAYER
      //%clientShape.createVehicle(%clientShape.getPosition(),0);//getRotation?   
      //echo("adding vehicle for sceneShape " @ %clientShape @ " position " @ %clientShape.getPosition() );
      
      //%shape.setHasGravity(false);
      
      //%shape.setDynamic(1);
      
      //%shape.setPartDynamic(0,0);
      //%shape.setPartDynamic(1,0);
      //%shape.setPartDynamic(2,0);  
      //%shape.setPartDynamic(3,0); 
      //%shape.setPartDynamic(4,0); 
      
      //%shape.setPartDynamic(5,0); 
      //%shape.setPartDynamic(6,0); 
      //%shape.setPartDynamic(7,0);
      //%shape.setPartDynamic(8,0);
      
      //%shape.setPartDynamic(9,0); 
      //%shape.setPartDynamic(10,0); 
      //%shape.setPartDynamic(11,0);
      //%shape.setPartDynamic(12,0);
       
      //%shape.setPartDynamic(13,0);
      //%shape.setPartDynamic(14,1);
      //%shape.setPartDynamic(15,1);
      
      //%shape.setPartDynamic(16,0);
      //%shape.setPartDynamic(17,0);
      //%shape.setPartDynamic(18,0);     
   } 
}





//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//For each form that we hook up to a top menu, give it an "expose" function to load it and call setup for it.
//OBSOLETE, TESTING /////////////////////////////
/*
function exposeMegaMotion()
{
   if (isDefined("MegaMotionWindow"))
      MegaMotionWindow.delete();
   
   makeSqlGuiForm($MegaMotionFormID);
   setupMegaMotionForm();   
}

function setupMegaMotionForm()
{
   if (!isDefined("MegaMotionWindow"))
      return;   
      
   %sceneSetList = MegaMotionWindow.findObjectByInternalName("sceneSetList");
   %sceneList = MegaMotionWindow.findObjectByInternalName("sceneList");
   
   %sceneSetList.add("Testing","1");
   %sceneSetList.add("Portlingrad","2");
   %sceneSetList.setSelected(1);
}
*/


//function testSpatialite()
//{
//   //%query = "CREATE TABLE spatialTest ( id INTEGER, name TEXT NOT NULL, geom BLOB NOT NULL);";
//   %query = "INSERT INTO spatialTest ( id , name, geom ) VALUES (1,'Test01',GeomFromText('POINT(1 2)'));";
   
//   %result = sqlite.query(%query, 0);
	
//   if (%result)
//      echo("spatialite inserted into a table with a geom!");
//   else
//      echo("spatialite failed to insert into a table with a geom!  "  );
//}

//OBSOLETE, TESTING /////////////////////////////

//Nice try, but even with modifications to GuiWindowCtrl, there is no way to intercept a mouse
//event that lands on a control. Hence, we're screwed if we want to select controls this way.
//function MegaMotionWindow::onMouseDown(%this,%pos)
//{
   //echo("MegaMotionWindow onMouseDown!!!!  this.pos " @ %this.getPosition() @ " mouse pos " @ %pos);  
//}
//
//function MegaMotionWindow::onMouseUp(%this,%pos)
//{
   //echo("MegaMotionWindow onMouseUp!!!!  this.pos " @ %this.getPosition() @ " mouse pos " @ %pos);  
//}

//Obsolete! We no longer want to load all keyframeSets at once, we want to deal with them
//one at a time, on demand.
/*
function mmLoadKeyframeSets()
{   
   echo("calling loadKeyframeSets!");
   %scene_id = $mmSceneList.getSelected();
   if ((%scene_id<=0)||(SceneShapes.getCount()==0))
      return;
  
  //First, make a distinct list of existing shapes, discarding duplicates.
   %numShapes = 0; //(Consider making this list global so we have it later.)
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);
      %shape_id = %shape.shapeID;  
      if (%numShapes==0)
      {
         %shapes[%numShapes] = %shape;
         %shape_ids[%numShapes] = %shape_id;
         %numShapes++;
      }
      else
      {
         %found = 0;
         for (%j=0;%j<%numShapes;%j++)
            if (%shape_ids[%j]==%shape_id)
               %found=1;         
         if (%found==0)
         {
            %shapes[%numShapes] = %shape;
            %shape_ids[%numShapes] = %shape_id;
            %numShapes++;
         }
      }
   }
   if (%numShapes==0)
      return;
      
   echo("found " @ %numShapes @ " distinct shapes!");
   
   for (%i=0;%i<%numShapes;%i++)
   {
      %shape = %shapes[%i];
      %shape_id = %shape_ids[%i];
      
      %query = "SELECT * FROM keyframeSet WHERE shape_id=" @ %shape_id @ ";";
      %resultSet = sqlite.query(%query,0);        
      while (!sqlite.endOfResult(%resultSet))
      { 
         %set_id = sqlite.getColumn(%resultSet,"id");
         %sequence = sqlite.getColumn(%resultSet,"sequence_name");
         %name = sqlite.getColumn(%resultSet,"name");
         
         %shape.addKeyframeSet(%sequence);
         
         %query2 = "SELECT * FROM keyframeSeries WHERE set_id=" @ %set_id @ ";";
         %resultSet2 = sqlite.query(%query2,0);
         while (!sqlite.endOfResult(%resultSet2))
         {
            %series_id = sqlite.getColumn(%resultSet2,"id");
            %type = sqlite.getColumn(%resultSet2,"type");
            %node = sqlite.getColumn(%resultSet2,"node");
            
            %shape.addKeyframeSeries(%type,%node);
            echo("adding a keyframeSeries! " @ %series_id);
            %query3 = "SELECT * FROM keyframe WHERE series_id=" @ %series_id @ " ORDER BY frame;";
            %resultSet3 = sqlite.query(%query3,0);
            while (!sqlite.endOfResult(%resultSet3))
            {
               %frame = sqlite.getColumn(%resultSet3,"frame");
               %x = sqlite.getColumn(%resultSet3,"x");
               %y = sqlite.getColumn(%resultSet3,"y");
               %z = sqlite.getColumn(%resultSet3,"z");
               
               %shape.addKeyframe(%frame,%x,%y,%z);
               echo("adding a keyframe! " @ %frame);
               
               sqlite.nextRow(%resultSet3); 
            }            
            sqlite.nextRow(%resultSet2); 
         }
         
         %shape.applyKeyframeSet();
         echo("applying keyframeSet! " @ %set_id);
         
         sqlite.nextRow(%resultSet);          
      }
   }   
}
*/
