/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Astrotite.Astrotite
          Join files in astrotite format.
	
    Copyright (C) 2004-2009
    Original author (C - code) - Daniel Martínez Contador <dmcontador@terra.es>
    C# translation by - Alberto Fernández  <infjaf00@yahoo.es>

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

using Dalle.Formatos;
using Dalle.Utilidades;

namespace Dalle.Formatos.Astrotite
{

	public struct Descript
	{
		public int length;
		public int blocks;
		public int namelength;
		public string name;
	}
	public struct Block 
	{
		public String block;
		public int size;
		public int crc;
	}
	
	public class Astrotite : Parte 
	{
		
		public Astrotite (): base ("astrotite", "Astrotite", "www.fantiusen.com/astrotite.html", false, false)
		{
		}

		protected override void _Unir (string fichero, string dirDest)
		{
			ArrayList listaFicheros = new ArrayList();
			int narchivos = 0;
			int i = 0;
			long startdata = 0;
			long leidos = 0;
			long totales = 0;
			
			byte[] initbuffer = new byte[Dalle.Consts.BUFFER_LENGTH];
			
			FileStream astReader = new FileStream (fichero, FileMode.Open);
			
			if (astReader.Read (initbuffer, 0, initbuffer.Length) < 22){
				throw new Dalle.Formatos.FileFormatException();
			}
			string tmp="";
			for (i = 0; i < "AST2www.astroteam.tk".Length; i++){
				tmp += Convert.ToChar (initbuffer[i]);
			}
			if (tmp != "AST2www.astroteam.tk")
				throw new Dalle.Formatos.FileFormatException();
			
			narchivos = UtArrays.LeerInt16(initbuffer, 20);
			
			startdata = 22;
			OnProgress (0, 1);
			
			// TODO: Mejorar, poner buffer.
			// Mejorar las lecturas
			for (i = 0; i < narchivos; i++){
				astReader.Seek (startdata, SeekOrigin.Begin);
				astReader.Read (initbuffer, 0, 255);
				Descript d;
				d.length = UtArrays.LeerInt32(initbuffer, 0);
				d.blocks = UtArrays.LeerInt32(initbuffer, 4);
				d.namelength = initbuffer[8];
				d.name = UtArrays.LeerTexto (initbuffer, 9, d.namelength);				
				d.name = d.name.Replace ('\\', Path.DirectorySeparatorChar);
				startdata += 9 + d.namelength;				
				listaFicheros.Add (d);	
				totales += d.length;			
			}
			
			// TODO Excepción personalizada.
			if (i != narchivos){
				throw new Exception ("AstrotiteException");
			}
			
			astReader.Seek (startdata, SeekOrigin.Begin);
			
			AstrotiteCRC crc = new AstrotiteCRC();
			foreach (Descript des in listaFicheros){
				astReader.Seek (3, SeekOrigin.Current);
				
				// Corregir la ruta 
				FileStream writer = UtilidadesFicheros.CreateWriter(des.name);				
				
				int quedan = des.length;
				long l = 0;
				
				while (quedan > 0){					
					Block block = readBlock(astReader);					
					crc.Reset();
					
					int quedan1 = block.size;
					while (quedan1 > 0) {
						l = astReader.Read(initbuffer, 0, Dalle.Consts.BUFFER_LENGTH < quedan1 ? Dalle.Consts.BUFFER_LENGTH: quedan1 );
						writer.Write (initbuffer, 0, (int)l);
						
						
						// FIXME warning
						if (block.crc != 0xFFFFFFFF){
							crc.Update(initbuffer, 0, (int) l);
						}
						quedan1 -= (int) l;				
						quedan -= (int) l;
						leidos += l;
						OnProgress (leidos, totales);
					}					
					// FIXME warning
					if ((block.crc != 0xFFFFFFFF) && ((long) block.crc != crc.Value)){
						throw new Dalle.Formatos.ChecksumVerificationException();
					}					
					
				}
				writer.Close();
							
			}
			astReader.Close();			
			
		}
		private Block readBlock(FileStream astReader){	
			byte[] info = new byte[9];		
			astReader.Read(info, 0, 9);
			Block block = new Block();
			block.block = UtArrays.LeerTexto (info, 0, 3);
			block.size  = UtArrays.LeerInt16 (info, 3);
			block.crc   = UtArrays.LeerInt32 (info, 5);
			return block;
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)){
				return false;
			}
			if (!fichero.ToUpper().EndsWith(".AST2")){
				return false;
			}
			
			byte[] initbuffer = new byte[22];
			FileStream astReader = new FileStream (fichero, FileMode.Open);
			
			if (astReader.Read (initbuffer, 0, initbuffer.Length) < 22){
				return false;
			}
			string tmp="";
			for (int i = 0; i < "AST2www.astroteam.tk".Length; i++){
				tmp += Convert.ToChar (initbuffer[i]);
			}
			if (tmp != "AST2www.astroteam.tk"){
				return false;
			}
			return true;
			
		}		
	}	
}
