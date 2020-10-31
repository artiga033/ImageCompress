# ImageCompress
**中文(简体)**| [English(US)](README_en.md)

使用GDI+ 进行图片压缩
## 需求

- .NET Core3.1 运行时环境
- GDI+ 支持(如果使用Linux，请安装 `libgdiplus`)

## 使用

```
ImageCompress.Cli [options] <source> <destination>

Arguments:
  source                  包含源文件/文件夹的路径
  destination             输出文件的路径        默认：.

Options:
  -q|--quality <QUALITY>  jpeg编码器的“quality”(质量) 参数        默认: 80
  --no-keep-struct        指定此选项以禁用保持目录结构
  --no-recurse            指定此选项以禁用递归(不会包含子文件夹)
  -?|-h|--help            显示帮助信息
```