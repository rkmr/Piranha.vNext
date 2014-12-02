/*
 * Piranha CMS
 * Copyright (c) 2014, Håkan Edling, All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using System;
using System.Collections.Generic;

namespace Piranha.Log
{
	/// <summary>
	/// Temporary internal log provider used before the 
	/// log provider is configured. This is to ensure that modules
	/// don't call the logger before the application has initialized.
	/// </summary>
	internal class LogQueue : ILog
	{
		#region Inner classes
		/// <summary>
		/// Class for temporarily storing a log message in memory.
		/// </summary>
		private class LogMessage 
		{
			public LogLevel Level { get; set; }
			public string Message { get; set; }
			public Exception Exception { get; set; }
		}
		#endregion

		#region Members
		private IList<LogMessage> Messages = new List<LogMessage>();
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogQueue() { }

		/// <summary>
		/// Writes the given message to the log.
		/// </summary>
		/// <param name="level">The level</param>
		/// <param name="message">The log message</param>
		/// <param name="exception">The optional exception</param>
		public void Log(LogLevel level, string message, Exception exception = null) {
			Messages.Add(new LogMessage() {
				Level = level,
				Message = message,
				Exception = exception
			});
		}

		/// <summary>
		/// Dumps all log messages in the queue to the given log provider.
		/// </summary>
		/// <param name="log">The log provider</param>
		public void Dump(ILog log) {
			foreach (var msg in Messages)
				log.Log(msg.Level, msg.Message, msg.Exception);
			Messages.Clear();
		}
	}
}
