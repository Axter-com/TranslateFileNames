# Translate File Names
Includes both a windows GUI and a command line utility to translate and rename files from their source language to the set language. By default, the set language is the local system TwoLetterISOLanguageName.

## What does it do?

The program renames all the files from their source language to the specified target language (default: locale).

Example use-case:
	Translate foreign movie *.mp4 file names.
	Translate names of songs ripped from an imported foreign album.

## Command Line Example Usage:


TranslateFileNames -r

TranslateFileNames "C:\Users\jane-doe\Pictures" -r -ext *.jpg

## Authors

* **David Maisonave** - [David-Maisonave](https://github.com/David-Maisonave)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
