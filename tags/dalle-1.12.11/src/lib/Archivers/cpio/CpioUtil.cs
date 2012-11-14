/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010 Alberto Fernández  <infjaf@gmail.com>
    Original java code: commons-compress, from apache (http://commons.apache.org/compress/)
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
using Dalle.Archivers;

namespace Dalle.Archivers.cpio
{


	public class CpioUtil
	{

			 /**
	     * Converts a byte array to a long. Halfwords can be swapped by setting
	     * swapHalfWord=true.
	     * 
	     * @param number
	     *            An array of bytes containing a number
	     * @param swapHalfWord
	     *            Swap halfwords ([0][1][2][3]->[1][0][3][2])
	     * @return The long value
	     * @throws UnsupportedOperationException if number length is not a multiple of 2
	     */
	    public static long ByteArray2long (byte[] number, bool swapHalfWord)
	    {
	    	if (number.Length % 2 != 0) {
	    		throw new NotSupportedException ();
	    	}
	
	        long ret = 0;
	    	int pos = 0;
	    	byte[] tmp_number = new byte[number.Length];
	  

			System.Array.Copy (number, tmp_number, tmp_number.Length);

			
	
	        if (!swapHalfWord) {
	            byte tmp = 0;
	            for (pos = 0; pos < tmp_number.Length; pos++) {
	                tmp = tmp_number[pos];
	                tmp_number[pos++] = tmp_number[pos];
	                tmp_number[pos] = tmp;
	            }
	        }
	
	        ret = tmp_number[0] & 0xFF;
	        for (pos = 1; pos < tmp_number.Length; pos++) {
	            ret <<= 8;
	            ret |= (uint) (tmp_number[pos] & 0xFF);
	        }
	        return ret;
	    }
	
	    /**
	     * Converts a long number to a byte array 
	     * Halfwords can be swapped by setting swapHalfWord=true.
	     * 
	     * @param number 
	     *            the input long number to be converted
	     *            
	     * @param length
	     *            The length of the returned array
	     * @param swapHalfWord
	     *            Swap halfwords ([0][1][2][3]->[1][0][3][2])
	     * @return The long value
	     * @throws UnsupportedOperationException if the length is not a positive multiple of two
	     */
	    public static byte[] Long2byteArray(long number, int length, bool swapHalfWord) {
	        byte[] ret = new byte[length];
	        int pos = 0;
	        long tmp_number = 0;
	
	        if (length % 2 != 0 || length < 2) {
	            throw new NotSupportedException();
	        }
	
	        tmp_number = number;
	        for (pos = length - 1; pos >= 0; pos--) {
	            ret[pos] = (byte) (tmp_number & 0xFF);
	            tmp_number >>= 8;
	        }
	
	        if (!swapHalfWord) {
	            byte tmp = 0;
	            for (pos = 0; pos < length; pos++) {
	                tmp = ret[pos];
	                ret[pos++] = ret[pos];
	                ret[pos] = tmp;
	            }
	        }
	
	        return ret;
	    }
	}
}


/* The original Java file had this header:
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */