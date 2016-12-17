//--- OBJECT WRITE BEGIN ---
new Root(FallingTree) {
   canSave = "1";
   canSaveDynamicFields = "1";


   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";      

      new Sequence() {
         canSave = "1";
         canSaveDynamicFields = "1";      
         
         new ScriptEval() {
            behaviorScript = "echo(\"Falling Tree!\");";
            defaultReturnStatus = "SUCCESS";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
         new RandomWait() {
            waitMinMs = "3000";
            waitMaxMs = "3500";
            canSave = "1";
            canSaveDynamicFields = "1";
         };         
         new ScriptEval() {
            behaviorScript = "%obj.setDynamic(0);";
            defaultReturnStatus = "SUCCESS";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
         new RandomWait() {
            waitMinMs = "30000";
            waitMaxMs = "35000";
            canSave = "1";
            canSaveDynamicFields = "1";
         };    
      };      
   };
};
//--- OBJECT WRITE END ---
