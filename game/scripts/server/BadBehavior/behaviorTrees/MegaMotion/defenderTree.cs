//--- OBJECT WRITE BEGIN ---
new Root(defenderTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";
      
      new ScriptEval() {
         behaviorScript = "%obj.onStartup(); %obj.openSteerNavVehicle();";//
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.findTargetPos();";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptedBehavior() {         
         preconditionMode = "ONCE";
         internalName = "goToTarget";
         class = "openSteerGoToTarget";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.setUseSteering(false);";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      
      new Loop() {
         numLoops = "0";
         terminationPolicy = "ON_FAILURE";
         canSave = "1";
         canSaveDynamicFields = "1";

         new Loop() {
            numLoops = "0";
            terminationPolicy = "ON_SUCCESS";
            canSave = "1";
            canSaveDynamicFields = "1";

            new Sequence() {
               canSave = "1";
               canSaveDynamicFields = "1";

               new Parallel() {
                  canSave = "1";
                  canSaveDynamicFields = "1";
                  returnPolicy = "REQUIRE_ALL";
                  
                  new ScriptedBehavior() {         
                     preconditionMode = "ONCE";
                     internalName = "playIdle";
                     class = "playSequence";
                     behaviorSequence = "ambient";
                     canSave = "1";
                     canSaveDynamicFields = "1";
                  };    
                  new ScriptEval() {
                     behaviorScript = "%obj.checkTeamProximity(5.0);";
                     defaultReturnStatus = "FAILURE";
                     canSave = "1";
                     canSaveDynamicFields = "1";
                  };   
               };
            };
         }; 
         new ScriptedBehavior() {         
            preconditionMode = "ONCE";
            internalName = "playPowerPunch";
            class = "playSequence";
            behaviorSequence = "power_punch_down";
            canSave = "1";
            canSaveDynamicFields = "1";
         }; 
         new ScriptEval() {
            behaviorScript = "echo(\"play power punch! looping?\");";
            defaultReturnStatus = "SUCCESS";
            canSave = "1";
            canSaveDynamicFields = "1";
         };   
      }; 
   };   
};
//--- OBJECT WRITE END ---
