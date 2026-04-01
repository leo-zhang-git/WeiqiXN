# -*- coding: utf-8 -*-
"""
检查器模块
每个特殊检查对应一个独立的检查器类
"""

from checker.base import BaseChecker, ColumnChecker, parse_extra_checkers

# 导入所有检查器
from checker.unique import UniqueChecker
from checker.enum import EnumChecker
from checker.require import RequireChecker

# 按注册顺序导出所有检查器
__all__ = [
    'BaseChecker',
    'ColumnChecker',
    'UniqueChecker',
    'EnumChecker',
    'RequireChecker',
    'parse_extra_checkers',
]
