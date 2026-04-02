# -*- coding: utf-8 -*-
"""
检查器基类
定义检查器的通用接口
"""

import re
from typing import Any, List, Tuple, Optional, Type

class BaseChecker:
    """检查器基类"""
    name: str = ""

    @classmethod
    def check(cls, values: List[Any], col: int, key: str, sheet_name: str,
              col_type: str = 'string', args: str = '') -> Tuple[bool, str]:
        """执行检查，子类需重写"""
        raise NotImplementedError

class ColumnChecker:
    """列特殊检查器管理类"""

    # 检查器注册表
    _checkers: List[Type[BaseChecker]] = []

    @classmethod
    def register(cls, checker_cls: Type[BaseChecker]) -> None:
        """注册检查器"""
        if checker_cls not in cls._checkers:
            cls._checkers.append(checker_cls)

    @classmethod
    def get_checker(cls, func_name: str) -> Optional[Type[BaseChecker]]:
        """获取检查器类"""
        func_name = func_name.lower()
        for checker in cls._checkers:
            if checker.name == func_name:
                return checker
        return None

    @classmethod
    def get_all_checkers(cls) -> List[Type[BaseChecker]]:
        """获取所有已注册的检查器"""
        return cls._checkers.copy()

def parse_extra_checkers(extra_str: str) -> List[Tuple[str, str]]:
    """
    解析第4行的特殊检查配置
    格式: #func1(a)#func2(b)#func3
    返回: [('func1', 'a'), ('func2', 'b'), ('func3', ''), ...]
    """
    result = []
    pattern = r'#([a-zA-Z_][a-zA-Z0-9_]*)(?:\(([^)]*)\))?'
    matches = re.findall(pattern, extra_str)
    for func_name, args in matches:
        result.append((func_name.lower(), args.strip() if args else ''))
    return result
