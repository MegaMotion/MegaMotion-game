new Root(playSequencesTree) {
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
            preconditionMode = "TICK";
            class = "playSequence";
            behaviorSequence = "TPose_2_march";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
         
         new Loop() {
            numLoops = "2";
            terminationPolicy = "ON_FAILURE";
            canSave = "1";
            canSaveDynamicFields = "1";
            
            new Sequence() {
               canSave = "1";
               canSaveDynamicFields = "1";
                   
               new ScriptedBehavior() {
                  preconditionMode = "TICK";
                  class = "playSequence";
                  behaviorSequence = "march";
                  canSave = "1";
                  canSaveDynamicFields = "1";
               };
            };
         };
      };
   };
};