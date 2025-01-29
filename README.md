Nmpq - A Fully-Managed C# MPQ Parser
==================================
[![NuGet](https://img.shields.io/nuget/v/Nmpq.Standard.svg)](https://www.nuget.org/packages/Nmpq.Standard/)

This is a fork from [Thenarden/nmpq](https://github.com/Thenarden/nmpq) that has been updated to .NET standard 2.0 for use with modern C# projects.

Rationale for a new MPQ parser
------------------------------

* I was having trouble using [MpqLib](https://github.com/Hernrup/MpqLib) in its various incarnations because it relies on [StormLib](https://github.com/stormlib/StormLib).
* StormLib (and by extension, MpqLib) does not directly support parsing in-memory MPQ archives. You must save archives to a file before parsing them. (I supposed I could have added this features to StormLib, but I really didn't want to work with C that day).
* StormLib does way more than I need - I do not need to compose MPQ files, only parse them efficiently.
* I had a bunch of spare time and I was going through a coding dry-spell; I needed an interesting project to chew on.

Useful Information
------------------
#### [MPQ File Format](http://www.zezula.net/en/mpq/mpqformat.html)
Detailed documentation of the MPQ file format. I have noticed some inconsistencies with my own observations (e.g., regarding version numbers), but for the most part it's dead on.

#### [StormLib](https://github.com/stormlib/StormLib)
_The_ native MPQ library that everyone uses.

#### [MpqLib](https://github.com/Hernrup/MpqLib)
C# bindings to StormLib. If you need to create/modify MPQ archives in a .NET language, you probably want to use this.

License
--------
Nmpq is licensed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

Features
--------

* Can directly parse in-memory MPQ archives from either a `byte[]` or a `Stream`. (Of course, it also supports parsing archives from disk)
* Supports MPQ archives version "1" and "3".
* Utility methods to parse serialized data structures from Starcraft 2 replay files. 
* Full suite of tests.

Limitations
-----------

I can't stress enough that __Nmpq was originally designed with a specific use-case in mind__. I need to parse MPQ archives uploaded as part of an HTTP request without saving them to disk. Specifically, I need to parse Starcraft 2 replay files. Thus, the library has the following limitations:

* Only supports archives that use Deflate and BZip2 compression.
* Only supports reading archives; there is no support for creating or modifying archives.

There are probably lots of other specific cases it doesn't support (e.g., older archives may have different hash table and block table formats/layouts that Nmpq wasn't designed to read). Again, this library was originally designed to fit my specific use case, not a general use case.

Contributing
------------

#### Testing
Honestly, the easiest way to contribute is to send me files that Nmpq can't successfully parse. All of the MPQ files that I regularly work with parse perfectly, but I'm only using a limited subset of the possible MPQ configurations.


#### Code
If you would like to add a feature or fix a bug, please:

* Write some code.
* Write some tests.
* Send me a pull request with a half-decent description of what your code does and why.

I will then: 

* Review your code.
* Make sure you have good test coverage and that your tests pass.
* Reformat your code to be consistent with the rest of the project, if necessary. 
* Merge your contribution into the main branch.
* Give you kudos. 
 
In general, I would like to make Nmpq more generic such that it supports more MPQ configurations. For the time being, I do _not_ want to add features oriented around _creating or modifying_ MPQ files; I want Nmpq to remain a read-only library for the sake of simplicity. As such, I will probably not accept contributions that add such features to Nmpq, unless they contain some really good rationale. If you think Nmpq would benefit from such features, send me a message and we'll chat.