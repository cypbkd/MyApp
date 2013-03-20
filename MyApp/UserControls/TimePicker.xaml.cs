using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyApp.UserControls;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace MyApp
{
    public sealed partial class TimePicker : UserControl
    {
        public event EventHandler<DateTimeValueChangedEventArgs> ValueChanged;

        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DateTime?), typeof(TimePicker), new PropertyMetadata(null, OnValueChanged));

        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((TimePicker)o).OnValueChanged((DateTime?)e.OldValue, (DateTime?)e.NewValue);
        }

        private void OnValueChanged(DateTime? oldValue, DateTime? newValue)
        {
            OnValueChanged(new DateTimeValueChangedEventArgs(oldValue, newValue));
        }

        /// <summary>
        /// Called when the value changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void OnValueChanged(DateTimeValueChangedEventArgs e)
        {
            EventHandler<DateTimeValueChangedEventArgs> handler = ValueChanged;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        public TimePicker()
        {
            this.InitializeComponent();

            for (int i = 0; i < 24; i++)
            {
                hour.Items.Add(i);
            }

            for (int i = 0; i < 60; i++)
            {
                minite.Items.Add(i);
            }

            Value = DateTime.Now;
            DataContext = this;
        }
    }
}
