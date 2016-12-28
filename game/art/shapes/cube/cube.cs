
singleton TSShapeConstructor(CubeDts)
{
   baseShape = "./cube.dts";
};

function CubeDts::onLoad(%this)
{
   %this.addNode("Col-1", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBox-1", "Col-1", "0 3.55271e-15 0.5 -1 0 0 1.5708", "0");
   %this.addCollisionDetail("-1", "Box", "Bounds", "4", "30", "30", "32", "30", "30", "30");
}
