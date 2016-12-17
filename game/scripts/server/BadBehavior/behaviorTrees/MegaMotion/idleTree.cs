new Root(idleTree) {
   canSave = "1";
   canSaveDynamicFields = "1";


   new Loop() {
      numLoops = "0";
      terminationPolicy = "ON_FAILURE";
      canSave = "1";
      canSaveDynamicFields = "1";
         
      new Sequence() {
         canSave = "1";
         canSaveDynamicFields = "1";
   
         new ScriptedBehavior() {         
            preconditionMode = "ONCE";
            class = "playActionSequence";
            behaviorSequence = "idle";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
      };
   };
};