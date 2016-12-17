//--- OBJECT WRITE BEGIN ---
new Root(openSteerGoToTargetTree) {
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
         behaviorScript = "%obj.openSteerSimpleVehicle(); %obj.findTargetShapePos(); %obj.groundMove();echo(\"starting openSteerGoToTargetTree.\");";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };          
      new Loop() {
         numLoops = "0";
         terminationPolicy = "ON_FAILURE"; 
         canSave = "1";
         canSaveDynamicFields = "1";            
         
         new Sequence() {
            canSave = "1";
            canSaveDynamicFields = "1";      
            
            new ScriptedBehavior() {
               preconditionMode = "TICK";
               class = "openSteerGoToTarget";
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
