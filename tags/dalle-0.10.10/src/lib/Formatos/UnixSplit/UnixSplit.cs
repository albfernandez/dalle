/*

	Dalle - A split/join file utility library
	Dalle.Formatos.HJSplit.HJSplit - 
		Split and Join files in HJSplit format.
	
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
using Dalle.Utilidades;
namespace Dalle.Formatos.UnixSplit
{

	public class UnixSplit : Parte {
	
		public UnixSplit ()
		{
			nombre = "unixsplit";
			descripcion = "unixsplit";
			web = "http://";
			parteFicheros = true;
			compatible = true;			
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			if ((sal1 == null) || (sal1 == string.Empty))
			{
				sal1 = new FileInfo (fichero).Name;
			}
			long tamano = new FileInfo (fichero).Length;
			long tFragmento = 1024 * kb;
			long transferidos = 0;
			
			
			int fragmentos = (int) Math.Ceiling((double)tamano / (double)tFragmento);
			
			int suffixLength =  CalculateSuffixLength(fragmentos);

			
			int contador = 0;
			
			OnProgress (0, tamano);
			int leidos = 0;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			FileStream fis = File.OpenRead (fichero);
			
			do 
			{
				string destino = sal1 + "." + GetSuffix (contador, suffixLength);
				Stream fos = UtilidadesFicheros.CreateWriter (destino);
				leidos = 0;
				int parcial = 0;
				while ((leidos = fis.Read (buffer, 0, Math.Min ((int)tFragmento - parcial, buffer.Length))) > 0)
				{
					fos.Write (buffer, 0, leidos);
					transferidos += leidos;
					parcial += leidos;
					OnProgress (transferidos, tamano);
				}
				fos.Close();
				contador++;				
			} while (transferidos < tamano);	
			fis.Close();
		}
		
	
		protected override void _Unir (string fichero, string dirDest)
		{
			int contador = 0;
			if (!fichero.EndsWith ("aa")) 
			{
				throw new Exception ("UnixSplitException");
			
			}
			int suffixLength = 2;
			int punto = fichero.LastIndexOf (".");
			string ficheroBase = "";
			if (punto > 0) 
			{
				ficheroBase = fichero.Substring (0, punto + 1);
				suffixLength = fichero.Length - punto - 1;					
			}
			else 
			{
				ficheroBase = fichero.Substring (0, fichero.Length - 2);
			}
			
			string destino = ficheroBase;
			if (destino.EndsWith (".")) 
			{
				destino = destino.Substring (0, destino.Length - 1);
			}			

			long datosTotales = 0;
			while (File.Exists (ficheroBase + GetSuffix (contador, suffixLength)))
			{
				datosTotales += new FileInfo (ficheroBase + GetSuffix (contador, suffixLength)).Length;
				contador++;
			}
			long transferidos = 0;
			contador = 0;
			OnProgress (0, datosTotales);
			Stream fos = UtilidadesFicheros.CreateWriter (destino);
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			while (File.Exists (ficheroBase + GetSuffix (contador, suffixLength))) {
				
				FileStream fin = File.OpenRead (ficheroBase + GetSuffix (contador, suffixLength));
				int leidas = 0;
				while ((leidas = fin.Read (buffer, 0, buffer.Length)) > 0)
				{
					fos.Write (buffer, 0, leidas);
					transferidos += leidas;
					OnProgress (transferidos, datosTotales);
				}
				fin.Close ();
				contador++;
			}
			fos.Close ();
		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero))
			{
				return false;
			}
			return fichero.EndsWith ("aa");

		}
		public static string GetSuffix (int n, int suffixLength)
		{
			char[] ca = new char[suffixLength];
			ca[ca.Length - 1] = (char)('a' + n % 26);
			for (int i = 2; i <= suffixLength; i++) {
				ca[ca.Length - i] = (char)('a' + ((n / Math.Pow (26, i - 1)) % 26));
			}
			return new string (ca);
			
		}
		public static int CalculateSuffixLength (int fragmentos)
		{
			int ret = 2;
			while (Math.Pow (26, ret) < fragmentos) 
			{
				ret++;
			}
			return ret;
		}

	
	}


}


/*
 * Copyright (C) 2002 - 2005 Leonardo Ferracci
 *
 * This file is part of JAxe.
 *
 * JAxe is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 * 
 * JAxe is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public
 * License along with JAxe; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA  02111-1307, USA.  Or, visit http://www.gnu.org/copyleft/gpl.html
 */
/*
package jd.plugins.optional.hjsplit.jaxe;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

public class UnixSplitJoiner extends JAxeJoiner {

    public UnixSplitJoiner(String sFile) {
        super(sFile);
        sDestDir = new File(sFile).getParent();
    }

    public UnixSplitJoiner(String sFile, String sDir) {
        super(sFile, sDir);
    }

    // @Override
    protected boolean checkNoOverwrite(File f) {
        File fTemp = new File(outputFile);

        return !fTemp.exists();
    }

    // @Override
    protected void computeJobSize() {
        long lReturn = 0;
        int i = 0;
        File fTemp;

        do {
            fTemp = new File(sJoinedFile + getSuffix(i));
            lReturn += fTemp.length();
            i++;
        } while (fTemp.exists());

        lJobSize = lReturn - (isCutkiller == true ? 8 : 0);
    }

    // @Override
    protected void doCleanup() {
        new File(outputFile).delete();
    }

    private String getSuffix(int n) {
        char[] ca = new char[2];

        ca[0] = (char) ('a' + n / 26);
        ca[1] = (char) ('a' + n % 26);

        return "." + new String(ca);
    }

    // @Override
    public void run() {
        File fToJoin, fTemp = null;
        InputStream is = null;
        BufferedOutputStream bos;
        int i = 1, nLength;
        byte[] ba = new byte[BUFFER_SIZE];

        bStopped = false;
        fToJoin = new File(sFileToJoin);

        if (!UnixSplitFileFilter.isSplitFile(sFileToJoin)) {
            dispatchEvent(new JobErrorEvent(this, "File to join does not seem a file split using Unix split"));
            return;
        }
        sJoinedFile = UnixSplitFileFilter.getJoinedFileName(sFileToJoin);
        if (isCutkiller) {
            outputFile = sJoinedFile + "." + CutKillerExt;
        } else {
            outputFile = sJoinedFile;
        }

        if (!fToJoin.exists() || fToJoin.isDirectory()) {
            dispatchEvent(new JobErrorEvent(this, "File to join does not exist or is a directory"));
            return;
        }

        if (!checkNoOverwrite(fToJoin)) {
            dispatchEvent(new JobErrorEvent(this, "Error: destination file already exists!"));
            return;
        }

        try {
            bos = new BufferedOutputStream(new FileOutputStream(outputFile));
        } catch (FileNotFoundException fnfe) {
            dispatchEvent(new JobErrorEvent(this, "Error while opening: " + outputFile + " (" + fnfe.getMessage() + ")"));
            return;
        }

        computeJobSize();
        System.out.println("Job size: " + lJobSize);
        initProgress();
        i = 0;
        try {
            do {
                if (is == null) {
                    if (i == 0) {
                        fTemp = new File(sFileToJoin);
                    } else {
                        fTemp = new File(sJoinedFile + getSuffix(i));
                    }

                    if (!fTemp.exists()) {
                        break;
                    } else {
                        is = new BufferedInputStream(new FileInputStream(fTemp));
                    }
                }

                if (!bStopped) {
                    nLength = is.read(ba, 0, BUFFER_SIZE);
                    if (nLength > 0) {
                        bos.write(ba, 0, nLength);
                        lCurrent += nLength;
                        dispatchProgress();
                    } else {
                        i++;
                        is.close();
                        is = null;
                    }
                }
            } while (!bStopped);
        } catch (FileNotFoundException fnfe) {
            dispatchEvent(new JobErrorEvent(this, "Error while opening: " + fTemp.getName()));
            return;
        } catch (IOException ioe) {
            dispatchEvent(new JobErrorEvent(this, "I/O error with file " + fTemp.getName() + " (" + ioe.getMessage() + ")"));
            return;
        } finally {
            try {
                bos.close();
                if (is != null) {
                    is.close();
                }
            } catch (IOException ioe) {
            }
        }

        if (bStopped) {
            doCleanup();
            dispatchEvent(new JobEndEvent(this, "Join stopped by user."));
        } else {
            dispatchProgress(lJobSize);
            dispatchEvent(new JobEndEvent(this, "Join terminated."));
        }
    }
}
*/
