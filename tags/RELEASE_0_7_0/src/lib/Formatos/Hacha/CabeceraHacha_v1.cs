/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Hacha.CabeceraHacha_v1 - 
		Header of files created by Hacha < 2.79
	
    Copyright (C) 2003  Alberto Fernández <infjaf00@yahoo.es>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/

using System;
using System.IO;

using I = Dalle.I18N.GetText;

namespace Dalle.Formatos.Hacha
{	
	
	public class CabeceraHacha_v1
	{
	
		private string nombreOriginal;
		const string separador="?????";
		private long tamano;
		private long tamanoFrag;
		private int crc = 7;
		
		public CabeceraHacha_v1()
		{
		}
		public int CRC{
			get{ return crc; }
			set{ crc = value; }
		}
		public string NombreOriginal{
			get{ return nombreOriginal; }
			set{ nombreOriginal = value; }
		}
		public long TamanoFragmento{
			get{ return tamanoFrag; }
			set{ tamanoFrag = value; }
		}
		public long Tamano {
			get{ return tamano; }
			set{ tamano = value; }
		}
		public int Size{
			get{ return this.ToByteArray().Length; }
		}
		private char CheckSum{
			get{
				return (Convert.ToChar (NombreOriginal.Length + 
						TamanoFragmento.ToString().Length + 
						Tamano.ToString().Length + 20)
						);
			}
		}
		public virtual string Version{
			get{ return "1"; }
		}
		
		public static CabeceraHacha_v1 NewFromVersion (string version)
		{
			switch (version){
				case "1":
				case "1.0": return new CabeceraHacha_v1();
				case "2": 
				case "2.0": return new CabeceraHacha_v2();
				default: return new CabeceraHacha_v1();
			}
		
		}
		
		public virtual byte[] ToByteArray()
		{
			
			string nulos = Convert.ToString('\x00') +
							Convert.ToString('\x00') +
							Convert.ToString('\x00');
			string cabecera = separador 
					+ Convert.ToString(CheckSum)
					+ nulos
					+ separador
					+ nombreOriginal
					+ separador
					+ tamano
					+ separador
					+ tamanoFrag
					+ separador;
			
			byte[] bytes = new byte[cabecera.Length];
			
			int i=0;
			foreach(char a in cabecera)
				bytes[i++] = Convert.ToByte(a);
	
			return bytes;
		}		
			
		public static CabeceraHacha_v1 LeerCabecera (String fichero)
		{
			// TODO: Crear excepciones personalizadas
			
			if ( (new FileInfo(fichero).Length) < 35){
				throw new Exception ();
			}
			
			FileStream f = new FileStream (fichero, FileMode.Open);
			for (int i=0; i < separador.Length; i++){
				if (f.ReadByte() != 0x3F){
					f.Close();
					throw new Exception ();
				}
			}
						
			int b0 = f.ReadByte();
			int b1 = f.ReadByte();
			int b2 = f.ReadByte();
			int b3 = f.ReadByte();
			
			for (int i=0; i < separador.Length; i++){
				if (f.ReadByte() != 0x3F){
					f.Close();
					throw new Exception ();
				}
			}
			
			String nOriginal = string.Empty;
			
			for (byte a = (byte) f.ReadByte (); a != 0x3F; a = (byte) f.ReadByte())
				nOriginal += Convert.ToChar(a);

			for (int i=0; i < separador.Length - 1; i++){
				if (f.ReadByte() != 0x3F){	
					f.Close();
					throw new Exception ();
				}
			}
			String stTamano = string.Empty;

			for (byte a = (byte)f.ReadByte(); a != 0x3F; a = (byte) f.ReadByte())
				stTamano += Convert.ToChar(a);
		
			for (int i=0; i < separador.Length -1; i++){
				if (f.ReadByte() != 0x3F){	
					f.Close();
					throw new Exception();
				}
				
			}
			String stTamanoFrag = string.Empty;
			
			for (byte a = (byte) f.ReadByte(); a != 0x3F; a = (byte) f.ReadByte())
				stTamanoFrag += Convert.ToChar (a);			

			for (int i=0; i < separador.Length -1; i++){
				if (f.ReadByte() != 0x3F){
					f.Close();
					throw new Exception();
				}				
			}
			f.Close();
			
			
			CabeceraHacha_v1 ret;		
			
			// Determinamos si hemos leido una cabecera v1 o v2. (según
			// el campo de verificación).
			int c = b0 + (b1 << 8) + (b2 << 16) + (b3 << 24);
			
			
			if ( (c < 0xFF) && ((c != 0) && (c != 0x07)) ){
				// Hacha v.1
				ret = new CabeceraHacha_v1 ();
				char check = Convert.ToChar (b0);
				ret.Tamano = Convert.ToInt64 (stTamano);
				ret.TamanoFragmento = Convert.ToInt64(stTamanoFrag);
				ret.NombreOriginal = nOriginal;
				if (check != ret.CheckSum)
					Console.WriteLine (I._("Warning: Header checksum verification failed"));
			}
			else{
				// Hacha v.2
				ret = new CabeceraHacha_v2 ();
				ret.CRC = c;
				ret.Tamano = Convert.ToInt64 (stTamano);
				ret.TamanoFragmento = Convert.ToInt64(stTamanoFrag);
				ret.NombreOriginal = nOriginal;
			}						
			return ret;
		}
		
		
	}	
}
