using BlazorBootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.WEB.Services
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
