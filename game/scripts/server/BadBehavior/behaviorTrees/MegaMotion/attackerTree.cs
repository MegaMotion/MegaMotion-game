//--- OBJECT WRITE BEGIN ---
new Root(attackerTree) {
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
      
            //new RandomWait() {
            //   waitMinMs = "5500";
            //   waitMaxMs = "6500";
            //   canSave = "1";
            //   canSaveDynamicFields = "1";               
            //};
  
            //new ScriptEval() {
            //   behaviorScript = "%obj.setUseSteering(false);";
            //   defaultReturnStatus = "SUCCESS";
            //   canSave = "1";
            //   canSaveDynamicFields = "1";
            //};           
            new Loop() {
               numLoops = "0";
               terminationPolicy = "ON_SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
               
               new Sequence() {
                  canSave = "1";
                  canSaveDynamicFields = "1";
                               
                  new ScriptEval() {
                     behaviorScript = "%obj.findTargetPos();";
                     defaultReturnStatus = "SUCCESS";
                     canSave = "1";
                     canSaveDynamicFields = "1";
                  };
                  new ScriptedBehavior() {         
                     preconditionMode = "ONCE";
                     internalName = "goToTarget";
                     class = "openSteerGoToTarget";
                     canSave = "1";
                     canSaveDynamicFields = "1";
                  }; 
               };  
            };       
            
                
            new Loop() {
               numLoops = "1";
               terminationPolicy = "ON_FAILURE";
               canSave = "1";
               canSaveDynamicFields = "1";
               
               new Sequence() {
                  canSave = "1";
                  canSaveDynamicFields = "1";
                      
                  new ScriptedBehavior() {         
                     preconditionMode = "ONCE";
                     class = "playAttackSequence";
                     behaviorSequence = "power_punch_down";
                     canSave = "1";
                     canSaveDynamicFields = "1";
                  };   
                  
               };  
            };        
            //new SubTree() {
               //subTreeName = "fallingTree";
               //internalName = "falling";
               //canSave = "1";
               //canSaveDynamicFields = "1";
            //};
            //new RandomWait() {
               //waitMinMs = "3500";
               //waitMaxMs = "4500";
               //canSave = "1";
               //canSaveDynamicFields = "1";               
            //};  
            //new ScriptEval() {
               //behaviorScript = "%obj.setDynamic(0); %obj.orientToPosition(%object.targetShape.position); %obj.actionSeq(\"getup\");";
               //defaultReturnStatus = "SUCCESS";
               //canSave = "1";
               //canSaveDynamicFields = "1";
            //};    
            //new RandomWait() {
               //waitMinMs = "3500";
               //waitMaxMs = "4500";
               //canSave = "1";
               //canSaveDynamicFields = "1";               
            //};  
         };
      };
   };
};
//--- OBJECT WRITE END ---
