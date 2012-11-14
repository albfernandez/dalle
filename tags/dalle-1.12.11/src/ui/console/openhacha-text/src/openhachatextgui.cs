/*

    OpenHacha, a "free as in freedom" implementation of Hacha.	
    Copyright (C) 2003  Ramón Rey Vicente <ramon.rey@hispalinux.es>


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
using System.Text;

namespace OpenHachaTextGui
{
	public class OpenHachaText
	{
		public static void Main (string[] args)
		{
			OpenHachaText OpenHachaTextMain = new OpenHachaText();
			
			if (args.Length > 1) {
				switch (args[0]) {
					case "split":
						if (File.Exists(args[1]) && (args.Length > 2)) {
							try {
								OpenHachaTextMain.SplitFile(args[1], Convert.ToInt32(args[2]));
							}
							catch (FormatException)
								{OpenHachaTextMain.ShowWarningMessage();}
							finally
							{}
						} else {
							Console.WriteLine("Warning: There is no file with that name");
						}
						break;
				
					case "paste":
						OpenHachaTextMain.PasteFile(args[1]);
						break;

					default:
						OpenHachaTextMain.ShowWarningMessage();
						break;
				}
			} else {
				OpenHachaTextMain.ShowWarningMessage();
			}
				
		}
		
		public void SplitFile (string file, int size)
		{
			Dalle.Formatos.Manager.GetInstance().Partir ("hacha1",
					file,
					file.Substring(0, file.LastIndexOf('.')),
					size);

		}
		
		public void PasteFile (string file)
		{
			Dalle.Formatos.Manager.GetInstance().Unir(file,	
					new FileInfo (file).DirectoryName);
		}

		public void ShowWarningMessage()
		{
			Console.WriteLine("\nWarning: Unknown command\n");
			Console.WriteLine("OpenHacha 0.5");
			Console.WriteLine("\nUsage:\n\t- split a file:\n\t\topenhacha-text split filename size(in KiB)\n\n\t- paste the fragments of a file:\n\t\topenhacha-text paste basename.0\n");	
		}
	}
}
