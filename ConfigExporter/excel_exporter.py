#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Excel JSON导出器
将Excel数据导出为JSON格式
"""

import json
from pathlib import Path
from typing import Optional, Tuple

try:
    from excel_checker import ExcelChecker, ValidationError
except ImportError:
    raise ImportError("缺少 excel_checker 模块，请确保在同一目录下")


class JsonExporter:
    """JSON导出器"""
    
    def __init__(self, excel_path: str | Path):
        """
        初始化导出器
        
        Args:
            excel_path: Excel文件路径
        """
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
        """
        导出数据到JSON文件
        
        Args:
            data: 要导出的数据字典
            output_path: 输出文件路径
            indent: JSON缩进空格数
            ensure_ascii: 是否转义非ASCII字符
            
        Returns:
            导出的JSON字符串
        """
        output_path = Path(output_path)
        json_str = json.dumps(data, indent=indent, ensure_ascii=ensure_ascii)
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(json_str)
        return json_str
        
    def export_sheet(self, sheet_name: str, output_dir: Optional[Path] = None) -> Tuple[bool, str, Optional[Path]]:
        """
        导出单个工作表
        
        Args:
            sheet_name: 工作表名称
            output_dir: 输出目录（可选，默认导出到DataJson/目录下）
            
        Returns:
            (是否成功, 消息, 输出文件路径)
        """
        try:
            self.checker.set_active_sheet(sheet_name)
            headers = self.checker.parse_headers(sheet_name)
            data = self.checker.validate_data(headers, sheet_name)
            
            # 生成输出文件名
            if sheet_name == self.excel_name:
                json_name = self.excel_name
            else:
                json_name = f"{self.excel_name}_{sheet_name}"
            
            if output_dir is None:
                # 默认导出到 DataJson/xlsx文件名/ 目录下
                script_dir = Path(__file__).parent
                output_dir = script_dir / 'DataJson' / self.excel_name
            
            # 确保输出目录存在
            output_dir.mkdir(parents=True, exist_ok=True)
            
            output_path = output_dir / f"{json_name}.json"
            
            # 导出JSON
            self.export_to_json(data, output_path)
            
            return True, f"[{sheet_name}] 导出成功，共 {len(data)} 条数据", output_path
            
        except ValidationError as e:
            return False, f"校验失败: {e}", None
        except FileNotFoundError as e:
            return False, f"文件错误: {str(e)}", None
        except Exception as e:
            return False, f"[{sheet_name}] 未知错误: {str(e)}", None
    
    def export_all(self, output_dir: Optional[Path] = None) -> list:
        """
        导出所有有效工作表
        
        Args:
            output_dir: 输出目录
            
        Returns:
            [(sheet名称, 是否成功, 消息, 输出路径), ...]
        """
        results = []
        for sheet_name in self.get_valid_sheets():
            success, message, output_path = self.export_sheet(sheet_name, output_dir)
            results.append((sheet_name, success, message, output_path))
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
""")


def main():
    """命令行入口"""
    import sys
    
    from pathlib import Path
    import argparse
    
    parser = argparse.ArgumentParser(
        description='Excel表格导出JSON工具',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog='''
表头格式要求（前4行）：
  第1行：列的中文名称（仅显示，不参与导出）
  第2行：导出到JSON的key名（必须是有效的变量名）
  第3行：数据类型限制（string, float, int, boolean）
  第4行：额外检查（必须是以#开头的字符串）
  第5行起：数据行

说明：
  - 自动遍历xlsx文件中所有不以#开头的sheet
  - 如果sheet名和xlsx文件名一致，json文件名为xlsx名
  - 否则为xlsx名_sheet名
  - 默认导出到 DataJson/xlsx文件名/ 目录下

示例：
  python excel_exporter.py config.xlsx
        '''
    )
    
    parser.add_argument('input', help='输入的Excel文件名（从xlsx文件夹中查找，可不带.xlsx后缀）')
    
    args = parser.parse_args()
    
    workspace = Path(__file__).parent
    xlsx_dir = workspace / 'xlsx'
    
    # 自动补全.xlsx后缀
    input_name = args.input
    if not input_name.lower().endswith('.xlsx'):
        input_name = input_name + '.xlsx'
    
    # 从xlsx文件夹查找文件
    xlsx_file = xlsx_dir / input_name
    if not xlsx_file.exists():
        print(f"错误: 未找到文件 '{input_name}'，请确认文件在xlsx文件夹中")
        sys.exit(1)
    
    # 创建导出器并加载工作簿
    exporter = JsonExporter(xlsx_file)
    exporter.load()
    
    # 获取所有有效sheet
    valid_sheets = exporter.get_valid_sheets()
    
    if not valid_sheets:
        print("错误: 没有找到有效的sheet（所有sheet名都以#开头）")
        exporter.close()
        sys.exit(1)
    
    # 检查DataJson链接是否存在
    if not check_datajson_link():
        prompt_setup_for_datajson()
        exporter.close()
        sys.exit(1)
    
    print(f"\n找到 {len(valid_sheets)} 个有效sheet: {', '.join(valid_sheets)}")
    print("-" * 50)
    
    # 导出所有sheet
    results = exporter.export_all()
    exporter.close()
    
    # 输出结果
    success_count = 0
    fail_count = 0
    
    for sheet_name, success, message, output_path in results:
        if success:
            print(f"[{sheet_name}] 成功: {message}")
            print(f"         输出: {output_path}")
            success_count += 1
        else:
            print(f"[{sheet_name}] 失败: {message}")
            fail_count += 1
    
    print("-" * 50)
    print(f"导出完成: {success_count} 成功, {fail_count} 失败")
    
    sys.exit(0 if fail_count == 0 else 1)


if __name__ == '__main__':
    main()
