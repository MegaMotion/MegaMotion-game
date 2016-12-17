
singleton Material(SplitRock_01_SR_TOP)
{
   mapTo = "SR_TOP";
   diffuseColor[0] = "0.145098 0.145098 0.145098 1";
   specular[0] = "0.9 0.9 0.9 1";
   specularPower[0] = "35";
   translucentBlendOp = "None";
   pixelSpecular[0] = "1";
   useAnisotropic[0] = "1";
};

singleton Material(SplitRock_01_SR_Brick)
{
   mapTo = "SR_Brick";
   diffuseMap[0] = "3td_WhiteBrick_02";
   specular[0] = "0.9 0.9 0.9 1";
   specularPower[0] = "64";
   translucentBlendOp = "None";
   normalMap[0] = "3td_WhiteBrick_02_NRM.png";
   pixelSpecular[0] = "1";
   useAnisotropic[0] = "1";
};

singleton Material(SplitRock_01_SR_Glass)
{
   mapTo = "SR_Glass";
   diffuseColor[0] = "0.709804 0.894118 0.984314 0.183";
   specularPower[0] = "1";
   translucentBlendOp = "LerpAlpha";
   pixelSpecular[0] = "1";
   useAnisotropic[0] = "1";
   doubleSided = "1";
   translucent = "1";
   alphaRef = "208";
   specular[0] = "0.996078 0.996078 0.996078 0.299";
   cubemap = "NewLevelSkyCubemap";
};

singleton Material(SplitRock_01_SR_Door)
{
   mapTo = "SR_Door";
   diffuseMap[0] = "door01";
   specular[0] = "0.9 0.9 0.9 1";
   specularPower[0] = "10";
   translucentBlendOp = "None";
   normalMap[0] = "door01_NRM.png";
   useAnisotropic[0] = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "50";
};

singleton Material(SplitRock_01_SR_White)
{
   mapTo = "SR_White";
   diffuseMap[0] = "3td_Stone_08";
   specular[0] = "0.9 0.9 0.9 1";
   specularPower[0] = "10";
   translucentBlendOp = "None";
   normalMap[0] = "3td_Stone_08_NRM.png";
   useAnisotropic[0] = "1";
};
