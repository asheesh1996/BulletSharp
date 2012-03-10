﻿using System;
using System.Drawing;
using System.Windows.Forms;
using BulletSharp;

namespace DemoFramework
{
    public abstract class Graphics
    {
        public Demo Demo
        {
            get;
            protected set;
        }

        public Form Form
        {
            get;
            protected set;
        }

        public virtual float FarPlane
        {
            get;
            set;
        }

        public float FieldOfView
        {
            get;
            protected set;
        }

        public virtual float AspectRatio
        {
            get
            {
                Size clientSize = Form.ClientSize;
                return (float)clientSize.Width / (float)clientSize.Height;
            }
        }

        public virtual bool IsFullScreen
        {
            get;
            set;
        }

        public abstract IDebugDraw GetPhysicsDebugDrawer();

        public Graphics(Demo demo)
        {
            Demo = demo;
            FarPlane = 200.0f;
            FieldOfView = (float)Math.PI / 4;
        }

        public virtual void Initialize()
        {
        }

        public abstract void Run();
        public abstract void UpdateView();

        public virtual void SetFormText(string text)
        {
            Form.Text = text;
        }

        public virtual void SetInfoText(string text)
        {
        }
    }
}
