//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

function PhysicsShapeData::damage(%this, %obj, %sourceObject, %position, %amount, %damageType)
{
   // Order of operations is extremely important here!
   // Verify that any changes will not cause this method to overflow the stack
   // recursively calling itself.
      
   // Note that invulerable, damageRadius, areaImpulse, radiusDamage, and damageType
   // are only dynamic fields... This is fine so long as you are only calling 
   // this method server-side, just keep in mind these fields are NOT networked.
   
   if (  %this.invulnerable ||
         %amount < 0 || 
         ( %this.minDamageAmount != 0 && %amount < %this.minDamageAmount ) )
      return;
      
   // We cannot destroy things twice.
   if ( %obj.isDestroyed() )
      return;
                        
   // This sets a maskbit on the server PhysicsShape which will cause the
   // client object to destroy ( spawn debris ) during the next ghost update.
   %obj.destroy();   
   
   // Single-player hack...
   // In a single-player situation the radial impulse NetEvent will
   // be applied client-side immediately when we call it, which means it will
   // happen before the next ghost update and the debris won't even exist yet!
   //
   // So we are explicitly calling destroy on the client-side object first,
   // before sending the event.
   //   
   if ( %obj.getClientObject() )
      %obj.getClientObject().destroy();
         
   if ( %this.damageRadius > 0 )
   {
      // Send impulse event to affect objects from the explosion of this object.
      // Happens server-side and client-side.
      if ( %this.areaImpulse > 0 )
         RadialImpulseEvent::send( %position, %this.damageRadius, %this.areaImpulse );
      
      // Apply damage to objects from the explosion of this object.   
      if ( %this.radiusDamage > 0 )
         radiusDamage( %obj, %position, %this.damageRadius, %this.radiusDamage, %this.damageType );
   }
}


function PhysicsShape::shapeSpecifics(%this)
{
   //echo("Calling shapeSpecifics for " @ %this.dataBlock @ ", server " @ %this.isServerObject());
   //Misc section for things that haven't found a better place yet.
   if (%this.dataBlock $= "M4Physics") 
   {     
      %this.setActionSeq("walk","walk");      
      %this.setActionSeq("run","run");
      
      %this.setActionSeq("ambient","ambient");//This might not always be idle, could be just breathing
      %this.setActionSeq("idle","ambient");// and idle could be that plus fidgeting, etc.

      %this.setActionSeq("rightGetup","rSideGetup");
      %this.setActionSeq("leftGetup","lSideGetup");
      %this.setActionSeq("frontGetup","frontGetup");
      %this.setActionSeq("backGetup","backGetup");   
         
   } 
   else if (%this.dataBlock $= "bo105Physics") 
   {            
   } 
   else if (%this.dataBlock $= "dragonflyPhysics") 
   {            
   } 
   else 
   if (%this.dataBlock $= "ka50Physics") 
   {
      
      %this.schedule(500,"showRotorBlades");
      //%this.schedule(500,"setUseDataSource",true);//vehicleDataSource networking not working yet in new build, 12/26/2016
      //%this.setIsRecording(true);
   }  
}

///////////////////////////////////////////////////////////////////////////////////////
//HERE: all this needs to be moved farther down into specific behavior trees. 
function PhysicsShape::onStartup(%this)
{
   //echo(%this @ " calling onStartup! position " @ %this.getPosition() @ " tree " @ %this.behaviorTree );
   /*
   if (%this.dataBlock $= "M4Physics")
   {      
      %this.setActionSeq("ambient","ambient");//This might not always be idle, could be just breathing
      %this.setActionSeq("idle","ambient");// and idle could be that plus fidgeting, etc.
      %this.setActionSeq("walk","walk");      
      %this.setActionSeq("run","run");   
         
      %this.setActionSeq("fall","runscerd");//Also doesn't matter if you use the same anim for multiple actionSeqs.
      %this.setActionSeq("getup","rSideGetup");   
      %this.setActionSeq("attack","power_punch_down");
      %this.setActionSeq("block","punch_uppercut");//TEMP, don't have any blocking anims atm
      
      //%this.setActionSeq("runscerd","runscerd");
      //%this.setActionSeq("crouch","crouch");
      
      //%this.setIsRecording(true);
      
      %this.groundMove();
      %this.currentAction = "walk";//LATER, dependent on which behaviorTree.
      
      //%this.setIsRecording(true);
      //echo("starting up a M4 physics shape!");      
   } 
   else if (%this.dataBlock $= "bo105Physics") //useDataSource: holding off on this for now.
   {
      //%this.setName("bo105");
      //%this.schedule(500,"setUseDataSource",true);
      //%this.setIsRecording(true);
      //%this.showNodes();     
   } 
   else if (%this.dataBlock $= "dragonflyPhysics") 
   {
      //%this.setName("dragonfly");
      //%this.schedule(500,"setUseDataSource",true);
   }
   else if (%this.dataBlock $= "ka50Physics") 
   { 
      //%this.setName("ka50");
      //%this.schedule(1900,"setUseDataSource",true);
      %this.schedule(2000,"showRotorBlades");
      //%this.showNodes();   
   }
   */
}

function PhysicsShape::openSteerSimpleVehicle(%this)
{   
   if (%this.getVehicleID()>0)
      return;//We've already done all this, so don't do it again.
      
   %this.currentAction = "walk";
   
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;   
      
   %clientShape.createVehicle(%clientShape.getPosition(),0);
   %clientShape.openSteerID = %this.openSteerID;
   
   if (%clientShape.openSteerID <= 0)
      return;
   
   //openSteerProfile table, with openSteer_id in sceneShape. Maybe move this to engine? Probably slow, esp. for blocks.
   %query = "SELECT * FROM openSteerProfile WHERE id=" @ %this.openSteerID @ ";";
   %resultSet = sqlite.query(%query,0);
   if (sqlite.numRows(%resultSet)==1)
   {
      %clientShape.setOpenSteerMass(sqlite.getColumn(%resultSet,"mass"));
      %clientShape.setOpenSteerRadius(sqlite.getColumn(%resultSet,"radius"));
      %clientShape.setOpenSteerMaxForce(sqlite.getColumn(%resultSet,"maxForce"));
      %clientShape.setOpenSteerMaxSpeed(sqlite.getColumn(%resultSet,"maxSpeed"));
      %clientShape.setOpenSteerWanderChance(sqlite.getColumn(%resultSet,"wanderChance"));
      %clientShape.setOpenSteerWanderWeight(sqlite.getColumn(%resultSet,"wanderWeight"));
      %clientShape.setOpenSteerSeekTargetWeight(sqlite.getColumn(%resultSet,"seekTargetWeight"));
      %clientShape.setOpenSteerAvoidTargetWeight(sqlite.getColumn(%resultSet,"avoidTargetWeight"));
      %clientShape.setOpenSteerSeekNeighborWeight(sqlite.getColumn(%resultSet,"seekNeighborWeight"));
      %clientShape.setOpenSteerAvoidNeighborWeight(sqlite.getColumn(%resultSet,"avoidNeighborWeight"));
      %clientShape.setOpenSteerAvoidNavMeshEdgeWeight(sqlite.getColumn(%resultSet,"avoidNavMeshEdgeWeight"));
      %clientShape.setOpenSteerDetectNavMeshEdgeRange(sqlite.getColumn(%resultSet,"detectNavMeshEdgeRange"));
   }
}

function PhysicsShape::openSteerNavVehicle(%this)
{   
   //echo("CALLING openSteerNavVehicle! isServer: " @ %this.isServerObject());
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;   
      
   %clientShape.currentPathNode = 0;
   %clientShape.sceneShapeID = %this.sceneShapeID;   
   %clientShape.openSteerID = %this.openSteerID;

   
   if (%clientShape.getVehicleID()>0)
      return;//We've already done all this, so don't do it again.
      
   if (!isObject(Nav))//TEMP! Search MissionGroup for all NavMesh objects, pick best one.
      return;
      
   //%clientShape.setNavMesh("Nav");
   %this.setNavMesh("Nav");//Is there any reason for this to happen? Should be client only right?
   %this.createVehicle(%clientShape.getPosition(),0);
   
   
   //%clientShape.createVehicle(%clientShape.getPosition(),0);
   //%clientShape.setNavMesh("Nav");//Hm, having navpath problems... trying
   
   
   %clientShape.currentAction = "walk";
   
   if (%clientShape.openSteerID <= 0)
      return;
   
   %id = %clientShape.openSteerID;
   %query = "SELECT * FROM openSteerProfile WHERE id=" @ %this.openSteerID @ ";";
   %resultSet = sqlite.query(%query,0);
   echo("trying to setup an opensteer vehicle! opensteer id " @ %id @ " maxForce " @ $openSteerProfile[%id].maxForce);
   
   if (%resultSet)
   {
      %clientShape.setOpenSteerMass(sqlite.getColumn(%resultSet,"mass"));
      %clientShape.setOpenSteerRadius(sqlite.getColumn(%resultSet,"radius"));
      %clientShape.setOpenSteerMaxForce(sqlite.getColumn(%resultSet,"maxForce"));
      %clientShape.setOpenSteerMaxSpeed(sqlite.getColumn(%resultSet,"maxSpeed"));
      %clientShape.setOpenSteerWanderChance(sqlite.getColumn(%resultSet,"wanderChance"));
      %clientShape.setOpenSteerWanderWeight(sqlite.getColumn(%resultSet,"wanderWeight"));
      %clientShape.setOpenSteerSeekTargetWeight(sqlite.getColumn(%resultSet,"seekTargetWeight"));
      %clientShape.setOpenSteerAvoidTargetWeight(sqlite.getColumn(%resultSet,"avoidTargetWeight"));
      %clientShape.setOpenSteerSeekNeighborWeight(sqlite.getColumn(%resultSet,"seekNeighborWeight"));
      %clientShape.setOpenSteerAvoidNeighborWeight(sqlite.getColumn(%resultSet,"avoidNeighborWeight"));
      %clientShape.setOpenSteerAvoidNavMeshEdgeWeight(sqlite.getColumn(%resultSet,"avoidNavMeshEdgeWeight"));
      %clientShape.setOpenSteerDetectNavMeshEdgeRange(sqlite.getColumn(%resultSet,"detectNavMeshEdgeRange"));
   } 
}

/*
   //AND... NOPE! Not yet.This will be trivial but no time right now for it. Array didn't work. Need to create
   // a new engine level object and communicate properties to script, so I can say new OpenSteerProfile() and
   // set up each of them once at load time.
   %clientShape.setOpenSteerMass($openSteerProfile[%id].mass);
   %clientShape.setOpenSteerRadius($openSteerProfile[%id].radius);
   %clientShape.setOpenSteerMaxForce($openSteerProfile[%id].maxForce);
   %clientShape.setOpenSteerMaxSpeed($openSteerProfile[%id].maxSpeed);
   %clientShape.setOpenSteerWanderChance($openSteerProfile[%id].wanderChance);
   %clientShape.setOpenSteerWanderWeight($openSteerProfile[%id].wanderWeight);
   %clientShape.setOpenSteerSeekTargetWeight($openSteerProfile[%id].seekTargetWeight);
   %clientShape.setOpenSteerAvoidTargetWeight($openSteerProfile[%id].avoidTargetWeight);
   %clientShape.setOpenSteerSeekNeighborWeight($openSteerProfile[%id].seekNeighborWeight);
   %clientShape.setOpenSteerAvoidNeighborWeight($openSteerProfile[%id].avoidNeighborWeight);
   %clientShape.setOpenSteerAvoidNavMeshEdgeWeight($openSteerProfile[%id].avoidNavMeshEdgeWeight);
   %clientShape.setOpenSteerDetectNavMeshEdgeRange($openSteerProfile[%id].detectNavMeshEdgeRange);
   */
   
function PhysicsShape::orientTo(%this, %dest)
{
   %pos = isObject(%dest) ? %dest.getPosition() : %dest;
   
   %this.orientToPos(%pos);
}

function PhysicsShape::moveTo(%this, %dest, %slowDown)
{
   %pos = isObject(%dest) ? %dest.getPosition() : %dest;
   
   %this.orientToPos(%pos);
   
   %this.actionSeq(%this.currentAction);
}

function PhysicsShape::say(%this, %message)//Testing, does this only work for AIPlayers?
{
   chatMessageAll(%this, '\c3%1: %2', %this.getid(), %message);  
}

function PhysicsShape::findTargetShapePos(%this)
{ 
   //echo(%this.sceneShapeID @ " is seeking target shape: " @ %this.targetShapeID @ " isServer " @ %this.isServerObject());
   if (%this.targetShapeID>0)
   {
      for (%i = 0; %i < SceneShapes.getCount();%i++)
      {
         %targ = SceneShapes.getObject(%i);  
         if (%targ.sceneShapeID==%this.targetShapeID)
         {
            %this.goalPos = %targ.getClientObject().getPosition();
            %this.targetItem = %targ.getClientObject();
            //echo(%this.sceneShapeID @ " found target shape: " @ %this.targetShapeID @ " pos " @ %this.goalPos);
            return;
         }
      }
   }   
   return;
}

function PhysicsShape::findRandomTargetPos(%this)
{ 
   //echo(%this.sceneShapeID @ " is seeking target shape: " @ %this.targetShapeID @ " isServer " @ %this.isServerObject());
   if (%this.targetShapeID==0)
   {
      %cubeCount = CubeShapes.getCount();
      %randomCube = getRandom(%cubeCount-1);
      for (%i = 0; %i < %cubeCount;%i++)
      {
         if (%i == %randomCube)
         {
            %targ = CubeShapes.getObject(%i);  
            %this.goalPos = %targ.getClientObject().getPosition();
            %this.targetItem = %targ.getClientObject();
            //echo(%this.sceneShapeID @ " found target shape: " @ %this.targetShapeID @ " pos " @ %this.goalPos);
            return;
         }
      }
   }   
   return;
}

function PhysicsShape::findSeriesTargetPos(%this)
{ 
   //echo(%this.sceneShapeID @ " is seeking target shape: " @ %this.targetShapeID @ " isServer " @ %this.isServerObject());
   %cubeCount = CubeShapes.getCount();
   if ((%this.targetItem==0)||(%this.currentCube==%cubeCount-1))
   {
      %this.currentCube = -1;
   }
   %this.currentCube++;
   for (%i = 0; %i < %cubeCount;%i++)
   {
      %targ = CubeShapes.getObject(%i);  
      if (!strcmp(%targ.name,"Cube_" @ %this.currentCube))
      {
         %this.goalPos = %targ.getClientObject().getPosition();
         %this.targetItem = %targ.getClientObject();
         //echo(%this.sceneShapeID @ " found series target: " @ %targ.sceneShapeID @ " count " @ %this.currentCube);
         return;
      }
   }   
   return;
}

function PhysicsShape::checkTeamProximity(%this,%detectDist)
{  //HERE: we need to loop through all members of enemy team, and find the closest one, and return
   //TRUE if the closest one is within our minimum threshold value. (?) Or just return the distance?
   
   echo("defender checking team proximity! detectDist " @ %detectDist);
    
   %diff = 0;
   //%minDiff = 9999999;//I guess we don't need this, if we're not returning the closest one.
   %clientShape = 0;
   
   if (%this.shapeGroupID==1)
      %enemyGroupID = 2;
   else if (%this.shapeGroupID==2)
      %enemyGroupID = 1;
   
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else  
      %clientShape = %this;
      
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %targ = SceneShapes.getObject(%i);  
      if (%targ.shapeGroupID==%enemyGroupID)
      {         
         %diff = VectorLen(VectorSub(%clientShape.position,%targ.getClientObject().position));
         echo(%this @ " checking an enemy shape! dist: " @ %diff);
         //if (%diff < %minDiff)
         //   %minDiff = %diff;
         if (%diff < %detectDist)
            return true;
      }
   }   
   return false;
}

function PhysicsShape::findPlayerPos(%this)
{ 
   %db = %this.dataBlock;//Consider getting findItemRange out of the datablock and into something else.
   initContainerRadiusSearch( %this.position, %db.findItemRange, $TypeMasks::PlayerObjectType );
      
   while ( (%item = containerSearchNext()) != 0 )
   {
      if ( %item.getClassName() $= "Player" )
      {
         %this.goalPos = %item.getPosition();  
         echo("player position: " @ %this.goalPos );          
         return;
      }
   }
   
   %this.goalPos = "0 0 0";
   return;
}



////////////////////////////////////////////////////

function PhysicsShape::relaxNeck(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("neck"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("head"),true);   
}

function PhysicsShape::relaxTorso(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("abdomen"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("chest"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("neck"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("head"),true);   
}

function PhysicsShape::relaxAll(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("abdomen"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("chest"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("neck"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("head"),true);  
    
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rCollar"),true); 
   
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lCollar"),true); 
   
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rThigh"),true); 
   
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lThigh"),true);  
}

function PhysicsShape::relaxRightArm(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rCollar"),true);  
}

function PhysicsShape::relaxLeftArm(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lCollar"),true);  
}

function PhysicsShape::relaxRightLeg(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rThigh"),true);   
}

function PhysicsShape::relaxLeftLeg(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lThigh"),true);  
}