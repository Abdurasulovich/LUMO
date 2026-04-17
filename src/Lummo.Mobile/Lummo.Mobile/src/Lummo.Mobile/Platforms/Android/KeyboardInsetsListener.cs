using Android.Views;
using AndroidX.Core.View;

namespace Lummo.Mobile.Platforms.Android;

public class KeyboardInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
{
    private int _lastImeHeight = 0;

    public WindowInsetsCompat? OnApplyWindowInsets(global::Android.Views.View? v, WindowInsetsCompat? insets)
    {
        if (v == null || insets == null)
            return insets;

        var imeInsets = insets.GetInsets(WindowInsetsCompat.Type.Ime());
        var navInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());

        // Keyboard haqiqiy balandligi = IME - navigation bar (ikki marta hisoblanmasin)
        var imeHeight = Math.Max(0, imeInsets.Bottom - navInsets.Bottom);

        if (imeHeight != _lastImeHeight)
        {
            _lastImeHeight = imeHeight;

            // Root view'ni EMAS, faqat padding orqali ichidagi contenta joy beramiz
            v.SetPadding(
                v.PaddingLeft,
                v.PaddingTop,
                v.PaddingRight,
                imeHeight  // keyboard ochilganda pastdan joy qo'shiladi
            );
        }

        return ViewCompat.OnApplyWindowInsets(v, insets);
    }
}