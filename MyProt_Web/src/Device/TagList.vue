<template>
  <div class="tag-list-container">
    <div class="toolbar">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="标签名">
          <el-input v-model="searchForm.tagName" placeholder="模糊搜索" clearable />
        </el-form-item>
        <el-form-item label="设备ID">
          <el-input v-model="searchForm.deviceId" placeholder="设备ID" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchTags">查询</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
      <el-button type="primary" @click="openTagDialog()">
        <el-icon><Plus /></el-icon>
        新增标签
      </el-button>
    </div>

    <el-table :data="tags" border stripe v-loading="loading">
      <el-table-column prop="tagName" label="标签名" min-width="160" />
      <el-table-column prop="deviceId" label="设备ID" width="150" />
      <el-table-column prop="operation" label="操作" width="200" />
      <el-table-column label="变量" min-width="200">
        <template #default="{ row }">
          <div class="variable-tags">
            <el-tag v-for="(val, key) in row.variables" :key="key" size="small" type="info">
              {{ key }}: {{ val }}
            </el-tag>
          </div>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="180" align="center">
        <template #default="{ row }">
          <el-button link type="primary" @click="openTagDialog(row)">编辑</el-button>
          <el-button link type="danger" @click="deleteTag(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 新增/编辑标签对话框 -->
    <el-dialog
      v-model="tagDialogVisible"
      :title="editingTag ? '编辑标签' : '新增标签'"
      width="600px"
      destroy-on-close
    >
      <el-form :model="tagForm" label-width="100px">
        <el-form-item label="标签名">
          <el-input v-model="tagForm.tagName" :disabled="!!editingTag" />
        </el-form-item>
        <el-form-item label="设备ID">
          <el-select v-model="tagForm.deviceId" filterable placeholder="选择设备">
            <el-option v-for="d in deviceOptions" :key="d.id" :value="d.id" :label="d.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="操作">
          <el-select v-model="tagForm.operation" placeholder="选择操作">
            <!-- 可根据设备协议动态加载，这里先静态示例 -->
            <el-option label="ReadHoldingRegisters" value="ReadHoldingRegisters" />
            <el-option label="ReadCoils" value="ReadCoils" />
            <el-option label="ReadDB" value="ReadDB" />
          </el-select>
        </el-form-item>
        <el-form-item label="变量(JSON)">
          <el-input
            v-model="variablesText"
            type="textarea"
            :rows="5"
            placeholder='{"UnitID":1,"StartAddress":0}'
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="tagDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveTag">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'

const loading = ref(false)
const tags = ref([])
const deviceOptions = ref([])
const searchForm = reactive({ tagName: '', deviceId: '' })

const tagDialogVisible = ref(false)
const editingTag = ref(null)
const tagForm = reactive({
  tagName: '',
  deviceId: '',
  operation: '',
  variables: {}
})
const variablesText = ref('')

// 加载设备选项（供标签选择）
async function loadDeviceOptions() {
  try {
  } catch { /* ignore */ }
}

// 获取标签列表
async function fetchTags() {
  loading.value = true
  try {
  } catch {
    ElMessage.error('加载标签列表失败')
  } finally {
    loading.value = false
  }
}

function resetSearch() {
  searchForm.tagName = ''
  searchForm.deviceId = ''
  fetchTags()
}

function openTagDialog(row) {
  editingTag.value = row || null
  if (row) {
    tagForm.tagName = row.tagName
    tagForm.deviceId = row.deviceId
    tagForm.operation = row.operation
    variablesText.value = JSON.stringify(row.variables, null, 2)
  } else {
    tagForm.tagName = ''
    tagForm.deviceId = ''
    tagForm.operation = ''
    variablesText.value = '{}'
  }
  tagDialogVisible.value = true
}

async function saveTag() {
  if (!tagForm.tagName || !tagForm.deviceId || !tagForm.operation) {
    ElMessage.warning('请填写必填项')
    return
  }
  try {
    tagForm.variables = JSON.parse(variablesText.value)
  } catch {
    ElMessage.error('变量 JSON 格式错误')
    return
  }
  try {
    if (editingTag.value) {
      ElMessage.success('标签更新成功')
    } else {
      ElMessage.success('标签创建成功')
    }
    tagDialogVisible.value = false
    fetchTags()
  } catch {
    ElMessage.error('保存失败')
  }
}

function deleteTag(row) {
  ElMessageBox.confirm(`确定要删除标签 "${row.tagName}" 吗？`, '警告', { type: 'warning' })
    .then(async () => {
      ElMessage.success('已删除')
      fetchTags()
    })
    .catch(() => {})
}

onMounted(() => {
  loadDeviceOptions()
  fetchTags()
})
</script>

<style scoped>
.tag-list-container {
  padding: 10px 0;
}
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
  flex-wrap: wrap;
}
.variable-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}
</style>