using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors.Recommended;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Memo.ViewModels
{
    public class SkinViewModel : BindableBase
    {
        public SkinViewModel()
        {
            ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
        }

        public IEnumerable<ISwatch> Swatches { get; } = new ISwatch[4]
        {
            new BlueSwatch(),
            new LightBlueSwatch(),
            new GreenSwatch(),
            new LightGreenSwatch(),
        };
        public DelegateCommand<object> ChangeHueCommand { get; private set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme => theme.SetBaseTheme(value ? Theme.Dark : Theme.Light));
                }
            }
        }


        private void ChangeHue(object? obj)
        {
            var hue = (Color)obj;
            ITheme theme = paletteHelper.GetTheme();
            theme.PrimaryLight = new ColorPair(hue.Lighten());
            theme.PrimaryMid = new ColorPair(hue);
            theme.PrimaryDark = new ColorPair(hue.Darken());
            paletteHelper.SetTheme(theme);
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }
    }
}
