﻿/*
	Copyright (c) 2017 Denis Zykov

	This is part of "Charon: Game Data Editor" Unity Plugin.

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
using UnityEditor;
using UnityEngine;

namespace GameDevWare.Charon.Utils
{
	internal static class ProgressUtils
	{
		public static Action<string, float> ShowProgressBar(string title, float from = 0.0f, float to = 1.0f)
		{
			return (t, p) => EditorUtility.DisplayProgressBar(title, t, Mathf.Clamp(from + (to - from) * Mathf.Clamp(p, 0.0f, 1.0f), 0.0f, 1.0f));
		}
		public static Action<string, float> ShowCancellableProgressBar(string title, float from = 0.0f, float to = 1.0f)
		{
			return (t, p) => EditorUtility.DisplayCancelableProgressBar(title, t, Mathf.Clamp(from + (to - from) * Mathf.Clamp(p, 0.0f, 1.0f), 0.0f, 1.0f));
		}
		public static void HideProgressBar(object state)
		{
			EditorUtility.ClearProgressBar();
		}
		public static Action<string, float> ReportToLog(string prefix)
		{
			return (t, p) => Debug.Log(prefix + t);
		}
	}
}
