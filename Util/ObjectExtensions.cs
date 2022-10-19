using System;
using GObject = Godot.Object;

namespace JamToolkit.Util
{
	public static class ObjectExtensions
	{
		public static bool IsDisposed(this GObject @this) => @this == null || @this.NativeInstance == IntPtr.Zero;

		public static void SafeDisconnect(this GObject @this, string signal, GObject target, string method)
		{
			if (@this.IsDisposed()) return;
			if (!@this.IsConnected(signal, target, method)) return;

			@this.Disconnect(signal, target, method);
		}

		public static void SafeConnect(this GObject @this, string signal, GObject target, string method)
		{
			if (@this.IsDisposed()) return;
			if (@this.IsConnected(signal, target, method)) return;

			@this.Connect(signal, target, method);
		}
	}
}
