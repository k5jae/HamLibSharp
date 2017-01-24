//
//  Rig.cs
//
//  Author:
//       Jae Stutzman <jaebird@gmail.com>
//
//  Copyright (c) 2016 Jae Stutzman
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using HamLibSharp.Utils;
using HamLibSharp.x86;
using HamLibSharp.x64;

namespace HamLibSharp
{
	internal interface INativeRig
	{
		IntPtr Caps { get; }

		IRigStateNative State { get; }

		IntPtr Callbacks { get; }
	};

	public partial class Rig : IDisposable
	{
		const int CommErrors = 10;
		// milliseconds
		const int UpdateRate = 250;
		const string ConfTokenDevice = "rig_pathname";
		const string ConfTokenSerialSpeed = "serial_speed";
		const string ConfTokenDatabits = "data_bits";
		const string ConfTokenStopbits = "stop_bits";
		const string ConfTokenSerialParity = "serial_parity";
		const string ConfTokenSerialHandshake = "serial_handshake";
		const string ConfTokenDataBits = "data_bits";


		IntPtr theRig;
		INativeRig nativeRig;
		RigCaps rigCaps;

		public RigCaps Caps { get { return rigCaps; } }

		bool disposed;
		int errorCount;

		BlockingCollection<Action> taskQueue;
		System.Timers.Timer timer;
		Thread thread;
		int updateRate;
		volatile bool commErrorClose = false;

		public event EventHandler CommErrorClose;

		/// <summary>
		/// Gets the unique model identifier
		/// </summary>
		/// <value>The model id.</value>
		public int Model {
			get {
				return rigCaps.RigModel;
			}
		}

		/// <summary>
		/// Gets the name of the manufacturer.
		/// </summary>
		/// <value>The name of the manufacturer.</value>
		public string MfgName {
			get {
				return rigCaps.MfgName;
			}
		}

		/// <summary>
		/// Gets the name of the model.
		/// </summary>
		/// <value>The name of the model.</value>
		public string ModelName {
			get {
				return rigCaps.ModelName;
			}
		}

		/// <summary>
		/// Gets the version of the backend.
		/// </summary>
		/// <value>The version.</value>
		public string Version {
			get {
				return rigCaps.Version;
			}
		}

		/// <summary>
		/// Gets the max serial baud rate.
		/// </summary>
		/// <value>The serial rate.</value>
		public int SerialRateMax {
			get {
				return rigCaps.SerialRateMax;
			}
		}

		/// <summary>
		/// Gets the min serial baud rate.
		/// </summary>
		/// <value>The serial rate.</value>
		public int SerialRateMin {
			get {
				return rigCaps.SerialRateMin;
			}
		}

		/// <summary>
		/// Gets the current serial baud rate.
		/// </summary>
		/// <value>The serial rate.</value>
		public int SerialRate {
			get {
				if (rigOpen) {
					return int.Parse (GetConf (ConfTokenSerialSpeed));
				} else {
					return 0;
				}
			}
		}

		/// <summary>
		/// Gets the rig path (i.e. serial port).
		/// </summary>
		/// <value>The rig path.</value>
		public string RigPath {
			get {
				if (rigOpen) {
					return GetConf (ConfTokenDevice);
				} else {
					return string.Empty;
				}
			}
		}

		double freq;

		/// <summary>
		/// Gets the current frequency of the rig. 
		/// Last updated by call to UpdateFrequency().
		/// </summary>
		/// <value>The frequency in Hz.</value>
		public double Freq {
			get {
				if (updateRate > 0) {
					lock (this) {
						return freq;
					}
				} else {
					return GetFrequency ();
				}
			}
			private set {
				lock (this) {
					freq = value;
				}
			}
		}

		long width;
		RigMode mode;
		public RigMode Mode {
			get {
				if (updateRate > 0) {
					lock (this) {
						return mode;
					}
				} else {
					return GetMode (ref width);
				}
			}
			private set {
				lock (this) {
					mode = value;
				}
			}
		}

		public string ModeText {
			get {
			 	return TextNameAttribute.GetTextName (Mode);
			}
		}

		volatile PttMode ptt;

		/// <summary>
		/// Gets the Push-To-Talk status.
		/// Last updated by call to UpdatePtt().
		/// </summary>
		/// <value>The PTT status.</value>
		public PttMode Ptt {
			get {
				if (updateRate > 0) {
					lock (this) {
						return ptt;
					}
				} else {
					return GetPtt ();
				}
			}
			private set {
				lock (this) {
					ptt = value;
				}
			}
		}

		volatile string lastStatus;

		/// <summary>
		/// Gets the last status.
		/// </summary>
		/// <value>The last status.</value>
		public string LastStatus {
			get {
				lock (this) {
					return lastStatus;
				}
			}
			private set {
				lock (this) {
					lastStatus = value;
				}
			}
		}

		volatile bool rigOpen;

		/// <summary>
		/// Gets a value indicating whether this <see cref="HamLibSharp.Rig"/> rig open.
		/// </summary>
		/// <value><c>true</c> if rig open; otherwise, <c>false</c>.</value>
		public bool RigOpen {
			get {
				lock (this) {
					return rigOpen;
				}
			}
			private set {
				lock (this) {
					rigOpen = value;
				}
			}
		}

		public RigMode ModeList {
			get {
				return nativeRig.State.Mode_list;
			}
		}

		public int VfoList {
			get {
				return nativeRig.State.Vfo_list;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HamLibSharp.Rig"/> class.
		/// This constructor attempts to find the correct backend based on input name.
		/// </summary>
		/// <param name="rigModel">Rig model name.</param>
		public Rig (string rigModel, int updateRate = UpdateRate)
		{
			this.updateRate = updateRate < UpdateRate ? UpdateRate : updateRate;

			if (!HamLib.Initialized) {
				HamLib.Initialize ();
			}

			// Look up model from name 
			RigCaps rig;
			if (!HamLib.Rigs.TryGetValue (rigModel, out rig)) {
				throw new RigException ("Rig model not found");
			}

			// init the underlying rig and library
			InitRig (rig.RigModel);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HamLibSharp.Rig"/> class.
		/// This constructor initializing the rig backend based on input model id.
		/// </summary>
		/// <param name="rigModel">Rig model id.</param>
		public Rig (int rigModel, int updateRate = UpdateRate)
		{
			this.updateRate = updateRate < UpdateRate ? UpdateRate : updateRate;

			if (!HamLib.Initialized) {
				HamLib.Initialize ();
			}

			InitRig (rigModel);
		}

		private void InitRig (int rigModel)
		{
			theRig = rig_init (rigModel);
			if (theRig == IntPtr.Zero)
				throw new RigException ("Rig initialization error");

			nativeRig = MarshalNativeRig (theRig);
			var caps = HamLib.MarshalRigCaps (nativeRig.Caps);
			rigCaps = new RigCaps (caps, this);

			// test...
			//Console.WriteLine("TEST>>>>>>");
			//Console.WriteLine (nativeRig.State.Itu_region);
			//Console.WriteLine ("HamLibPortNative: {0}", Marshal.SizeOf<HamLibPortNative> ());

			// end test....

		}

		internal static INativeRig MarshalNativeRig (IntPtr rig_ptr)
		{
			INativeRig rig = null;

			switch (HamLib.hamLibVersion) {
			case HamLibVersion.Current:
			case HamLibVersion.V301:
				// if the platform is 64-bit, but not windows
				if (!HamLib.isWindows && HamLib.bitsize64) {
					rig = Marshal.PtrToStructure<NativeRig64> (rig_ptr);
				} else {
					rig = Marshal.PtrToStructure<NativeRig32> (rig_ptr);
				}
				break;
			case HamLibVersion.V2:
				// if the platform is 64-bit, but not windows
				if (!HamLib.isWindows && HamLib.bitsize64) {
					rig = Marshal.PtrToStructure<NativeRig64v2> (rig_ptr);
				} else {
					rig = Marshal.PtrToStructure<NativeRig32v2> (rig_ptr);
				}
				break;
			default:
				throw new RigException ("Unknown or Incompatible HamLib library found");
			}

			return rig;
		}

		private static int OnFrequency (IntPtr theRig, int vfo, double freq, IntPtr rig_ptr)
		{
			// Console.WriteLine (freq);

			return 1;
		}

		private void Dispose (bool disposing)
		{
			if (!this.disposed) {
				if (disposing) {
					// Dispose here any managed resources
				}

				if (theRig != IntPtr.Zero) {
					Stop ();
					Close ();

					var ret = rig_cleanup (theRig);
					if (ret != RigError.OK) {
						throw new RigException (ErrorString (ret));
					}
					theRig = IntPtr.Zero;
					rigCaps = null;
					nativeRig = null;
				}
			}
			disposed = true;         
		}

		/// <summary>
		/// Releases all resource used by the <see cref="HamLibSharp.Rig"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="HamLibSharp.Rig"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="HamLibSharp.Rig"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="HamLibSharp.Rig"/> so the garbage
		/// collector can reclaim the memory that the <see cref="HamLibSharp.Rig"/> was occupying.</remarks>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~Rig ()
		{
			Dispose (false);
		}

		/// <summary>
		/// Opens communication to a radio. If it is a serial connected rig, 
		/// searches for the correct baud rate.
		/// </summary>
		/// <param name="device">Device path (i.e. serial port).</param>
		public void Open (string devicePath)
		{
			if (rigCaps.PortType == RigPort.Serial) {
				// find the baud...
				var foundBaud = false;
				foreach (int baud in Enum.GetValues(typeof(RigSerialBaudRate))) {
					if (baud >= SerialRateMin && baud <= SerialRateMax) {
						try {
							SetConf (ConfTokenSerialSpeed, baud);
							OpenInternal (devicePath);
							// throws exception if IO error
							GetPtt ();
							foundBaud = true;
							break;
						} catch (RigException) {
							Close ();
						}
					}
				}
				if (!foundBaud) {
					throw new RigException ("Unable to communicate with rig");
				}
			} else {
				OpenInternal (devicePath);
			}
		}

		/// <summary>
		/// Opens communication to a radio.
		/// </summary>
		/// <param name="device">Device path (i.e. serial port).</param>
		/// <param name="baud">baud rate of serial port.</param>
		public void Open (string device, RigSerialBaudRate baud, RigSerialHandshake handshake, int databits, int stopbits)
		{
			if (rigCaps.PortType == RigPort.Serial) {
				SetConf (ConfTokenSerialSpeed, (int)baud);
				SetConf (ConfTokenSerialHandshake, TextNameAttribute.GetTextName(handshake));
				SetConf (ConfTokenDatabits, databits);
				SetConf (ConfTokenStopbits, stopbits);

				OpenInternal (device);
			} else {
				throw new RigException (string.Format ("Device is not serial, but {0}", rigCaps.PortType));
			}
		}

		/// <summary>
		/// Open communication to a radio
		/// </summary>
		public void Open ()
		{
			OpenInternal (null);
		}

		private void OpenInternal (string device)
		{
			if (device != null && device != string.Empty) {
				SetConf (ConfTokenDevice, device);
			}

			var ret = rig_open (theRig);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
			rigOpen = true;

			// untested since I don't have a rig that supports this
			ret = rig_set_freq_callback (theRig, OnFrequency, IntPtr.Zero);
			if (ret != RigError.OK) {
				LastStatus = ErrorString (ret);
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Closes communication to a radio that was previously open with Open().
		/// </summary>
		public void Close ()
		{
			if (rigOpen) {
				var ret = rig_close (theRig);
				if (ret != RigError.OK) {
					LastStatus = ErrorString (ret);
					throw new RigException (ErrorString (ret));
				}
				Freq = 0;
				rigOpen = false;
			}
		}

		/// <summary>
		/// Start will create a timer and a thread to manage the invocation
		/// of calls to the rig. This is useful when an application cannot
		/// block on rig calls. If Start is not called, each call is 
		/// called syncronously.
		/// The timer will begin calling UpdateFrequency() and UpdatePtt()
		/// to update the state properties on a periodic basis.
		/// </summary>
		public void Start ()
		{
			commErrorClose = false;
			errorCount = 0;
			taskQueue = new BlockingCollection<Action> ();

			lock (this) {
				try {
					Freq = GetFrequency (RigVfo.Current);
				} catch (RigException ex) {
					LastStatus = ex.Message;
					Freq = 0;
				}
			}

			if (updateRate > 0) {
				timer = new System.Timers.Timer (updateRate);
				timer.AutoReset = false;
				timer.Elapsed += Timer_Elapsed;
				timer.Enabled = true;
				timer.Start ();
			}

			thread = new Thread (TaskThread);
			thread.Start ();
		}

		void Timer_Elapsed (object sender, System.Timers.ElapsedEventArgs e)
		{
			//logger.Debug ("Timer elapsed, Id: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

			if (commErrorClose) {
				Stop ();
				Close ();
				if (CommErrorClose != null) {
					CommErrorClose (this, new EventArgs ());
				}
			} else {
				UpdateFrequency (RigVfo.Current);
				UpdateMode (RigVfo.Current);
				UpdatePtt (RigVfo.Current);
				timer.Enabled = true;
			}
		}

		/// <summary>
		/// Stop the thread and timer that was started by the
		/// call to Start().
		/// </summary>
		public void Stop ()
		{
			if (timer != null) {
				timer.Dispose ();
				timer = null;
			}

			if (taskQueue != null) {
				// this will cause thread to exit by throwing InvalidOperationException
				taskQueue.CompleteAdding ();

				// wait for taskQueue thread to exit
				thread.Join ();
				thread = null;
			}
		}

		private void TaskThread ()
		{
			var running = true;
			while (running) {
				try {
					taskQueue.Take ().Invoke ();
					errorCount = 0;
					LastStatus = string.Empty;
				} catch (InvalidOperationException) {
					running = false;
					taskQueue = null;
				} catch (RigException e) {
					LastStatus = e.Message;
					errorCount++;
				}

				if (errorCount > CommErrors) {
					commErrorClose = true;
				}
			}
		}

		/// <summary>
		/// Sets a radio configuration parameter.
		/// </summary>
		/// <param name="token">Parameter Token.</param>
		/// <param name="val">Value to set.</param>
		public void SetConf (int token, string val)
		{
			var ret = rig_set_conf (theRig, token, val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Sets a radio configuration parameter.
		/// </summary>
		/// <param name="token">Parameter Token.</param>
		/// <param name="val">Value to set.</param>
		public void SetConf (int token, int val)
		{
			var ret = rig_set_conf (theRig, token, val.ToString ());
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Sets a radio configuration parameter.
		/// </summary>
		/// <param name="name">Name of parameter.</param>
		/// <param name="val">Value to set.</param>
		public void SetConf (string name, string val)
		{
			var ret = rig_set_conf (theRig, TokenLookup (name), val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Sets a radio configuration parameter.
		/// </summary>
		/// <param name="name">Name of parameter.</param>
		/// <param name="val">Value to set.</param>
		public void SetConf (string name, int val)
		{
			var ret = rig_set_conf (theRig, TokenLookup (name), val.ToString ());
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the value of a configuration parameter associated with token.
		/// </summary>
		/// <returns>The configuration parameter.</returns>
		/// <param name="token">Parameter Token.</param>
		public string GetConf (int token)
		{
			string val;

			var ret = rig_get_conf (theRig, token, out val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return val;
		}

		/// <summary>
		/// Retrieves the value of a configuration parameter associated with token.
		/// </summary>
		/// <returns>The configuration parameter.</returns>
		/// <param name="name">Name of parameter.</param>
		public string GetConf (string name)
		{
			string val;

			var ret = rig_get_conf (theRig, TokenLookup (name), out val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return val;
		}

		/// <summary>
		/// Simple lookup returning token id assicated with name.
		/// </summary>
		/// <returns>The token ID</returns>
		/// <param name="name">The name of the configuration parameter.</param>
		public int TokenLookup (string name)
		{
			return rig_token_lookup (theRig, name);
		}

		/// <summary>
		/// Sets the frequency of the target VFO
		/// </summary>
		/// <param name="freq">Frequency in Hz.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetFrequency (double freq, int vfo = RigVfo.Current)
		{
			if (thread != null) {
				taskQueue.Add (() => setfreq (vfo, freq));
			} else {
				setfreq (vfo, freq);
			}
		}

		private void setfreq (int vfo, double freq)
		{
			var ret = rig_set_freq (theRig, vfo, freq);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the updated frequency from the targeted VFO.
		/// Will use Task Queue if Start() has been called.
		/// </summary>
		/// <param name="vfo">The target VFO.</param>
		public void UpdateFrequency (int vfo = RigVfo.Current)
		{
			if (thread != null) {
				taskQueue.Add (() => Freq = GetFrequency (vfo));
			} else {
				Freq = GetFrequency (vfo);
			}
		}

		/// <summary>
		/// Retrieves the updated frequency from the targeted VFO.
		/// Blocks until data received.
		/// </summary>
		/// <returns>The frequency in Hz.</returns>
		/// <param name="vfo">The target VFO.</param>
		public double GetFrequency (int vfo = RigVfo.Current)
		{
			double freq;

			var ret = rig_get_freq (theRig, vfo, out freq);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return freq;
		}

		/// <summary>
		/// Sets the mode and associated passband of the target VFO. 
		/// The passband width must be supported by the backend of the rig.
		/// </summary>
		/// <param name="mode">The mode to set to.</param>
		/// <param name="width">The passband width to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetMode (RigMode mode, long width, int vfo = RigVfo.Current)
		{
			if (thread != null) {
				taskQueue.Add (() => setmode (mode, width, vfo));
			} else {
				setmode (mode, width, vfo);
			}
		}

		private void setmode (RigMode mode, long width, int vfo = RigVfo.Current)
		{
			var ret = rig_set_mode (theRig, vfo, mode, width);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the updated mode and passband of the target VFO.
		/// Will use Task Queue if Start() has been called.
		/// </summary>
		/// <param name="vfo">The target VFO.</param>
		public void UpdateMode (int vfo = RigVfo.Current)
		{
			if (thread != null) {
				taskQueue.Add (() => Mode = GetMode(ref width, vfo));
			} else {
				Mode = GetMode(ref width, vfo);
			}
		}

		/// <summary>
		/// Retrieves the mode and passband of the target VFO. If the backend is 
		/// unable to determine the width, the width will be set to RIG_PASSBAND_NORMAL 
		/// as a default. The value stored at mode location equals RIG_MODE_NONE when 
		/// the current mode of the VFO is not defined (e.g. blank memory).
		/// </summary>
		/// <returns>The current mode.</returns>
		/// <param name="vfo">The target VFO.</param>
		/// <param name="width">Retrieves the current passband width.</param>
		public RigMode GetMode (ref long width, int vfo = RigVfo.Current)
		{
			RigMode mode;

			var ret = rig_get_mode (theRig, vfo, out mode, out width);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return mode;
		}

		/// <summary>
		/// Sets the current VFO. The VFO can be RIG_VFO_A, RIG_VFO_B, RIG_VFO_C 
		/// for VFOA, VFOB, VFOC respectively or RIG_VFO_MEM for Memory mode. 
		/// Supported VFOs depends on rig capabilities.
		/// </summary>
		/// <param name="vfo">The target VFO.</param>
		public void SetVFO (int vfo = RigVfo.Current)
		{
			var ret = rig_set_vfo (theRig, vfo);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current VFO. The VFO can be RIG_VFO_A, RIG_VFO_B, 
		/// RIG_VFO_C for VFOA, VFOB, VFOC respectively or RIG_VFO_MEM for 
		/// Memory mode. Supported VFOs depends on rig capabilities.
		/// </summary>
		/// <returns>The current VFO.</returns>
		public int GetVFO ()
		{
			int vfo;

			var ret = rig_get_vfo (theRig, out vfo);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return vfo;
		}

		/// <summary>
		/// Sets "Push-To-Talk" on/off.
		/// </summary>
		/// <param name="ptt">The PTT status to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetPtt (PttMode ptt, int vfo = RigVfo.Current)
		{
			if (thread != null) {
				taskQueue.Add (() => SetPttInternal (ptt, vfo));
			} else {
				SetPttInternal (ptt, vfo);
			}
		}

		private void SetPttInternal (PttMode ptt, int vfo)
		{
			var ret = rig_set_ptt (theRig, vfo, ptt);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Updates the Ptt property
		/// </summary>
		/// <param name="vfo">The target VFO.</param>
		public void UpdatePtt (int vfo = RigVfo.Current)
		{
			if (thread != null) {
				taskQueue.Add (() => Ptt = GetPtt (vfo));
			} else {
				Ptt = GetPtt (vfo);
			}
		}

		/// <summary>
		/// Returns PttMode
		/// </summary>
		/// <returns>The ptt.</returns>
		/// <param name="vfo">The target VFO.</param>
		public PttMode GetPtt (int vfo = RigVfo.Current)
		{
			PttMode ptt;

			var ret = rig_get_ptt (theRig, vfo, out ptt);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return ptt;
		}

		/// <summary>
		/// Retrieves the status of DCD (is squelch open?).
		/// </summary>
		/// <returns>The status of the DCD.</returns>
		/// <param name="vfo">The target VFO.</param>
		public DcdState GetDCD (int vfo = RigVfo.Current)
		{
			DcdState dcd;

			var ret = rig_get_dcd (theRig, vfo, out dcd);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return dcd;
		}

		/// <summary>
		/// Sets the level of a setting. The level value val can be a float or an integer.
		/// </summary>
		/// <param name="level">The level setting.</param>
		/// <param name="val">The value to set the level setting to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetLevel (RigLevel level, int val, int vfo = RigVfo.Current)
		{
			var ret = rig_set_level (theRig, vfo, (uint)level, val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Sets the level of a setting. The level value val can be a float or an integer.
		/// </summary>
		/// <param name="level">The level setting.</param>
		/// <param name="val">The value to set the level setting to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetLevel (RigLevel level, float val, int vfo = RigVfo.Current)
		{
			var ret = rig_set_level (theRig, vfo, (uint)level, val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		private bool LevelIsFloat (RigLevel level)
		{
			return level == (level & (RigLevel.Volume | RigLevel.RF | RigLevel.Squelch | RigLevel.AudioPeakFilter |
			RigLevel.NoiseReduction | RigLevel.TwinPbtIn | RigLevel.TwinPbtOut | RigLevel.RFPower |
			RigLevel.MicGain | RigLevel.Compressor | RigLevel.Balance | RigLevel.Swr |
			RigLevel.Alc | RigLevel.VoxGain | RigLevel.AntiVox));
		}

		/// <summary>
		/// Retrieves the value of a level. The level value val can be a float or an integer.
		/// </summary>
		/// <param name="level">The level setting.</param>
		/// <param name="val">Get the value of the level setting.</param>
		/// <param name="vfo">The target VFO.</param>
		public void GetLevel (RigLevel level, out int val, int vfo = RigVfo.Current)
		{
			if (LevelIsFloat (level))
				throw new RigException (ErrorString (-(int)RigError.InvalidParameter));

			var ret = rig_get_level (theRig, vfo, (uint)level, out val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the value of a level. The level value val can be a float or an integer.
		/// </summary>
		/// <param name="level">The level setting.</param>
		/// <param name="val">Get the value of the level setting.</param>
		/// <param name="vfo">The target VFO.</param>
		public void GetLevel (RigLevel level, out float val, int vfo = RigVfo.Current)
		{
			if (!LevelIsFloat (level))
				throw new RigException (ErrorString (-(int)RigError.InvalidParameter));

			var ret = rig_get_level (theRig, vfo, (uint)level, out val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		private bool ParmIsFloat (RigParm parm)
		{
			return parm == (parm & (RigParm.Backlight | RigParm.BatteryLevel));
		}

		/// <summary>
		/// Sets a parameter. The parameter value val can be a float or an integer. 
		/// </summary>
		/// <param name="parm">The parameter.</param>
		/// <param name="val">The value to set the parameter.</param>
		public void SetParm (RigParm parm, int val)
		{
			var ret = rig_set_parm (theRig, (uint)parm, val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Sets a parameter. The parameter value val can be a float or an integer. 
		/// </summary>
		/// <param name="parm">The parameter.</param>
		/// <param name="val">The value to set the parameter.</param>
		public void SetParm (RigParm parm, float val)
		{
			var ret = rig_set_parm (theRig, (uint)parm, val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the value of a parm. The parameter value val can be a float or an integer.
		/// </summary>
		/// <param name="parm">The parameter.</param>
		/// <param name="val">The retrieved value.</param>
		public void GetParm (RigParm parm, out int val)
		{
			if (ParmIsFloat (parm))
				throw new RigException (ErrorString (-(int)RigError.InvalidParameter));

			var ret = rig_get_parm (theRig, (uint)parm, out val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the value of a parm. The parameter value val can be a float or an integer.
		/// </summary>
		/// <param name="parm">The parameter.</param>
		/// <param name="val">The retrieved value.</param>
		public void GetParm (RigParm parm, out float val)
		{
			if (!ParmIsFloat (parm))
				throw new RigException (ErrorString (-(int)RigError.InvalidParameter));

			var ret = rig_get_parm (theRig, (uint)parm, out val);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Sets the split(TX) frequency.
		/// </summary>
		/// <param name="tx_freq">Tx frequency.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetSplitFreq (double tx_freq, int vfo = RigVfo.Current)
		{
			var ret = rig_set_split_freq (theRig, vfo, tx_freq);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current split(TX) frequency.
		/// </summary>
		/// <returns>The split freq.</returns>
		/// <param name="vfo">The target VFO.</param>
		public double GetSplitFreq (int vfo = RigVfo.Current)
		{
			double tx_freq;

			var ret = rig_get_split_freq (theRig, vfo, out tx_freq);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return tx_freq;
		}

		/// <summary>
		/// Sets the split(TX) mode.
		/// </summary>
		/// <param name="mode">The transmit split mode to set to.</param>
		/// <param name="width">The transmit split width to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetSplitMode (RigMode mode, int width, int vfo = RigVfo.Current)
		{
			var ret = rig_set_split_mode (theRig, vfo, mode, width);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current split(TX) mode and passband. If the backend is 
		/// unable to determine the width, the tx_width will be set to RIG_PASSBAND_NORMAL 
		/// as a default. The value stored at tx_mode location equals RIG_MODE_NONE when 
		/// the current mode of the VFO is not defined (e.g. blank memory).
		/// </summary>
		/// <returns>Retrieve the current split mode.</returns>
		/// <param name="width">Retrieve the current split width.</param>
		/// <param name="vfo">The target VFO.</param>
		public RigMode GetSplitMode (out int width, int vfo = RigVfo.Current)
		{
			RigMode mode;

			var ret = rig_get_split_mode (theRig, vfo, out mode, out width);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return mode;
		}

		/// <summary>
		/// Sets the current split mode.
		/// </summary>
		/// <param name="split">The split mode to set to.</param>
		/// <param name="tx_vfo">The target Tx VFO.</param>
		/// <param name="rx_vfo">The target Rx VFO.</param>
		public void SetSplitVFO (RigSplit split, int tx_vfo, int rx_vfo = RigVfo.Current)
		{
			var ret = rig_set_split_vfo (theRig, rx_vfo, split, tx_vfo);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current split mode.
		/// </summary>
		/// <returns>The split mode.</returns>
		/// <param name="tx_vfo">Retrieves the transmit VFO.</param>
		/// <param name="rx_vfo">Rx vfo.</param>
		public RigSplit GetSplitVFO (out int tx_vfo, int rx_vfo = RigVfo.Current)
		{
			RigSplit split;

			var ret = rig_get_split_vfo (theRig, rx_vfo, out split, out tx_vfo);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return split;
		}

		/// <summary>
		/// Checks if a rig is capable of *getting* a level setting. Since the level
		/// is an OR'ed bitwise argument, more than one level can be checked at the same time.
		/// </summary>
		/// <returns><c>true</c> if this instance has get level the specified level; otherwise, <c>false</c>.</returns>
		/// <param name="level">Level.</param>
		public bool HasGetLevel (uint level)
		{
			return rig_has_get_level (theRig, level) == level;
		}

		/// <summary>
		/// Checks if a rig can *set* a level setting. Since the level 
		/// is an OR'ed bitwise argument, more than one level can be check at the same time.
		/// </summary>
		/// <returns><c>true</c> if this instance has set level the 
		/// specified level; otherwise, <c>false</c>.</returns>
		/// <param name="level">Level.</param>
		public bool HasSetLevel (uint level)
		{
			return rig_has_set_level (theRig, level) == level;
		}

		/// <summary>
		/// Checks if a rig is capable of *getting* a parm setting. Since the 
		/// parm is an OR'ed bitwise argument, more than one parameter can 
		/// be checked at the same time.
		/// </summary>
		/// <returns><c>true</c> if this instance has get parm the 
		/// specified parm; otherwise, <c>false</c>.</returns>
		/// <param name="parm">Parm.</param>
		public bool HasGetParm (uint parm)
		{
			return rig_has_get_parm (theRig, parm) == parm;
		}

		/// <summary>
		/// Checks if a rig can *set* a parameter setting. Since the parm 
		/// is an OR'ed bitwise argument, more than one parameter can be 
		/// check at the same time.
		/// </summary>
		/// <returns><c>true</c> if this instance has set parm the 
		/// specified parm; otherwise, <c>false</c>.</returns>
		/// <param name="parm">Parm.</param>
		public bool HasSetParm (uint parm)
		{
			return rig_has_set_parm (theRig, parm) == parm;
		}

		/// <summary>
		/// Retrieves some general information from the radio. This can 
		/// include firmware revision, exact model name, or just nothing.
		/// </summary>
		/// <returns>The info.</returns>
		public string GetInfo ()
		{
			return RigGetInfo (theRig);
		}

		/// <summary>
		/// Returns the normal (default) passband for the given mode.
		/// </summary>
		/// <returns>The passband in Hz if the operation has been sucessful, or a 0 
		/// if an error occured</returns>
		/// <param name="mode">The mode to get the passband.</param>
		public int PassbandNormal (RigMode mode)
		{
			return rig_passband_normal (theRig, mode);
		}

		/// <summary>
		/// Returns the narrow (closest) passband for the given mode. 
		/// EXAMPLE: SetMode(RIG_MODE_LSB, PassbandNarrow(RIG_MODE_LSB));
		/// </summary>
		/// <returns>The passband in Hz if the operation has been sucessful, or a 0 
		/// if an error occured</returns>
		/// <param name="mode">The mode to get the passband.</param>
		public int PassbandNarrow (RigMode mode)
		{
			return rig_passband_narrow (theRig, mode);
		}

		/// <summary>
		/// Returns the wide (default) passband for the given mode. 
		/// EXAMPLE: SetMode(RIG_MODE_AM, PassbandWide(RIG_MODE_AM));
		/// </summary>
		/// <returns>The passband in Hz if the operation has been sucessful, or a 0 
		/// if an error occured</returns>
		/// <param name="mode">The mode to get the passband.</param>
		public int PassbandWide (RigMode mode)
		{
			return rig_passband_wide (theRig, mode);
		}

		/// <summary>
		/// Sets the current repeater shift.
		/// </summary>
		/// <param name="rptr_shift">The repeater shift to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetRepeaterShift (RepeaterShift rptr_shift, int vfo = RigVfo.Current)
		{
			if (thread != null) {
				taskQueue.Add (() => SetRepeaterShiftInternal (rptr_shift, vfo));
			} else {
				SetRepeaterShiftInternal (rptr_shift, vfo);

			}
		}

		private void SetRepeaterShiftInternal (RepeaterShift rptr_shift, int vfo = RigVfo.Current)
		{
			var ret = rig_set_rptr_shift (theRig, vfo, rptr_shift);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current repeater shift.
		/// </summary>
		/// <returns>The repeater shift.</returns>
		/// <param name="vfo">The target VFO.</param>
		public RepeaterShift GetRepeaterShift (int vfo = RigVfo.Current)
		{
			RepeaterShift rptr_shift;

			var ret = rig_get_rptr_shift (theRig, vfo, out rptr_shift);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return rptr_shift;
		}

		/// <summary>
		/// Sets the current repeater offset.
		/// </summary>
		/// <param name="rptr_offs">Repeater offset.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetRepeaterOffset (int rptr_offs, int vfo = RigVfo.Current)
		{
			var ret = rig_set_rptr_offs (theRig, vfo, rptr_offs);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current repeater offset.
		/// </summary>
		/// <returns>The repeater offset.</returns>
		/// <param name="vfo">The target VFO.</param>
		public int GetRepeaterOffset (int vfo = RigVfo.Current)
		{
			int rptr_offs;

			var ret = rig_get_rptr_offs (theRig, vfo, out rptr_offs);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
				
			return rptr_offs;
		}

		/// <summary>
		/// Sets the Tuning Step.
		/// </summary>
		/// <param name="ts">Tuning step to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetTuningStep (int ts, int vfo = RigVfo.Current)
		{
			var ret = rig_set_ts (theRig, vfo, ts);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current tuning step.
		/// </summary>
		/// <returns>The tuning step.</returns>
		/// <param name="vfo">The target VFO.</param>
		public int GetTuningStep (int vfo = RigVfo.Current)
		{
			int ts;

			var ret = rig_get_ts (theRig, vfo, out ts);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return ts;
		}

		/// <summary>
		/// Sets the current Continuous Tone Controlled Squelch 
		/// System (CTCSS) sub-audible tone frequency. 
		/// </summary>
		/// <param name="tone">Tone to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetCTCSS (uint tone, int vfo = RigVfo.Current)
		{
			var ret = rig_set_ctcss_tone (theRig, vfo, tone);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current Continuous Tone Controlled Squelch 
		/// System (CTCSS) sub-audible tone frequency.
		/// </summary>
		/// <returns>The current tone.</returns>
		/// <param name="vfo">The target VFO.</param>
		public uint GetCTCSS (int vfo = RigVfo.Current)
		{
			uint tone;

			var ret = rig_get_ctcss_tone (theRig, vfo, out tone);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return tone;
		}

		/// <summary>
		/// Sets the current encoding Digitally-Coded Squelch code.
		/// </summary>
		/// <param name="code">The code to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetDCS (uint code, int vfo = RigVfo.Current)
		{
			var ret = rig_set_dcs_code (theRig, vfo, code);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}
		}

		/// <summary>
		/// Retrieves the current encoding Digitally-Coded Squelch code.
		/// </summary>
		/// <returns>The current code.</returns>
		/// <param name="vfo">The target VFO.</param>
		public uint GetDCS (int vfo = RigVfo.Current)
		{
			uint code;

			var ret = rig_get_dcs_code (theRig, vfo, out code);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}

			return code;
		}

		/// <summary>
		/// Sets the current Continuous Tone Controlled Squelch 
		/// System (CTCSS) sub-audible *squelch* tone.
		/// Note: tone is NOT in Hz, but in tenth of Hz! This way, 
		/// if you want to set subaudible squelch tone of 88.5 Hz for 
		/// example, then pass 885 to this function.
		/// NB: the tone squelch has to be explicitly enabled or disabled 
		/// through a call to SetFunc() with arg RIG_FUNC_TSQL, 
		/// unless it is unavailable and the tone arg has to be set to 0.
		/// </summary>
		/// <param name="tone">The PL tone to set the squelch to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetCTCSSsql (uint tone, int vfo = RigVfo.Current)
		{
			var ret = rig_set_ctcss_sql (theRig, vfo, tone);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}			
		}

		/// <summary>
		/// Retrieves the current Continuous Tone Controlled Squelch 
		/// System (CTCSS) sub-audible *squelch* tone. 
		/// Note: tone is NOT in Hz, but in tenth of Hz! This way, 
		/// the function returns a subaudible tone of 885 for example,
		/// then the real tone is 88.5 Hz. Also a value of 0 for tone
		/// means that the Tone Squelch is disabled.
		/// </summary>
		/// <returns>The PL squelch tone.</returns>
		/// <param name="vfo">The target VFO.</param>
		public uint GetCTCSSsql (int vfo = RigVfo.Current)
		{
			uint tone;

			var ret = rig_get_ctcss_sql (theRig, vfo, out tone);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return tone;
		}

		/// <summary>
		/// Sets the current Digitally-Coded *Squelch* code.
		/// </summary>
		/// <param name="code">The squelch code to set to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetDCSsql (uint code, int vfo = RigVfo.Current)
		{
			var ret = rig_set_dcs_sql (theRig, vfo, code);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}				
		}

		/// <summary>
		/// Retrieves the current Digitally-Coded *Squelch* code.
		/// </summary>
		/// <returns>The the squelch code.</returns>
		/// <param name="vfo">The target VFO.</param>
		public uint GetDCSsql (int vfo = RigVfo.Current)
		{
			uint code;

			var ret = rig_get_dcs_sql (theRig, vfo, out code);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}				

			return code;
		}

		/// <summary>
		/// Activate/de-activate a function of the radio.
		/// The status argument is true for "activate", false for "de-activate"
		/// </summary>
		/// <param name="func">The functions to activate/de-activate.</param>
		/// <param name="status">true or false</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetFunc (uint func, bool status, int vfo = RigVfo.Current)
		{
			var ret = rig_set_func (theRig, vfo, func, status ? 1 : 0);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}			
		}

		/// <summary>
		/// Retrieves the status (on/off) of a function of the radio.
		/// </summary>
		/// <returns>true if function is "on", false if function is "off"</returns>
		/// <param name="func">The functions to get the status.</param>
		/// <param name="vfo">The target VFO.</param>
		public bool GetFunc (uint func, int vfo = RigVfo.Current)
		{
			int status;

			var ret = rig_get_func (theRig, vfo, func, out status);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}		

			return status != 0;
		}

		/// <summary>
		/// Performs Memory/VFO operation.
		/// </summary>
		/// <param name="op">VfoOperation to perform.</param>
		/// <param name="vfo">The target VFO.</param>
		public void VfoOP (RigVfoOperation op, int vfo = RigVfo.Current)
		{
			var ret = rig_vfo_op (theRig, vfo, op);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}		
		}

		/// <summary>
		/// Checks if a rig is capable of executing a VFO operation. 
		/// Since the op is an OR'ed bitmap argument, more than one 
		/// op can be checked at the same time.
		/// </summary>
		/// <returns><c>true</c> if the rig supports the specified op; otherwise, <c>false</c>.</returns>
		/// <param name="op">VfoOperation to check.</param>
		public bool HasVfoOP (RigVfoOperation op)
		{
			return rig_has_vfo_op (theRig, op) == op;
		}

		/// <summary>
		/// Performs scanning operation.
		/// </summary>
		/// <param name="scan">The scanning operation to perform.</param>
		/// <param name="ch">Optional channel argument used for the scan.</param>
		/// <param name="vfo">The target VFO.</param>
		public void Scan (RigScanOperation scan, int ch, int vfo = RigVfo.Current)
		{
			var ret = rig_scan (theRig, vfo, scan, ch);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}		
		}

		/// <summary>
		/// Checks if a rig is capable of performing a scan operation. 
		/// Since the scan parameter is an OR'ed bitmap argument, 
		/// more than one op can be checked at the same time.
		/// </summary>
		/// <returns><c>true</c> if the rig supports the specified op; otherwise, <c>false</c>.</returns>
		/// <param name="scan">Scan.</param>
		public bool HasScan (RigScanOperation scan)
		{
			return rig_has_scan (theRig, scan) == scan;
		}

		/// <summary>
		/// Sets the current RIT offset. A value of 0 for rit disables RIT.
		/// </summary>
		/// <param name="rit">The RIT offset to adjust to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetRit (int rit, int vfo = RigVfo.Current)
		{
			var ret = rig_set_rit (theRig, vfo, rit);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Retrieves the current RIT offset.
		/// </summary>
		/// <returns>The current RIT offset.</returns>
		/// <param name="vfo">The target VFO.</param>
		public int GetRit (int vfo = RigVfo.Current)
		{
			int rit;

			var ret = rig_get_rit (theRig, vfo, out rit);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return rit;
		}

		/// <summary>
		/// Sets the current XIT offset. A value of 0 for xit disables XIT.
		/// </summary>
		/// <param name="xit">The XIT offset to adjust to.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetXit (int xit, int vfo = RigVfo.Current)
		{
			var ret = rig_set_xit (theRig, vfo, xit);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Retrieves the current XIT offset.
		/// </summary>
		/// <returns>The current XIT offset.</returns>
		/// <param name="vfo">The target VFO.</param>
		public int GetXit (int vfo = RigVfo.Current)
		{
			int xit;

			var ret = rig_get_xit (theRig, vfo, out xit);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return xit;
		}

		/// <summary>
		/// Select the antenna connector.
		/// Example: SetAntenna(RIG_ANT_1);  // apply to both TX&RX
		/// SetAntenna(RIG_ANT_2, RIG_VFO_RX);
		/// </summary>
		/// <param name="ant">The anntena to select.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetAntenna (int ant, int vfo = RigVfo.Current)
		{
			var ret = rig_set_ant (theRig, vfo, ant);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

		}

		/// <summary>
		/// Retrieves the current antenna.
		/// </summary>
		/// <returns>The current antenna selected.</returns>
		/// <param name="vfo">The target VFO.</param>
		public int GetAntenna (int vfo = RigVfo.Current)
		{
			int ant;

			var ret = rig_get_ant (theRig, vfo, out ant);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return ant;
		}

		/// <summary>
		/// Sends DTMF digits. See DTMF change speed, etc. 
		/// </summary>
		/// <param name="digits">String of digits to send.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SendDtmf (string digits, int vfo = RigVfo.Current)
		{
			var ret = rig_send_dtmf (theRig, vfo, digits);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Receives DTMF digits (not blocking). See DTMF change speed, etc.
		/// </summary>
		/// <returns>The string of digits received.</returns>
		/// <param name="vfo">The target VFO.</param>
		public string RecvDtmf (int vfo = RigVfo.Current)
		{
			string digits;

			var ret = rig_recv_dtmf (theRig, vfo, out digits);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return digits;
		}

		/// <summary>
		/// Sends morse message. See keyer change speed, etc.
		/// </summary>
		/// <param name="msg">Message to send.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SendMorse (string msg, int vfo = RigVfo.Current)
		{
			var ret = rig_send_morse (theRig, vfo, msg);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Returns the best frequency resolution of the rig, for a given mode.
		/// </summary>
		/// <returns>The frequency resolution in Hertz if the operation has been sucessful.</returns>
		/// <param name="mode">The mode to get the frequency resolution for.</param>
		public int GetResolution (RigMode mode)
		{
			int res;

			res = rig_get_resolution (theRig, mode);
			if (res < 0)
				throw new RigException (ErrorString (res));

			return res;
		}

		/// <summary>
		/// Resets the radio. See RIG_RESET_NONE, RIG_RESET_SOFT and 
		/// RIG_RESET_MCALL defines for the reset.
		/// </summary>
		/// <param name="reset">The reset operation to perform.</param>
		void Reset (RigReset reset)
		{
			if (thread != null) {
				taskQueue.Add (() => rig_reset (theRig, reset));
			} else {
				var ret = rig_reset (theRig, reset);
				if (ret != RigError.OK) {
					throw new RigException (ErrorString (ret));
				}

			}
		}

		/// <summary>
		/// Checks if a rig supports a set of functions. 
		/// Since the func is an OR'ed bitwise argument, more than one function 
		/// can be checked at the same time.
		/// </summary>
		/// <returns><c>true</c> if this rig as the ability of radio functions ; otherwise, <c>false</c>.</returns>
		/// <param name="func">The Functions.</param>
		public bool HasGetFunc (uint func)
		{
			return rig_has_get_func (theRig, func) == func;
		}

		/// <summary>
		/// Checks if a rig supports a set of functions. 
		/// Since the func is an OR'ed bitwise argument, more than one 
		/// function can be checked at the same time.
		/// </summary>
		/// <returns><c>true</c> if this instance has set func the specified func; otherwise, <c>false</c>.</returns>
		/// <param name="func">Func.</param>
		public bool HasSetFunc (uint func)
		{
			return rig_has_set_func (theRig, func) == func;
		}

		/// <summary>
		/// Converts a power value expressed in a range on a [0.0 .. 1.0] relative 
		/// scale to the real transmit power in milli Watts the radio would emit. 
		/// The freq and mode where the conversion should take place must be also 
		/// provided since the relative power is peculiar to a specific freq and 
		/// mode range of the radio.
		/// </summary>
		/// <returns>The converted power in mW.</returns>
		/// <param name="power">The relative power.</param>
		/// <param name="freq">The frequency where the conversion should take place.</param>
		/// <param name="mode">The mode where the conversion should take place.</param>
		public uint Power2Milliwatt (float power, double freq, RigMode mode)
		{
			uint mwpower;

			var ret = rig_power2mW (theRig, out mwpower, power, freq, mode);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return mwpower;
		}

		/// <summary>
		/// Converts a power value expressed in the real transmit power in milli Watts 
		/// the radio would emit to a range on a [0.0 .. 1.0] relative scale. The freq
		///  and mode where the conversion should take place must be also provided since 
		/// the relative power is peculiar to a specific freq and mode range of the radio.
		/// </summary>
		/// <returns>The converted relative power.</returns>
		/// <param name="mwpower">The power in mW.</param>
		/// <param name="freq">The frequency where the conversion should take place.</param>
		/// <param name="mode">The mode where the conversion should take place.</param>
		public float Milliwatt2power (uint mwpower, double freq, RigMode mode)
		{
			float power;

			var ret = rig_mW2power (theRig, out power, mwpower, freq, mode);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return power;
		}

		/// <summary>
		/// Enable/disable the transceive handling of a rig and kick off async mode.
		/// </summary>
		/// <param name="trn">The transceive status to set to.</param>
		public void SetTransceive (int trn)
		{
			var ret = rig_set_trn (theRig, trn);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Retrieves the current status of the transceive mode, i.e. if radio 
		/// sends new status automatically when some changes happened on the radio.
		/// </summary>
		/// <returns>The current transceive mode</returns>
		public int GetTransceive ()
		{
			int trn;

			var ret = rig_get_trn (theRig, out trn);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return trn;
		}

		/// <summary>
		/// Sets the current memory bank number. It is not mandatory 
		/// for the radio to be in memory mode. Actually it depends on rigs.
		/// </summary>
		/// <param name="bank">The memory bank.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetBank (int bank, int vfo = RigVfo.Current)
		{
			var ret = rig_set_bank (theRig, vfo, bank);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Sets the current memory channel number. It is not mandatory for 
		/// the radio to be in memory mode. Actually it depends on rigs.
		/// </summary>
		/// <param name="ch">The memory channel number.</param>
		/// <param name="vfo">The target VFO.</param>
		public void SetMem (int ch, int vfo = RigVfo.Current)
		{
			var ret = rig_set_mem (theRig, vfo, ch);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Retrieves the current memory channel number. It is not mandatory 
		/// for the radio to be in memory mode. Actually it depends on rigs.
		/// </summary>
		/// <returns>The current memory channel number</returns>
		/// <param name="vfo">The target VFO.</param>
		public int GetMem (int vfo = RigVfo.Current)
		{
			int ch;

			var ret = rig_get_mem (theRig, vfo, out ch);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return ch;
		}

		// TODO: channel struct is not yet defined in wrapper
		//		void SetChannel (const channel_t *chan)
		//		{
		//			CHECK_RIG( rig_set_channel(theRig, chan) );
		//		}
		//
		//		void GetChannel (channel_t *chan)
		//		{
		//			CHECK_RIG( rig_get_channel(theRig, chan) );
		//		}

		/// <summary>
		/// turns on/off the radio. See RIG_POWER_ON, RIG_POWER_OFF 
		/// and RIG_POWER_STANDBY defines for the status.
		/// </summary>
		/// <param name="status">The status to set to.</param>
		public void SetPowerState (PowerState status)
		{
			var ret = rig_set_powerstat (theRig, status);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	
		}

		/// <summary>
		/// Retrieve the status of the radio. See RIG_POWER_ON, 
		/// RIG_POWER_OFF and RIG_POWER_STANDBY defines for the status.
		/// </summary>
		/// <returns>The power state.</returns>
		public PowerState GetPowerState ()
		{
			PowerState status;

			var ret = rig_get_powerstat (theRig, out status);
			if (ret != RigError.OK) {
				throw new RigException (ErrorString (ret));
			}	

			return status;
		}

		//		RadioMode RngRxModes (freq_t freq)
		//		{
		//			unsigned modes = RIG_MODE_NONE;
		//			freq_range_t *rng;
		//			int i;
		//
		//			for (i=0; i<FRQRANGESIZ; i++) {
		//				rng = &theRig->state.rx_range_list[i];
		//				if (RIG_IS_FRNG_END(*rng)) {
		//					return (RadioMode)modes;
		//				}
		//				if (freq >= rng->start && freq <= rng->end)
		//					modes |= (unsigned)rng->modes;
		//			}
		//
		//			return (RadioMode)modes;
		//		}
		//
		//		RadioMode RngTxModes (freq_t freq)
		//		{
		//			unsigned modes = RIG_MODE_NONE;
		//			freq_range_t *rng;
		//			int i;
		//
		//			for (i=0; i<FRQRANGESIZ; i++) {
		//				rng = &theRig->state.tx_range_list[i];
		//				if (RIG_IS_FRNG_END(*rng)) {
		//					return (RadioMode)modes;
		//				}
		//				if (freq >= rng->start && freq <= rng->end)
		//					modes |= (unsigned)rng->modes;
		//			}
		//
		//			return (RadioMode)modes;
		//		}
		//

		static public int Hz (int f)
		{
			return f;
		}

		static public int kHz (int f)
		{
			return f * 1000;
		}

		static public int MHz (int f)
		{
			return f * 1000000;
		}

		static public int GHz (int f)
		{
			return f * 1000000000;
		}

		public static string FrequencyToString (double frequencyHz)
		{
			double f;
			string hz;

			if (frequencyHz >= GHz (1)) {
				hz = "GHz";
				f = (double)frequencyHz / GHz (1);
			} else if (frequencyHz >= MHz (1)) {
				hz = "MHz";
				f = (double)frequencyHz / MHz (1);
			} else if (frequencyHz >= kHz (1)) {
				hz = "kHz";
				f = (double)frequencyHz / kHz (1);
			} else {
				hz = "Hz";
				f = (double)frequencyHz;
			}

			return string.Format ("{0} {1}", f, hz);
		}

		internal IConfigurationParameter[] GetExtParm (IntPtr ptr)
		{
			List<IConfigurationParameter> cparams = new List<IConfigurationParameter> ();
			Rig.rig_ext_parm_foreach (theRig, (rig, confPtr, rig_ptr) => {
				var conf = confparam_marshal (confPtr);
				cparams.Add (conf);
				return 1;
			}, IntPtr.Zero);

			return cparams.ToArray ();
		}

		internal IConfigurationParameter[] GetExtLevels (IntPtr ptr)
		{
			List<IConfigurationParameter> cparams = new List<IConfigurationParameter> ();
			Rig.rig_ext_level_foreach (theRig, (rig, confPtr, rig_ptr) => {
				var conf = confparam_marshal (confPtr);
				cparams.Add (conf);
				return 1;
			}, IntPtr.Zero);

			return cparams.ToArray ();
		}

	}
}
