using Foundation;
using UIKit;
using HBR.OTPAuthenticator;
using HBR.OTPAuthenticator.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(iOSLongPressedEffect), "LongPressedEffect")]
namespace HBR.OTPAuthenticator.iOS
{
    public class iOSSimplePressedEffect : PlatformEffect
    {
        private bool _attached;
        private readonly UITapGestureRecognizer _PressRecognizer;

        public iOSSimplePressedEffect()
        {
            _PressRecognizer = new UITapGestureRecognizer(HandleLongClick);
        }


        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time
            if (!_attached)
            {
                Container.AddGestureRecognizer(_PressRecognizer);
                _attached = true;
            }
        }


        // Invoke the command if there is one       
        private void HandleLongClick(UITapGestureRecognizer sender)
        {
            if (sender.State == UIGestureRecognizerState.Began)
            {
                var command = SimplePressedEffect.GetCommand(Element);
                command?.Execute(SimplePressedEffect.GetCommandParameter(Element));
            }


        }

        protected override void OnDetached()
        {
            if (_attached)
            {
                Container.RemoveGestureRecognizer(_PressRecognizer);
                _attached = false;
            }
        }

    }
}