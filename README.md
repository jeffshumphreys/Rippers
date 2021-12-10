# Rippers (SSISRipper specifically)
Decompiles, Decomposes, Deconstructs, Reverse Engineers various complex objects, files or in situ.  Currently, this is actually the Solution SSISRipper, but it was originally presupposing I would create one project per class of Ripper, but that may not be realistic.

I don't see any good SSIS package decompilers.  I just want to run through folders of SSIS package files and list out the Execute SQL Tasks, the Script Tasks, the relationships and flow between tasks, and the variables and usage of dynamic expressions, and the connection details.

Current tools I find out there either cost, or closed source, or don't explode all the data. Also, I don't want to see properties that are default values; too much clutter.

# Environment
.NET 4.62 Framework (_Necessary for SSIS DTS DAC objects used_)

C# is at 7.3, though some nugets are .net standard 2.1, which is 8.0.

VS 2019 16.11.7 (_won't be upgrading to 2022 for while. No SSDT_) DEBUG mode, both TRACE and DEBUG defined. Currently decided not to Allow unsafe code, since that's a blocker for many users.

32-bit x86 (_some libraries balk at 64-bit_)

SSDT 16,9,62111,11970

SSIS 15.0.2000.170 (_may just be for VS, not the referenced dll's in the project_)

