#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Excel配置导出工具
检查Excel数据并导出为JSON
"""

import sys
from pathlib import Path

try:
    import openpyxl
except ImportError:
    print("错误: 缺少必要的依赖库，请先运行 setup.bat")
    input("按回车键退出...")
    sys.exit(1)

from excel_checker import ExcelChecker
from excel_exporter import JsonExporter, check_datajson_link, prompt_setup_for_datajson

def main():
    """命令行入口"""
    if len(sys.argv) < 2:
        print("用法: python main.py <excel文件>")
        print("示例: python main.py test.xlsx")
        sys.exit(1)

    input_name = sys.argv[1]
    if not input_name.lower().endswith('.xlsx'):
        input_name += '.xlsx'

    xlsx_file = Path(__file__).parent / 'xlsx' / input_name
    if not xlsx_file.exists():
        print(f"错误: 未找到文件 '{input_name}'")
        sys.exit(1)

    print(f"正在检查并导出: {input_name}")
    print("-" * 50)

    # 1. 先检查数据
    checker = ExcelChecker(xlsx_file)
    checker.load()

    valid_sheets = checker.get_valid_sheets()
    if not valid_sheets:
        print("错误: 没有找到有效的sheet")
        checker.close()
        sys.exit(1)

    print(f"找到 {len(valid_sheets)} 个有效sheet")
    print("-" * 50)

    results = checker.check_all()
    checker.close()

    # 输出检查结果
    success_count = 0
    fail_count = 0
    for sheet_name, success, message, count in results:
        status = "通过" if success else "失败"
        print(f"[{sheet_name}] {status}: {message}")
        if success:
            success_count += 1
        else:
            fail_count += 1

    print("-" * 50)

    # 如果检查失败，不导出
    if fail_count > 0:
        print(f"检查未通过，终止导出")
        sys.exit(1)

    # 2. 检查DataJson链接
    if not check_datajson_link():
        prompt_setup_for_datajson()
        sys.exit(1)

    # 3. 导出JSON
    print("检查通过，开始导出...")
    exporter = JsonExporter(xlsx_file)
    exporter.load()

    export_results = exporter.export_all()
    exporter.close()

    for sheet_name, success, message, output_path in export_results:
        if success:
            print(f"[{sheet_name}] 导出成功 -> {output_path}")
        else:
            print(f"[{sheet_name}] 导出失败: {message}")

    print("-" * 50)
    print(f"完成: {success_count} 成功, {fail_count} 失败")
    sys.exit(0 if fail_count == 0 else 1)

if __name__ == '__main__':
    main()
