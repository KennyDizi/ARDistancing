using System;
using System.Collections.Generic;
using System.Linq;
using ARDistancing.iOS.Renderers;
using ARDistancing.Views;
using ARKit;
using Foundation;
using SceneKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ARView), typeof(ARViewRenderer))]

namespace ARDistancing.iOS.Renderers
{
    public class ARViewRenderer : ViewRenderer<ARView, ARSCNView>, IARSCNViewDelegate
    {
        private readonly List<SCNNode> _spheres = new();

        protected override void OnElementChanged(ElementChangedEventArgs<ARView> e)
        {
            base.OnElementChanged(e);

            var arView = new ARSCNView
            {
                Delegate = this
            };
            arView.Session.Delegate = new SessionDelegate();

            var configuration = new ARWorldTrackingConfiguration
            {
                PlaneDetection = ARPlaneDetection.Horizontal,
                LightEstimationEnabled = true
            };

            // Add a tap gesture recognizer
            var tapGesture = new UITapGestureRecognizer(HandleTap)
            {
                NumberOfTapsRequired = 1
            };
            arView.AddGestureRecognizer(tapGesture);

            SetNativeControl(arView);

            // Run the view's session
            Control.Session.Run(configuration, ARSessionRunOptions.ResetTracking);
        }

        #region Private Methods

        private void HandleTap(UIGestureRecognizer gestureRecognize)
        {
            var location = gestureRecognize.LocationInView(Control);

            var hitTest = Control.HitTest(location, ARHitTestResultType.FeaturePoint);

            var result = hitTest.LastOrDefault();

            if (result == null)
                return;

            var vector = new SCNVector3(result.WorldTransform.M14, result.WorldTransform.M24,
                result.WorldTransform.M34);

            var sphere = NewSphere(vector);

            var first = _spheres.FirstOrDefault();
            if (first != null)
            {
                _spheres.Add(sphere);

                var distancing = DistanceOfTwoSCNNode(sphere, first);
                Element.Text = distancing + "inches";
                Element.Invoke(distancing, DistancingDirection.Vertical);

                if (_spheres.Count > 2)
                {
                    foreach (var item in _spheres)
                    {
                        item.RemoveFromParentNode();
                    }

                    _spheres.Clear();
                }
            }
            else
            {
                _spheres.Add(sphere);
            }

            foreach (var _ in _spheres)
            {
                Control.Scene.RootNode.AddChildNode(sphere);
            }
        }

        #endregion


        private static SCNNode NewSphere(SCNVector3 atPosition)
        {
            // Creates an SCNSphere with a radius of 0.01
            const float radius = 0.01f;
            var sphere = SCNSphere.Create(radius);

            var node = SCNNode.FromGeometry(sphere);
            node.Position = atPosition;

            var material = SCNMaterial.Create();
            material.Diffuse.Contents = UIColor.Orange;

            material.LightingModelName = new NSString("blinn");
            sphere.FirstMaterial = material;

            return node;
        }

        private static float DistanceOfTwoSCNNode(SCNNode node1, SCNNode node2)
        {
            const float inches = 39.3701f;
            var dx = node2.Position.X - node1.Position.X;
            var dy = node2.Position.Y - node1.Position.Y;
            var dz = node2.Position.Z - node1.Position.Z;

            var meters = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);

            return meters * inches;
        }
    }

    public class SessionDelegate : ARSessionDelegate
    {
        #region Constructors

        public SessionDelegate()
        {
        }

        #endregion

        #region Override Methods

        public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
        {
            var state = "";
            var reason = "";

            switch (camera.TrackingState)
            {
                case ARTrackingState.NotAvailable:
                    state = "Tracking Not Available";
                    break;
                case ARTrackingState.Normal:
                    state = "Tracking Normal";
                    break;
                case ARTrackingState.Limited:
                    state = "Tracking Limited";
                    switch (camera.TrackingStateReason)
                    {
                        case ARTrackingStateReason.ExcessiveMotion:
                            reason = "because of excessive motion";
                            break;
                        case ARTrackingStateReason.Initializing:
                            reason = "because tracking is initializing";
                            break;
                        case ARTrackingStateReason.InsufficientFeatures:
                            reason = "because of insufficient features in the environment";
                            break;
                        case ARTrackingStateReason.None:
                            reason = "because of an unknown reason";
                            break;
                    }

                    break;
            }

            // Inform user
            Console.WriteLine("{0} {1}", state, reason);
        }

        #endregion
    }
}