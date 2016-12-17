//--- OBJECT WRITE BEGIN ---
new Root(dropAndRoll) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Loop() {
      numLoops = "0";
      terminationPolicy = "ON_FAILURE";
      canSave = "1";
      canSaveDynamicFields = "1";
      
      new ScriptEval() {
         behaviorScript = "%obj.actionSeq(\"walk\");";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new RandomWait() {
         waitMinMs = "4000";
         waitMaxMs = "5000";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.actionSeq(\"attack\");";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new RandomWait() {
         waitMinMs = "3000";
         waitMaxMs = "4000";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
   };
};
//--- OBJECT WRITE END ---
