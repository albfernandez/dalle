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
	public enum RpmType
	{
		RPM_NULL_TYPE		=  0,
		RPM_CHAR_TYPE		=  1,
		RPM_INT8_TYPE		=  2,
		RPM_INT16_TYPE		=  3,
		RPM_INT32_TYPE		=  4,
		RPM_INT64_TYPE		=  5,
		RPM_STRING_TYPE		=  6,
		RPM_BIN_TYPE		=  7,
		RPM_STRING_ARRAY_TYPE	=  8,
		RPM_I18NSTRING_TYPE		=  9,
	};
}
