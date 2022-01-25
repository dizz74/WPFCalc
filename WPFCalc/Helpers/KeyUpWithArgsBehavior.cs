using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFCalc
{
    public class KeyUpWithArgsBehavior : Behavior<UIElement>
    {
        /*
         * Класс для привязки VM и нажатия клавиш 
         */
        public ICommand KeyUpCommand
        {
            get { return (ICommand)GetValue(KeyUpCommandProperty); }
            set { SetValue(KeyUpCommandProperty, value); }
        }

        public static readonly DependencyProperty KeyUpCommandProperty =
            DependencyProperty.Register("KeyUpCommand", typeof(ICommand), typeof(KeyUpWithArgsBehavior), new UIPropertyMetadata(null));


        protected override void OnAttached()
        {
            AssociatedObject.KeyUp += new KeyEventHandler(AssociatedObjectKeyUp);
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= new KeyEventHandler(AssociatedObjectKeyUp);
            base.OnDetaching();
        }

        private void AssociatedObjectKeyUp(object sender, KeyEventArgs e)
        {
            if (KeyUpCommand != null)
            {
                KeyUpCommand.Execute(e.Key);
            }
        }
    }
}
