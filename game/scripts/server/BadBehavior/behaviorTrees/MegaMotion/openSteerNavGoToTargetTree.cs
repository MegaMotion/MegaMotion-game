//--- OBJECT WRITE BEGIN ---
new Root(openSteerNavGoToTargetTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";

      new Wait() {
         waitMs = "3000";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.openSteerNavVehicle(); %obj.findTargetShapePos(); %obj.groundMove(); echo(\"starting openSteerNavGoToTargetTree.\");";
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
            new SubTree() {
               subTreeName = "idleTree";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
         };
      };
   };
};
//--- OBJECT WRITE END ---
