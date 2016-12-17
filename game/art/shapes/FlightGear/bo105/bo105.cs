
singleton TSShapeConstructor(Bo105_Dts)
{
   baseShape = "./bo105.dts";
};

function Bo105_Dts::onLoad(%this)
{
   %this.addNode("Col-1", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBoxL-7_bo", "Col-1", "0.113031 0.879284 -0.158876 1 0 -1.9961e-011 2.27285", "0");
   //%this.addNode("ColBoxP-1_bo", "Col-1", "0.030669 0.33929 1.81498 0.577572 0.576192 0.578285 4.18705", "0");
   %this.addCollisionDetail("-1", "Sphere", "Bounds", "4", "30", "30", "32", "30", "30", "30");
   //%this.addSequence("art/shapes/fg_convert/bo105/test01.dsq", "ambient", "0", "-1");
   //%this.setSequenceCyclic("ambient", "1");
   //%this.addSequence("art/shapes/fg_convert/bo105/test02.dsq", "test02", "0", "-1");
   //%this.setSequenceCyclic("test02", "1");   
}
