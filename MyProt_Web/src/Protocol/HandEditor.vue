<template>
  <div>
    <!-- 请求模板编辑（动态列表） -->
    <el-form-item label="请求模板">
      <div v-for="(item, index) in op.requestTemplate" :key="index" style="display:flex; margin-bottom:8px;">
        <el-input v-model="op.requestTemplate[index]" placeholder="hex或{变量:func:fmt}" style="flex:1" />
        <el-button @click="removeLine(index)" :disabled="op.requestTemplate.length <= 1"
          style="margin-left:8px;">删除</el-button>
      </div>
      <el-button @click="addLine" size="small">添加一行</el-button>
      <template #extra>
        <span>支持格式：固定十六进制（如 03 00）或变量占位符（如 {UnitID:X2}）</span>
      </template>
    </el-form-item>
    <el-form-item label="数据起始索引">
      <el-input-number v-model="op.responseParser.dataStartIndex" :min="0" />
    </el-form-item>
    <el-form-item label="数据长度表达式">
      <el-input v-model="op.responseParser.dataLengthExpr" placeholder="resp[8] 或数字 4" />
      <template #extra>
        <span>支持 <code>resp[N]</code> 形式动态获取长度</span>
      </template>
    </el-form-item>
    <el-form-item label="值类型">
      <el-select v-model="op.responseParser.valueType">
        <el-option label="原始字节数组" value="ByteArray" />
        <el-option label="无符号16位整数" value="UInt16" />
        <el-option label="32位浮点数" value="Float" />
        <el-option label="位数组" value="BitArray" />
      </el-select>
    </el-form-item>
    <el-form-item label="有效性条件">
      <el-input v-model="op.responseParser.validCondition" placeholder="resp[7] == 0x03" />
    </el-form-item>
  </div>
</template>

<script setup>
import { defineProps } from 'vue'

const props = defineProps({ op: Object })

function addLine() {
  props.op.requestTemplate.push('')
}
function removeLine(index) {
  props.op.requestTemplate.splice(index, 1)
}
</script>