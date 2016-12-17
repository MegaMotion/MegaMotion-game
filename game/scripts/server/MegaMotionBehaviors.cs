


//==============================================================================
// Play a sequence, waiting until it's done before handing back control.
//==============================================================================
function playSequence::precondition(%this, %obj)
{
   return (strlen(%this.behaviorSequence)>0); 
}

function playSequence::onEnter(%this, %obj)
{   
   %obj.playSeq(%this.behaviorSequence);  
   %obj.seqDurMS = %obj.getSeqDuration(%obj.getSeqNum(%this.behaviorSequence)) * 1000;
   %obj.startTime = getSimTime();
}

function playSequence::behavior(%this, %obj)
{
   if ((getSimTime()-%obj.startTime) > %obj.seqDurMS)
      return SUCCESS;
   else
      return RUNNING;
}

//==============================================================================
// Play an action sequence, waiting until it's done before handing back control.
//==============================================================================
function playActionSequence::precondition(%this, %obj)
{
   //echo("testing playActionSequence! seq " @ %this.behaviorSequence @ " action seq " @ %obj.getActionSeq(%this.behaviorSequence) );
   return ((strlen(%this.behaviorSequence)>0)&&(%obj.getActionSeq(%this.behaviorSequence)>=0)); 
}

function playActionSequence::onEnter(%this, %obj)
{   
   //echo("entering playActionSequence! seq " @ %this.behaviorSequence);
   %obj.actionSeq(%this.behaviorSequence);  
   %seqIndex = %obj.getActionSeq(%this.behaviorSequence);
   %obj.seqDurMS = %obj.getSeqDuration(%seqIndex) * 1000;
   %obj.startTime = getSimTime();
}

function playActionSequence::behavior(%this, %obj)
{
   //echo("running playActionSequence! time remaining " @ (%obj.seqDurMS - (getSimTime()-%obj.startTime)));
   if ((getSimTime()-%obj.startTime) > %obj.seqDurMS)
      return SUCCESS;
   else
      return RUNNING;
}


//==============================================================================
// Play an attack sequence, if we have a target in range.
//==============================================================================
function playAttackSequence::precondition(%this, %obj)
{
   %result = true;
   %diff = "0 0 0";
   //echo("attackSequence precondition: targetItem " @ %obj.targetItem @ " shapeId " @ %obj.targetShapeID);
   if (%obj.targetItem)
   {
      %diff = VectorSub(%obj.getClientObject().position,%obj.targetItem.position);
      //echo("attackSequence distance to targetItem: " @ VectorLen(%diff) @ " targetPos " @ %obj.targetItem.position @ " foundItem " @ %obj.foundItemDistance);
      if (VectorLen(%diff)>%obj.dataBlock.foundItemDistance) //HERE: using foundItemDistance just to get it working, but
         %result = false;  // we need to store an attack range for each attack anim. *Not* a weapon property,
                           // because a defensive slash has a different range than an all out thrust. 
   }
   
   if (strlen(%this.behaviorSequence)<=0)
      %result = false;
   
   return %result; 
}

function playAttackSequence::onEnter(%this, %obj)
{   
   %obj.orientToPos(%obj.targetItem.position);
   %obj.playSeq(%this.behaviorSequence);  
   %obj.seqDurMS = %obj.getSeqDuration(%obj.getSeqNum(%this.behaviorSequence)) * 1000;
   %obj.startTime = getSimTime();
}

function playAttackSequence::behavior(%this, %obj)
{
   //HERE: we can add logic to cause damage, or create missiles, at a certain time, per anim.   
   
   if ((getSimTime()-%obj.startTime) > %obj.seqDurMS)
      return SUCCESS;
   else
      return RUNNING;
}

///////////////////////////////////
//getUp::precondition()
//getUp::onEnter()
//getUp::onExit()

function getUp::behavior(%this, %obj)
{
   %obj.actionSeq("getup");
   return SUCCESS;
}


////////////////////////////////////////////

//=============================================================================
// goToTarget task
//=============================================================================
function goToTarget::precondition(%this, %obj)
{
   return (VectorLen(%obj.goalPos)>0);
}

function goToTarget::onEnter(%this, %obj)
{
   echo("goToTarget::onEnter");
   
   %obj.currentAction = "run";
   
   %obj.moveTo(%obj.findGroundPosition(%obj.goalPos));  
}

function goToTarget::behavior(%this, %obj)
{
   echo("goToTarget::behavior");
   // succeed when we reach the item  
   %clientGroundPos = %obj.findGroundPosition(%obj.getClientPosition());
   %diff = VectorSub(%obj.goalPos,%clientGroundPos);

   if ( VectorLen(%diff) > %obj.dataBlock.foundItemDistance )
   {      
      return RUNNING;
   }
   else
   {
      %obj.actionSeq("idle");
      return SUCCESS;
   }
}

//=============================================================================
// navGoToTarget task
//=============================================================================
function navGoToTarget::precondition(%this, %obj)
{
   return (VectorLen(%obj.goalPos)>0);
}

function navGoToTarget::onEnter(%this, %obj)
{
   %obj.currentAction = "run";
   %obj.setNavMesh("Nav");
   echo("navGoToTarget::onEnter, goalPos " @ %obj.goalPos);
   
   if (%obj.currentPathNode==0)//Well this is weird, this time it's calling onEnter all the time...??
   {
      %obj.setNavPathTo(%obj.goalPos);
   
      %obj.currentPathNode = 1;
      %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      echo("openSteerGoToTarget, onEnter, first goal: " @ %obj.currentPathGoal @ 
               " ultimate target " @ %obj.targetItem.position);
      %obj.moveTo(%obj.currentPathGoal);
      //%clientObj.setMoveTarget(%obj.currentPathGoal);

   }
}

function navGoToTarget::behavior(%this, %obj)
{
   // succeed when we reach the item
   //HERE: we need targetitem position to be on the ground, not at the actual position, 
   //or else we can never be closer than the height of the object.
  
   %clientObj = 0;
   if (%obj.isServerObject())
   {
      %clientObj =  %obj.getClientObject();
   }
   
   %groundPos = %obj.findGroundPosition(%obj.goalPos);
   //%targetMove = VectorLen(%groundPos - %obj.getNavPathNode(%obj.getNavPathSize()-1));
   %clientGroundPos = %obj.findGroundPosition(%obj.getClientPosition());//NOTE: this "client" refers to the player
   %diff = VectorSub(%groundPos,%clientGroundPos);//Where "clientObj" above refers to client ghost of this shape.
   //if(!%obj.atDestination)
   
   //if ((%obj.currentPathNode == 0)||(%targetMove>2.0))//2.0=%obj.targetMoveThreshold?
   //{    
      //echo("setting NavPathTo, pathNode=0");
   //   %obj.setNavPathTo(%groundPos);   
   //   %obj.currentPathNode = 1;
   //   %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      //echo("New Target, first goal: " @ %obj.currentPathGoal);
   //   %obj.moveTo(%obj.currentPathGoal);   
   //}
   if ( VectorLen(%diff) > %obj.dataBlock.foundItemDistance )
   {      
      //First, remove z axis for purposes of finding proximity.
      %pathGoal = getWord(%obj.currentPathGoal,0) @ " " @ getWord(%obj.currentPathGoal,1) @ " 0";
      %clientPos = getWord(%clientGroundPos,0) @ " " @ getWord(%clientGroundPos,1) @ " 0";
      //%nodeDiff = VectorSub(%obj.currentPathGoal,%clientGroundPos);
      %nodeDiff = VectorSub(%pathGoal,%clientPos);
      echo("checking distance to path node: " @ VectorLen(%nodeDiff) @ " my pos " @ %clientGroundPos @ " target pos " @ %obj.currentPathGoal );
      //echo(%obj.getId() @ " is looking for target, my position " @ %clientGroundPos @ 
      //      " target position " @ %obj.currentPathGoal @  " distance = " @ VectorLen(%diff) );

      if (VectorLen(%nodeDiff) < %obj.dataBlock.foundItemDistance)
      {
         %obj.currentPathNode++;
         %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
         
         //echo("setting new path goal: " @ %obj.currentPathGoal @ "  current node " @ %obj.currentPathNode @
         //          " total nodes " @ %obj.getNavPathSize() );
         %obj.moveTo(%obj.currentPathGoal); 
      }
      return RUNNING;
   }
   else
   {
      %obj.currentPathNode = 0;
      %obj.actionSeq("idle");
      return SUCCESS;
   }
}


//=============================================================================
// openSteerGoToTarget task
//=============================================================================
function openSteerGoToTarget::precondition(%this, %obj)
{
   return (VectorLen(%obj.goalPos)>0);
}

function openSteerGoToTarget::onEnter(%this, %obj)
{
   if (%obj.currentPathNode==0)//Well this is weird, this time it's calling onEnter all the time...??
   {
      if (%obj.getVehicleID()==0)
         %obj.openSteerNavVehicle();
      %obj.setUseSteering(true);
      
      %obj.setNavPathTo(%obj.goalPos);
   
      %obj.currentPathNode = 1;
      %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      //echo("openSteerGoToTarget, onEnter, first goal: " @ %obj.currentPathGoal @ 
      //         " ultimate target " @ %obj.targetItem.position);
      %clientObj = %obj.getClientObject();
      %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
      %clientObj.setUseSteering(true);
   }
}

function openSteerGoToTarget::behavior(%this, %obj)
{
   // succeed when we reach the item
   //HERE: we need targetitem position to be on the ground, not at the actual position, 
   //or else we can never be closer than the height of the object.
  
   %clientObj = 0;
   if (%obj.isServerObject())
   {
      %clientObj =  %obj.getClientObject();
   }
   
   %groundPos = %obj.findGroundPosition(%obj.goalPos);
   %targetMove = VectorLen(%groundPos - %obj.getNavPathNode(%obj.getNavPathSize()-1));
   %clientGroundPos = %obj.findGroundPosition(%obj.getClientPosition());//NOTE: this "client" refers to the player
   %diff = VectorSub(%groundPos,%clientGroundPos);//Where "clientObj" above refers to client ghost of this shape.
   //if(!%obj.atDestination)   
   //echo("my position " @ %obj.getClientPosition() @ " goal " @ %obj.currentPathGoal @ 
   //      " diff " @ VectorLen(%diff)  @  "  target move " @ %targetMove @ " target pos " @ 
   //       %obj.targetItem.position );
   
   if ((%obj.currentPathNode == 0)||(%targetMove>2.0))//2.0=%obj.targetMoveThreshold?
   {    
      //echo("setting NavPathTo, pathNode=0");
      %obj.setNavPathTo(%groundPos);   
      %obj.currentPathNode = 1;
      %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      //echo("New Target, first goal: " @ %obj.currentPathGoal);
      //%obj.moveTo(%obj.currentPathGoal);   
      %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
   }
   if ( VectorLen(%diff) > %obj.dataBlock.foundItemDistance )
   {      
      %nodeDiff = VectorSub(%obj.currentPathGoal,%clientGroundPos);
      //echo("checking distance to path node: " @ VectorLen(%nodeDiff) );
      //echo(%obj.getId() @ " is looking for target, my position " @ %clientGroundPos @ 
      //      " target position " @ %obj.currentPathGoal @  " distance = " @ VectorLen(%diff) );

      if (VectorLen(%nodeDiff) < %obj.dataBlock.foundItemDistance)
      {
         %obj.currentPathNode++;
         %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
         
         //echo("setting new path goal: " @ %obj.currentPathGoal @ "  current node " @ %obj.currentPathNode @
         //          " total nodes " @ %obj.getNavPathSize() );
         //%obj.moveTo(%obj.currentPathGoal); 
         %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
      }
      return RUNNING;
   }
   else
   {
      %obj.currentPathNode = 0;
      //%obj.currentPathGoal = %obj.getClientPosition();
      %obj.actionSeq("ambient");
      %obj.setOpenSteerSpeed(0.0);//HERE: I think we need to do more, this doesn't stop it from thinking.
      %obj.setUseSteering(false);
      return SUCCESS;
   }
}


//=============================================================================
// openSteerNavGoToTarget task
//=============================================================================
function openSteerNavGoToTarget::precondition(%this, %obj)
{
   //echo(%obj.sceneShapeID @ " openSteerNavGoToTarget::precondition - goalPos " @ %obj.goalPos @ 
   //      " len " @ VectorLen(%obj.goalPos) @ " currentGoal " @ %obj.currentPathGoal);
         
   return (VectorLen(%obj.goalPos)>0);
}

function openSteerNavGoToTarget::onEnter(%this, %obj)
{
   //echo(%obj.sceneShapeID @ " openSteerNavGoToTarget::onEnter, currentGoal " @ %obj.currentPathGoal);
   %clientObj = %obj.getClientObject();
   if (%obj.currentPathNode==0)//Well this is weird, this time it's calling onEnter all the time...??
   {
      if (%clientObj.getVehicleID()==0)
         %clientObj.openSteerNavVehicle();
         
      %obj.setUseSteering(true);
      
      %obj.setNavPathTo(%obj.goalPos);
   
      %obj.currentPathNode = 1;
      %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      //echo("setting new path goal: " @ %obj.currentPathGoal @ "  current node " @ %obj.currentPathNode @
      //             " total nodes " @ %obj.getNavPathSize() );
      
      %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
      %clientObj.setUseSteering(true);
   }
}

function openSteerNavGoToTarget::behavior(%this, %obj)
{
   //echo("openSteerNavGoToTarget::behavior");
   // succeed when we reach the item
   
   %clientObj = 0;
   if (%obj.isServerObject())
   {
      %clientObj =  %obj.getClientObject();
   }
   //if (%obj.targetShapeID>0)
   
   //%groundPos = %obj.findGroundPosition(%obj.goalPos);
   %currentTargPos = %obj.findGroundPosition(%obj.targetItem.getPosition());
   %moveDiff = VectorSub(%obj.goalPos,%currentTargPos);
   
   %clientGroundPos = %obj.findGroundPosition(%obj.getClientPosition());//Note: getClientPosition refers to player
   %finalDiff = VectorSub(%obj.findGroundPosition(%clientObj.getPosition()),%currentTargPos);
   
   if (VectorLen(%moveDiff)>(%obj.dataBlock.foundItemDistance*2))
   {    
      %obj.setNavPathTo(%currentTargPos);   
      %obj.currentPathNode = 1;
      %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
      return RUNNING;
   }
   if ( VectorLen(%finalDiff) > %obj.dataBlock.foundItemDistance )
   {
      %nodeDiff = VectorSub(%obj.currentPathGoal,%clientGroundPos);
      //echo("checking distance to path node: " @ VectorLen(%nodeDiff) );
      //echo(%obj.getId() @ " is looking for target, my position " @ %clientGroundPos @ 
      //      " target position " @ %obj.currentPathGoal @  " distance = " @ VectorLen(%diff) );

      if (VectorLen(%nodeDiff) < %obj.dataBlock.foundItemDistance)
      {
         %obj.currentPathNode++;
         %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode-1);
         
         //%obj.moveTo(%obj.currentPathGoal); 
         %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
      }
      return RUNNING;
   }
   else
   {
      %obj.currentPathNode = 0;
      //%obj.currentPathGoal = %obj.getClientPosition();
      //%obj.actionSeq("ambient");//Here: change to idle tree? No, play idle action while waiting for target to move away again.
      %obj.setOpenSteerSpeed(0.0);//HERE: I think we need to do more, this doesn't stop it from thinking.
      %obj.setUseSteering(false);
      return SUCCESS;
   }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////

//  [Currently obsoleted by findTargetShapePos() function]
//=============================================================================
// findTarget task - same as goToTarget, but calling it didn't work...?
//=============================================================================
function openSteerFindTarget::behavior(%this, %obj)
{
   // get the objects datablock
   %db = %obj.dataBlock;
   %category = %obj.targetType;

   //HERE: need to deal with A) known sceneShape target_shapes, B) shapeGroups, and C) goalPos
   
   if (%obj.targetShapeID>0)
   {
      for (%i = 0; %i < SceneShapes.getCount();%i++)
      {
         %targ = SceneShapes.getObject(%i);  
         if (%targ.sceneShapeID==%obj.targetShapeID)
         {
            %obj.targetItem = %targ.getClientObject();
            echo(%obj @ " trying to assign target scene shape: " @ %targ @ " isServer " @ %targ.isServerObject());
            return SUCCESS;  
         }
      }
   }
   

   //TEMP, till we figure out a better plan.
   if ((strlen(%category)==0)||(%category $= "Health"))
      %category = "Player";
   else if (%category $= "Player")
      %category = "Health";
      
   //echo(%this.getId() @ " trying to find target: " @ %obj.targetType @ "pos " @ %obj.position);
   // do a container search for items
   
   //HERE: let's use category to switch between player and item searches. This could get more involved.
   if (%category $= "Player")
      initContainerRadiusSearch( %obj.position, %db.findItemRange, %db.targetObjectTypes );
   else//if not player, assume an item, but more (esp. PhysicsShape) categories could be added.
      initContainerRadiusSearch( %obj.position, %db.findItemRange, %db.itemObjectTypes );
   
   while ( (%item = containerSearchNext()) != 0 )
   {
      if ( (%category$="Player") ||
            (%item.dataBlock.category $= %category && %item.isEnabled() && !%item.isHidden()) )
      {      
         %diff = VectorSub(%obj.position,%item.position);
      
         // check that the item is within the bots view cone
         //if(%obj.checkInFov(%item, %db.visionFov))
         if (true)// (We don't have a checkInFov for physicsShapes yet)
         {
            // set the targetItem field on the bot
            %obj.targetItem = %item;
            //echo("FOUND TARGET: " @ %item  @ "  " @ %item.getClassName() @ "  " @ %item.getPosition() );
            break;
         }
      }
   }
   
   return isObject(%obj.targetItem) ? SUCCESS : FAILURE;
}


//  [Currently obsoleted by findTargetShapePos() function]
//=============================================================================
// findTarget task
//=============================================================================
function findTarget::behavior(%this, %obj)
{
   // get the objects datablock
   %db = %obj.dataBlock;
   %category = %obj.targetType;
   //echo(%this.getId() @ " trying to find target: " @ %obj.targetType @ "pos " @ %obj.position);
   // do a container search for items
   
   //HERE: let's use category to switch between player and item searches. This could get more involved.
   if (%category $= "Player")
      initContainerRadiusSearch( %obj.position, %db.findItemRange, %db.targetObjectTypes );
   else//if not player, assume an item, but more (esp. PhysicsShape) categories could be added.
      initContainerRadiusSearch( %obj.position, %db.findItemRange, %db.itemObjectTypes );
   
   while ( (%item = containerSearchNext()) != 0 )
   {
      if ( (%category$="Player") ||
            (%item.dataBlock.category $= %category && %item.isEnabled() && !%item.isHidden()) )
      {      
         %diff = VectorSub(%obj.position,%item.position);
      
         // check that the item is within the bots view cone
         //if(%obj.checkInFov(%item, %db.visionFov))
         if (true)// (We don't have a checkInFov for physicsShapes yet)
         {
            // set the targetItem field on the bot
            %obj.targetItem = %item;
            //echo("FOUND TARGET: " @ %item  @ "  " @ %item.getClassName() @ "  " @ %item.getPosition() );
            break;
         }
      }
   }
   
   return isObject(%obj.targetItem) ? SUCCESS : FAILURE;
}
