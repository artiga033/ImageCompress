# ImageCompress
[中文(简体)](README.md) | **English(US)**

Compress images to .jpg format using GDI+.
## Requirements

- .NET Core3.1 runtime
- GDI+ support(For linux,install `libgdiplus`)

## Usage

```
ImageCompress.Cli [options] <source> <destination>

Arguments:
  source                  Path to the file/folder containing raw files
  destination             Path where the output will be        Default: .

Options:
  -q|--quality <QUALITY>  The "quality" parameter for the jpeg encoder        Default: 80
  --no-keep-struct        If specified, the output folder won't keep the raw folder's directory struct
  --no-recurse            If specifiled, images in raw folder's child folders won't be included
  -?|-h|--help            Show help information
```