//--- OBJECT WRITE BEGIN ---
new Root(testTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";      
      
      new ScriptEval() {
         behaviorScript = "echo(\"STARTING UP\");";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };   
      new ScriptEval() {
         behaviorScript = "%obj.onStartup();";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.openSteerVehicle();";
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
               waitMinMs = "1500";
               waitMaxMs = "2500";
               canSave = "1";
               canSaveDynamicFields = "1";
            };   
            new ScriptEval() {
               behaviorScript = "echo(\"Still working...\");";
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            };   
         };
      };
   };
};
//--- OBJECT WRITE END ---
