//--- OBJECT WRITE BEGIN ---
new Root(vehicleTree) {
   canSave = "1";
   canSaveDynamicFields = "1";


   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";      
      
      new ScriptEval() {
         behaviorScript = "%obj.onStartup();";
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
            
            new RandomWait() {
               waitMinMs = "5000";
               waitMaxMs = "7000";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
         };
      };
   };
};
//--- OBJECT WRITE END ---
