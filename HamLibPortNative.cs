//
//  HamLibPortNative.cs
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
using System.Runtime.InteropServices;

namespace HamLibSharp
{
	// TODO: The port struct is not really useful yet. It is used to as place
	// holder for Marshal copying
//	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
//	internal struct HamLibCommPortNative
//	{
//		RigPort rig;
//
//		int fd;
//		/*!< File descriptor */
//		IntPtr handle;
//		/*!< handle for USB */
//
//		int write_delay;
//		/*!< Delay between each byte sent out, in mS */
//		int post_write_delay;
//		/*!< Delay between each commands send out, in mS */
//		int tv_sec;
//		int tv_usec;
//		//} post_write_date;	/*!< hamlib internal use */
//		int timeout;
//		/*!< Timeout, in mS */
//		int retry;
//		/*!< Maximum number of retries, 0 to disable */
//
//		[MarshalAs (UnmanagedType.ByValTStr, SizeConst = Rig.FILPATHLEN)]
//		string pathname;
//		/*!< Port pathname */
//	
//		int rate;
//		/*!< Serial baud rate */
//		int data_bits;
//		/*!< Number of data bits */
//		int stop_bits;
//		/*!< Number of stop bits */
//		RigSerialParity parity;
//		/*!< Serial parity */
//		RigSerialHandshake handshake;
//		/*!< Serial handshake */
//		RigSerialControlState rts_state;
//		/*!< RTS set state */
//		RigSerialControlState dtr_state;
//		/*!< DTR set state */
//
//		int test;
//		/*!< alternate */
//		int test2;
//		/*!< alternate */
//	}

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct HamLibPortNative
	{
		//			union {
		//				rig_port_t rig;		/*!< Communication port type */
		//				ptt_type_t ptt;		/*!< PTT port type */
		//				dcd_type_t dcd;		/*!< DCD port type */
		//			} type;
		IntPtr rig_ptt_dcd;

		IntPtr fd;
		/*!< File descriptor */
		IntPtr handle;
		/*!< handle for USB */

		int write_delay;
		/*!< Delay between each byte sent out, in mS */
		int post_write_delay;
		/*!< Delay between each commands send out, in mS */
		int tv_sec;
		int tv_usec;
		//} post_write_date;	/*!< hamlib internal use */
		int timeout;
		/*!< Timeout, in mS */
		int retry;
		/*!< Maximum number of retries, 0 to disable */

		[MarshalAs (UnmanagedType.ByValTStr, SizeConst = Rig.FILPATHLEN)]
		string pathname;
		/*!< Port pathname */

		int vid;
		/*!< Vendor ID */
		int pid;
		/*!< Product ID */
		int conf;
		/*!< Configuration */
		int iface;
		/*!< interface */
		int alt;
		/*!< alternate */
		IntPtr vendor_name;
		/*!< Vendor name (opt.) */
		IntPtr product;
		/*!< Product (opt.) */

		//int test;
		/*!< alternate */
		//int test2;
		/*!< alternate */

		//		union {
		//				struct {
		//					int rate;	/*!< Serial baud rate */
		//					int data_bits;	/*!< Number of data bits */
		//					int stop_bits;	/*!< Number of stop bits */
		//					enum serial_parity_e parity;		/*!< Serial parity */
		//					enum serial_handshake_e handshake;	/*!< Serial handshake */
		//					enum serial_control_state_e rts_state;	/*!< RTS set state */
		//					enum serial_control_state_e dtr_state;	/*!< DTR set state */
		//				} serial;		/*!< serial attributes */
		//				struct {
		//					int pin;	/*!< Parallel port pin number */
		//				} parallel;		/*!< parallel attributes */
		//				struct {
		//					int ptt_bitnum;	/*< Bit number for CM108 GPIO PTT */
		//				} cm108;		/*!< CM108 attributes */
		//				struct {
		//					int vid;	/*!< Vendor ID */
		//					int pid;	/*!< Product ID */
		//					int conf;	/*!< Configuration */
		//					int iface;	/*!< interface */
		//					int alt;	/*!< alternate */
		//					char *vendor_name; /*!< Vendor name (opt.) */
		//					char *product;     /*!< Product (opt.) */
		//				} usb;			/*!< USB attributes */
		//				struct {
		//					int on_value;
		//					int value;
		//				} gpio;
		//			} parm;			/*!< Port parameter union */
		//		} hamlib_port_t;
	}
}
