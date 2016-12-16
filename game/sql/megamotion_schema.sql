----------------- MEGAMOTION DATABASE SCHEMA -----------------


-- Table: uiBitmap
CREATE TABLE uiBitmap ( 
    id   INTEGER PRIMARY KEY AUTOINCREMENT
                 NOT NULL
                 UNIQUE,
    path TEXT    NOT NULL 
);

-- Table: uiElement
CREATE TABLE uiElement ( 
    id                 INTEGER PRIMARY KEY AUTOINCREMENT
                               NOT NULL
                               UNIQUE,
    form_id            INTEGER DEFAULT ( 0 ),
    parent_id          INTEGER DEFAULT ( 0 ),
    left_anchor        INTEGER DEFAULT ( 0 ),
    right_anchor       INTEGER DEFAULT ( 0 ),
    top_anchor         INTEGER DEFAULT ( 0 ),
    bottom_anchor      INTEGER DEFAULT ( 0 ),
    type               TEXT    DEFAULT ( '' ),
    name               TEXT    DEFAULT ( '' ),
    content            TEXT    DEFAULT ( '' ),
    value              TEXT    DEFAULT ( '' ),
    bitmap_id          INTEGER DEFAULT ( 0 ),
    command            TEXT    DEFAULT ( '' ),
    alt_command        TEXT    DEFAULT ( '' ),
    variable           TEXT    DEFAULT ( '' ),
    tooltip            TEXT    DEFAULT ( '' ),
    button_type        TEXT    DEFAULT ( '' ),
    profile            TEXT    DEFAULT ( '' ),
    group_num          INTEGER DEFAULT ( -1 ),
    width              INTEGER DEFAULT ( 40 ),
    height             INTEGER DEFAULT ( 20 ),
    horiz_align        INTEGER DEFAULT ( 0 ),
    vert_align         INTEGER DEFAULT ( 0 ),
    pos_x              REAL    DEFAULT ( 0 ),
    pos_y              REAL    DEFAULT ( 0 ),
    horiz_padding      INTEGER DEFAULT ( 0 ),
    vert_padding       INTEGER DEFAULT ( 0 ),
    horiz_edge_padding INTEGER DEFAULT ( 0 ),
    vert_edge_padding  INTEGER DEFAULT ( 0 )
);

-----------------------------------------


-- Table: vector3
CREATE TABLE vector3 ( 
    id INTEGER PRIMARY KEY AUTOINCREMENT
               UNIQUE,
    x  REAL    DEFAULT ( 0 ),
    y  REAL    DEFAULT ( 0 ),
    z  REAL    DEFAULT ( 0 ) 
);

-- Table: rotation
CREATE TABLE rotation ( 
    id    INTEGER PRIMARY KEY AUTOINCREMENT
                  UNIQUE,
    x     REAL    DEFAULT ( 0 ),
    y     REAL    DEFAULT ( 0 ),
    z     REAL    DEFAULT ( 1 ),
    angle REAL    DEFAULT ( 0 ) 
);

-- Table: project
CREATE TABLE project ( 
    id   INTEGER PRIMARY KEY AUTOINCREMENT
                 NOT NULL
                 UNIQUE,
    name TEXT    NOT NULL
                 UNIQUE
                 DEFAULT ( '' ) 
);

-- Table: scene
CREATE TABLE scene ( 
    id          INTEGER PRIMARY KEY AUTOINCREMENT
                        NOT NULL
                        UNIQUE,
    pos_id      INTEGER DEFAULT ( 0 ),
    project_id  INTEGER DEFAULT ( 0 ),
    name        VARCHAR NOT NULL
                        DEFAULT ( '' ),
    description VARCHAR DEFAULT ( '' )
);

-- Table: shapeGroup
CREATE TABLE shapeGroup ( 
    id   INTEGER PRIMARY KEY AUTOINCREMENT
                 NOT NULL
                 UNIQUE,
    name VARCHAR 
);

-- Table: shapeVehicle
CREATE TABLE shapeVehicle ( 
    id                  INTEGER PRIMARY KEY AUTOINCREMENT
                                NOT NULL
                                UNIQUE,
    rudder_range        REAL,
    rudder_offset_id    INTEGER,
    elevator_range      REAL,
    elevator_offset_id  INTEGER,
    aileron_range       REAL,
    aileron_offset_id   INTEGER,
    rudder_nodes        TEXT,
    elevator_nodes      TEXT,
    right_aileron_nodes TEXT,
    left_aileron_nodes  TEXT,
    propeller_nodes     TEXT,
    rotor_nodes_a       TEXT,
    rotor_nodes_b       TEXT,
    tail_rotor_nodes    TEXT,
    prop_blur_speed     REAL,
    prop_disc_speed     REAL,
    prop_blur_alpha     REAL,
    prop_disc_alpha     REAL 
);

-- Table: px3Joint
CREATE TABLE px3Joint ( 
    id                  INTEGER PRIMARY KEY,
    name                VARCHAR DEFAULT ( '' ),
    jointType           INTEGER DEFAULT ( 5 ),
    twistLimit          REAL    DEFAULT ( 0 ),
    swingLimit          REAL    DEFAULT ( 0 ),
    swingLimit2         REAL    DEFAULT ( 0 ),
    xLimit              REAL    DEFAULT ( 0 ),
    yLimit              REAL    DEFAULT ( 0 ),
    zLimit              REAL    DEFAULT ( 0 ),
    localAxis_x         REAL    DEFAULT ( 1 ),
    localAxis_y         REAL    DEFAULT ( 0 ),
    localAxis_z         REAL    DEFAULT ( 0 ),
    localNormal_x       REAL    DEFAULT ( 0 ),
    localNormal_y       REAL    DEFAULT ( 0 ),
    localNormal_z       REAL    DEFAULT ( 1 ),
    swingSpring         REAL    DEFAULT ( 0 ),
    twistSpring         REAL    DEFAULT ( 0 ),
    springDamper        REAL    DEFAULT ( 0 ),
    motorSpring         REAL    DEFAULT ( 0 ),
    motorDamper         REAL    DEFAULT ( 0 ),
    maxForce            REAL    DEFAULT ( 1000 ),
    maxTorque           REAL    DEFAULT ( 1000 ),
    numLimitPlanes      INTEGER,
    limitPoint_x        REAL,
    limitPoint_y        REAL,
    limitPoint_z        REAL,
    limitPlaneAnchor1_x REAL,
    limitPlaneAnchor1_y REAL,
    limitPlaneAnchor1_z REAL,
    limitPlaneNormal1_x REAL,
    limitPlaneNormal1_y REAL,
    limitPlaneNormal1_z REAL,
    limitPlaneAnchor2_x REAL,
    limitPlaneAnchor2_y REAL,
    limitPlaneAnchor2_z REAL,
    limitPlaneNormal2_x REAL,
    limitPlaneNormal2_y REAL,
    limitPlaneNormal2_z REAL,
    limitPlaneAnchor3_x REAL,
    limitPlaneAnchor3_y REAL,
    limitPlaneAnchor3_z REAL,
    limitPlaneNormal3_x REAL,
    limitPlaneNormal3_y REAL,
    limitPlaneNormal3_z REAL,
    limitPlaneAnchor4_x REAL,
    limitPlaneAnchor4_y REAL,
    limitPlaneAnchor4_z REAL,
    limitPlaneNormal4_x REAL,
    limitPlaneNormal4_y REAL,
    limitPlaneNormal4_z REAL 
);

-- Table: physicsShape
CREATE TABLE physicsShape ( 
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    vehicle_id INTEGER DEFAULT ( 0 ),
    name       TEXT    NOT NULL
                       DEFAULT ( '' ),
    datablock  TEXT    DEFAULT ( '' ),
    path       TEXT 
);

-- Table: physicsShapePart
CREATE TABLE physicsShapePart ( 
    id                INTEGER PRIMARY KEY,
    physicsShape_id   INTEGER DEFAULT ( 0 ),
    px3Joint_id       INTEGER DEFAULT ( 0 ),
    name              VARCHAR DEFAULT ( '' ),
    baseNode          VARCHAR DEFAULT ( '' ),
    childNode         VARCHAR DEFAULT ( '' ),
    shapeType         INTEGER DEFAULT ( 0 ),
    dimensions_x      REAL    DEFAULT ( 0.1 ),
    dimensions_y      REAL    DEFAULT ( 0.1 ),
    dimensions_z      REAL    DEFAULT ( 0.1 ),
    orientation_x     REAL    DEFAULT ( 0 ),
    orientation_y     REAL    DEFAULT ( 0 ),
    orientation_z     REAL    DEFAULT ( 0 ),
    offset_x          REAL    DEFAULT ( 0 ),
    offset_y          REAL    DEFAULT ( 0 ),
    offset_z          REAL    DEFAULT ( 0 ),
    joint_x           REAL    DEFAULT ( 0 ),
    joint_y           REAL    DEFAULT ( 0 ),
    joint_z           REAL    DEFAULT ( 0 ),
    joint_x_2         REAL    DEFAULT ( 0 ),
    joint_y_2         REAL    DEFAULT ( 0 ),
    joint_z_2         REAL    DEFAULT ( 0 ),
    density           REAL    DEFAULT ( 1 ),
    mass              REAL    DEFAULT ( 1 ),
    bodypartChain     INTEGER DEFAULT ( 0 ),
    damageMultiplier  REAL    DEFAULT ( 1 ),
    inflictMultiplier REAL    DEFAULT ( 1 ),
    isInflictor       BOOLEAN DEFAULT ( 0 ),
    isKinematic       BOOLEAN DEFAULT ( 0 ),
    isNoGravity       BOOLEAN DEFAULT ( 0 ),
    childVerts        INTEGER DEFAULT ( 0 ),
    parentVerts       INTEGER DEFAULT ( 0 ),
    farVerts          INTEGER DEFAULT ( 0 ),
    weightThreshold   REAL    DEFAULT ( 0 ),
    ragdollThreshold  REAL    DEFAULT ( 0 )
);

-- Table: openSteerProfile
CREATE TABLE openSteerProfile ( 
    id                     INTEGER PRIMARY KEY AUTOINCREMENT
                                   NOT NULL
                                   UNIQUE,
    name                   TEXT    DEFAULT ( '' ),
    mass                   REAL    DEFAULT ( 1 ),
    radius                 REAL    DEFAULT ( 0.5 ),
    maxForce               REAL    DEFAULT ( 0.1 ),
    maxSpeed               REAL    DEFAULT ( 1 ),
    wanderChance           REAL    DEFAULT ( 0 ),
    wanderWeight           REAL    DEFAULT ( 0 ),
    seekTargetWeight       REAL    DEFAULT ( 1 ),
    avoidTargetWeight      REAL    DEFAULT ( 1 ),
    seekNeighborWeight     REAL    DEFAULT ( 1 ),
    avoidNeighborWeight    REAL    DEFAULT ( 1 ),
    avoidNavMeshEdgeWeight REAL    DEFAULT ( 1 ),
    detectNavMeshEdgeRange REAL    DEFAULT ( 1 )
);

-- Table: action
CREATE TABLE [action] ( 
    id   INTEGER PRIMARY KEY AUTOINCREMENT
                 NOT NULL
                 UNIQUE,
    name TEXT    NOT NULL 
);


-- Table: actionProfile
CREATE TABLE actionProfile ( 
    id   INTEGER PRIMARY KEY AUTOINCREMENT
                 NOT NULL
                 UNIQUE,
    name TEXT 
);


-- Table: actionProfileSequence
CREATE TABLE actionProfileSequence ( 
    id            INTEGER PRIMARY KEY AUTOINCREMENT
                          NOT NULL
                          UNIQUE,
    profile_id    INTEGER NOT NULL,
    action_id     INTEGER NOT NULL,
    sequence_file TEXT 
);

-- Table: bvhProfile
CREATE TABLE bvhProfile ( 
    id    INTEGER         PRIMARY KEY,
    name  VARCHAR( 255 ),
    scale REAL            DEFAULT ( 1 ) 
);


-- Table: bvhProfileSkeleton
CREATE TABLE bvhProfileSkeleton ( 
    id          INTEGER PRIMARY KEY,
    profile_id  INTEGER,
    skeleton_id INTEGER 
);


-- Table: bvhProfileNode
CREATE TABLE bvhProfileNode ( 
    id            INTEGER PRIMARY KEY,
    profile_id    INTEGER,
    parent_id     INTEGER,
    name          VARCHAR,
    nodeGroup     INTEGER DEFAULT ( 0 ),
    offset_x      REAL,
    offset_y      REAL,
    offset_z      REAL,
    channels      INTEGER,
    channelRots_0 INTEGER,
    channelRots_1 INTEGER,
    channelRots_2 INTEGER
);

-- Table: bvhProfileSkeletonNode
CREATE TABLE bvhProfileSkeletonNode ( 
    id                    INTEGER PRIMARY KEY,
    bvhProfileSkeleton_id INTEGER,
    bvhNodeName           VARCHAR,
    skeletonNodeName      VARCHAR,
    nodeGroup             INTEGER,
    poseRotA_x            REAL    DEFAULT ( 0 ),
    poseRotA_y            REAL    DEFAULT ( 0 ),
    poseRotA_z            REAL    DEFAULT ( 0 ),
    poseRotB_x            REAL    DEFAULT ( 0 ),
    poseRotB_y            REAL    DEFAULT ( 0 ),
    poseRotB_z            REAL    DEFAULT ( 0 ),
    fixRotA_x             REAL    DEFAULT ( 0 ),
    fixRotA_y             REAL    DEFAULT ( 0 ),
    fixRotA_z             REAL    DEFAULT ( 0 ),
    fixRotB_x             REAL    DEFAULT ( 0 ),
    fixRotB_y             REAL    DEFAULT ( 0 ),
    fixRotB_z             REAL    DEFAULT ( 0 ) 
);

-- Table: keyframeSet
CREATE TABLE keyframeSet ( 
    id            INTEGER PRIMARY KEY AUTOINCREMENT
                          NOT NULL
                          UNIQUE,
    shape_id      INTEGER,
    sequence_name TEXT,
    name          TEXT 
);


-- Table: keyframeSeries
CREATE TABLE keyframeSeries ( 
    id     INTEGER PRIMARY KEY AUTOINCREMENT
                   NOT NULL
                   UNIQUE,
    set_id INTEGER,
    type   INTEGER,
    node   INTEGER 
);


-- Table: keyframe
CREATE TABLE keyframe ( 
    id        INTEGER PRIMARY KEY AUTOINCREMENT
                      NOT NULL
                      UNIQUE,
    series_id INTEGER,
    frame     INTEGER,
    x         REAL    DEFAULT ( 0 ),
    y         REAL    DEFAULT ( 0 ),
    z         REAL    DEFAULT ( 0 ) 
);


-- Table: sceneShape
CREATE TABLE sceneShape ( 
    id                  INTEGER PRIMARY KEY AUTOINCREMENT
                                NOT NULL
                                UNIQUE,
    scene_id            INTEGER NOT NULL
                                DEFAULT ( 0 ),
    shape_id            INTEGER NOT NULL
                                DEFAULT ( 0 ),
    pos_id              INTEGER DEFAULT ( 0 ),
    rot_id              INTEGER DEFAULT ( 0 ),
    scale_id            INTEGER DEFAULT ( 0 ),
    name                TEXT    DEFAULT ( '' ),
    behavior_tree       TEXT    DEFAULT ( '' ),
    target_shape_id     INTEGER DEFAULT ( 0 ),
    shapeGroup_id       INTEGER DEFAULT ( 0 ),
    openSteerProfile_id INTEGER DEFAULT ( 0 ),
    actionProfile_id    INTEGER DEFAULT ( 1 )
);


-- Table: shapeNode
CREATE TABLE shapeNode ( 
    id              INTEGER PRIMARY KEY AUTOINCREMENT
                            NOT NULL
                            UNIQUE,
    physicsShape_id INTEGER NOT NULL,
    name            TEXT    NOT NULL,
    parent_id       INTEGER,
    node_index      INTEGER,
    parent_index    INTEGER,
    defaultTrans_id INTEGER,
    defaultRot_id   INTEGER 
);


-- Table: shapeMount
CREATE TABLE shapeMount ( 
    id              INTEGER PRIMARY KEY AUTOINCREMENT
                            NOT NULL
                            UNIQUE,
    parent_shape_id INTEGER NOT NULL
                            DEFAULT ( 0 ),
    child_shape_id  INTEGER NOT NULL
                            DEFAULT ( 0 ),
    offset_id       INTEGER DEFAULT ( 0 ),
    orientation_id  INTEGER DEFAULT ( 0 ),
    scale_id        INTEGER DEFAULT ( 0 ),
    joint_id        INTEGER DEFAULT ( 0 ),
    parent_node     INTEGER DEFAULT ( 0 ),
    child_node      INTEGER DEFAULT ( 0 )
);
