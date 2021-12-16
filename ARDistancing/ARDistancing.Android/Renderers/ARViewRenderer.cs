using System;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using Android.Runtime;
using AndroidX.Fragment.App;
using ARDistancing.Droid.Renderers;
using ARDistancing.Views;
using Google.AR.Core;
using Google.AR.Sceneform;
using Google.AR.Sceneform.Math;
using Google.AR.Sceneform.Rendering;
using Google.AR.Sceneform.UX;
using Java.Util.Functions;
using SAFETREES.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;
using static Google.AR.Sceneform.UX.BaseArFragment;
using Color = Google.AR.Sceneform.Rendering.Color;

[assembly: ExportRenderer(typeof(ARView), typeof(ARViewRenderer))]

namespace ARDistancing.Droid.Renderers
{
    public sealed class ARViewRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<ARView, AView>, IOnTapArPlaneListener, IConsumer, IFunction, Scene.IOnUpdateListener
    {
        private readonly AView _arLayout;
        private readonly ArFragment _arFragment;
        private readonly Fragment _fragment;
        private ViewRenderable _distanceCardViewRenderable;
        private ModelRenderable _cubeRenderable;

        // New code
        private readonly List<Anchor> _placedAnchors;
        private readonly List<AnchorNode> _placedAnchorNodes;
        private readonly Dictionary<string, Anchor> _midAnchors;
        private readonly Dictionary<string, AnchorNode> _midAnchorNodes;

        public ARViewRenderer(Context context) : base(context)
        {
            _placedAnchors = new List<Anchor>();
            _placedAnchorNodes = new List<AnchorNode>();
            _midAnchors = new Dictionary<string, Anchor>();
            _midAnchorNodes = new Dictionary<string, AnchorNode>();

            _arLayout = LayoutInflater.From(context)?.Inflate(Resource.Layout.ar_layout, null);
            var fragmentManager = XamForms.SupportFragmentManager;
            _fragment = fragmentManager.FindFragmentById(Resource.Id.sceneform_fragment);
            _arFragment = _fragment.As<ArFragment>();

            // Set distance layout
            _ = ViewRenderable.InvokeBuilder()
                .SetView(context, Resource.Layout.distance_layout)
                .Build()
                .ThenAccept(this)
                ?.Exceptionally(this);

            // Set cube layout
            _ = MaterialFactory.MakeTransparentWithColor(context, new Color(r: 255, g: 0, b: 0))
                .ThenAccept(this)
                ?.Exceptionally(this);
        }

        [Obsolete]
        public ARViewRenderer(IntPtr a, JniHandleOwnership b)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ARView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && Control == null)
            {
                SetNativeControl(_arLayout);
                _arFragment.SetOnTapArPlaneListener(this);
                _arFragment.ArSceneView.Scene.AddOnUpdateListener(this);
            }
        }

        public void OnTapPlane(HitResult hitResult, Plane plane, MotionEvent motionEvent)
        {
            if (_placedAnchorNodes.Count == 0)
            {
                PlaceAnchor(hitResult: hitResult, renderable: _cubeRenderable);
            }
            else if (_placedAnchorNodes.Count == 1)
            {
                PlaceAnchor(hitResult: hitResult, renderable: _cubeRenderable);
                var midPointX = (_placedAnchorNodes[0].WorldPosition.X + _placedAnchorNodes[1].WorldPosition.X) / 2;
                var midPointY = (_placedAnchorNodes[0].WorldPosition.Y + _placedAnchorNodes[1].WorldPosition.Y) / 2;
                var midPointZ = (_placedAnchorNodes[0].WorldPosition.Z + _placedAnchorNodes[1].WorldPosition.Z) / 2;
                var midPoints = new[] { midPointX, midPointY, midPointZ };
                var quaternion = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
                var pose = new Pose(midPoints, quaternion);

                PlaceMidAnchor(pose: pose, renderable: _distanceCardViewRenderable, between: new[] { 0, 1 });
            }
            else
            {
                ClearAllAnchors();

                // Set new point as the first point
                PlaceAnchor(hitResult: hitResult, renderable: _cubeRenderable);
            }
        }

        public void Accept(Java.Lang.Object t)
        {
            if (t is ViewRenderable renderable)
            {
                _distanceCardViewRenderable = renderable;
                _distanceCardViewRenderable.ShadowCaster = false;
                _distanceCardViewRenderable.ShadowReceiver = false;
            }
            else if (t is Material material)
            {
                _cubeRenderable = ShapeFactory.MakeSphere(0.02f, Vector3.Zero(), material);
                _cubeRenderable.ShadowCaster = false;
                _cubeRenderable.ShadowReceiver = false;
            }
        }

        public Java.Lang.Object Apply(Java.Lang.Object t)
        {
            return t;
        }

        private void ClearAllAnchors()
        {
            _placedAnchors.Clear();
            foreach (var anchorNode in _placedAnchorNodes)
            {
                _arFragment.ArSceneView.Scene.RemoveChild(anchorNode);
                anchorNode.Enabled = false;
                anchorNode.Anchor.Detach();
                anchorNode.SetParent(null);
            }

            _placedAnchorNodes.Clear();

            _midAnchors.Clear();
            foreach (var (k, midAnchorNode) in _midAnchorNodes)
            {
                _arFragment.ArSceneView.Scene.RemoveChild(midAnchorNode);
                midAnchorNode.Enabled = false;
                midAnchorNode.Anchor.Detach();
                midAnchorNode.SetParent(null);
            }

            _midAnchorNodes.Clear();
        }

        private void PlaceAnchor(HitResult hitResult, Renderable renderable)
        {
            var anchor = hitResult.CreateAnchor();
            _placedAnchors.Add(anchor);

            var anchorNode = new AnchorNode(anchor)
            {
                Smoothed = true
            };
            anchorNode.SetParent(_arFragment.ArSceneView.Scene);
            _placedAnchorNodes.Add(anchorNode);

            var node = new TransformableNode(_arFragment.TransformationSystem);
            node.RotationController.Enabled = false;
            node.ScaleController.Enabled = false;
            node.TranslationController.Enabled = true;
            node.Renderable = renderable;
            node.SetParent(anchorNode);
            _arFragment.ArSceneView.Scene.AddChild(anchorNode);
            _ = node.Select();
        }

        private void PlaceMidAnchor(Pose pose, Renderable renderable, IReadOnlyList<int> between)
        {
            var midKey = $"{between[0]}_{between[1]}";
            var anchor = _arFragment.ArSceneView.Session.CreateAnchor(pose);
            _midAnchors.Add(midKey, anchor);

            var anchorNode = new AnchorNode(anchor)
            {
                Smoothed = true
            };
            anchorNode.SetParent(_arFragment.ArSceneView.Scene);
            _midAnchorNodes.Add(midKey, anchorNode);

            var node = new TransformableNode(_arFragment.TransformationSystem);
            node.RotationController.Enabled = false;
            node.ScaleController.Enabled = false;
            node.TranslationController.Enabled = true;
            node.Renderable = renderable;
            node.SetParent(anchorNode);
            _arFragment.ArSceneView.Scene.AddChild(anchorNode);
        }

        public void OnUpdate(FrameTime p0)
        {
            const float inches = 39.3701f;

            if (_placedAnchorNodes?.Count == 2)
            {
                var distanceMeter = CalculateDistance(
                    _placedAnchorNodes[0].WorldPosition,
                    _placedAnchorNodes[1].WorldPosition);

                var distancingInInches = distanceMeter * inches;
                MeasureDistanceOf2Points(distancingInInches);
            }
        }

        private void MeasureDistanceOf2Points(float distancing)
        {
            Element.Invoke(distancing, DistancingDirection.Vertical);
        }

        private static float CalculateDistance(Vector3 objectPose0, Vector3 objectPose1)
        {
            return CalculateDistance(
                objectPose0.X - objectPose1.X,
                objectPose0.Y - objectPose1.Y,
                objectPose0.Z - objectPose1.Z
            );
        }

        private static float CalculateDistance(float x, float y, float z)
        {
            return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(x, 2) + Math.Pow(x, 2));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _arFragment.ArSceneView.Scene.RemoveOnUpdateListener(this);
                var fragmentManager = XamForms.SupportFragmentManager;
                var fragmentTransaction = fragmentManager.BeginTransaction();
                _ = fragmentTransaction.Remove(_fragment);
                _ = fragmentTransaction.Commit();
            }
        }
    }
}