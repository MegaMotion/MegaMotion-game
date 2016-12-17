


singleton Material(ka50_prop_texture)
{
   mapTo = "ka50_prop";
   diffuseMap[0] = "art/shapes/FlightGear/ka50/Models/MainRotor/prop.png";
   specular[0] = "0.32 0.32 0.32 1";
   specularPower[0] = "50";
   doubleSided = "1";
   translucentBlendOp = "None";
   alphaTest = "1";
   alphaRef = "20";
};

singleton Material(ka50_rotor_texture)
{
   mapTo = "ka50_rotor";
   diffuseMap[0] = "art/shapes/FlightGear/ka50/Models/MainRotor/colors.png";
   specular[0] = "0.32 0.32 0.32 1";
   specularPower[0] = "50";
   translucentBlendOp = "None";
   materialTag0 = "Miscellaneous";
};
