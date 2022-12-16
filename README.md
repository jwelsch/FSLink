# FSLink

A command line application for creating and viewing symbolic links, hard links, and junctions. It also shows data stored in reparse points.

## Commands

- `create`: Creates a symbolic link, hard link, or junction.
  - `link-type`: The type of link to create. Valid values include: `SymbolicLink`, `HardLink`, `Junction`
  - `link-path`: The path of the link in the file system.
  - `target-path`: The path the link points to in the file system.
- `delete`: Deletes a symbolic link, hard link, or junction.
  - `path`: The path of the link to delete.
- `relink`: Resets the target of a link to a new path.
  - `link-path`: The path of the link in the file system.
  - `new-target-path`: The path of the new target in the file system.
- `scan`: Scans a directory for file system links and prints the results to the console.
  - `path`: Path of the directory to scan.
  - `recurse`: Include to recurse through child directories, exclude to only scan the top level directory.
- `reparse`: Displays information in the console about a reparse point attached to the item with the specified path.
  - `path`: Path to the file system item with an attached reparse point.

Copyright © Justin Welsch 2020-2022