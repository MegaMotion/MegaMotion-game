//--- OBJECT WRITE BEGIN ---
new Root(openSteerNavRandomTargetTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";

      new Wait() {
         waitMs = "10000";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.openSteerNavVehicle(); %obj.findRandomTargetPos();";
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
            
            new ScriptedBehavior() {
               preconditionMode = "TICK";
               class = "openSteerNavGoToTarget";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
            new ScriptEval() {
               behaviorScript = "%obj.findRandomTargetPos();";
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
         };
      };
   };
};
//--- OBJECT WRITE END ---
