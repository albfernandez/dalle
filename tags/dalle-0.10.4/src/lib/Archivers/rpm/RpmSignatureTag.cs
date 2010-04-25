/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2004-2010
    Original java code: commons-commpress, from apache (http://commons.apache.org/compress/)
    C# translation by - Alberto Fernández  <infjaf@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;

namespace Dalle.Archivers.rpm
{
	public enum RpmSignatureTag 
	{
		RPMSIGTAG_HEADERSIGNATURES = 62,
		RPMSIGTAG_BADSHA1_1	= 264,
		RPMSIGTAG_BADSHA1_2 = 265,
		RPMSIGTAG_DSA = 267,
		RPMSIGTAG_RSA = 268,
		RPMSIGTAG_SHA1 =269,
		RPMSIGTAG_SIZE	= 1000,	/*!< internal Header+Payload size (32bit) in bytes. */
		RPMSIGTAG_LEMD5_1	= 1001,	/*!< internal Broken MD5, take 1 @deprecated legacy. */
		RPMSIGTAG_PGP	= 1002,	/*!< internal PGP 2.6.3 signature. */
		RPMSIGTAG_LEMD5_2	= 1003,	/*!< internal Broken MD5, take 2 @deprecated legacy. */
		RPMSIGTAG_MD5	= 1004,	/*!< internal MD5 signature. */
		RPMSIGTAG_GPG	= 1005, /*!< internal GnuPG signature. */
		RPMSIGTAG_PGP5	= 1006,	/*!< internal PGP5 signature @deprecated legacy. */
		RPMSIGTAG_PAYLOADSIZE = 1007,/*!< internal uncompressed payload size (32bit) in bytes. */
	}
}
