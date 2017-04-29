//-----------------------------------------------------------------------------
// Copyright (c) 2017 MegaMotion Software, LLC
//-----------------------------------------------------------------------------

//--- OBJECT WRITE BEGIN ---
new Root(openSteerNavGroupTargetTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";

      new Wait() {
         waitMs = "1000";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.openSteerNavVehicle(); ";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new Loop() {
         numLoops = "0";
         terminationPolicy = "ON_SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
         
         new Sequence() {
            canSave = "1";
            canSaveDynamicFields = "1";     
            
            new ScriptEval() {
               behaviorScript = "%obj.findTargetShapePos(); ";
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
            new ScriptedBehavior() {
               preconditionMode = "TICK";
               class = "openSteerNavGoToTarget";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
            new Wait() {
               waitMs = "10";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
            //new RandomSelector(){
               //canSave = "1";
               //canSaveDynamicFields = "1";   
               //internalName = "choose find target reaction";
               
               //new ScriptedBehavior() {         
                  //preconditionMode = "ONCE";
                  //class = "playSequence";
                  //behaviorSequence = "cower";
                  //canSave = "1";
                  //canSaveDynamicFields = "1";
               //};
               //new ScriptedBehavior() {         
                  //preconditionMode = "ONCE";
                  //class = "playSequence";
                  //behaviorSequence = "jump";
                  //canSave = "1";
                  //canSaveDynamicFields = "1";
               //};
               //new ScriptedBehavior() {         
                  //preconditionMode = "ONCE";
                  //class = "playSequence";
                  //behaviorSequence = "ambient";
                  //canSave = "1";
                  //canSaveDynamicFields = "1";
               //};
            //};
            //new ScriptEval() {
            //   behaviorScript = "%obj.findRandomTargetPos();";
            //   defaultReturnStatus = "SUCCESS";
            //   canSave = "1";
            //   canSaveDynamicFields = "1";
            //};
            
         };
      };
   };
};
//--- OBJECT WRITE END ---
