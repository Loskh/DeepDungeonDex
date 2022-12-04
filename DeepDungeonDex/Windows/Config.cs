using System.Linq;
using Dalamud.Interface.Windowing;
using DeepDungeonDex.Storage;
using DeepDungeonDex.Models;
using ImGuiNET;
using System.Numerics;
using DeepDungeonDex.Requests;

namespace DeepDungeonDex.Windows
{
    public class Config : Window
    {
        private static Config _instance;
        private readonly StorageHandler _handler;
        private readonly Language _language;
        private float _opacity;
        private bool _clickthrough;
        private bool _hideRed;
        private bool _hideJob;
        private bool _debug;
        private bool _legacy;
        private int _loc;

        public Config(StorageHandler handler, CommandHandler command, Language language) : base("DeepDungeonDex Config", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse)
        {
            _handler = handler;
            _language = language;
            _instance = this;
            var _config = _handler.GetInstance<Configuration>()!;
            _config.OnChange += OnChange;
            SizeConstraints = new WindowSizeConstraints
            {
                MaximumSize = new Vector2(400 * _config.WindowSizeScaled, 600),
                MinimumSize = new Vector2(250 * _config.WindowSizeScaled, 100)
            };
            _opacity = _config.Opacity;
            _clickthrough = _config.Clickthrough;
            _hideRed = _config.HideRed;
            _hideJob = _config.HideJob;
            _debug = _config.Debug;
            _legacy = _config.LegacyWindow;
            _loc = _config.Locale;
            command.AddCommand(new[] { "config", "cfg" }, () => _instance.IsOpen = true, "Opens the config window.");
        }

        private void OnChange(Configuration config)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MaximumSize = new Vector2(400 * config.WindowSizeScaled, 600),
                MinimumSize = new Vector2(250 * config.WindowSizeScaled, 100)
            };
            BgAlpha = config.Opacity;
        }

        public override void Draw()
        {
            var _config = _handler.GetInstance<Configuration>()!;
            var _localeKeys = _handler.GetInstance<LocaleKeys>()!;
            var lang = _localeKeys.LocaleDictionary.Keys.ToArray()[_loc];
            var _locale = (Locale)_handler.GetInstance($"{lang}/main.yml")!;
            ImGui.PushFont(Font.RegularFont);
            ImGui.SetNextWindowSizeConstraints(new Vector2(250 * _config.WindowSizeScaled, 100), new Vector2(400 * _config.WindowSizeScaled, 600));
            if (ImGui.SliderFloat(_locale.GetLocale("Opacity"), ref _opacity, 0.0f, 1.0f))
            {
                _config.Opacity = _opacity;
            }
            ImGui.Columns(4, null, false);
            ImGui.Text(_locale.GetLocale("FontSize"));
            foreach (var f in new int[] { 12, 14, 16, 18, 24, 32 })
            {
                ImGui.NextColumn();
                if (ImGui.RadioButton($"{f}px", _config.FontSize == f))
                {
                    _config.FontSize = f;
                }
            }
            ImGui.Columns(1);
            if (ImGui.Checkbox(_locale.GetLocale("IsClickthrough"), ref _clickthrough))
            {
                _config.Clickthrough = _clickthrough;
            }
            if (ImGui.Checkbox(_locale.GetLocale("HideRedVulns"), ref _hideRed))
            {
                _config.HideRed = _hideRed;
            }
            if (ImGui.Checkbox(_locale.GetLocale("HideBasedOnJob"), ref _hideJob))
            {
                _config.HideJob = _hideJob;
            }
            if (ImGui.Checkbox(_locale.GetLocale("ShowId"), ref _debug))
            {
                _config.Debug = _debug;
            }
            if(ImGui.Checkbox(_locale.GetLocale("Legacy"), ref _legacy))
            {
                _config.LegacyWindow = _legacy;
            }

            var locales = _localeKeys.LocaleDictionary.Values.ToArray();
            if (ImGui.Combo("Locale", ref _loc, locales, locales.Length))
            {
                _config.Locale = _loc;
                _language.ChangeLanguage();
            }
            ImGui.NewLine();
            if (ImGui.Button(_locale.GetLocale("Save")))
            {
                IsOpen = false;
                _config.Save(_handler.GetFilePath(_config.GetType()));
            }
            ImGui.SameLine();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(400f);
                ImGui.TextWrapped(_locale.GetLocale("Thanks"));
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            };
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Button, 0xFF5E5BFF);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xFF5E5BAA);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xFF5E5BDD);
            ImGui.PopStyleColor(3);
            ImGui.PopFont();
        }
    }
}
