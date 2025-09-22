# sharp-grep

## Overview

- **SharpGrep** is a cross-platform, command-line clone of the Unix grep command, written from scratch in C#. The tool reads text from standard input or from one or more files/directories given on the command line, searches it with a regular-expression pattern, and prints results according to specific options.

---

## Key Features

- **Pattern matching** with case-insensitive (`-i`) and whole-word (`-w`) options.
- **Output modes:** print full lines (default) or only matched words (`-o`).
- **Context controls:** trailing (`-A N`), leading (`-B N`), or both (`-C N`).
- **Summaries:** count only (`-c`), list files with matches (`-l`) or without (`-L`).
- **Recursive search:** recursive directory search (`-r`).
- **Early stop:** stop after N matching lines per file (`-m N`).
- **Binary detector:** detect likely binary files (NUL-byte heuristic) and print `Binary file <path> matches`.
- **Usability:** help (`-h`, `--help`).

---
## User Guide

### Installation & Building

```bash
# clone
git clone https://github.com/glaukaliu/sharp-grep.git
cd sharp-grep

# build
dotnet build

# run 
dotnet run --project src/SharpGrep -- -h

# tests
dotnet test
```
---

### Arguments

```
-i            Ignore case distinctions
-w            Match whole words only
-o            Print only the matched parts of a line
-A N	      Print N lines of context AFTER matching lines
-B N    	  Print N lines of context BEFORE matching lines
-C NU         Print N lines of context BEFORE and AFTER matching lines
-c            Print only a count of matching lines per file
-l            Print only names of files with matches
-L            Print only names of files without matches
-r            Recursively search directories
-m N          Stop reading a file after N matching lines
-h, --help    Show this help message and exit
```

### Basic examples
replace `shgrep` with `dotnet run --project src/SharpGrep`
```bash
# stdin
echo "Hello cat" | shgrep cat
# single file
shgrep -i cat file.txt
# matches only
shgrep -o cat file.txt
# context
shgrep -C 2 cat file.txt
# count matches per file
shgrep -c cat src/*
# recursive
shgrep -r cat files
# stop after first match
shgrep -m 1 cat file.txt
```


---
## Developer Guide

### Main Functionality

At a high level:

1. **Argument parsing → `Options`**  
   `ArgParser` validates flags, numbers, mutual exclusions (`-l` vs `-L`, etc.), pattern, and inputs.

2. **Choose output type**  
   - If you requested summaries (`-c`, `-l`, `-L`), `SummaryPrint` runs (counts and lists).  
   - Otherwise, `NormalPrint` streams lines and prints them (or only matches) with optional context.

3. **Expand inputs**  
   `FileEnumerator` turns file/dir arguments (plus `-r`) into a flat file list and sets `multipleFiles` so formatting can prefix file names when needed.

4. **Binary detection**  
   Before streaming, `BinaryDetector` goes through up to 8 KiB and checks for a NUL byte, and the number of control bytes present. If found and the pattern matches, we print: 
   `Binary file <path> matches`.

5. **Matching**  
   `RegexMatcher` compiles the pattern once and exposes `IsMatch` and `CollectMatches`.  
   - `-i` adds `RegexOptions.IgnoreCase`.  
   - `-w` wraps the pattern with `\b(?: … )\b`.  
   - Does not support regex patterns.

6. **Context**  
   `Context` holds a fixed-size queue for *before* lines. Then, the *after* lines are tracked with a simple counter.


### Components 

- **Program**: Parse → choose output type → run; handles `HelpRequestedException` to print help/version.
- **ArgParser**: Strict parser that:
  - Validates numeric options (`-A/-B/-C/-m`) are non-negative (or positive) integers.
  - Does not accept conflicting flags (`-l` with `-L`, `-c` with `-l/-L`).
  - Fills an `Options` instance.
- **Options**: - Class that contains settings (pattern, inputs, ignoreCase, wholeWord, onlyMatches, before/after, countOnly, listWithMatches, listWithoutMatches, recursive, searchStop).
- **NormalOutput**: Line-by-line processing with:
  - `Context` for before lines and `remainingAfter` for after lines.
  - Two types of outputs: full lines vs only matches mode (`-o`).
- **SummaryOutput**: Optimized for `-c/-l/-L`.
- **RegexMatcher**: Uses `isMatch` and `collectMatches` to find pattern matches in the text.
- **FileEnumerator**: Directory expansion with `-r`.
- **Context**: A fixed-size FIFO queue storing the **before** lines. 
- **BinaryDetector**: Checks for any NUL byte, and the number of control bytes.
- **Output**: Multiple functions for different types of printing.
- **ExitCodes**: `Success = 0`, `NoMatchesFound = 1`, `ArgumentError = 2`, `FileError = 3`.
- **Exceptions**: Declares `HelpRequestedException` exception, used for detecting `--help/-h` option.

---
## License
MIT
