# -*- coding: utf-8 -*-
"""
必填检查器
检查该列所有单元格是否都不为空
"""

from typing import Any, List, Tuple

from checker.base import BaseChecker, ColumnChecker


class RequireChecker(BaseChecker):
    """#require - 检查该列所有单元格是否都不为空"""

    name = 'require'

    @classmethod
    def check(cls, values: List[Any], col: int, key: str, sheet_name: str,
              col_type: str = 'string', args: str = '') -> Tuple[bool, str]:
        for i, value in enumerate(values):
            # 检查空值：None、空字符串、仅空白字符
            if value is None or str(value).strip() == '':
                return False, f"第{i + 5}行 '{key}' 列存在空单元格"
        return True, ""


# 注册检查器
ColumnChecker.register(RequireChecker)
