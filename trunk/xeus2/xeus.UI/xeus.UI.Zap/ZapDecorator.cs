using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using xeus2.xeus.UI.xeus.UI.Zap;

namespace xeus2.xeus.UI.xeus.UI.Zap
{
    public class ZapDecorator : Decorator
    {
        private const double DIFF = .0001;

        public static readonly DependencyProperty TargetIndexProperty = DependencyProperty.Register("TargetIndex",
                                                                                                    typeof (int),
                                                                                                    typeof (ZapDecorator
                                                                                                        ));

        private double _percentOffset = 0;
        private TranslateTransform _traslateTransform = new TranslateTransform();
        private double _velocity = 0;
        private ZapPanel _zapPanel;

        public ZapDecorator()
        {
            Unloaded += ZapDecorator_Unloaded;
            Loaded += ZapDecorator_Loaded;
        }

        public int TargetIndex
        {
            get
            {
                return (int) GetValue(TargetIndexProperty);
            }
            set
            {
                SetValue(TargetIndexProperty, value);
            }
        }

        private void ZapDecorator_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void ZapDecorator_Unloaded(object sender, RoutedEventArgs e)
        {
            //SUPER IMPORTANT!
            //If we don't un-register, this instance will leak!
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        //TODO: be smart about item changes...make sure the control doesn't jump around
        //TODO: be smart about wiring/unwiring this callback. Expensive!!
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (Child != _zapPanel)
            {
                _zapPanel = Child as ZapPanel;
                _zapPanel.RenderTransform = _traslateTransform;
            }
            if (_zapPanel != null)
            {
                int actualTargetIndex = Math.Max(0, Math.Min(_zapPanel.Children.Count - 1, TargetIndex));

                double targetPercentOffset = -actualTargetIndex / (double) _zapPanel.Children.Count;
                double currentPercent = _percentOffset;
                double deltaPercent = targetPercentOffset - currentPercent;

                if (!double.IsNaN(deltaPercent) && !double.IsInfinity(deltaPercent))
                {
                    _velocity *= .9;
                    _velocity += deltaPercent * .01;

                    if (Math.Abs(_velocity) > DIFF || Math.Abs(deltaPercent) > DIFF)
                    {
                        _percentOffset += _velocity;
                    }
                    else
                    {
                        _percentOffset = targetPercentOffset;
                        _velocity = 0;
                    }
                }

                double targetPixelOffset = _percentOffset * (RenderSize.Width * _zapPanel.Children.Count);


                _traslateTransform.X = targetPixelOffset;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize = new Size();
            UIElement element1 = Child;

            if (element1 != null)
            {
                element1.Measure(constraint);
                desiredSize = element1.DesiredSize;
            }

            return desiredSize ;
        }
    }
}