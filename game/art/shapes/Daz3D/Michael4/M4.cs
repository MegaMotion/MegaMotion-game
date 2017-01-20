singleton TSShapeConstructor(M4Dts)
{
   baseShape = "./M4.dts";
};

function M4Dts::onLoad(%this)
{
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/TPose.dsq", "tpose", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Root4.dsq", "ambient", "0", "-1");
   //%this.addSequence("art/shapes/Daz3D/Michael4/sequences/CMU_16_22.dsq", "walk", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/01_02_step_smooth.dsq", "walk", "0", "-1");
   //%this.addSequence("art/shapes/Daz3D/Michael4/work/dribble01.dsq", "walk", "0", "-1");
   //%this.addSequence("art/shapes/Daz3D/Michael4/sequences/MedRun6.dsq", "run", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/jog_02_03_three.dsq", "run", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/standing_jump.dsq", "jump", "0", "-1");

   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/runscerd1.dsq", "runscerd", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/backGetup.dsq", "backGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/frontGetup.dsq", "frontGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/rSideGetup02.dsq", "rSideGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/lSideGetup02.dsq", "lSideGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/zombiePunt2.dsq", "zombiePunt", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Mime.dsq", "mime", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Sneak.dsq", "sneak", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/SneakCrop01.dsq", "sneakCrop01", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/SneakCrop02.dsq", "sneakCrop02", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/power_punch_down.dsq", "power_punch_down", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/punch_uppercut.dsq", "punch_uppercut", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/RoundHouse.dsq", "roundhouse", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Strut.dsq", "strut", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/soldier_march.dsq", "march", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/01_13_swing_under_grcap.dsq", "swingUnder", "0", "-1");
   
   %this.addSequence("art/shapes/CMU/02/02_01.dsq", "CMU_02_01", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_02.dsq", "CMU_02_02", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_03.dsq", "CMU_02_03", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_04.dsq", "CMU_02_04", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_05.dsq", "CMU_02_05", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_06.dsq", "CMU_02_06", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_07.dsq", "CMU_02_07", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_08.dsq", "CMU_02_08", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_09.dsq", "CMU_02_09", "0", "-1");
   %this.addSequence("art/shapes/CMU/02/02_10.dsq", "CMU_02_10", "0", "-1");
   
   %this.addSequence("art/shapes/CMU/03/03_01.dsq", "CMU_03_01", "0", "-1");
   %this.addSequence("art/shapes/CMU/03/03_02.dsq", "CMU_03_02", "0", "-1");
   %this.addSequence("art/shapes/CMU/03/03_03.dsq", "CMU_03_03", "0", "-1");
   %this.addSequence("art/shapes/CMU/03/03_04.dsq", "CMU_03_04", "0", "-1");
   
   %this.addSequence("art/shapes/CMU/139/139_09.dsq", "CMU_139_09", "0", "-1");
   
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/BellyDancer.dsq", "bellyDancer", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/ACCAD_sway.dsq", "ACCAD_sway", "0", "-1");
   
   %this.addSequence("art/shapes/Daz3D/Michael4/work/third_swing.dsq", "third_swing", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/underside_climb1.dsq", "underside_climb", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/walk_06.dsq", "walk_06", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/sneak_idle.dsq", "sneak_idle", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/blockfall_01_.dsq", "blockfall_01", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/blockfall_02_.dsq", "blockfall_02", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/blockfall_03.dsq", "blockfall_03", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/blockfall_04.dsq", "blockfall_04", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/blockfall_no_grav_.dsq", "blockfall_no_grav", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/cower_01.dsq", "cower", "0", "-1");

   %this.addSequence("art/shapes/Daz3D/Michael4/work/runscerd_738.dsq", "runscerd_738", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/scene_734.dsq", "scene_734", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/scene_738.dsq", "scene_738", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/TPoseTest.dsq", "TPoseTest", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/work/newTPose.dsq", "newTPose", "0", "-1");

   %this.setSequenceCyclic("ambient", "1");
   %this.setSequenceCyclic("walk", "1");
   %this.setSequenceCyclic("strut", "1");   
   %this.setSequenceCyclic("march", "1");
   %this.setSequenceCyclic("run", "1");
   %this.setSequenceCyclic("runscerd", "1");
   %this.setSequenceCyclic("tpose", "1");
   %this.setSequenceCyclic("swingUnder", "1");
   %this.setSequenceCyclic("cower", "1");
   %this.setSequenceCyclic("jump", "1");
      
   %this.addNode("Col-1","root");
   %this.addCollisionDetail(-1,"box","bounds");   
} 
