
singleton TSShapeConstructor(TuberocketDts)
{
   baseShape = "./tuberocket.dts";
};

function TuberocketDts::onLoad(%this)
{
   %this.addNode("Col-1", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBox-1", "Col-1", "-8.83361e-007 -0.00115578 0.000437777 0.999986 0 -0.00533435 0.362078", "0");
   %this.addCollisionDetail("-1", "Convex Hulls", "Bounds", "4", "30", "30", "32", "30", "30", "30");
}
