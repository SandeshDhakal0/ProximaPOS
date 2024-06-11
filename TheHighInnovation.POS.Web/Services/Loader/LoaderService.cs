using BlazorBootstrap;

namespace TheHighInnovation.POS.Web.Services.Loader
{
    public class LoaderService
    {
        public event Action<SpinnerColor>? OnShow;
        
        public event Action? OnHide;

        public void Show(SpinnerColor spinnerColor)
        {
            OnShow?.Invoke(spinnerColor);
        }

        public void Hide()
        {
            OnHide?.Invoke();
        }
    }
}
