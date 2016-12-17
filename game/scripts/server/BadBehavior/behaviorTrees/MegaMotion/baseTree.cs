//--- OBJECT WRITE BEGIN ---
new Root(baseTree) {
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
               waitMinMs = "1500";
               waitMaxMs = "2500";
               canSave = "1";
               canSaveDynamicFields = "1";
            };   
            new ScriptEval() {
               behaviorScript = "%obj.targetType=\"Player\";%obj.currentAction = \"runscerd\";";//Health
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            }; 
            new SubTree() {
               subTreeName = "goToTargetTree";
               internalName = "go to target";
               canSave = "1";
               canSaveDynamicFields = "1";
            };                
            new RandomWait() {
               waitMinMs = "500";
               waitMaxMs = "1500";
               canSave = "1";
               canSaveDynamicFields = "1";
            };  
            new ScriptEval() {
               behaviorScript = "%obj.targetType=\"Health\";%obj.currentAction = \"run\";";
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            }; 
            new SubTree() {
               subTreeName = "goToTargetTree";
               internalName = "go to target";
               canSave = "1";
               canSaveDynamicFields = "1";
            };    
         };
      };   
   };
};
//--- OBJECT WRITE END ---
