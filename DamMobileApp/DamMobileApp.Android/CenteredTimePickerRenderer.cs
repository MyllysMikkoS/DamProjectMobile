using Android.Content;
using Android.Views;
using DamMobileApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.TimePicker), typeof(CenteredTimePickerRenderer))]
namespace DamMobileApp.Droid
{
    class CenteredTimePickerRenderer : TimePickerRenderer
    {
        public CenteredTimePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Gravity = GravityFlags.Center;
            }
        }
    }
}