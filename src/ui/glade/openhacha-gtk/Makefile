# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU Library General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.




BASEPATH=../../../..
include $(BASEPATH)/rules.make

PROGNAME=openhacha-gtk.exe
WRAPNAME=openhacha

TARGET=$(BUILDDIR)/$(PROGNAME)
WRAPPER=$(BUILDDIR)/$(WRAPNAME)
SOURCEDIR=src
GNOME_ASSEMBLY = -r gnome-sharp
GLADE_ASSEMBLY = -r glade-sharp
RESOURCES = -resource:src/openhacha.glade,openhacha.glade


all: $(TARGET)


$(TARGET): $(LIBRARY) $(SOURCEDIR)/*.cs
	@echo $(LINEA)
	@echo "Compilando OpenHacha (gtk)"
	@echo $(LINEA)
	$(MCS) $(MCS_FLAGS) -o $(TARGET) -r $(LIBRARY) $(GTK_RESOURCES) \
	$(GNOME_ASSEMBLY) $(GLADE_ASSEMBLY) $(RESOURCES) -recurse:$(SOURCEDIR)/*.cs 
	@echo "#!/bin/sh" > $(WRAPPER)
	@echo "" >> $(WRAPPER)
	@echo "$(MONO) $(MONO_FLAGS) $(PREFIX)/bin/$(PROGNAME)" >> $(WRAPPER)
	@echo "" >> $(WRAPPER)
	   
install: all
	$(INSTALL) $(TARGET) $(DESTDIR)/$(PREFIX)/bin
	$(INSTALL) $(WRAPPER)  $(DESTDIR)/$(PREFIX)/bin

uninstall:
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(WRAPNAME)
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(PROGNAME)
	
clean:
	rm -f $(TARGET) $(WRAPPER)
	
.PHONY: clean uninstall
