/*

	Dalle - A split/join file utility library
	Dalle.Formatos.SF.CabeceraSF - Header of sf files.
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

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

namespace Dalle.Formatos.SF
{
	
	
	public class CabeceraSF 
	{
	
		public const int LIMITE = 512;
		private byte numero;
		private string nombre;
		
		public CabeceraSF ()
		{
		}
		public CabeceraSF(string fichero)
		{
			FileStream reader = null;
			
			try{
				reader = new FileStream (fichero, FileMode.Open);
				reader.Seek(4,SeekOrigin.Begin);
				int contador = 4;
				do{
					contador++;
				}while ((contador < LIMITE) && (contador < reader.Length) && (reader.ReadByte()!= 0));
				//TODO: Poner limite al while
				
				reader.Seek(0, SeekOrigin.Begin);
				byte[] b = new byte[contador];
				reader.Read (b, 0, contador);
				
				if ((b[0]!=0x53) || (b[1]!=0x46) || (b[2]!=0) || (b[3]!=0) || (b[4] != 6)){
					// TODO: Poner una excepcion personalizada
					throw new Exception();
				}
				numero = b[5];
				nombre = "";
				for (int i=6; (i < b.Length) && (b[i]!=0); i++)
					nombre += Convert.ToChar(b[i]);
			}
			catch (Exception){
				throw;
			}
			finally{
				if (reader != null){
					reader.Close();
				}
			}
					
		}
		public int Numero{
			get{return (int)numero;}
			set{numero = (byte) value;}
		}
		public String Nombre{
			get{return nombre;}
			set{nombre = value;}
		}
		public int Tamano{
			get{return (Nombre.Length + 7);}
		}
		public byte[] ToByteArray ()
		{
			byte[] ret = new byte[Tamano];
			ret[0]=0x53;
			ret[1]=0x46;
			ret[4]=0x06;
			ret[5]=numero;
			for (int i=0; i < Nombre.Length; i++)
			{
				ret[i+6] = Convert.ToByte(Nombre[i]);
			}
			return ret;		
		}
	}
}
