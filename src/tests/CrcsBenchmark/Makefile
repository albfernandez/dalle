
BASEPATH=../../..
include $(BASEPATH)/rules.make

EXES=CrcsBenchmark.exe


all : $(EXES)




$(EXES): %.exe : %.cs 
	$(MCS) $(MCS_FLAGS) -r $(LIBRARY) -r ICSharpCode.SharpZipLib -o $@ $<

clean:
	rm -f *.exe *.dll
