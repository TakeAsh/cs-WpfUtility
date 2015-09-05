using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfUtility {

    public class MonitorDpi {

        public const double DefaultDpi = 96;

        private PresentationSource _source;

        public MonitorDpi(Visual visual) {
            _source = PresentationSource.FromVisual(visual);
            if (_source == null) {
                throw new ArgumentException("Failed to get a PresentationSource from the visual.");
            }
            Update();
        }

        public double X { get; private set; }
        public double Y { get; private set; }

        public override string ToString() {
            return "X:" + X + ", Y:" + Y;
        }

        public void Update() {
            X = DefaultDpi * _source.CompositionTarget.TransformToDevice.M11;
            Y = DefaultDpi * _source.CompositionTarget.TransformToDevice.M22;
        }
    }
}
