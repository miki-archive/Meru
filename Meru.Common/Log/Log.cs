using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Common.Log
{
    public static class Log
    {
		public static event Action<string> OnLogReceived;

		/// <summary>
		/// Prints text to the event objects
		/// </summary>
		/// <param name="fmt">text you want to print</param>
		public static void Print(string msg)
		{
			OnLogReceived.Invoke(msg);

		}

		/// <summary>
		/// Prints text to the event objects
		/// </summary>
		/// <param name="fmt">formatted text: use {0}, {1} to assign objects</param>
		/// <param name="objects">any object that has a ToString function</param>
		public static void Print(string fmt, params object[] objects)
		{
			OnLogReceived.Invoke(string.Format(fmt, objects));
		}

		/// <summary>
		/// Prints a line to the event objects
		/// </summary>
		/// <param name="fmt">text you want to print</param>
		public static void PrintLine(string msg)
		{
			OnLogReceived.Invoke(msg + "\n");
		}

		/// <summary>
		/// Prints a line to the event objects
		/// </summary>
		/// <param name="fmt">formatted text: use {0}, {1} to assign objects</param>
		/// <param name="objects">any object that has a ToString function</param>
		public static void PrintLine(string fmt, params object[] objects)
		{
			OnLogReceived.Invoke(string.Format(fmt + "\n", objects));
		}
	}
}