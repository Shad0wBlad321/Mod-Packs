namespace MechanoidNightVision
{
	using System.IO;
	using System.Reflection;
	using HarmonyLib;
	using Verse;

	internal class MechanoidNightVision : Mod
	{
		private static readonly string Identifier = "MechanoidNightVision";
		internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier($"winggar.{Identifier}").RootDir.FullName, "Version.txt");
		public static string CurrentVersion { get; private set; }

		public MechanoidNightVision(ModContentPack content) : base(content)
		{
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

			if (Prefs.DevMode)
			{
				File.WriteAllText(VersionDir, CurrentVersion);
			}

			new Harmony($"winggar.{Identifier}").PatchAll();
		}
	}
}