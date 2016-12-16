
datablock PhysicsShapeData( PSCube )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/cube/cube.dts";
   emap = 1;
   //simType = 2;
   
   shapeID = 11;  
   mass = "0.5";
   massCenter = "0 0 0.5";      // Center of mass for rigid body
   massBox = "1 1 1";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.2;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
};