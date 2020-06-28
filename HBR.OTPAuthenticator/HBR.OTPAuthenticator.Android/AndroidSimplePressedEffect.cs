using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HBR.OTPAuthenticator.Droid;
using HBR.OTPAuthenticator;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidSimplePressedEffect), "SimplePressedEffect")]
namespace HBR.OTPAuthenticator.Droid
{
    /// <summary>
    /// Android long pressed effect.
    /// </summary>
    public class AndroidSimplePressedEffect : PlatformEffect
    {
        private bool _attached;

        /// <summary>
        /// Initializer to avoid linking out
        /// </summary>
        public static void Initialize() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Yukon.Application.AndroidComponents.Effects.AndroidSimplePressedEffect"/> class.
        /// Empty constructor required for the odd Xamarin.Forms reflection constructor search
        /// </summary>
        public AndroidSimplePressedEffect()
        {
        }

        /// <summary>
        /// Apply the handler
        /// </summary>
        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time.
            if (!_attached)
            {
                if (Control != null)
                {
                    Control.Clickable = true;
                    Control.Click += Control_Click;
                }
                else
                {
                    Container.Clickable = true;
                    Container.Click += Control_Click;
                }
                _attached = true;
            }
        }

        /// <summary>
        /// Invoke the command if there is one
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Control_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Invoking click command");
            var command = SimplePressedEffect.GetCommand(Element);
            command?.Execute(SimplePressedEffect.GetCommandParameter(Element));
        }

        /// <summary>
        /// Clean the event handler on detach
        /// </summary>
        protected override void OnDetached()
        {
            if (_attached)
            {
                if (Control != null)
                {
                    Control.Clickable = true;
                    Control.Click -= Control_Click;
                }
                else
                {
                    Container.Clickable = true;
                    Container.Click -= Control_Click;
                }
                _attached = false;
            }
        }
    }
}