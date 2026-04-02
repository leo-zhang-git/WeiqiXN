# -*- coding: utf-8 -*-

"""

枚举检查器

检查该列所有值是否在指定的枚举值列表中

"""

from typing import Any, List, Tuple

from checker.base import BaseChecker, ColumnChecker

def parse_enum_args(args_str: str) -> List[str]:

    """解析 enum 参数，如 'a,b,c' -> ['a', 'b', 'c']"""

    if not args_str.strip():

        return []

    return [arg.strip() for arg in args_str.split(',')]

class EnumChecker(BaseChecker):

    """#enum(a,b,c) - 检查该列所有值是否在指定的枚举值列表中"""

    name = 'enum'

    @classmethod

    def check(cls, values: List[Any], col: int, key: str, sheet_name: str,

              col_type: str = 'string', args: str = '') -> Tuple[bool, str]:

        # enum检查只支持基础类型，不支持list/tuple等高级类型
        if col_type.startswith('list('):
            return False, "enum检查不支持list类型，只能使用基础类型(string, int, float, boolean)"
        if col_type.startswith('tuple('):
            return False, "enum检查不支持tuple类型，只能使用基础类型(string, int, float, boolean)"

        enum_values = parse_enum_args(args)

        if not enum_values:

            return False, "enum() 缺少参数，格式: #enum(a,b,c)"

        # 检查枚举值本身是否有重复

        seen_enum = {}

        for ev in enum_values:

            if ev in seen_enum:

                return False, f"枚举值 '{ev}' 在参数中重复出现"

            seen_enum[ev] = True

        # 检查枚举值是否符合列类型

        for ev in enum_values:

            if col_type == 'int':

                try:

                    float(ev)

                    if '.' in ev:

                        return False, f"枚举值 '{ev}' 不是有效的整数"

                except ValueError:

                    return False, f"枚举值 '{ev}' 不是有效的整数"

            elif col_type == 'float':

                try:

                    float(ev)

                except ValueError:

                    return False, f"枚举值 '{ev}' 不是有效的浮点数"

        # 将枚举值转换为集合

        enum_set = set(enum_values)

        # 检查列中的每个值是否在枚举值中，跳过None和空值

        for value in values:

            if value is None or str(value).strip() == '':

                continue

            value_str = str(value).strip()

            if value_str not in enum_set:

                return False, f"值 '{value_str}' 不在允许的枚举值 {enum_values} 中"

        return True, ""

# 注册检查器

ColumnChecker.register(EnumChecker)

