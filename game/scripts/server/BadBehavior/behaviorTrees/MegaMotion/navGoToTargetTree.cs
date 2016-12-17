//--- OBJECT WRITE BEGIN ---
new Root(navGoToTargetTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";

      new Wait() {
         waitMs = "500";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.findTargetShapePos(); %obj.groundMove();";
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
               class = "navGoToTarget";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
         };
      };
   };
};
//--- OBJECT WRITE END ---
