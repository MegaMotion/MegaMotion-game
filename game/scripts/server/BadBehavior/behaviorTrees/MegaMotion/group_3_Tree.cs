//-----------------------------------------------------------------------------
// Copyright (c) 2017 MegaMotion Software, LLC
//-----------------------------------------------------------------------------

new Root(group_3_Tree) {
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
   
         new Wait() {
            waitMs = "10";
            canSave = "1";
            canSaveDynamicFields = "1";
         };     
         
         new ScriptedBehavior() {
            preconditionMode = "TICK";
            class = "group_C_think";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
      };
   };
};