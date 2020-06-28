using Foundation;
using UIKit;
using HBR.OTPAuthenticator;
using HBR.OTPAuthenticator.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("OTPAuthenticator")]
[assembly: ExportEffect(typeof(iOSLongPressedEffect), "LongPressedEffect")]
namespace HBR.OTPAuthenticator.iOS
{
    public class iOSLongPressedEffect : PlatformEffect
    {
        private bool _attached;
        private readonly UILongPressGestureRecognizer _longPressRecognizer;

        public iOSLongPressedEffect()
        {
            _longPressRecognizer = new UILongPressGestureRecognizer(HandleLongClick);
        }


        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time
            if (!_attached)
            {
                Container.AddGestureRecognizer(_longPressRecognizer);
                _attached = true;
            }
        }


        // Invoke the command if there is one       
        private void HandleLongClick(UILongPressGestureRecognizer sender)
        {
            if (sender.State == UIGestureRecognizerState.Began)
            {
                var command = LongPressedEffect.GetCommand(Element);
                command?.Execute(LongPressedEffect.GetCommandParameter(Element));
            }


        }

        protected override void OnDetached()
        {
            if (_attached)
            {
                Container.RemoveGestureRecognizer(_longPressRecognizer);
                _attached = false;
            }
        }

    }
}