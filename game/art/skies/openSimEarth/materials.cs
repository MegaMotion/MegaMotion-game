//-----------------------------------------------------------------------------
// Copyright (c) 2016 Chris Calef

singleton CubemapData( OSE_SkyboxCubemap )
{
   cubeFace[0] = "./skybox_90";//RIGHT = EAST
   cubeFace[1] = "./skybox_270";//LEFT = WEST
   cubeFace[2] = "./skybox_00";//FORWARD = NORTH
   cubeFace[3] = "./skybox_180";//BACKWARD = SOUTH
   cubeFace[4] = "./skybox_up";//UP
   cubeFace[5] = "./skybox_down";//DOWN
};

singleton Material( OSE_SkyboxMaterial )
{
   cubemap = OSE_SkyboxCubemap;
   materialTag0 = "Skies";
};
