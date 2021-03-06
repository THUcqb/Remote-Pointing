﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace Tsinghua.Kinect.RemotePoint
{
    class Painter
    {
        public Painter(KinectSensor sensor, Brush color)
        {
            this.sensor = sensor;
            this.color = color;

            TrackedBonePen = new Pen(color, 6);
            InferredBonePen = new Pen(Brushes.Gray, 2);
            //SightLinePen = new Pen(Brushes.LightGoldenrodYellow, 3);
        }

        private KinectSensor sensor;

        private Brush color;

        private readonly double HeadHandThickness = 4;

        private readonly double BodyCenterThickness = 10;

        private readonly Brush HeadHandBrush = Brushes.LightGreen;

        private readonly Pen TrackedBonePen;

        private readonly Pen InferredBonePen;

        private readonly Pen SightLinePen;

        /// <summary>
        /// Draw the skeletons on the screen
        /// </summary>
        public void DrawSkeleton(DrawingContext dc, Skeleton skeleton)
        {
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                this.DrawBones(dc, skeleton);
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="dc">drawing context to draw to</param>
        private void DrawBones(DrawingContext dc, Skeleton skeleton)
        {
            // Render Torso
            this.DrawBone(dc, skeleton, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(dc, skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(dc, skeleton, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(dc, skeleton, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(dc, skeleton, JointType.Spine, JointType.HipCenter);
            this.DrawBone(dc, skeleton, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(dc, skeleton, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(dc, skeleton, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(dc, skeleton, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(dc, skeleton, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(dc, skeleton, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(dc, skeleton, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(dc, skeleton, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(dc, skeleton, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(dc, skeleton, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(dc, skeleton, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(dc, skeleton, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(dc, skeleton, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(dc, skeleton, JointType.AnkleRight, JointType.FootRight);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(DrawingContext dc, Skeleton skeleton, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.InferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.TrackedBonePen;
            }

            dc.DrawLine(drawPen, RoomSetting.CameraPointToObservePoint(SkeletonPointToCameraPoint(joint0.Position)), RoomSetting.CameraPointToObservePoint(SkeletonPointToCameraPoint(joint1.Position)));
        }

        public SpacePoint SkeletonPointToCameraPoint(SkeletonPoint point)
        {
            DepthImagePoint depthPoint =
                this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(point, this.sensor.DepthStream.Format);

            return new SpacePoint(640 - depthPoint.X, depthPoint.Y, depthPoint.Depth);
        }

        /*
        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        public Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to color space.  
            ColorImagePoint colorPoint = this.sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skelpoint, this.sensor.ColorStream.Format);
            return new Point(colorPoint.X, colorPoint.Y);
        }
        */

        /*
        /// <summary>
        /// Draw the sight on the screen
        /// </summary>
        public void DrawSight(DrawingContext dc, Point startPoint, Point endPoint)
        {
            dc.DrawEllipse(this.HeadHandBrush, null, startPoint, HeadHandThickness, HeadHandThickness);
            dc.DrawEllipse(this.HeadHandBrush, null, endPoint, HeadHandThickness, HeadHandThickness);
            dc.DrawLine(this.SightLinePen, startPoint, endPoint);
        }
        */

    }
}
