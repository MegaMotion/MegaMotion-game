//--- OBJECT WRITE BEGIN ---
new Root(baitTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";

      new ScriptEval() {
         behaviorScript = "%obj.onStartup();  %obj.openSteerNavVehicle();";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new Sequence() {
         canSave = "1";
         canSaveDynamicFields = "1";

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
         new Loop() {
            numLoops = "5";
            terminationPolicy = "ON_FAILURE";
            canSave = "1";
            canSaveDynamicFields = "1";            
            
            new Sequence() {
               canSave = "1";
               canSaveDynamicFields = "1";

               new ScriptedBehavior() {         
                  preconditionMode = "ONCE";
                  internalName = "playpowerpunch";
                  class = "playSequence";
                  behaviorSequence = "power_punch_down";
                  canSave = "1";
                  canSaveDynamicFields = "1";
               };  
               //new ScriptEval() {
               //   behaviorScript = "echo(\"Playing bellydancer loop\");";
               //   defaultReturnStatus = "SUCCESS";
               //   canSave = "1";
               //   canSaveDynamicFields = "1";
               //};
            };
         };
         new ScriptedBehavior() {         
            preconditionMode = "ONCE";
            internalName = "playbellydancer";
            class = "playSequence";
            behaviorSequence = "bellyDancer";
            canSave = "1";
            canSaveDynamicFields = "1";
         };  
      };
   };
};
//--- OBJECT WRITE END ---
