using System;
using System.Collections.Generic;
using System.Reflection;

using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;

using XIVComboVX.Config;

namespace XIVComboVX {
	public sealed class XIVComboVX: IDalamudPlugin {
		private bool disposed = false;

		internal const string command = "/pcombo";

		private readonly WindowSystem? windowSystem;
		private readonly ConfigWindow? configWindow;
		private readonly bool registeredDefaultCommand = false;

		public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version!;
		public static readonly bool Debug =
#if DEBUG
			true;
#else
			false;
#endif
		private static readonly string assemblyName = Assembly.GetExecutingAssembly().GetName().Name!;

		public string Name => assemblyName;

		public XIVComboVX(DalamudPluginInterface pluginInterface) {

			FFXIVClientStructs.Resolver.Initialize();
			pluginInterface.Create<Service>();

			Service.Plugin = this;
			Service.Logger = new();
			Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
			Service.Address = new();

			Service.Configuration.UpgradeIfNeeded();

			Service.Address.Setup();

			if (Service.Address.LoadSuccessful) {
				Service.DataCache = new();
				Service.IconReplacer = new();
				Service.GameState = new();
				Service.ChatUtils = new();

				this.configWindow = new();
				this.windowSystem = new(this.GetType().Namespace!);
				this.windowSystem.AddWindow(this.configWindow);

				Service.Interface.UiBuilder.OpenConfigUi += this.toggleConfigUi;
				Service.Interface.UiBuilder.Draw += this.windowSystem.Draw;
			}
			else {
				Service.Commands.ProcessCommand("/xllog");
			}

			CommandInfo handler = new(this.onPluginCommand) {
				HelpMessage = Service.Address.LoadSuccessful ? "Open a window to edit custom combo settings." : "Do nothing, because the plugin failed to initialise.",
				ShowInHelp = true
			};

			Service.Commands.AddHandler(command + "vx", handler);
			if (Service.Configuration.RegisterCommonCommand) {
				Service.Commands.AddHandler(command, handler);
				this.registeredDefaultCommand = true;
			}

			PluginLog.Information($"{this.Name} v{Version} {(Debug ? "(debug build) " : "")}initialised {(Service.Address.LoadSuccessful ? "" : "un")}successfully");
			if (Service.Configuration.IsFirstRun) {
				Service.UpdateAlert = new(null, Version);

				Service.Configuration.LastVersion = Version;
				Service.Configuration.Save();
			}
			else if (!Service.Configuration.LastVersion.Equals(Version)) {
				PluginLog.Information("This is a different version than was last loaded - features may have changed.");

				Service.UpdateAlert = new(Service.Configuration.LastVersion, Version);

				Service.Configuration.LastVersion = Version;
				Service.Configuration.Save();
			}

		}

		#region Disposable

		public void Dispose() {
			this.dispose(true);
			GC.SuppressFinalize(this);
		}

		private void dispose(bool disposing) {
			if (this.disposed)
				return;
			this.disposed = true;

			if (disposing) {
				Service.Commands.RemoveHandler(command + "vx");
				if (this.registeredDefaultCommand)
					Service.Commands.RemoveHandler(command);

				Service.Interface.UiBuilder.OpenConfigUi -= this.toggleConfigUi;
				if (this.windowSystem is not null)
					Service.Interface.UiBuilder.Draw -= this.windowSystem.Draw;

				Service.IconReplacer?.Dispose();
				Service.DataCache?.Dispose();
				Service.UpdateAlert?.Dispose();
				Service.ChatUtils?.Dispose();
				Service.GameState?.Dispose();
				Service.Logger?.Dispose();
			}
		}

		#endregion


		internal void toggleConfigUi() {
			if (this.configWindow is not null) {
				this.configWindow.IsOpen = !this.configWindow.IsOpen;
			}
			else {
				PluginLog.Error("Cannot toggle configuration window, reference does not exist");
			}
		}

		internal void onPluginCommand(string command, string arguments) {
			if (!Service.Address.LoadSuccessful) {
				Service.ChatGui.PrintError($"The plugin failed to initialise and cannot run:\n{Service.Address.LoadFailReason!.Message}");
				return;
			}

			string[] argumentsParts = arguments.Split();

			switch (argumentsParts[0]) {
				case "debug": {
						Service.Logger.EnableNextTick();
						Service.ChatGui.Print("Enabled debug message snapshot");
					}
					break;
				case "reset": {
						PluginConfiguration config = new();
						config.IsFirstRun = false;
						config.LastVersion = XIVComboVX.Version;
						Service.Configuration = config;
						config.Save();
						List<Payload> parts = new(new Payload[] {
							new TextPayload("Your "),
							new UIForegroundPayload(35),
							new TextPayload(Service.Plugin.Name),
							new UIForegroundPayload(0),
							new TextPayload("configuration has been reset to the defaults.")
						});
						if (this.configWindow is not null && !this.configWindow.IsOpen) {
							parts.AddRange(new Payload[] {
								new TextPayload("\nYou will need to "),
								new UIForegroundPayload(ChatUtil.clfgOpenConfig),
								new UIGlowPayload(ChatUtil.clbgOpenConfig),
								Service.ChatUtils.clplOpenConfig,
								new TextPayload($"[open the settings]"),
								RawPayload.LinkTerminator,
								new UIGlowPayload(0),
								new UIForegroundPayload(0),
								new TextPayload(" to enable your desired features.")
							});
						}
						Service.ChatUtils.print(XivChatType.SystemMessage, parts.ToArray());
					}
					break;
				case "showUpdate": {
						Service.UpdateAlert?.displayMessage();
					}
					break;
				default:
					this.toggleConfigUi();
					break;
			}

			Service.Interface.SavePluginConfig(Service.Configuration);
		}
	}
}
