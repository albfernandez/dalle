/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010
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
using System.IO;
using System.Collections;
using Dalle.Archivers;
using Dalle.Streams;

namespace Dalle.Archivers.ar
{



	public class ArArchiveInputStream : ArchiveInputStream
	{
		private byte[] buffer = new byte[1024];
		private ArArchiveEntry currentEntry = null;
		private Hashtable gnuNames = new Hashtable();
		
	    public ArArchiveInputStream (Stream inputStream) : base(inputStream)
	    {
	    	this.closed = false;
	    }
		public ArArchiveEntry GetNextArEntry ()
		{
			// read to the end of current entry data
			if (currentEntry != null) 
			{
				long iread = 0;
				do {
					iread = this.Read (buffer, 0, buffer.Length);
				} while (iread > 0);
				currentEntry = null;
			}
			
			if (this.Position == 0)
			{
				// First entry
				// File header
				byte[] expected = new System.Text.ASCIIEncoding ().GetBytes (ArArchiveEntry.HEADER);
				byte[] realized = new byte[expected.Length];
				int read = inputStream.Read (realized, 0, expected.Length);
				this.Count (read);
				if (read != expected.Length)
				{
					throw new IOException ("failed to read header. Occured at byte: " + this.Position);
				}
				for (int i = 0; i < expected.Length; i++) 
				{
					if (expected[i] != realized[i]) 
					{
						throw new IOException ("invalid header " + new System.Text.ASCIIEncoding ().GetString (realized));
					}
				}
				
			}
			if (this.Position % 2 != 0) 
			{
				if (inputStream.ReadByte () < 0) 
				{
					// hit eof
					return null;
				}
				this.Count (1);
				
			}
			byte[] header = new byte[60];
			int rea = this.inputStream.Read (header, 0, header.Length);
			this.Count (rea);
			if (rea <= 0) 
			{
				// Clean end of file;
				return null;
			}
			if (rea != header.Length)
			{
				throw new IOException ("invalid header");
			}
			if (header[58] != 0x60 || header[59] != 0x0A)
			{
				throw new IOException ("invalid magic tail on header");	
			}
			string name = Utilidades.UtArrays.LeerTexto (header, 0, 16).Trim ();
			long size = long.Parse (Utilidades.UtArrays.LeerTexto (header, 48, 10).Trim ());
			
			if (name.EndsWith ("/"))
			{
				name = name.Substring (0, name.Length - 1);
			}
			// TODO Leer todos los campos de la cabecera
			currentEntry = new ArArchiveEntry (name, size);
			
			
			
			// GNU AR
			if (currentEntry.Name.Equals ("/"))
			{
				ReadGNUFilenamesEntry ();
				return GetNextArEntry ();
			}
			else if (currentEntry.Name.StartsWith ("/") && gnuNames.Count > 0)
			{
				currentEntry.name = (string)gnuNames[currentEntry.Name.Substring(1)];
			}
			
			
			// BSD AR
			if (currentEntry.Name.StartsWith ("#1/"))
			{
				string t = currentEntry.Name.Substring (3);
				int s = int.Parse (t);
				
				if (s > 2048) {
					throw new IOException ("Filename too long (bsd)");
				}
				byte[] buffer2 = new byte[s];
				int l = this.inputStream.Read (buffer2, 0, buffer2.Length);
				if (l != s) {
					throw new IOException ("Filename error (bsd)");
				}
				currentEntry.name = Utilidades.UtArrays.LeerTexto (buffer2, 0);
			}
			dataStream = new SizeLimiterStream (this.inputStream, currentEntry.Size);
			
			return currentEntry;
			
		}
		private void ReadGNUFilenamesEntry ()
		{
			dataStream = new SizeLimiterStream (this.inputStream, this.currentEntry.Size);
			byte[] datos = new byte[this.currentEntry.Size];
			int l = this.Read (datos, 0, datos.Length);
			if (l != datos.Length)
			{
				throw new IOException ();
			}
			string c = Utilidades.UtArrays.LeerTexto (datos, 0);
			string[] ficheros = c.Split (new char[] { '\n' });
			
			int pos = 0;
			foreach (string f in ficheros) 
			{

				string nombre = f;
				if (nombre.EndsWith("/")){
					nombre = nombre.Substring (0, nombre.Length - 1);
				}
				gnuNames.Add ("" + pos, nombre);
				pos += f.Length + 1;
			}
		}
		public override ArchiveEntry GetNextEntry () 
		{
        		return GetNextArEntry();
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