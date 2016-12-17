
singleton TSShapeConstructor(SkyscraperDts)
{
   baseShape = "./skyscraper.dts";
};

function SkyscraperDts::onLoad(%this)
{
   %this.addNode("Col-1", "", "0 0 0 0 0 1 0", "0");
   %this.addNode("ColBoxB-1", "Col-1", "24.175 27.78 138.7 0.572594 -0.585918 -0.573442 2.08418", "1");
   %this.addCollisionDetail("-1", "Convex Hulls", "SketchUpB", "4", "30", "30", "32", "30", "30", "30");
}
