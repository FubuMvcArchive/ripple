using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace ripple
{
	public class RippleLog
	{
		private static readonly Lazy<ILogger> _logger;
		private static readonly IList<ILogListener> Listeners;

		static RippleLog()
		{
			Listeners = new List<ILogListener>();
			_logger = new Lazy<ILogger>(() => new Logger(Listeners, new ILogModifier[0]));

			RegisterListener(new RippleLogger());
			// TODO -- Add the file writing logger
		}

		private static ILogger Logger { get { return _logger.Value; } }

		public static void RegisterListener(ILogListener listener)
		{
			Listeners.Add(listener);
		}

		public static void DebugMessage(LogTopic message)
		{
			Logger.DebugMessage(message);
		}

		public static void InfoMessage(LogTopic message)
		{
			Logger.InfoMessage(message);
		}

		public static void Debug(string message)
		{
			Logger.Debug(message);
		}

		public static void Info(string message)
		{
			Logger.Info(message);
		}

		public static void Error(string message, Exception ex)
		{
			Logger.Error(message, ex);
		}

		public class RippleLogger : ILogListener
		{
			public bool ListensFor(Type type)
			{
				return true;
			}

			public void Debug(string message)
			{
				Console.WriteLine(message);
			}

			public void DebugMessage(object message)
			{
				Debug(message.ToDescriptionText());
			}

			public void Info(string message)
			{
				writeWithColor(ConsoleColor.Cyan, () => Console.WriteLine(message));
			}

			public void InfoMessage(object message)
			{
				Info(message.ToDescriptionText());
			}

			public void Error(string message, Exception ex)
			{
				writeWithColor(ConsoleColor.Red, () =>
				{
					Console.WriteLine(message);
					Console.WriteLine(ex.ToDescriptionText());
				});
			}

			public void Error(object correlationId, string message, Exception ex)
			{
				Error(message, ex);
			}

			private void writeWithColor(ConsoleColor color, Action action)
			{
				Console.ForegroundColor = color;
				action();
				Console.ResetColor();
			}

			public bool IsDebugEnabled { get { return true; } }
			public bool IsInfoEnabled { get { return true; } }
		}
	}
}