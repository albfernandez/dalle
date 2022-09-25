
MONO = mono
MCS = mcs
MCSFLAGS = -debug
MONOFLAGS = --debug
MAKEWEB = MakeWeb.exe
TEMPLATE = src/template/index.html.in
SOURCEDIR = src
DESTDIR = deploy

all: $(MAKEWEB) $(SOURCEDIR)/NEWS $(SOURCEDIR)/web.xml $(TEMPLATE) $(DESTDIR)/styles.css
	$(MONO) $(MONOFLAGS) $(MAKEWEB) $(SOURCEDIR)/web.xml $(TEMPLATE)  $(DESTDIR)

$(MAKEWEB) : MakeWeb.cs
	$(MCS) $(MCSFLAGS) MakeWeb.cs
$(DESTDIR)/styles.css: $(SOURCEDIR)/template/styles.css
	cp $< $@
	
$(SOURCEDIR)/NEWS: ../../../NEWS
	cp $< $@
	
clean:
	rm -f $(DESTDIR)/*.html *.exe $(DESTDIR)/*.css src/NEWS *.exe.mdb

# upload:
#	 scp -r deploy/*.* infjaf@web.sourceforge.net:/home/project-web/dalle/htdocs
	

.PHONY: clean
