/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Kamaleon.InfoFicheroKamaleon		
	
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

namespace Dalle.Formatos.Kamaleon
{	

	public class InfoFicheroKamaleon
	{
		public InfoFicheroKamaleon ()
		{
		}
		public static InfoFicheroKamaleon NewFromVersion (String version)
		{
			switch (version){
				case "1":
				case "1.0": return new InfoFicheroKamaleon();
				case "2":
				case "2.0": return new InfoFicheroKamaleon_v2();
				default: return new InfoFicheroKamaleon();
			}
		}
		public virtual byte[] ToByteArray ()
		{
			byte[] ret = new byte[560];
			ret[0] = (byte) NombreOriginal.Length;
			SetText(ret, NombreOriginal, 0x01);
			SetInt (ret, TamanoOriginal, 0x100);
			ret[0x104] = (byte) Password.Length;
			SetText(ret, Password, 0x105);
			ret[0x123] = (byte) NombreFragmento.Length;
			SetText(ret,NombreFragmento, 0x124);
			SetInt (ret, TamanoFragmento, 0x224);
			ret[0x228] = PrimerByte;
			ret[0x229] = UltimoByte;
			SetInt (ret, TamanoPiel, 0x22C);
			return ret;
		}
		public InfoFicheroKamaleon (byte[] bytes)
		{
			nombreOriginal = GetText (bytes, 0x1);
			password = GetText (bytes, 0x105);
			tamanoOriginal = GetInt(bytes, 0x100);
			nombreFragmento = GetText (bytes, 0x124);
			tamanoFragmento = GetInt (bytes, 0x224);
			primerByte = bytes[0x228];
			ultimoByte = bytes[0x229];
			tamanoPiel = GetInt (bytes, 0x22C);
		}
		protected void SetText (byte[] bytes, String text, int pos_ini)
		{
			foreach (char c in text)
				bytes[pos_ini++] = Convert.ToByte(c);
		}
		
		protected String GetText (byte[] bytes, int pos_ini)
		{
			String ret = "";
			for (int i= pos_ini; bytes[i] != 0; i++)
			{
				ret += Convert.ToChar(bytes[i]);
			}
			return ret;
		}
		protected int GetInt (byte[] bytes, int pos_ini){
			byte b0, b1, b2, b3;
			b0 = bytes[pos_ini];
			b1 = bytes[pos_ini + 1];
			b2 = bytes[pos_ini + 2];
			b3 = bytes[pos_ini + 3];
			
			return ((int) (b0 + (b1 << 8) + (b2 << 16) + (b3 << 24)));			
		}
		protected static void SetInt (byte[] bytes, int value,int pos)
		{
			int i0, i1, i2, i3;
			
			i0 = value & 0x000000FF;
			i1 = (value & 0x0000FF00) >> 8;
			i2 = (value & 0x00FF0000) >> 16;
			i3 = unchecked ((value & (int)0xFF000000)>>24);
		
			bytes[pos] = (byte) i0;
			bytes[pos+1] = (byte) i1;
			bytes[pos+2] = (byte) i2;
			bytes[pos+3] = (byte) i3;
		}
		private String nombreOriginal;
		private int tamanoOriginal;
		protected String password;
		private String nombreFragmento;
		private int tamanoFragmento;
		private byte primerByte;
		private int tamanoPiel;
		private byte ultimoByte;
		protected String nombrePiel;
		protected long checksum = 0;
				
		public string NombreOriginal{
			get{ return nombreOriginal; }
			set{
				if (value.Length > 0x100){
					// Lanzar una excepcion.
					throw new Exception ();
				}
				nombreOriginal = value;
			}
		}		
		public int TamanoOriginal{
			get{ return tamanoOriginal; }
			set{ tamanoOriginal = value;}
		}
		public string Password{
			get{
				if (password == null)
					return "";
				return password;
			}
			set{ password = value; }
		}		
		public string NombreFragmento{
			get{ return nombreFragmento; }
			set{
				// TODO: comprobar valor.
				nombreFragmento = value;
			}
		}
		
		public int TamanoFragmento{
			get{ return tamanoFragmento; }
			set{ tamanoFragmento = value; }
		}
		
		public byte PrimerByte{
			get{ return primerByte; }
			set{ primerByte = value; }
		}
		
		public byte UltimoByte{
			get{ return ultimoByte; }
			set{ ultimoByte = value; }
		}
		
		public int TamanoPiel{
			get{ return tamanoPiel; }
			set{ tamanoPiel = value; }
		}
		public string NombrePiel{
			get{
				if (nombrePiel == null)
					return "piel.jpg";
				else
					return nombrePiel;
			}
			set{ nombrePiel = value; }
		}
		public int TamanoDatos{
			get{ return (TamanoFragmento - TamanoPiel); }
		}
		public long Checksum {
			get { return checksum; }
			set { checksum = value; }
		}
	}
}
