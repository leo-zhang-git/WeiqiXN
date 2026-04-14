#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Excel导出器
将Excel数据导出为JSON格式和C#数据类
"""

import json
from pathlib import Path
from typing import Optional, Tuple, List, Dict, Any

try:
    from excel_checker import ExcelChecker, ValidationError, _is_list_type, _get_inner_type, _is_tuple_type, _get_tuple_types
except ImportError:
    raise ImportError("缺少 excel_checker 模块，请确保在同一目录下")


def to_camel_case(name: str) -> str:
    """将下划线命名转为驼峰命名"""
    parts = name.split('_')
    return parts[0] + ''.join(word.capitalize() for word in parts[1:])


def type_to_csharp(type_name: str) -> str:
    """将Excel类型转为C#类型"""
    type_map = {
        'string': 'string',
        'int': 'int',
        'float': 'float',
        'boolean': 'bool',
    }
    if _is_list_type(type_name):
        inner_type = _get_inner_type(type_name)
        cs_inner = type_map.get(inner_type, inner_type)
        return f"{cs_inner}[]"
    if _is_tuple_type(type_name):
        tuple_types = _get_tuple_types(type_name)
        cs_types = ', '.join(type_map.get(t, t) for t in tuple_types)
        return f"({cs_types})"
    return type_map.get(type_name, type_name)


class ExcelExporter:
    """Excel导出器"""

    def __init__(self, excel_path: str | Path):
        """初始化导出器"""
        self.excel_path = Path(excel_path)
        self.excel_name = self.excel_path.stem
        self.checker = ExcelChecker(excel_path)

    def load(self):
        """加载Excel工作簿"""
        self.checker.load()

    def close(self):
        """关闭工作簿"""
        self.checker.close()

    def get_valid_sheets(self):
        """获取有效sheet列表"""
        return self.checker.get_valid_sheets()

    def export_to_json(self, data: dict, output_path: str | Path,
                       indent: int = 2, ensure_ascii: bool = False) -> str:
        """导出数据到JSON文件"""
        output_path = Path(output_path)
        json_str = json.dumps(data, indent=indent, ensure_ascii=ensure_ascii)
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(json_str)
        return json_str

    def excel_to_csharp(self, headers: List[Dict[str, Any]], sheet_name: str, output_dir: Path) -> Tuple[bool, str, Optional[Path]]:
        """根据表头导出C#数据类"""
        try:
            def capitalize(name: str) -> str:
                """首字母大写"""
                if not name:
                    return name
                return name[0].upper() + name[1:]

            if sheet_name == self.excel_name:
                class_name = f"{capitalize(to_camel_case(self.excel_name))}DataType"
            else:
                class_name = f"{capitalize(to_camel_case(self.excel_name))}{capitalize(to_camel_case(sheet_name))}DataType"

            # JSON 文件名直接用 sheet 名
            json_file_name = f"{sheet_name}.json"

            lines = [
                "using Newtonsoft.Json.Linq;",
                "using System;",
                "using System.Collections.Generic;",
                "using System.IO;",
                "using XNClient.Logger;",
                "",
                "public class " + class_name,
                "{",
            ]

            for header in headers:
                field_name = header['key']
                type_name = header['type']
                display_name = header['display_name']
                cs_type = type_to_csharp(type_name)
                comment = f"  // {display_name}" if display_name else ""
                lines.append(f"    public {cs_type} {field_name};{comment}")

            # 添加静态字典字段
            lines.append("")
            dict_class_name = class_name.replace("DataType", "") + "Dict"
            lines.append(f"    public static Dictionary<string, {class_name}> {dict_class_name};")

            # 添加静态获取方法
            lines.append("")
            lines.append(f"    public static {class_name} GetConfigData(string id)")
            lines.append("    {")
            lines.append(f"        if ({dict_class_name} == null) {{")
            lines.append(f"            {dict_class_name} = new Dictionary<string, {class_name}>();")
            json_path_line = f'            string jsonPath = Path.Combine(GlobalConfig.PATH_CONFIG_JSON, "{self.excel_name}", "{json_file_name}");'
            lines.append(json_path_line)
            lines.append("            var jsonObj = JObject.Parse(File.ReadAllText(jsonPath));")
            lines.append("            foreach (var property in jsonObj.Properties()) {")
            lines.append("                try {")
            lines.append(f"                    var item = property.Value.ToObject<{class_name}>();")
            lines.append(f"                    {dict_class_name}[property.Name] = item;")
            lines.append("                }")
            lines.append("                catch (Exception ex) {")
            lines.append("                    XNLogger.LogError($\"读表错误，跳过条目 {property.Name}: {ex.Message}\");")
            lines.append("                }")
            lines.append("            }")
            lines.append("        }")
            lines.append(f"        if ({dict_class_name}.TryGetValue(id, out {class_name} data)) {{")
            lines.append("            return data;")
            lines.append("        } else {")
            lines.append("            return null;")
            lines.append("        }")
            lines.append("    }")

            lines.append("}")

            cs_code = "\n".join(lines)

            output_dir.mkdir(parents=True, exist_ok=True)
            output_path = output_dir / f"{class_name}.cs"
            with open(output_path, 'w', encoding='utf-8') as f:
                f.write(cs_code)

            return True, "", output_path
        except Exception as e:
            return False, str(e), None

    def export_sheet(self, sheet_name: str, output_dir: Optional[Path] = None) -> Tuple[bool, str, Optional[Path], Optional[Path]]:
        """导出单个工作表"""
        try:
            self.checker.set_active_sheet(sheet_name)
            headers = self.checker.parse_headers(sheet_name)
            data = self.checker.validate_data(headers, sheet_name)

            if output_dir is None:
                script_dir = Path(__file__).parent
                output_dir = script_dir / 'DataJson' / self.excel_name

            output_dir.mkdir(parents=True, exist_ok=True)

            # 导出JSON到 DataJson/xlsx名/（json文件名直接用sheet名）
            json_dir = output_dir
            json_path = json_dir / f"{sheet_name}.json"
            self.export_to_json(data, json_path)

            # 导出C#到 DataType/xlsx名/
            cs_dir = Path(__file__).parent / 'DataType' / self.excel_name
            cs_success, cs_error, cs_path = self.excel_to_csharp(headers, sheet_name, cs_dir)

            return True, f"共 {len(data)} 条数据", json_path, cs_path

        except ValidationError as e:
            return False, f"校验失败: {e}", None, None
        except FileNotFoundError as e:
            return False, f"文件错误: {str(e)}", None, None
        except Exception as e:
            return False, f"未知错误: {str(e)}", None, None

    def export_all(self, output_dir: Optional[Path] = None) -> list:
        """导出所有有效工作表"""
        results = []
        for sheet_name in self.get_valid_sheets():
            success, message, json_path, cs_path = self.export_sheet(sheet_name, output_dir)
            results.append((sheet_name, success, message, json_path, cs_path))
        return results


def check_datajson_link() -> bool:
    """检查DataJson符号链接是否存在"""
    import os
    script_dir = Path(__file__).parent
    datajson_path = script_dir / 'DataJson'
    if not datajson_path.exists():
        return False
    try:
        return os.path.islink(datajson_path) or os.path.isdir(datajson_path)
    except:
        return False


def prompt_setup_for_datajson():
    """提示用户运行setup.bat创建DataJson链接"""
    print("""
============================================================
[错误] DataJson 符号链接不存在
============================================================

导出JSON需要将文件保存到Unity项目中，但链接尚未创建。

请先运行 setup.bat 创建链接：
  1. 双击运行 setup.bat
  2. 如果提示需要管理员权限，请允许

创建链接后，重新运行导出命令。
============================================================
"""
)


def main():
    """命令行入口"""
    import sys
    from pathlib import Path
    import argparse
    parser = argparse.ArgumentParser(
        description='Excel表格导出JSON和C#工具',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog='''
表头格式要求（前4行）：
  第1行：列的中文名称（仅显示，不参与导出）
  第2行：导出到JSON的key名（必须是有效的变量名）
  第3行：数据类型限制（string, float, int, boolean, list(...)）
  第4行：额外检查（必须是以#开头的字符串）
  第5行起：数据行

说明：
  - 自动遍历xlsx文件中所有不以#开头的sheet
  - 导出JSON到 DataJson/xlsx文件名/ 目录下
  - 导出C#到 DataType/xlsx文件名/ 目录下

示例：
  python excel_exporter.py config.xlsx
        '''
    )
    parser.add_argument('input', help='输入的Excel文件名（从xlsx文件夹中查找，可不带.xlsx后缀）')
    args = parser.parse_args()
    workspace = Path(__file__).parent
    xlsx_dir = workspace / 'xlsx'
    input_name = args.input
    if not input_name.lower().endswith('.xlsx'):
        input_name = input_name + '.xlsx'
    xlsx_file = xlsx_dir / input_name
    if not xlsx_file.exists():
        print(f"错误: 未找到文件 '{input_name}'，请确认文件在xlsx文件夹中")
        sys.exit(1)
    exporter = ExcelExporter(xlsx_file)
    exporter.load()
    valid_sheets = exporter.get_valid_sheets()
    if not valid_sheets:
        print("错误: 没有找到有效的sheet（所有sheet名都以#开头）")
        exporter.close()
        sys.exit(1)
    if not check_datajson_link():
        prompt_setup_for_datajson()
        exporter.close()
        sys.exit(1)
    print(f"\n找到 {len(valid_sheets)} 个有效sheet: {', '.join(valid_sheets)}")
    print("-" * 50)
    results = exporter.export_all()
    exporter.close()
    success_count = 0
    fail_count = 0
    for sheet_name, success, message, json_path, cs_path in results:
        if success:
            print(f"[{sheet_name}] 成功: {message}")
            print(f"         JSON: {json_path}")
            print(f"         C#:   {cs_path}")
            success_count += 1
        else:
            print(f"[{sheet_name}] 失败: {message}")
            fail_count += 1
    print("-" * 50)
    print(f"导出完成: {success_count} 成功, {fail_count} 失败")
    sys.exit(0 if fail_count == 0 else 1)


if __name__ == '__main__':
    main()
