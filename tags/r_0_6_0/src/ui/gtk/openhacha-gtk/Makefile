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
WRAPNAME1=openhacha-gtk
WRAPNAME2=openhacha

TARGET=$(BUILDDIR)/$(PROGNAME)
WRAPPER=$(BUILDDIR)/$(WRAPNAME1)
WRAPPER2=$(BUILDDIR)/$(WRAPNAME2)
SOURCEDIR=src
GNOME_ASSEMBLY = -r gnome-sharp.dll
GLADE_ASSEMBLY = -r glade-sharp.dll
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
	@echo "$(MONO) $(MONO_FLAGS) $(PREFIX)/bin/$(PROGNAME)" >>$(WRAPPER)
	@echo "" >> $(WRAPPER)
	@echo "#!/bin/sh" > $(WRAPPER2)
	@echo "$(PREFIX)/bin/$(WRAPNAME1)" >> $(WRAPPER2)
	@echo "" >> $(WRAPPER2)
	   
install: all
	$(INSTALL) $(TARGET) $(DESTDIR)/$(PREFIX)/bin
	$(INSTALL) $(WRAPPER)  $(DESTDIR)/$(PREFIX)/bin
	$(INSTALL) $(WRAPPER2) $(DESTDIR)/$(PREFIX)/bin

uninstall:
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(WRAPNAME1)
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(WRAPNAME2)
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(PROGNAME)
	
clean:
	rm -f $(TARGET) $(WRAPPER) $(WRAPPER2)
	
.PHONY: clean uninstall
