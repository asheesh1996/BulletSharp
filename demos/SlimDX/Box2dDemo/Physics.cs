﻿using BulletSharp;
using DemoFramework;
using SlimDX;

namespace Box2dDemo
{
    class Physics : PhysicsContext
    {
        ///create 25 (5x5) dynamic objects
        int ArraySizeX = 5, ArraySizeY = 5;
        public float Depth = 0.04f;

        public Physics()
        {
            // collision configuration contains default setup for memory, collision setup
            CollisionConf = new DefaultCollisionConfiguration();

            // Use the default collision dispatcher. For parallel processing you can use a diffent dispatcher.
            Dispatcher = new CollisionDispatcher(CollisionConf);

            VoronoiSimplexSolver simplex = new VoronoiSimplexSolver();
            MinkowskiPenetrationDepthSolver pdSolver = new MinkowskiPenetrationDepthSolver();

            Convex2DConvex2DAlgorithm.CreateFunc convexAlgo2d = new Convex2DConvex2DAlgorithm.CreateFunc(simplex, pdSolver);

            Dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeType.Convex2DShape, BroadphaseNativeType.Convex2DShape, convexAlgo2d);
            Dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeType.Box2DShape, BroadphaseNativeType.Convex2DShape, convexAlgo2d);
            Dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeType.Convex2DShape, BroadphaseNativeType.Box2DShape, convexAlgo2d);
            Dispatcher.RegisterCollisionCreateFunc(BroadphaseNativeType.Box2DShape, BroadphaseNativeType.Box2DShape, new Box2DBox2DCollisionAlgorithm.CreateFunc());

            Broadphase = new DbvtBroadphase();

            // the default constraint solver.
            Solver = new SequentialImpulseConstraintSolver();

            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf);
            World.Gravity = new Vector3(0, -10, 0);

            // create a few basic rigid bodies
            CollisionShape groundShape = new BoxShape(150, 7, 150);
            CollisionShapes.Add(groundShape);
            RigidBody ground = LocalCreateRigidBody(0, Matrix.Identity, groundShape);
            ground.UserObject = "Ground";

            // create a few dynamic rigidbodies
            // Re-using the same collision is better for memory usage and performance
            float u = 0.96f;
            Vector3[] points = { new Vector3(0, u, 0), new Vector3(-u, -u, 0), new Vector3(u, -u, 0) };
            ConvexShape colShape = new Convex2DShape(new BoxShape(1, 1, Depth));
            ConvexShape colShape2 = new Convex2DShape(new ConvexHullShape(points));
            ConvexShape colShape3 = new Convex2DShape(new CylinderShapeZ(1, 1, Depth));

            colShape.Margin = 0.03f;
            CollisionShapes.Add(colShape);
            CollisionShapes.Add(colShape2);
            CollisionShapes.Add(colShape3);

            float mass = 1.0f;
            Vector3 localInertia = colShape.CalculateLocalInertia(mass);

            Matrix startTransform;

            Vector3 x = new Vector3(-ArraySizeX, 8, -20);
            Vector3 y = Vector3.Zero;
            Vector3 deltaX = new Vector3(1, 2, 0);
            Vector3 deltaY = new Vector3(2, 0, 0);

            int i, j;
            for (i = 0; i < ArraySizeY; i++)
            {
                y = x;
                for (j = 0; j < ArraySizeX; j++)
                {
                    startTransform = Matrix.Translation(y - new Vector3(-10, 0, 0));

                    //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
                    DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

                    RigidBodyConstructionInfo rbInfo;
                    switch (j % 3)
                    {
                        case 0:
                            rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, colShape, localInertia);
                            break;
                        case 1:
                            rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, colShape3, localInertia);
                            break;
                        default:
                            rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, colShape2, localInertia);
                            break;
                    }
                    RigidBody body = new RigidBody(rbInfo);
                    //body.ActivationState = ActivationState.IslandSleeping;
                    body.LinearFactor = new Vector3(1, 1, 0);
                    body.AngularFactor = new Vector3(0, 0, 1);

                    World.AddRigidBody(body);

                    y += deltaY;
                }
                x += deltaX;
            }
        }
    }
}