/*
	Copyright (c) 2015 Denis Zykov

	This is part of Charon Game Data Editor Unity Plugin.

	Charon Game Data Editor Unity Plugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Unity.Charon.Editor
{
	[InitializeOnLoad, Serializable]
	class Settings : ScriptableObject
	{
		public const string PREF_PREFIX = "Charon_";
		public const string SETTINGS_PATH = "Assets/Unity.Charon/Editor/Settings.asset";
		public const string LIST_SPLITTER = ";";
		public readonly static char[] ListSplitterChars = LIST_SPLITTER.ToArray();

		public static Settings Current;

		public string ToolsPath;
		public string BrowserPath;
		public Browser Browser;
		public int ToolsPort;
		public List<string> GameDataPaths;
		public bool Verbose;
		public bool SuppressRecoveryScripts;
		[HideInInspector]
		public int Version;

		static Settings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<Settings>(SETTINGS_PATH);

			if (settings == null)
			{
				settings = ScriptableObject.CreateInstance<Settings>();
				settings.ToolsPort = 43210;
				settings.ToolsPath = (from id in AssetDatabase.FindAssets("t:DefaultAsset Charon")
									  let path = FileUtils.MakeProjectRelative(AssetDatabase.GUIDToAssetPath(id))
									  where path != null && path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
									  select path).FirstOrDefault();
				settings.GameDataPaths = (from id in AssetDatabase.FindAssets("t:TextAsset GameData")
										  let path = FileUtils.MakeProjectRelative(AssetDatabase.GUIDToAssetPath(id))
										  where path != null && path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
										  select path).ToList();
				settings.Verbose = true;
			}
			settings.Validate();

			if ((EditorGUIUtility.SerializeMainMenuToString() ?? "").Contains("Charon"))
				UnityEditor.Menu.SetChecked("Tools/Charon/Verbose Logs", settings.Verbose);


			if ((EditorGUIUtility.SerializeMainMenuToString() ?? "").Contains("Charon"))
				UnityEditor.Menu.SetChecked("Tools/Charon/Recovery Scripts", !settings.SuppressRecoveryScripts);

			Current = settings;
		}
		public void Save()
		{
			this.Validate();

			try
			{
				if (AssetDatabase.LoadAssetAtPath<Settings>(SETTINGS_PATH) == null)
					AssetDatabase.CreateAsset(this, SETTINGS_PATH);
				AssetDatabase.SaveAssets();
			}
			catch (Exception e)
			{
				Debug.LogError(string.Format("Failed to save settings in file '{0}'.", SETTINGS_PATH));
				Debug.LogError(e);
			}

			if (this.SuppressRecoveryScripts)
				RecoveryScripts.Clear();
			else
				RecoveryScripts.Generate();
		}

		public void Validate()
		{
			if (this.GameDataPaths == null) this.GameDataPaths = new List<string>();
			var newPaths = this.GameDataPaths
				.Select<string, string>(FileUtils.MakeProjectRelative)
				.Where(p => !string.IsNullOrEmpty(p) && File.Exists(p))
				.Distinct()
				.ToList();

			if (!newPaths.SequenceEqual(this.GameDataPaths))
			{
				this.GameDataPaths = newPaths;
				this.Version++;
			}

			this.ToolsPath = FileUtils.MakeProjectRelative(this.ToolsPath) ?? ToolsPath;

			if (this.ToolsPort < 5000)
			{
				this.ToolsPort = 5000;
				this.Version++;
			}
			if (this.ToolsPort > 65535)
			{
				this.ToolsPort = 65535;
				this.Version++;
			}
		}

		public override string ToString()
		{
			return "Tools Path: " + this.ToolsPath + Environment.NewLine + " " +
				   "Tool Port: " + this.ToolsPort + Environment.NewLine + " " +
				   "Game Data Paths: " + string.Join(", ", this.GameDataPaths.ToArray()) + Environment.NewLine + " ";
		}
	}
}

