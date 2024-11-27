using ManagedCommon;
using System;
using System.Collections.Generic;
using System.Windows;
using Resources = Community.PowerToys.Run.Plugin.KcalConverter.Properties.Resources;
using Wox.Plugin;


namespace Community.PowerToys.Run.Plugin.KcalConverter
{
    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public class Main : IPlugin, IPluginI18n, IDisposable
    {
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "2E63C5D0F56542BCA8137F6992F57200";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => Resources.wox_plugin_kcalconverter_plugin_name;

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => Resources.wox_plugin_kcalconverter_plugin_description;

        private PluginInitContext Context { get; set; }

        private string IconPath { get; set; }

        private bool Disposed { get; set; }

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);
            var isGlobalQuery = string.IsNullOrEmpty(query.ActionKeyword);
            if (string.IsNullOrEmpty(query.Search) || isGlobalQuery)
            {
                return [];
            }

            var success = decimal.TryParse(query.Search, out var number);
            if (!success)
            {
                return ErrorHandler.OnError(IconPath, query.RawQuery, "Invalid number format");
            }

            try
            {
                var result = number * 0.239006m;
                var resultStr = result.ToString("F2");
                return [
                    new Result
                    {
                        Title = $"{number} kJ = {resultStr} kcal",
                        SubTitle = "将结果复制到剪贴板",
                        IcoPath = IconPath,
                        Action = _ =>
                        {
                            Clipboard.SetDataObject(resultStr);
                            return true;
                        },
                    },
                ];
            }
            catch (Exception e)
            {
                return ErrorHandler.OnError(IconPath, query.RawQuery, errorMessage: e.Message, exception: e);
            }
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
        }
        
        public string GetTranslatedPluginTitle()
        {
            return Resources.wox_plugin_kcalconverter_plugin_name;
        }
        
        public string GetTranslatedPluginDescription()
        {
            return Resources.wox_plugin_kcalconverter_plugin_description;
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/kcalconverter.light.png" : "Images/kcalconverter.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);
    }
}
