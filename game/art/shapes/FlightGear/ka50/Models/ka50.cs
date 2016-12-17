
singleton TSShapeConstructor(ka50_Dts)
{
   baseShape = "./ka50.dts";
};

function ka50_Dts::onLoad(%this)
{
   //%this.addMesh("mainrotor","art/shapes/fg_convert/ka50/Models/MainRotor/MainRotor.dts","mainrotor 2");
   //%this.addMesh("partrotorB1","art/shapes/fg_convert/ka50/Models/MainRotor/MainRotor.dts","partrotorB1 2");
   //%this.addMesh("partrotorB2","art/shapes/fg_convert/ka50/Models/MainRotor/MainRotor.dts","partrotorB2 2");
   %this.addNode("Col-1_ka", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBoxL-7_ka", "Col-1_ka", "0.113031 0.879284 -0.158876 1 0 -1.9961e-011 2.27285", "0");
   //%this.addNode("ColBoxP-1_bo", "Col-1", "0.030669 0.33929 1.81498 0.577572 0.576192 0.578285 4.18705", "0");
   %this.addCollisionDetail("-1", "Sphere", "Bounds", "4", "30", "30", "32", "30", "30", "30");
   %this.addNode("rotormount", "", "0.168 0 0.973 0 0 1 0", "0");
   
   %this.addSequence("art/shapes/FlightGear/ka50/Models/sequences/15_611.dsq", "flight", "0", "-1");
   %this.setSequenceCyclic("flight", "1");
   //%this.addSequence("art/shapes/fg_convert/bo105/test02.dsq", "test02", "0", "-1");
   //%this.setSequenceCyclic("test02", "1");   
}