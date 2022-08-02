[![Screenshot1](doc/screenshot1.png)](https://github.com/David-Maisonave/TranslateFileNames)

# Translate File Names
Includes both a windows GUI and a command line utility to translate and rename files from their source language to the set language. By default, the set language is the local system TwoLetterISOLanguageName.

## What does it do?

The program renames all the files from their source language to the specified target language (default: locale).

Example use-case:
	Translate foreign movie *.mp4 file names.
	Translate names of songs ripped from an imported foreign album.

# Content

[Features](README.md#Features)
-  [1. Translates all file names in folder](README.md#1.-Translates-all-file-names-in-folder)


## Features

#### Translates all file names in folder

Scans all files in selected folder, and only displays files having names in different language from the targetted language.

File renaming does not occur untill one of the following options is selected.
-  [Rename All]

		Renames all files displayed on the list.

-  [Rename Selectedl]

		Renames only selected files on the list.

-  [Rename Unselected]

		Renames only files that are not selected.

-  [Rename Checked]
	
		Renames only files that have been checked.

-  [Rename Unchecked]
	
		Renames only files which have not been checked.




#### 2.  Modify translated name

The translated name on the list, can be edited before performing the rename action.

#### 3.  Search Recursively

By default, only the files on the root directory are scanned, but by selecting this option, the scan will also search all sub folders.

#### 4.  Long Path Support

By selecting this option, the rename will occur even if the full path is longer then 255 characters.

#### 5.  Append Original Name 

When selected, this options includes the original file name when renaming the file.  Example: TranslatedName(OriginalName).gif

#### 6.  Append Language Name

When selected, appends the source language name to the renamed file. Example: TranslatedName_[Russian].gif

#### 7.  File Type

This option can be used to scan only file a specific file extension. Example: .gif

#### 8.  Max Threads

By default the programs uses the ProcessorCount to determine the maximum threads to use.  This options allows the end user to override that option.  The minimum value is 4, and the maximum value is 400.

#### 9.  Max Translation Len

The translation length is used when the program translates many files in a single translation request.  This happens if there are many files (over 100), or if the user selects option to translate many files per request.

This value is set to 10000 by default.  The minimum value is 255, and the maximum value is 10,000.

#### 10  Files-Per-Translation-Req

This options determines if one file is used per translation request, or if many files are used per translation request. The following are the possible options to select from the combobox window.

-  [Auto]

		This is the default option. It automatically sets the best method depending on the totoal number of files to check for translation and the maximum thread settings.

-  [OnePerFile]

		Only one file is sent per translation request. This is the preferred option if the files have different languages.

-  [Many]

		Multiple files are sent per translation request. This is the perferred option if the files are all in either the source language or the target language.


#### 11  Target Language

This setting is an [ISO 639-1](https://wikipedia.org/wiki/List_of_ISO_639-1_codes) two letter code.
By default, the target language is set to the operating system language settings. (CurrentCulture.TwoLetterISOLanguageName)

Use this option to override the target language.  See [ISO 639-1](https://wikipedia.org/wiki/List_of_ISO_639-1_codes) link to get desired language code.

#### 12  Source Language

This is empty by default. When this value is empty, the translation works in "Auto" mode, which lets the translator determine the source language.

For most use cases, this value should be left empty.  If populated, use [ISO 639-1](https://wikipedia.org/wiki/List_of_ISO_639-1_codes) link to get desired language code.

#### 13  Filter

This option allows user to use keywords to find or filter the list to only items having the keyword(s).

When this option is used, and the "Rename All" option is selected, only the resulting filtered items displayed are renamed.

[![Screenshot2](doc/screenshot2.png)](https://github.com/David-Maisonave/TranslateFileNames)

# Console Program

#### Command Line Example Usage:

-  TranslateFileNames -r

-  TranslateFileNames "C:\Users\jane-doe\Pictures" -r -ext *.jpg


# Authors

* **David Maisonave** - [David-Maisonave](https://github.com/David-Maisonave)


# License

-  This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
