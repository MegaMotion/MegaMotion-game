
singleton TSShapeConstructor(BladeDts)
{
   baseShape = "./blade.dts";
};

function BladeDts::onLoad(%this)
{
   %this.addNode("Col-1", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBoxB-1", "Col-1", "0.266464 0.00124388 0.00143682 0.999965 0 0.00832705 1.52692", "0");
   %this.addCollisionDetail("-1", "Convex Hulls", "Bounds", "4", "30", "30", "32", "30", "30", "30");
}
