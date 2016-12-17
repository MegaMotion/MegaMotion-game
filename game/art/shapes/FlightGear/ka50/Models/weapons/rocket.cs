
singleton TSShapeConstructor(RocketDts)
{
   baseShape = "./rocket.dts";
};

function RocketDts::onLoad(%this)
{
   %this.addNode("Col-1", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBoxG-1", "Col-1", "1.02663 9.40278e-011 -2.32258e-010 -1 0 -9.1589e-011 1.5708", "0");
   %this.addNode("ColBoxH-1", "Col-1", "1.02663 -2.33574e-010 1.3604e-010 0.999998 0 0.00179807 0", "0");
   %this.addCollisionDetail("-1", "Convex Hulls", "Bounds", "4", "30", "30", "32", "30", "30", "30");
}
