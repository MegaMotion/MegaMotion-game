//--- OBJECT WRITE BEGIN ---
new Root(goToTargetTree) {
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
               class = "goToTarget";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
         };
      };
   };
};
//--- OBJECT WRITE END ---
