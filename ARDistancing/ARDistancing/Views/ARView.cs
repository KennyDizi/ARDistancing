using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace ARDistancing.Views
{
    public class ARView : View
    {
        public ARView()
        {
        }

        public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
            typeof(ARView), string.Empty, BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public void Invoke(float distancing, DistancingDirection direction)
        {
            var data = new ObjectMeasurement { Distancing = distancing, Direction = direction };
            DistancingDetected?.Invoke(this, data);
        }

        public event EventHandler<ObjectMeasurement> DistancingDetected;
    }

    public enum DistancingDirection
    {
        Horizontal,
        Vertical
    }

    public class ObjectMeasurement
    {
        public float Distancing { get; set; }

        public DistancingDirection Direction { get; set; }
    }

    public static class ObjectExtensions
    {
        public static T As<T>(this object instance, string context = null)
        {
            Debug.WriteLine($"Call from: {context}");

            if (instance == null)
#if DEBUG
                throw new System.ArgumentNullException(nameof(instance),
                    $"Unable to sure cast null instance as type '{typeof(T).Name}");
#else
                return default(T);
#endif
            return (T)instance;
        }
    }
}